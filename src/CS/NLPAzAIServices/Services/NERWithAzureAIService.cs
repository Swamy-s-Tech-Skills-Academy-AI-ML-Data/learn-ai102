﻿using AzAIServicesCommon.Configuration;
using Azure;
using Azure.AI.TextAnalytics;

namespace NLPAzAIServices.Services;

#pragma warning disable CA1303

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

            WriteLine("NER with Azure AI Service");

            // Create client using endpoint and key
            AzureKeyCredential credentials = new(appConfig?.NLPAzAIServices?.NERAIService?.Key!);
            Uri endpoint = new(appConfig?.NLPAzAIServices?.NERAIService?.Endpoint!);

            TextAnalyticsClient aiClient = new(endpoint, credentials);

            // Read each text file in the ads folder
            List<TextDocumentInput> batchedDocuments = [];
            var folderPath = Path.GetFullPath(@"D:\STSAAIMLDT\learn-ai102\src\Data\NLP\NER\ads");
            DirectoryInfo folder = new(folderPath);
            FileInfo[] files = folder.GetFiles("*.txt");

            foreach (var file in files)
            {
                // Read the file contents
                StreamReader sr = file.OpenText();
                var text = await sr.ReadToEndAsync().ConfigureAwait(false);
                sr.Close();

                TextDocumentInput doc = new(file.Name, text)
                {
                    Language = "en",
                };
                batchedDocuments.Add(doc);
            }

            // Extract entities
            RecognizeCustomEntitiesOperation operation = await aiClient.RecognizeCustomEntitiesAsync(WaitUntil.Completed, batchedDocuments, appConfig?.NLPAzAIServices?.NERAIService?.ProjectName, appConfig?.NLPAzAIServices?.NERAIService?.DeploymentName).ConfigureAwait(false);

            await foreach (RecognizeCustomEntitiesResultCollection documentsInPage in operation.Value.ConfigureAwait(false))
            {
                foreach (RecognizeEntitiesResult documentResult in documentsInPage)
                {
                    ForegroundColor = ConsoleColor.Yellow;
                    WriteLine($"Result for \"{documentResult.Id}\":");

                    ForegroundColor = ConsoleColor.DarkGreen;
                    if (documentResult.HasError)
                    {
                        WriteLine($"  Error!");
                        WriteLine($"  Document error code: {documentResult.Error.ErrorCode}");
                        WriteLine($"  Message: {documentResult.Error.Message}");
                        WriteLine();
                        continue;
                    }

                    WriteLine($"  Recognized {documentResult.Entities.Count} entities:");

                    foreach (CategorizedEntity entity in documentResult.Entities)
                    {
                        WriteLine($"  Entity: {entity.Text}");
                        WriteLine($"  Category: {entity.Category}");
                        WriteLine($"  Offset: {entity.Offset}");
                        WriteLine($"  Length: {entity.Length}");
                        WriteLine($"  ConfidenceScore: {entity.ConfidenceScore}");
                        WriteLine($"  SubCategory: {entity.SubCategory}");
                        WriteLine();
                    }

                    WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine(ex.Message);
            throw;
        }
        finally
        {
            ResetColor();
        }
    }
}
