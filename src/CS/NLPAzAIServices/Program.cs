using AzAIServicesCommon.Configuration;
using AzAIServicesCommon.Extensions;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLPAzAIServices.Services;
using System.Text;

#pragma warning disable CA1303

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();

// Set console output to encoding
OutputEncoding = Encoding.Unicode;

// Optional: Set input encoding if reading Unicode input
InputEncoding = Encoding.Unicode;

// ******************** 5. Named Entity Recognition ********************
await NERWithAzureAIService.ShowNERDemoWithAzureAIService(appConfig).ConfigureAwait(false);
// ******************** 5. Named Entity Recognition ********************

// ******************** 8. Translate Speech ********************
await TranslateSpeechWithAzureAIService.ShowTranslateSpeechDemoWithAzureAIService(appConfig).ConfigureAwait(false);
// ******************** 8. Translate Speech ********************

WriteLine("\n\nPress any key to exit...");
ReadKey();