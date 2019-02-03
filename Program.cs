using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Settings.Configuration;
using StreamDeckLib;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Fritz.SmartHome
{
	    class Program
    {
        static ILoggerFactory GetLoggerFactory()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(dir);

            var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

            var loggerFactory = new LoggerFactory()
                .AddSerilog(Log.Logger);

            TopLogger = loggerFactory.CreateLogger("top");

            TopLogger.LogInformation("Plugin started");

            return loggerFactory;
        }

        private static Microsoft.Extensions.Logging.ILogger TopLogger;

        static async Task Main(string[] args)
        {


            using (var source = new CancellationTokenSource())
            {
                using (var loggerFactory = GetLoggerFactory())
                {
                    try
                    {

                        await ConnectionManager.Initialize(args, loggerFactory)
                            .SetPlugin(new WemoPlugin() { Logger = loggerFactory.CreateLogger("WeMo") })
                            .StartAsync(source.Token);

                        //Console.ReadLine();

                    }
                    catch (Exception ex)
                    {
                        TopLogger.LogError(ex, "Error while running the plugin");
                    }
                    source.Cancel();
                }

            }
        }
    }
}
