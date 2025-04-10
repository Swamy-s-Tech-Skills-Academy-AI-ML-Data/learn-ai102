namespace AzAIServicesCommon.Configuration;

public sealed class AzAISvcAppConfiguration
{
    public NLPAzAIServices? NLPAzAIServices { get; set; }

    public GenAIAzOpenAIServices? GenAIAzOpenAIServices { get; set; }
}
