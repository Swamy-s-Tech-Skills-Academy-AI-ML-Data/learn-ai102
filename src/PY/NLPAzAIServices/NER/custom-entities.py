from dotenv import load_dotenv
import os

# import namespaces
from azure.core.credentials import AzureKeyCredential
from azure.ai.textanalytics import TextAnalyticsClient


def main():
    try:
        # Get Configuration Settings
        load_dotenv()

        # Get configuration settings
        ai_endpoint = os.getenv('NER_SERVICE_ENDPOINT')
        ai_key = os.getenv('NER_SERVICE_KEY')
        project_name = os.getenv('NER_PROJECT')
        deployment_name = os.getenv('NER_DEPLOYMENT')

        # Validate environment variables
        if not all([ai_endpoint, ai_key, project_name, deployment_name]):
            raise ValueError("One or more required environment variables are missing.")
        
        # Create client using endpoint and key
        credential = AzureKeyCredential(ai_key)
        ai_client = TextAnalyticsClient(
            endpoint=ai_endpoint, credential=credential)

        ads_folder = r"D:\STSAAIMLDT\learn-ai102\src\Data\NLP\NER\ads"

        # Ensure the folder exists
        if not os.path.exists(ads_folder):
            raise FileNotFoundError(f"Ads folder not found: {ads_folder}")

        print("Reading files from ads folder...")
        # Read each text file in the ads folder
        batchedDocuments = []
        # ads_folder = 'ads'
        files = os.listdir(ads_folder)
        for file_name in files:
            # Read the file contents
            text = open(os.path.join(ads_folder, file_name),
                        encoding='utf8').read()
            batchedDocuments.append(text)

        # Extract entities

    except Exception as ex:
        print(ex)


if __name__ == "__main__":
    main()
