namespace AiPeopleFinder.Infrastructure.Configuration;

public class Config
{
    public OpenAiConfig OpenAi { get; set; }
    public int CacheTtlInMinutes { get; set; }
}