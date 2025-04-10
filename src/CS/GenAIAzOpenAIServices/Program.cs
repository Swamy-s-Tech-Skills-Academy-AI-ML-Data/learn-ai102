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

//Initialize messages list

do
{
    // Pause for system message update
    Console.WriteLine("-----------\nPausing the app to allow you to change the system prompt.\nPress any key to continue...");
    Console.ReadKey();

    Console.WriteLine("\nUsing system message from system.txt");
    string systemMessage = System.IO.File.ReadAllText("system.txt");
    systemMessage = systemMessage.Trim();

    Console.WriteLine("\nEnter user message or type 'quit' to exit:");
    string userMessage = Console.ReadLine() ?? "";
    userMessage = userMessage.Trim();

    if (systemMessage.ToLower() == "quit" || userMessage.ToLower() == "quit")
    {
        break;
    }
    else if (string.IsNullOrEmpty(systemMessage) || string.IsNullOrEmpty(userMessage))
    {
        Console.WriteLine("Please enter a system and user message.");
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
    Console.WriteLine("\nSending prompt to Azure OpenAI endpoint...\n\n");

    if (string.IsNullOrEmpty(oaiEndpoint) || string.IsNullOrEmpty(oaiKey) || string.IsNullOrEmpty(oaiDeploymentName))
    {
        Console.WriteLine("Please check your appsettings.json file for missing or incorrect values.");
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

    Console.WriteLine($"{completion.Role}: {completion.Content[0].Text}");

}


WriteLine("\n\nPress any key to exit...");
ReadKey();