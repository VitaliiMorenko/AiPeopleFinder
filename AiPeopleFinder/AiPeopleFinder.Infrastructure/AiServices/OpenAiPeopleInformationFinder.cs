using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using AiPeopleFinder.Application.AiServices;
using AiPeopleFinder.Domain;
using AiPeopleFinder.Infrastructure.Http;

namespace AiPeopleFinder.Infrastructure.AiServices;

public class OpenAiPeopleInformationFinder(IHttpClientFactory httpClientFactory) : IAiPeopleInformationFinder
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
        
    private const string Schema = """
    {
      "type":"object",
      "additionalProperties":false,
      "properties":{
        "Name":{"type":"string"},
        "Company":{"type":"string"},
        "CurrentRole":{"type":"string"},
        "KeyFacts":{
          "type":"array","minItems":5,"maxItems":5,
          "items": {"type":"string","maxLength":200}
          
        },
        "PastRolesCompanies":{"type":"string"}
      },
      "required":["Name","Company","CurrentRole","KeyFacts","PastRolesCompanies"]
    }
    """;

    private const string Model = "gpt-4o-mini";

    public async Task<PersonProfile?> SearchInformation(string searchTerm)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("OPENAI_API_KEY is required");

        using var client = httpClientFactory.CreateHttpClient("https://api.openai.com/v1/");
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        
        const string systemInstruction = """
                              You verify the person using ONLY public professional sources (LinkedIn, employer site, trusted press).
                              Protocol:
                              1) Use web_search to query: "<Name> <Email> <Company>" and variations.
                              2) Pick the best matching person by exact name + employer match.
                              3) Visit 2–4 top sources; extract CURRENT role title, responsibilities, highlights, skills, affiliations, education, and past roles.
                              4) If conflicting info, prefer employer site > recent press > LinkedIn.
                              5) Only if after >=3 distinct searches and >=2 site visits you cannot verify an item, output exactly "Not enough public info found." for that item.
                              6) Output JSON strictly by the schema. Truncate at word boundaries.
                              7) Do NOT include sources in fields, only facts.
                              """;
        var userInput = $"Search person by email and/or name and/or company: {searchTerm}";
        
        var body = new
        {
            model = Model,
            input = new object[]
            {
                new { role = "system", content = new object[] { new { type = "input_text", text = systemInstruction } } },
                new { role = "user", content = new object[] { new { type = "input_text", text = userInput } } }
            },
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "persona_brief_profile",
                    schema = JsonSerializer.Deserialize<object>(Schema)!,
                    strict = true
                }
            },
            tools = new[]{ new { type="web_search" } },
            max_output_tokens = 800,
            temperature = 0.2
        };

        var resp = await client.PostAsJsonAsync("responses", body, JsonOpts);
        resp.EnsureSuccessStatusCode();
        
        var root = JsonNode.Parse(await resp.Content.ReadAsStringAsync())!;
        if (root["output"] is not JsonArray output)
            return null;
        
        foreach (var outputItem in output)
        {
            var type = outputItem?["type"]?.GetValue<string>();
            if (type != "message") continue;
            if (outputItem?["content"] is not JsonArray contentArray) continue;
            
            foreach (var contentItem in contentArray)
            {
                type = contentItem?["type"]?.GetValue<string>();
                if (type != "output_text") continue;
                var text = contentItem?["text"]?.GetValue<string>();
                var profile = JsonSerializer.Deserialize<PersonProfile>(text!, JsonOpts);
                return profile;
            }
        }

        return null;
    }
}