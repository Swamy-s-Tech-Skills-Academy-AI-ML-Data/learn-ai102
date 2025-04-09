namespace AzAIServicesCommon.Configuration;

public sealed class AzAISvcAppConfiguration
{
    public NLPAzAIServices? NLPAzAIServices { get; set; }
}

public sealed class NLPAzAIServices
{
    public NERAIService? NERAIService { get; set; }
}

public sealed class NERAIService
{
    public string? Endpoint { get; set; }

    public string? Key { get; set; }

    public string? ProjectName { get; set; }

    public string? DeploymentName { get; set; }
}
