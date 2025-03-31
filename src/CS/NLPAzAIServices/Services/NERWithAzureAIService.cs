using AzAIServicesCommon.Configuration;
using Azure.AI.TextAnalytics;
using Azure;
using Azure.AI.TextAnalytics;

namespace NLPAzAIServices.Services;

internal sealed class NERWithAzureAIService
{
    public static async Task ShowNERDemoWithAzureAIService(AzAISvcAppConfiguration appConfig)
    {
        try
        {
            if (string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.NERAIService?.Endpoint)
                || string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.NERAIService?.Key)
                || string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.NERAIService?.ProjectName)
                || string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.NERAIService?.DeploymentName))
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Please check your appsettings.json file for missing or incorrect values.");

                return;
            }

            ForegroundColor = ConsoleColor.DarkGreen;

            WriteLine("NER with Azure AI Service");

            // Create client using endpoint and key
            AzureKeyCredential credentials = new(appConfig?.NLPAzAIServices?.NERAIService?.Key!);
            Uri endpoint = new(appConfig?.NLPAzAIServices?.NERAIService?.Endpoint!);

            TextAnalyticsClient aiClient = new(endpoint, credentials);

            // Read each text file in the ads folder
            List<TextDocumentInput> batchedDocuments = [];
            var folderPath = Path.GetFullPath("./ads");
            DirectoryInfo folder = new(folderPath);
            FileInfo[] files = folder.GetFiles("*.txt");
            foreach (var file in files)
            {
                // Read the file contents
                StreamReader sr = file.OpenText();
                var text = sr.ReadToEnd();
                sr.Close();
                TextDocumentInput doc = new(file.Name, text)
                {
                    Language = "en",
                };
                batchedDocuments.Add(doc);
            }

            // Extract entities


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
