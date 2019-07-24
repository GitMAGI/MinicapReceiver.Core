using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Core;
using Serilog;
using Serilog.Formatting.Display;
using Serilog.Sinks.SystemConsole.Themes;

namespace CA_Main
{
    public class Logger
    {
        private static Serilog.Core.Logger _instance;

        private Logger() { }
        
        private static void Initialize(){
            _instance = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {Level:u} | {MachineName} | {ThreadId} | {SourceContext:1} >>> {Message}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();
        }

        public static Serilog.Core.Logger GetInstance()
        {
            if (_instance == null)
                Initialize();
            return _instance;
        }
    }
}
