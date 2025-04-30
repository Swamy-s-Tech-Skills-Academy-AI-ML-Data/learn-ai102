using AzAIServicesCommon.Configuration;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System.Media;
using System.Text;

namespace NLPAzAIServices.Services;

#pragma warning disable CA1303
#pragma warning disable CA1308
#pragma warning disable CA1031

internal sealed class TranslateSpeechWithAzureAIService
{

    public static async Task ShowTranslateSpeechDemoWithAzureAIService(AzAISvcAppConfiguration appConfig)
    {
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
            string azureServiceKey = appConfig?.NLPAzAIServices?.SpeechAIService?.Key!;
            string azureServiceRegion = appConfig?.NLPAzAIServices?.SpeechAIService?.Region!;

            // Configure translation
            translationConfig = SpeechTranslationConfig.FromSubscription(azureServiceKey, azureServiceRegion);
            translationConfig.SpeechRecognitionLanguage = "en-US";
            translationConfig.AddTargetLanguage("fr");
            translationConfig.AddTargetLanguage("es");
            translationConfig.AddTargetLanguage("hi");
            WriteLine("Ready to translate from " + translationConfig.SpeechRecognitionLanguage);

            // Configure speech
            speechConfig = SpeechConfig.FromSubscription(azureServiceKey, azureServiceRegion);

            string targetLanguage = "";
            while (targetLanguage != "quit")
            {
                WriteLine("\nEnter a target language\n fr = French\n es = Spanish\n hi = Hindi\n Enter anything else to stop\n");
                targetLanguage = ReadLine()?.ToLowerInvariant()!;
                if (translationConfig.TargetLanguages.Contains(targetLanguage))
                {
                    await Translate(targetLanguage, speechConfig, translationConfig).ConfigureAwait(false);
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

    }

    private static async Task Translate(string targetLanguage, SpeechConfig speechConfig, SpeechTranslationConfig translationConfig)
    {
        string translation = "";
        TranslationRecognitionResult result;

        // Ask the user for input method choice
        WriteLine("\nSelect input source:\n 1 = Microphone\n 2 = Audio file\n");
        string inputChoice = ReadLine()?.Trim() ?? "2"; // Default to file if no input

        if (inputChoice == "1")
        {
            // ******************** Translate speech from microphone ********************
            using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using TranslationRecognizer translator = new(translationConfig, audioConfig);

            WriteLine("Speak now...");
            result = await translator.RecognizeOnceAsync().ConfigureAwait(false);
            // ******************** Translate speech from microphone ********************
        }
        else
        {
            // ******************** Translate speech from file ********************
            string audioFilePath = @"D:\STSAAIMLDT\learn-ai102\src\Data\NLP\Speech\station.wav";

            // Platform-specific audio playback
            if (OperatingSystem.IsWindows())
            {
                WriteLine("Getting speech from file...");
                using SoundPlayer wavPlayer = new(audioFilePath);
                wavPlayer.Play();
            }
            else
            {
                WriteLine("Audio playback not supported on this platform. Continuing with translation...");
            }

            using AudioConfig audioConfig = AudioConfig.FromWavFileInput(audioFilePath);
            using TranslationRecognizer translator = new(translationConfig, audioConfig);

            result = await translator.RecognizeOnceAsync().ConfigureAwait(false);
            // ******************** Translate speech from file ********************
        }

        // Common processing for both input methods
        WriteLine($"Translating '{result.Text}'");
        translation = result.Translations[targetLanguage];
        OutputEncoding = Encoding.UTF8;
        WriteLine(translation);

        // Synthesize translation
        var languageVoiceMap = new Dictionary<string, string>
        {
            ["fr"] = "fr-FR-HenriNeural",
            ["es"] = "es-ES-ElviraNeural",
            ["hi"] = "hi-IN-MadhurNeural"
        };
        speechConfig.SpeechSynthesisVoiceName = languageVoiceMap[targetLanguage];

        using SpeechSynthesizer speechSynthesizer = new(speechConfig);

        SpeechSynthesisResult speak = await speechSynthesizer.SpeakTextAsync(translation).ConfigureAwait(false);
        if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
        {
            WriteLine(speak.Reason);
        }
    }
}
