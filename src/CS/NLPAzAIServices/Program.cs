using AzAIServicesCommon.Configuration;
using AzAIServicesCommon.Extensions;
using HeaderFooter.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Media;

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

if (string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.SpeechAIService?.Endpoint) ||
    string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.SpeechAIService?.Key) ||
    string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.SpeechAIService?.Region))
{
    WriteLine("Please check your appsettings.json file for missing or incorrect values.");
    return;
}

try
{
    // Get config settings from AppSettings
    string aiSvcKey = appConfig?.NLPAzAIServices?.SpeechAIService?.Key!;
    string aiSvcRegion = appConfig?.NLPAzAIServices?.SpeechAIService?.Region!;

    // Configure translation
    translationConfig = SpeechTranslationConfig.FromSubscription(aiSvcKey, aiSvcRegion);
    translationConfig.SpeechRecognitionLanguage = "en-US";
    translationConfig.AddTargetLanguage("fr");
    translationConfig.AddTargetLanguage("es");
    translationConfig.AddTargetLanguage("hi");
    WriteLine("Ready to translate from " + translationConfig.SpeechRecognitionLanguage);

    // Configure speech
    speechConfig = SpeechConfig.FromSubscription(aiSvcKey, aiSvcRegion);

    string targetLanguage = "";
    while (targetLanguage != "quit")
    {
        WriteLine("\nEnter a target language\n fr = French\n es = Spanish\n hi = Hindi\n Enter anything else to stop\n");
        targetLanguage = ReadLine()?.ToLowerInvariant()!;
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
    WriteLine(ex.Message);
}

async Task Translate(string targetLanguage)
{
    string translation = "";

    //// Translate speech from microphone
    //using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    //using TranslationRecognizer translator = new(translationConfig, audioConfig);

    //WriteLine("Speak now...");
    //TranslationRecognitionResult result = await translator.RecognizeOnceAsync();
    //WriteLine($"Translating '{result.Text}'");
    //translation = result.Translations[targetLanguage];
    //OutputEncoding = Encoding.UTF8;
    //WriteLine(translation);

    // Translate speech from file
    string audioFile = @"D:\STSAAIMLDT\learn-ai102\src\Data\NLP\Speech\station.wav";
    SoundPlayer wavPlayer = new(audioFile);
    wavPlayer.Play();
    using AudioConfig audioConfig = AudioConfig.FromWavFileInput(audioFile);
    using TranslationRecognizer translator = new(translationConfig, audioConfig);
    WriteLine("Getting speech from file...");
    TranslationRecognitionResult result = await translator.RecognizeOnceAsync();
    WriteLine($"Translating '{result.Text}'");
    translation = result.Translations[targetLanguage];
    OutputEncoding = Encoding.UTF8;
    WriteLine(translation);
    
    // Synthesize translation

    await Task.CompletedTask.ConfigureAwait(false);
}

// ******************** 8. Translate Speech ********************

//await NERWithAzureAIService.ShowNERDemoWithAzureAIService(appConfig).ConfigureAwait(false);

WriteLine("\n\nPress any key to exit...");
ReadKey();