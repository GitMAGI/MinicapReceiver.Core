using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Core;
using Serilog;
using Serilog.Formatting.Display;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog.Events;
using System.Runtime.CompilerServices;
using Serilog.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace CA_Main
{
    public class Logger
    {
        private static Serilog.Core.Logger _instance;

        private Logger() { }
        
        private static void Initialize(){
            _instance = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                //.Enrich.WithScope()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.ffffff} | {Level:u} | {MachineName} | {ThreadId} | {Scope} >>> {Message}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();
        }

        public static Serilog.Core.Logger GetInstance()
        {
            if (_instance == null)
                Initialize();
            return (Serilog.Core.Logger) _instance.ForContext(new ScopeEnricher());
        }
    }

    public class ReleaseNumberEnricher : ILogEventEnricher
    {
        LogEventProperty _cachedProperty;

        public const string PropertyName = "ReleaseNumber";

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(GetLogEventProperty(propertyFactory));
        }

        private LogEventProperty GetLogEventProperty(ILogEventPropertyFactory propertyFactory)
        {
            // Don't care about thread-safety, in the worst case the field gets overwritten and one property will be GCed
            if (_cachedProperty == null)
                _cachedProperty = CreateProperty(propertyFactory);

            return _cachedProperty;
        }

        // Qualify as uncommon-path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static LogEventProperty CreateProperty(ILogEventPropertyFactory propertyFactory)
        {
            var value = Environment.GetEnvironmentVariable("RELEASE_NUMBER") ?? "local";
            return propertyFactory.CreateProperty(PropertyName, value);
        }
    }
   
    public class ScopeEnricher : ILogEventEnricher
    {
        LogEventProperty _cachedProperty;

        public const string PropertyName = "Scope";

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(GetLogEventProperty(propertyFactory));
        }

        private LogEventProperty GetLogEventProperty(ILogEventPropertyFactory propertyFactory)
        {
            // Don't care about thread-safety, in the worst case the field gets overwritten and one property will be GCed
            if (_cachedProperty == null)
                _cachedProperty = CreateProperty(propertyFactory);

            return _cachedProperty;
        }

        // Qualify as uncommon-path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static LogEventProperty CreateProperty(ILogEventPropertyFactory propertyFactory)
        {
            //StackTrace stackTrace = new StackTrace();
            //int stackLevel = stackTrace.FrameCount - 1;
            int stackLevel = 6;

            StackFrame frame = new StackFrame(stackLevel);
            MethodBase method = frame.GetMethod();

            string methodName = method != null ? method.Name : "unknown-method";
            string className = method != null && method.ReflectedType != null ? method.ReflectedType.Name : "unknown-class";
            string nameSpace = method.ReflectedType != null ? method.ReflectedType.Namespace : "unknown-namespace";
            string value = nameSpace + "." + className + "." + methodName;

            return propertyFactory.CreateProperty(PropertyName, value);
        }
    }

    public static class LoggingExtensions
    {
        public static LoggerConfiguration WithReleaseNumber(this LoggerEnrichmentConfiguration enrich)
        {
            if (enrich == null)
                throw new ArgumentNullException(nameof(enrich));

            return enrich.With<ReleaseNumberEnricher>();
        }

        public static LoggerConfiguration WithScope(this LoggerEnrichmentConfiguration enrich)
        {
            if (enrich == null)
                throw new ArgumentNullException(nameof(enrich));

            return enrich.With<ScopeEnricher>();
        }
    }
    
    // Example of configuration with appsetting.json
    //{
    //  "Serilog": 
    //  {
    //      "MinimumLevel": 
    //      {
    //          "Default": "Debug"
    //      },
    //      "WriteTo": 
    //      [
    //          {
    //              "Name": "Console"
    //          }
    //      ],
    //      "Using": [ "Example.Assembly.Name" ],
    //      "Enrich": [ "FromLogContext", "WithReleaseNumber", "WithScope" ]
    //  }
    //}
}