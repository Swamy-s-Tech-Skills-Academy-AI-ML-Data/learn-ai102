using AzAIServicesCommon.Configuration;
using AzAIServicesCommon.Extensions;
using GenAIAzOpenAIServices.Services;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

await new AzureOpenAIChatCompletion(appConfig).ShowChatCompletionDemowithAzAOI().ConfigureAwait(false);

WriteLine("\n\nPress any key to exit...");
ReadKey();