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

        try
        {
            if (string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.SpeechAIService?.Endpoint) ||
                string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.SpeechAIService?.Key) ||
                string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.SpeechAIService?.Region))
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Please check your appsettings.json file for missing or incorrect values.");
                return;
            }

            // Get config settings from AppSettings
            string azureServiceKey = appConfig?.NLPAzAIServices?.SpeechAIService?.Key!;
            string azureServiceRegion = appConfig?.NLPAzAIServices?.SpeechAIService?.Region!;

            ForegroundColor = ConsoleColor.Cyan;
            WriteLine("🎤 Speech Translation with Azure AI Speech Service 🌍");
            ResetColor();

            // Configure translation
            translationConfig = SpeechTranslationConfig.FromSubscription(azureServiceKey, azureServiceRegion);
            translationConfig.SpeechRecognitionLanguage = "en-US";
            translationConfig.AddTargetLanguage("fr");
            translationConfig.AddTargetLanguage("es");
            translationConfig.AddTargetLanguage("hi");
            
            ForegroundColor = ConsoleColor.Blue;
            WriteLine($"Ready to translate from {translationConfig.SpeechRecognitionLanguage}");
            ResetColor();

            // Configure speech
            speechConfig = SpeechConfig.FromSubscription(azureServiceKey, azureServiceRegion);

            string targetLanguage = "";
            while (targetLanguage != "quit")
            {
                ForegroundColor = ConsoleColor.Magenta;
                WriteLine("\nEnter a target language");
                ForegroundColor = ConsoleColor.White;
                WriteLine(" fr = French\n es = Spanish\n hi = Hindi");
                ForegroundColor = ConsoleColor.DarkGray;
                WriteLine(" Enter anything else to stop\n");
                ResetColor();
                
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
            ForegroundColor = ConsoleColor.Red;
            WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            ResetColor();
        }
    }

    private static async Task Translate(string targetLanguage, SpeechConfig speechConfig, SpeechTranslationConfig translationConfig)
    {
        string translation = "";
        TranslationRecognitionResult result;

        try
        {
            // Ask the user for input method choice
            ForegroundColor = ConsoleColor.Green;
            WriteLine("\nSelect input source:");
            ForegroundColor = ConsoleColor.White;
            WriteLine(" 1 = Microphone\n 2 = Audio file\n");
            ResetColor();
            
            string inputChoice = ReadLine()?.Trim() ?? "2"; // Default to file if no input

            if (inputChoice == "1")
            {
                // ******************** Translate speech from microphone ********************
                using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                using TranslationRecognizer translator = new(translationConfig, audioConfig);

                ForegroundColor = ConsoleColor.Yellow;
                WriteLine("🎙️ Speak now...");
                ResetColor();
                
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
                    ForegroundColor = ConsoleColor.Yellow;
                    WriteLine("🔊 Playing audio file...");
                    ResetColor();
                    
                    using SoundPlayer wavPlayer = new(audioFilePath);
                    wavPlayer.Play();
                }
                else
                {
                    ForegroundColor = ConsoleColor.DarkYellow;
                    WriteLine("Audio playback not supported on this platform. Continuing with translation...");
                    ResetColor();
                }

                using AudioConfig audioConfig = AudioConfig.FromWavFileInput(audioFilePath);
                using TranslationRecognizer translator = new(translationConfig, audioConfig);

                ForegroundColor = ConsoleColor.Cyan;
                WriteLine("Getting speech from file...");
                ResetColor();
                
                result = await translator.RecognizeOnceAsync().ConfigureAwait(false);
                // ******************** Translate speech from file ********************
            }

            // Common processing for both input methods
            ForegroundColor = ConsoleColor.Blue;
            WriteLine($"📝 Original text: '{result.Text}'");
            
            translation = result.Translations[targetLanguage];
            OutputEncoding = Encoding.UTF8;
            
            ForegroundColor = ConsoleColor.Green;
            WriteLine($"🌐 Translation ({targetLanguage}): {translation}");
            ResetColor();

            // Synthesize translation
            var languageVoiceMap = new Dictionary<string, string>
            {
                ["fr"] = "fr-FR-HenriNeural",
                ["es"] = "es-ES-ElviraNeural",
                ["hi"] = "hi-IN-MadhurNeural"
            };
            speechConfig.SpeechSynthesisVoiceName = languageVoiceMap[targetLanguage];

            ForegroundColor = ConsoleColor.Magenta;
            WriteLine($"🔊 Playing synthesized speech in {languageVoiceMap[targetLanguage]}...");
            ResetColor();
            
            using SpeechSynthesizer speechSynthesizer = new(speechConfig);

            SpeechSynthesisResult speak = await speechSynthesizer.SpeakTextAsync(translation).ConfigureAwait(false);
            if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine($"Speech synthesis error: {speak.Reason}");
                ResetColor();
            }
        }
        catch (Exception ex)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine($"Error during translation: {ex.Message}");
        }
        finally
        {
            ResetColor();
        }
    }
}
