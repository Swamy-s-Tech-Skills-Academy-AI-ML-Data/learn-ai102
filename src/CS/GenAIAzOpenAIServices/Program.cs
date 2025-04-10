using AzAIServicesCommon.Configuration;
using AzAIServicesCommon.Extensions;
using Azure.AI.OpenAI;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;
using System.ClientModel;

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();

string? oaiEndpoint;
string? oaiKey;
string? oaiDeploymentName;

if (string.IsNullOrEmpty(appConfig?.GenAIAzOpenAIServices?.AzureOpenAIChatService?.AzureOAIEndpoint)
                || string.IsNullOrEmpty(appConfig?.GenAIAzOpenAIServices?.AzureOpenAIChatService?.AzureOAIKey)
                || string.IsNullOrEmpty(appConfig?.GenAIAzOpenAIServices?.AzureOpenAIChatService?.AzureOAIDeploymentName))
{
    ForegroundColor = ConsoleColor.Red;
    WriteLine("Please check your appsettings.json file for missing or incorrect values.");

    return;
}

oaiEndpoint = appConfig?.GenAIAzOpenAIServices?.AzureOpenAIChatService?.AzureOAIEndpoint;
oaiKey = appConfig?.GenAIAzOpenAIServices?.AzureOpenAIChatService?.AzureOAIKey;
oaiDeploymentName = appConfig?.GenAIAzOpenAIServices?.AzureOpenAIChatService?.AzureOAIDeploymentName;

// Configure the Azure OpenAI client
AzureOpenAIClient azureClient = new(new Uri(oaiEndpoint!), new ApiKeyCredential(oaiKey!));
ChatClient chatClient = azureClient.GetChatClient(oaiDeploymentName);

do
{
    // Pause for system message update
    ForegroundColor = ConsoleColor.White;
    WriteLine("-----------\nPausing the app to allow you to change the system prompt.\n\nPress any key to continue...");
    ReadKey();

    WriteLine("\nUsing system message from system.txt");
    var folderPath = Path.GetFullPath(@"D:\STSAAIMLDT\learn-ai102\src\Data\GenAI\AOIChat\appdev");
    var systemFilePath = Path.Combine(folderPath, "system.txt");
    string systemMessage = File.ReadAllText(systemFilePath).Trim();

    ForegroundColor = ConsoleColor.DarkGreen;
    WriteLine("\nEnter user message or type 'quit' to exit:");
    string userMessage = ReadLine() ?? "";
    userMessage = userMessage.Trim();
    ResetColor();

    if (systemMessage.ToLower() == "quit" || userMessage.ToLower() == "quit")
    {
        ResetColor();
        break;
    }
    else if (string.IsNullOrEmpty(systemMessage) || string.IsNullOrEmpty(userMessage))
    {
        WriteLine("Please enter a system and user message.");
        continue;
    }
    else
    {
        // Format and send the request to the model

        GetResponseFromOpenAI(systemMessage, userMessage);
    }
} while (true);

void GetResponseFromOpenAI(string systemMessage, string userMessage)
{
    WriteLine("\nSending prompt to Azure OpenAI endpoint...\n\n");

    if (string.IsNullOrEmpty(oaiEndpoint) || string.IsNullOrEmpty(oaiKey) || string.IsNullOrEmpty(oaiDeploymentName))
    {
        WriteLine("Please check your appsettings.json file for missing or incorrect values.");
        return;
    }

    // Get response from Azure OpenAI
    ChatCompletionOptions chatCompletionOptions = new ChatCompletionOptions()
    {
        Temperature = 0.7f,
        MaxOutputTokenCount = 800
    };

    ChatCompletion completion = chatClient.CompleteChat(
        [
            new SystemChatMessage(systemMessage),
         new UserChatMessage(userMessage)
        ],
        chatCompletionOptions
    );

    ForegroundColor = ConsoleColor.DarkYellow;
    WriteLine($"{completion.Role}: {completion.Content[0].Text}");
    ResetColor();

}

ResetColor();

WriteLine("\n\nPress any key to exit...");
ReadKey();