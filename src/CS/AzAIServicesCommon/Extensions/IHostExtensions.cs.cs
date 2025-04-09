﻿using AzAIServicesCommon.Configuration;

namespace AzAIServicesCommon.Extensions;

public static class IHostExtensions
{
    public static IHost GetHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    IConfiguration configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddUserSecrets("12087404-AI102CodeSamples")
                        .Build();

                    AzAISvcAppConfiguration appConfig = new();
                    configuration.GetSection("AzAISvcAppConfiguration").Bind(appConfig);

                    services.AddSingleton(appConfig);

                    services.ConfigureServices();
                })
                .Build();
    }
}
