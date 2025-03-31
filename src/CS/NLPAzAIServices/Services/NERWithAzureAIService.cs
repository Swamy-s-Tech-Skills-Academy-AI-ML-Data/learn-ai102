using AzAIServicesCommon.Configuration;

namespace NLPAzAIServices.Services;

internal sealed class NERWithAzureAIService
{
    public static async Task ShowNERDemoWithAzureAIService(AzAISvcAppConfiguration appConfig)
    {
        try
        {
            ForegroundColor = ConsoleColor.DarkGreen;

            WriteLine("NER with Azure AI Service");


        }
        catch (Exception ex)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine(ex.Message);
        }
        finally
        {
            ResetColor();
        }
    }
}
