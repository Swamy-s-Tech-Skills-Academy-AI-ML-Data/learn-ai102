using AzAIServicesCommon.Configuration;
using AzAIServicesCommon.Extensions;
using Azure.AI.OpenAI;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;
using System.ClientModel;
using System.Text;

#pragma warning disable CA1303

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();

// Set console output to UTF-8
Console.OutputEncoding = Encoding.UTF8;

// Optional: Set input encoding if reading Unicode input
Console.InputEncoding = Encoding.UTF8;

string? oaiEndpoint;
string? oaiKey;
string? oaiDeploymentName;
bool sendGroundingContext = false;
ChatCompletionOptions chatCompletionOptions = new()
{
    Temperature = 0.7f,
    MaxOutputTokenCount = 800
};

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

var folderPath = Path.GetFullPath(@"D:\STSAAIMLDT\learn-ai102\src\Data\GenAI\AOIChat\appdev");

do
{
    // Pause for system message update
    ForegroundColor = ConsoleColor.White;
    WriteLine("-----------\nPausing the app to allow you to change the system prompt.\n\nPress any key to continue...");
    ReadKey();

    WriteLine("\nUsing system message from system.txt");
    var systemFilePath = Path.Combine(folderPath, "system.txt");
    string systemMessage = await File.ReadAllTextAsync(systemFilePath).ConfigureAwait(false);
    systemMessage = systemMessage.Trim();

    ForegroundColor = ConsoleColor.DarkGreen;
    WriteLine("\nEnter user message or type 'quit' to exit:");
    ForegroundColor = ConsoleColor.Green;
    string userMessage = ReadLine() ?? "";
    userMessage = userMessage.Trim();
    ResetColor();

    if (systemMessage.Equals("quit", StringComparison.OrdinalIgnoreCase) || userMessage.Equals("quit", StringComparison.OrdinalIgnoreCase))
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
        if (sendGroundingContext)
        {
            // Initialize messages list
            WriteLine("\nAdding grounding context from grounding.txt");
            var groundingFilePath = Path.Combine(folderPath, "grounding.txt");
            string groundingText = await File.ReadAllTextAsync(groundingFilePath).ConfigureAwait(false);
            groundingText = groundingText.Trim();

            // Format and send the request to the model
            var messagesList = new List<ChatMessage>
            {
                new UserChatMessage(groundingText),
                new SystemChatMessage(systemMessage),
                new UserChatMessage(userMessage)
            };

            await GetResponseFromOpenAIV1(chatClient, chatCompletionOptions, messagesList).ConfigureAwait(false);
        }

        await GetResponseFromOpenAI(chatClient, chatCompletionOptions, systemMessage, userMessage).ConfigureAwait(false);
    }
} while (true);

async Task GetResponseFromOpenAIV1(ChatClient chatClient, ChatCompletionOptions chatCompletionOptions, List<ChatMessage> messagesList)
{
    ForegroundColor = ConsoleColor.DarkYellow;
    WriteLine("\nSending prompt to Azure OpenAI endpoint WITH Grounding Context...\n\n");
    ResetColor();

    // Get response from Azure OpenAI
    ChatCompletion completion = await chatClient.CompleteChatAsync(
        messagesList,
        chatCompletionOptions
    ).ConfigureAwait(false);

    ForegroundColor = ConsoleColor.Yellow;
    WriteLine($"{completion.Role}: {completion.Content[0].Text}");
    messagesList.Add(new AssistantChatMessage(completion.Content[0].Text));
}

async Task GetResponseFromOpenAI(ChatClient chatClient, ChatCompletionOptions chatCompletionOptions, string systemMessage, string userMessage)
{
    ForegroundColor = ConsoleColor.DarkCyan;
    WriteLine("\nSending prompt to Azure OpenAI endpoint WITHOUT Grounding Context ...\n\n");
    ResetColor();

    // Get response from Azure OpenAI
    ChatCompletion completion = await chatClient.CompleteChatAsync(
        [
            new SystemChatMessage(systemMessage),
            new UserChatMessage(userMessage)
        ],
        chatCompletionOptions
    ).ConfigureAwait(false);

    ForegroundColor = ConsoleColor.Cyan;
    WriteLine($"{completion.Role}: {completion.Content[0].Text}\n\n");
    ResetColor();
}

WriteLine("\n\nPress any key to exit...");
ReadKey();