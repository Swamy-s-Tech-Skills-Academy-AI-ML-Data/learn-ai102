from dotenv import load_dotenv
from datetime import datetime
import os
import sys
# Import namespaces
import azure.cognitiveservices.speech as speech_sdk
from playsound import playsound


def main():
    try:
        global speech_config
        global translation_config

        # Print header
        print("\033[96m🎤 Speech Translation with Azure AI Speech Service 🌍\033[0m")

        # Specify the path to the .env file
        env_path = r"D:\DataStores\Envs\.env"

        # Load the .env file from the specified path
        load_dotenv(dotenv_path=env_path)

        # Get Configuration Settings
        load_dotenv()

        ai_key = os.getenv('SPEECH_KEY')
        ai_region = os.getenv('SPEECH_REGION')

        # Validate configuration
        if not ai_key or not ai_region:
            print(
                "\033[91mPlease check your .env file for missing SPEECH_KEY or SPEECH_REGION values.\033[0m")
            return

        # Configure translation
        translation_config = speech_sdk.translation.SpeechTranslationConfig(
            subscription=ai_key, region=ai_region)
        translation_config.speech_recognition_language = 'en-US'
        translation_config.add_target_language('fr')
        translation_config.add_target_language('es')
        translation_config.add_target_language('hi')

        print('\033[94mReady to translate from',
              translation_config.speech_recognition_language, '\033[0m')

        # Configure speech
        speech_config = speech_sdk.SpeechConfig(
            subscription=ai_key, region=ai_region)

        # Get user input
        targetLanguage = ''
        while targetLanguage != 'quit':
            print("\n\033[95mEnter a target language\033[0m")
            print("\033[97m fr = French\n es = Spanish\n hi = Hindi\033[0m")
            print("\033[90m Enter anything else to stop\n\033[0m")

            targetLanguage = input().lower()
            if targetLanguage in translation_config.target_languages:
                Translate(targetLanguage)
            else:
                targetLanguage = 'quit'

    except Exception as ex:
        print("\033[91mError:", ex, "\033[0m")


def Translate(targetLanguage):
    translation = ''

    try:
        # Ask the user for input method choice
        print("\n\033[92mSelect input source:\033[0m")
        print("\033[97m 1 = Microphone\n 2 = Audio file\n\033[0m")

        inputChoice = input().strip() or "2"  # Default to file if no input

        if inputChoice == "1":
            # Translate speech using the default microphone
            audio_config = speech_sdk.AudioConfig(use_default_microphone=True)
            translator = speech_sdk.translation.TranslationRecognizer(
                translation_config, audio_config=audio_config)

            print("\033[93m🎙️ Speak now...\033[0m")
            result = translator.recognize_once_async().get()
        else:
            # Translate speech from file
            print("\033[96mGetting speech from file...\033[0m")
            print("\033[93m🔊 Playing audio file...\033[0m")
            audioFile = r'D:\STSAAIMLDT\learn-ai102\src\Data\NLP\Speech\station.wav'
            try:
                playsound(audioFile)
            except Exception as ex:
                print("\033[93mAudio playback issue:", ex,
                      "\n  Continuing with translation...\033[0m")

            audio_config = speech_sdk.AudioConfig(filename=audioFile)
            translator = speech_sdk.translation.TranslationRecognizer(
                translation_config, audio_config=audio_config)
            result = translator.recognize_once_async().get()

        # Process translation results
        print(f"\033[94m📝 Original text: '{result.text}'\033[0m")
        translation = result.translations[targetLanguage]
        print(
            f"\033[92m🌐 Translation ({targetLanguage}): {translation}\033[0m")

        # Synthesize translation
        voices = {
            "fr": "fr-FR-HenriNeural",
            "es": "es-ES-ElviraNeural",
            "hi": "hi-IN-MadhurNeural"
        }

        print(
            f"\033[95m🔊 Playing synthesized speech in {voices.get(targetLanguage)}...\033[0m")
        speech_config.speech_synthesis_voice_name = voices.get(targetLanguage)
        speech_synthesizer = speech_sdk.SpeechSynthesizer(speech_config)
        speak = speech_synthesizer.speak_text_async(translation).get()

        if speak.reason != speech_sdk.ResultReason.SynthesizingAudioCompleted:
            print(f"\033[91mSpeech synthesis error: {speak.reason}\033[0m")

    except Exception as ex:
        print(f"\033[91mError during translation: {ex}\033[0m")


if __name__ == "__main__":
    main()
