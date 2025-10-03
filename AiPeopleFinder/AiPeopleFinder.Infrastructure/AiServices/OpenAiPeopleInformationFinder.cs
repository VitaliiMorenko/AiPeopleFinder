using System.Text.Json;
using AiPeopleFinder.Application.AiServices;
using OpenAI.Responses;

namespace AiPeopleFinder.Infrastructure.AiServices;

public class OpenAiPeopleInformationFinder(OpenAIResponseClient client) : IAiPeopleInformationFinder
{

    public async Task<string> SearchInformation(string searchTerm)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        var responseOptions = new ResponseCreationOptions
        {
            Tools = { ResponseTool.CreateWebSearchTool() }
        };

        var response = await client.CreateResponseAsync(
            userInputText: $"Find public information for person {searchTerm}",
            options: responseOptions);

        return response.Value.GetOutputText();
    }
}