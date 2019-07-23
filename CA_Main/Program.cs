using System;
using System.Diagnostics;
using Serilog.Core;
using Serilog;
using Serilog.Formatting.Display;
using Serilog.Sinks.SystemConsole.Themes;

namespace CA_Main
{
    class Program
    {
        public static string appName = "MinicapReceiver.Core";

        static void Main(string[] args)
        {
            Logger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {Level} | {ThreadId} | {Method} >>>> {Message}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {                
                logger.Information(string.Format("Starting Application {0} ...", appName));

                Console.WriteLine("Hello World!");
            }
            catch(Exception ex)
            {
                logger.Fatal(ex, string.Format("An Error Occurred! Message: {0}", ex.Message));
            }
            finally
            {
                stopwatch.Stop();
                logger.Information(string.Format("Application {0} completed in {1}", appName, Utils.ElapsedTime(stopwatch.Elapsed)));
            }
        }
    }
}
