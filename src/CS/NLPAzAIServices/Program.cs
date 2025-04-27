using AzAIServicesCommon.Configuration;
using AzAIServicesCommon.Extensions;
using HeaderFooter.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

#pragma warning disable CA1303

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();

// Set console output to encoding
OutputEncoding = Encoding.Unicode;

// Optional: Set input encoding if reading Unicode input
InputEncoding = Encoding.Unicode;

// ******************** 8. Translate Speech ********************
SpeechConfig speechConfig;
SpeechTranslationConfig translationConfig;

if (string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.SpeechAIService?.SpeechEndpoint) || string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.SpeechAIService?.SpeechKey) || string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.SpeechAIService?.SpeechRegion))
{
    WriteLine("Please check your appsettings.json file for missing or incorrect values.");
    return;
}

try
{
    // Get config settings from AppSettings
    string aiSvcKey = appConfig?.NLPAzAIServices?.SpeechAIService?.SpeechKey!;
    string aiSvcRegion = appConfig?.NLPAzAIServices?.SpeechAIService?.SpeechRegion!;

    // Configure translation
    translationConfig = SpeechTranslationConfig.FromSubscription(aiSvcKey, aiSvcRegion);
    translationConfig.SpeechRecognitionLanguage = "en-US";
    translationConfig.AddTargetLanguage("fr");
    translationConfig.AddTargetLanguage("es");
    translationConfig.AddTargetLanguage("hi");
    Console.WriteLine("Ready to translate from " + translationConfig.SpeechRecognitionLanguage);

    // Configure speech


    string targetLanguage = "";
    while (targetLanguage != "quit")
    {
        Console.WriteLine("\nEnter a target language\n fr = French\n es = Spanish\n hi = Hindi\n Enter anything else to stop\n");
        targetLanguage = Console.ReadLine().ToLower();
        if (translationConfig.TargetLanguages.Contains(targetLanguage))
        {
            await Translate(targetLanguage);
        }
        else
        {
            targetLanguage = "quit";
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

async Task Translate(string targetLanguage)
{
    string translation = "";

    // Translate speech


    // Synthesize translation


}

// ******************** 8. Translate Speech ********************

//await NERWithAzureAIService.ShowNERDemoWithAzureAIService(appConfig).ConfigureAwait(false);

WriteLine("\n\nPress any key to exit...");
ReadKey();