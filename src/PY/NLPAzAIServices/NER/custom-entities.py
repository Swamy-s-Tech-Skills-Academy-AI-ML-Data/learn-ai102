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
            raise ValueError(
                "One or more required environment variables are missing.")

        # Create client using endpoint and key
        credential = AzureKeyCredential(ai_key)
        ai_client = TextAnalyticsClient(
            endpoint=ai_endpoint, credential=credential)

        ads_folder = r"D:\STSAAIMLDT\learn-ai102\src\Data\NLP\NER\ads"

        # Ensure the folder exists
        if not os.path.exists(ads_folder):
            raise FileNotFoundError(f"Ads folder not found: {ads_folder}")

        # Read each text file in the ads folder
        print("Reading files from ads folder...")
        batched_documents = []
        files = os.listdir(ads_folder)

        for file_name in files:
            file_path = os.path.join(ads_folder, file_name)
            if os.path.isfile(file_path):  # Ensure it's a file
                with open(file_path, encoding='utf8') as file:
                    batched_documents.append(file.read())

        if batched_documents:
            print(f"Loaded {len(batched_documents)} documents for processing.")

        # Extract entities
        operation = ai_client.begin_recognize_custom_entities(
            batched_documents,
            project_name=project_name,
            deployment_name=deployment_name
        )

        document_results = operation.result()

        for doc, custom_entities_result in zip(files, document_results):
            print(doc)
            if custom_entities_result.kind == "CustomEntityRecognition":
                for entity in custom_entities_result.entities:
                    print(
                        "\tEntity '{}' has category '{}' with confidence score of '{}'".format(
                            entity.text, entity.category, entity.confidence_score
                        )
                    )
            elif custom_entities_result.is_error is True:
                print("\tError with code '{}' and message '{}'".format(
                    custom_entities_result.error.code, custom_entities_result.error.message
                    )
                )

    except Exception as ex:
        print(ex)


if __name__ == "__main__":
    main()
