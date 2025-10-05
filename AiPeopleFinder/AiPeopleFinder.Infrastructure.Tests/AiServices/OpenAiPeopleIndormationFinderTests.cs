using System.Net;
using System.Text.Json.Nodes;
using AiPeopleFinder.Application.AiServices;
using AiPeopleFinder.Infrastructure.AiServices;
using AiPeopleFinder.Infrastructure.Configuration;
using AiPeopleFinder.Infrastructure.Tests.Helpers;
using AiPeopleFinder.Infrastructure.Utilities.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace AiPeopleFinder.Infrastructure.Tests.AiServices;

[TestFixture]
    public class OpenAiPeopleInformationFinderTests
    {
        private Mock<IHttpClientFactory> _httpFactoryMock = null;
        private IOptions<Config> _options = null;
        private const string ApiKey = "sk-test";
        private const string Model = "gpt-4o-mini";

        [SetUp]
        public void SetUp()
        {
            _httpFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            _options = Options.Create(new Config
            {
                OpenAi = new OpenAiConfig { ApiKey = ApiKey, Model = Model }
            });
        }

        private static HttpClient CreateClient(MockHttpMessageHandler handler, string baseAddress)
        {
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseAddress)
            };
            return client;
        }

        private static IAiPeopleInformationFinder CreateSut(Mock<IHttpClientFactory> factory, IOptions<Config> options, MockHttpMessageHandler handler)
        {
            factory
                .Setup(f => f.CreateHttpClient("https://api.openai.com/v1/"))
                .Returns(CreateClient(handler, "https://api.openai.com/v1/"));

            return new OpenAiPeopleInformationFinder(factory.Object, options);
        }

        [Test]
        public async Task SearchInformation_Success_ParsesPersonProfile_AndBuildsCorrectRequest()
        {
            // arrange
            var profileJson = """
            {
              "Name":"Ada Lovelace",
              "Company":"Analytical Engine",
              "CurrentRole":"Mathematician",
              "KeyFacts":["Pioneer","Algorithm","Babbage","Notes","Vision"],
              "PastRolesCompanies":"—"
            }
            """;

            var apiResponse = new JsonObject
            {
                ["output"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["type"] = "message",
                        ["content"] = new JsonArray
                        {
                            new JsonObject
                            {
                                ["type"] = "output_text",
                                ["text"] = profileJson
                            }
                        }
                    }
                }
            }.ToJsonString();

            var handler = new MockHttpMessageHandler(_ =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(apiResponse, System.Text.Encoding.UTF8, "application/json")
                };
                return Task.FromResult(resp);
            });

            var sut = CreateSut(_httpFactoryMock, _options, handler);

            // act
            var result = await sut.SearchInformation("Ada Lovelace");

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Ada Lovelace"));
            Assert.That(result.Company, Is.EqualTo("Analytical Engine"));
            Assert.That(result.KeyFacts, Has.Count.EqualTo(5));

            var req = handler.LastRequest;
            Assert.That(req, Is.Not.Null);
            Assert.That(req.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(req.RequestUri.ToString(), Does.EndWith("/responses"));
            
            Assert.That(req.Headers.Authorization != null && req.Headers.Authorization.Scheme == "Bearer", Is.True);
            Assert.That(req.Headers.Authorization.Parameter, Is.EqualTo(ApiKey));

            var body = JsonNode.Parse(handler.LastRequestBody);
            Assert.That(body["model"].GetValue<string>(), Is.EqualTo(Model));

            var tools = body["tools"].AsArray();
            Assert.That(tools.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(tools[0]["type"].GetValue<string>(), Is.EqualTo("web_search"));

            Assert.That(body["text"]["format"]["type"].GetValue<string>(), Is.EqualTo("json_schema"));
            
            Assert.That(body["max_output_tokens"].GetValue<int>(), Is.EqualTo(800));
            Assert.That(body["temperature"].GetValue<double>(), Is.EqualTo(0.2).Within(0.0001));

            _httpFactoryMock.VerifyAll();
        }

        [Test]
        public async Task SearchInformation_NoOutput_ReturnsNull()
        {
            // arrange
            var apiResponse = new JsonObject
            {
                ["foo"] = "bar"
            }.ToJsonString();

            var handler = new MockHttpMessageHandler(_ =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(apiResponse, System.Text.Encoding.UTF8, "application/json")
                };
                return Task.FromResult(resp);
            });

            var sut = CreateSut(_httpFactoryMock, _options, handler);

            // act
            var result = await sut.SearchInformation("Someone");

            // assert
            Assert.That(result, Is.Null);
            _httpFactoryMock.VerifyAll();
        }

        [Test]
        public async Task SearchInformation_MessageWithoutOutputText_ReturnsNull()
        {
            // arrange
            var apiResponse = new JsonObject
            {
                ["output"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["type"] = "message",
                        ["content"] = new JsonArray
                        {
                            new JsonObject { ["type"] = "other_chunk", ["text"] = "not json" }
                        }
                    }
                }
            }.ToJsonString();

            var handler = new MockHttpMessageHandler(_ =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(apiResponse, System.Text.Encoding.UTF8, "application/json")
                };
                return Task.FromResult(resp);
            });

            var sut = CreateSut(_httpFactoryMock, _options, handler);

            // act
            var result = await sut.SearchInformation("Someone");

            // assert
            Assert.That(result, Is.Null);
            _httpFactoryMock.VerifyAll();
        }

        [Test]
        public void SearchInformation_NonSuccessStatus_Throws()
        {
            // arrange
            var handler = new MockHttpMessageHandler(_ =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("""{"error":"unauthorized"}""", System.Text.Encoding.UTF8, "application/json")
                };
                return Task.FromResult(resp);
            });

            var sut = CreateSut(_httpFactoryMock, _options, handler);

            // act/assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await sut.SearchInformation("X"));
            _httpFactoryMock.VerifyAll();
        }
    }