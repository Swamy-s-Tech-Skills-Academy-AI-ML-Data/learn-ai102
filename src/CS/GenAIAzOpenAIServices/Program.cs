// Implicit using statements are included
using AzAIServicesCommon.Configuration;
using AzAIServicesCommon.Extensions;
using Azure.AI.OpenAI;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;
using System.ClientModel;
using System.Reflection.PortableExecutable;

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();


// Build a config object and retrieve user settings.

static string? oaiEndpoint;
static string? oaiKey;
static string? oaiDeploymentName;
if (string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.NERAIService?.Endpoint)
        || string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.NERAIService?.Key)
        || string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.NERAIService?.ProjectName)
        || string.IsNullOrEmpty(appConfig?.NLPAzAIServices?.NERAIService?.DeploymentName))
{
    ForegroundColor = ConsoleColor.Red;
    WriteLine("Please check your appsettings.json file for missing or incorrect values.");

    return;
}

oaiEndpoint = config["AzureOAIEndpoint"];
oaiKey = config["AzureOAIKey"];
oaiDeploymentName = config["AzureOAIDeploymentName"];

// Configure the Azure OpenAI client
AzureOpenAIClient azureClient = new(new Uri(oaiEndpoint), new ApiKeyCredential(oaiKey));
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


// Define the function that gets the response from Azure OpenAI endpoint
void GetResponseFromOpenAI(string systemMessage, string userMessage)
{
    Console.WriteLine("\nSending prompt to Azure OpenAI endpoint...\n\n");

    if (string.IsNullOrEmpty(oaiEndpoint) || string.IsNullOrEmpty(oaiKey) || string.IsNullOrEmpty(oaiDeploymentName))
    {
        Console.WriteLine("Please check your appsettings.json file for missing or incorrect values.");
        return;
    }

    // Configure the Azure OpenAI client



    // Get response from Azure OpenAI




}

