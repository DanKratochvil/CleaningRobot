using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;


namespace CleaningRobot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage>");
                Console.WriteLine("CleaningRobot <source.json> <result.json>");
                return;
            }

            string logFileName = Debugger.IsAttached ? "..\\..\\..\\logs\\app.log" : "logs/app.log";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(logFileName, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var host = Host.CreateDefaultBuilder()
                .UseSerilog()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<CleaningService>(provider =>
                    {
                        var logger = provider.GetRequiredService<ILogger<CleaningService>>();
                        var cleaningInfo = CleaningSerializer.DeserializeInput(args[0]);
                        return new CleaningService(cleaningInfo, logger);
                    });
                })
                .Build();

            try
            {                        
                var service = host.Services.GetRequiredService<CleaningService>();
                var cleaningResult = service.CleanRoom();
                CleaningSerializer.SerializeOutput(cleaningResult, args[1]);
            }
            catch (Exception ex)
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex.Message);
            }

        }

    }
}