﻿using AzAIServicesCommon.Configuration;
using AzAIServicesCommon.Extensions;
using NLPAzAIServices.Services;

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();


await NERWithAzureAIService.ShowNERDemoWithAzureAIService(appConfig);

WriteLine("\n\nPress any key to exit...");
ReadKey();