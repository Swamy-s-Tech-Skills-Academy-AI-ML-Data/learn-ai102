namespace AzAIServicesCommon.Configuration;

public sealed class AzAISvcAppConfiguration
{
    public NLPAzAIServices? NLPAzAIServices { get; set; }

    public GenAIAzOpenAIServices? GenAIAzOpenAIServices { get; set; }
}

public sealed class NLPAzAIServices
{
    public NERAIService? NERAIService { get; set; }

    public SpeechAIService? SpeechAIService { get; set; }
}


public sealed class NERAIService
{
    public string? Endpoint { get; set; }

    public string? Key { get; set; }

    public string? ProjectName { get; set; }

    public string? DeploymentName { get; set; }
}

public sealed class GenAIAzOpenAIServices
{
    public AzureOpenAIChatService? AzureOpenAIChatService { get; set; }
}

public sealed class AzureOpenAIChatService
{
    public string? AzureOAIEndpoint { get; set; }

    public string? AzureOAIKey { get; set; }

    public string? AzureOAIDeploymentName { get; set; }
}

public sealed class SpeechAIService
{
    public string? SpeechEndpoint { get; set; }

    public string? SpeechKey { get; set; }

    public string? SpeechRegion { get; set; }
}

