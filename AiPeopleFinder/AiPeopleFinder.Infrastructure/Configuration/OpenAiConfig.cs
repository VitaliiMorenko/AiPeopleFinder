﻿namespace AiPeopleFinder.Infrastructure.Configuration;

public class OpenAiConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o-mini";
}