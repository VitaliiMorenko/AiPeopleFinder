using AiPeopleFinder.Application.AiServices;
using AiPeopleFinder.Application.Repositories;
using AiPeopleFinder.Domain;
using Moq;
using NUnit.Framework;

namespace AiPeopleFinder.Application.Tests;

[TestFixture]
public class PeopleFinderServiceTests
{
    private Mock<IAiPeopleInformationFinder> _aiMock = null!;
        private Mock<ISearchRequestDetailsRepository> _repoMock = null!;
        private PeopleFinderService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _aiMock = new Mock<IAiPeopleInformationFinder>(MockBehavior.Strict);
            _repoMock = new Mock<ISearchRequestDetailsRepository>(MockBehavior.Strict);
            _sut = new PeopleFinderService(_aiMock.Object, _repoMock.Object);
        }

        [Test]
        public async Task GetSearchRequestDetails_WhenCachedExists_ReturnsCached_AndDoesNotCallAiOrUpsert()
        {
            // arrange
            const string term = "Ada Lovelace";
            var cached = new SearchRequestDetails
            {
                SearchTerm = term,
                Profile = new PersonProfile(
                    Name: "Ada Lovelace",
                    Company: "N/A",
                    CurrentRole: "Mathematician",
                    KeyFacts: new() { "Analytical Engine", "First programmer" },
                    PastRolesCompanies: "—")
            };

            _repoMock
                .Setup(r => r.GetBySearchTerm(term))
                .ReturnsAsync(cached);

            // act
            var result = await _sut.GetSearchRequestDetails(term);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(cached, Is.EqualTo(result));

            _aiMock.Verify(a => a.SearchInformation(It.IsAny<string>()), Times.Never);
            _repoMock.Verify(r => r.CreateOrUpdate(It.IsAny<SearchRequestDetails>()), Times.Never);
            _repoMock.Verify(r => r.GetBySearchTerm(term), Times.Once);
            _repoMock.VerifyNoOtherCalls();
            _aiMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetSearchRequestDetails_WhenNoCache_CallsAi_Saves_AndReturnsComposedResult()
        {
            // arrange
            const string term = "Grace Hopper";
            var aiProfile = new PersonProfile(
                Name: "Grace Hopper",
                Company: "US Navy",
                CurrentRole: "Rear Admiral",
                KeyFacts: new() { "COBOL", "Compiler pioneer" },
                PastRolesCompanies: "Harvard; UNIVAC");

            _repoMock
                .Setup(r => r.GetBySearchTerm(term))
                .ReturnsAsync((SearchRequestDetails?)null);

            _aiMock
                .Setup(a => a.SearchInformation(term))
                .ReturnsAsync(aiProfile);

            SearchRequestDetails? saved = null;
            _repoMock
                .Setup(r => r.CreateOrUpdate(It.IsAny<SearchRequestDetails>()))
                .Callback<SearchRequestDetails>(x => saved = x)
                .Returns(Task.CompletedTask);

            // act
            var result = await _sut.GetSearchRequestDetails(term);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.SearchTerm, Is.EqualTo(term));
            Assert.That(result.Profile, Is.Not.Null);
            Assert.That(result.Profile!.Name, Is.EqualTo("Grace Hopper"));
            
            Assert.That(saved, Is.Not.Null, "CreateOrUpdate is not called");
            Assert.That(saved!.SearchTerm, Is.EqualTo(term));
            Assert.That(saved.Profile, Is.EqualTo(aiProfile));

            _repoMock.Verify(r => r.GetBySearchTerm(term), Times.Once);
            _aiMock.Verify(a => a.SearchInformation(term), Times.Once);
            _repoMock.Verify(r => r.CreateOrUpdate(It.IsAny<SearchRequestDetails>()), Times.Once);

            _repoMock.VerifyNoOtherCalls();
            _aiMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetSearchRequestDetails_WhenAiReturnsNull_SavesAndReturnsWithNullProfile()
        {
            // arrange
            const string term = "Unknown Person <noreply@example.com>";

            _repoMock
                .Setup(r => r.GetBySearchTerm(term))
                .ReturnsAsync((SearchRequestDetails?)null);

            _aiMock
                .Setup(a => a.SearchInformation(term))
                .ReturnsAsync((PersonProfile?)null);

            SearchRequestDetails? saved = null;
            _repoMock
                .Setup(r => r.CreateOrUpdate(It.IsAny<SearchRequestDetails>()))
                .Callback<SearchRequestDetails>(x => saved = x)
                .Returns(Task.CompletedTask);

            // act
            var result = await _sut.GetSearchRequestDetails(term);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.SearchTerm, Is.EqualTo(term));
            Assert.That(result.Profile, Is.Null, "Profile should be null, if AI does not find anything");

            Assert.That(saved, Is.Not.Null);
            Assert.That(saved!.SearchTerm, Is.EqualTo(term));
            Assert.That(saved.Profile, Is.Null);

            _repoMock.Verify(r => r.GetBySearchTerm(term), Times.Once);
            _aiMock.Verify(a => a.SearchInformation(term), Times.Once);
            _repoMock.Verify(r => r.CreateOrUpdate(It.IsAny<SearchRequestDetails>()), Times.Once);

            _repoMock.VerifyNoOtherCalls();
            _aiMock.VerifyNoOtherCalls();
        }
}