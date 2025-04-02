from dotenv import load_dotenv
import os
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
        file_names = []
        # files = os.listdir(ads_folder)

        for file_name in os.listdir(ads_folder):
            file_path = os.path.join(ads_folder, file_name)
            if os.path.isfile(file_path):  # Ensure it's a file
                with open(file_path, encoding='utf8') as file:
                    batched_documents.append(file.read())
                    file_names.append(file_name)

        if not batched_documents:
            print("No documents found for processing.")
            return

        print(f"Loaded {len(batched_documents)} documents for processing.")

        # Extract entities
        print("Extracting entities...")
        try:
            operation = ai_client.begin_recognize_custom_entities(
                batched_documents,
                project_name=project_name,
                deployment_name=deployment_name,
            )

            document_results = operation.result()
        except Exception as api_error:
            print(f"Error calling Azure AI service: {api_error}")
            return

        # Process results
        for doc, custom_entities_result in zip(file_names, document_results):
            print(f"\nFile: {doc}")

            if custom_entities_result.kind == "CustomEntityRecognition":
                for entity in custom_entities_result.entities:
                    print(
                        f"\tEntity: '{entity.text}', "
                        f"Category: '{entity.category}', "
                        f"Confidence Score: {entity.confidence_score:.2f}"
                    )
            elif custom_entities_result.is_error:
                print(
                    f"\tError Code: {custom_entities_result.error.code}, "
                    f"Message: {custom_entities_result.error.message}"
                )

    except FileNotFoundError as fnf_error:
        print(fnf_error)
    except ValueError as val_error:
        print(val_error)
    except Exception as ex:
        print(f"Unexpected error: {ex}")


if __name__ == "__main__":
    main()
