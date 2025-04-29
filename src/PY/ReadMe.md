# AI-102 Azure AI Services Python Examples

This repository contains Python sample code demonstrating various Azure AI Services for the AI-102 certification learning path.

## Project Structure

- `/GenAIAzOpenAIServices` - Azure OpenAI service examples
  - `/AZAOIChat` - Azure OpenAI Chat completion examples

- `/NLPAzAIServices` - Natural Language Processing Azure AI Services examples
  - `/NER` - Named Entity Recognition with Azure AI Language service
  - `/Speech` - Speech-related services with Azure AI Speech service

## Prerequisites

- Python 3.8 or later
- Azure subscription with access to:
  - Azure AI Language service (for NER)
  - Azure AI Speech service (for Speech translation)
  - Azure OpenAI service (for chat completion)

## Setup

1. Clone this repository
2. Install the required packages:
   ```
   pip install -r requirements.txt
   ```
3. Create a `.env` file based on `example.env` and update with your Azure service credentials:
   ```
   # For Named Entity Recognition
   NER_SERVICE_ENDPOINT=https://your-language-resource.cognitiveservices.azure.com/
   NER_SERVICE_KEY=your_language_resource_key
   NER_PROJECT=CustomEntityLab
   NER_DEPLOYMENT=AdEntities
   
   # For Speech Translation
   SPEECH_KEY=your_speech_resource_key
   SPEECH_REGION=your_speech_resource_region
   ```

## Sample Applications

### Named Entity Recognition (NER)

The `custom-entities.py` script demonstrates how to:
- Connect to Azure AI Language service
- Use a custom NER model to extract entities from text documents
- Process multiple files in batch
- Display extracted entities with their categories and confidence scores

Example usage:
```
python NLPAzAIServices/NER/custom-entities.py
```

### Speech Translation

The `translator.py` script demonstrates how to:
- Connect to Azure AI Speech service
- Translate spoken English to multiple languages (French, Spanish, Hindi)
- Process audio files for translation
- Convert translated text to speech in the target language

Example usage:
```
python NLPAzAIServices/Speech/translator/translator.py
```

## Dependencies

This project relies on the following Azure SDK packages:
- `azure-ai-textanalytics` - For text analysis and NER
- `azure-cognitiveservices-speech` - For speech translation and synthesis
- `python-dotenv` - For environment variable management
- `playsound` - For audio playback

## Troubleshooting

- If you encounter issues with audio playback using the `playsound` package, try installing version 1.2.2:
  ```
  pip install playsound==1.2.2
  ```

- For more complex audio needs, consider using `pygame` instead:
  ```
  pip install pygame
  ```

## Additional Resources

- [Azure AI Services Documentation](https://learn.microsoft.com/azure/ai-services/)
- [Azure AI Language Documentation](https://learn.microsoft.com/azure/ai-services/language-service/)
- [Azure AI Speech Documentation](https://learn.microsoft.com/azure/ai-services/speech-service/)
- [Azure OpenAI Service Documentation](https://learn.microsoft.com/azure/ai-services/openai/)