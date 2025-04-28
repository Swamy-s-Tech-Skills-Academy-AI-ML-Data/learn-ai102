from dotenv import load_dotenv
from datetime import datetime
import os

# Import namespaces


def main():
    try:
        global speech_config
        global translation_config

        # Specify the path to the .env file
        env_path = r"D:\DataStores\Envs\.env"

        # Load the .env file from the specified path
        load_dotenv(dotenv_path=env_path)

        # Get Configuration Settings
        load_dotenv()

        ai_key = os.getenv('SPEECH_KEY')
        ai_region = os.getenv('SPEECH_REGION')

        # Configure translation


        # Configure speech


        # Get user input
        targetLanguage = ''
        while targetLanguage != 'quit':
            targetLanguage = input('\nEnter a target language\n fr = French\n es = Spanish\n hi = Hindi\n Enter anything else to stop\n').lower()
            if targetLanguage in translation_config.target_languages:
                Translate(targetLanguage)
            else:
                targetLanguage = 'quit'
                

    except Exception as ex:
        print(ex)

def Translate(targetLanguage):
    translation = ''

    # Translate speech


    # Synthesize translation



if __name__ == "__main__":
    main()