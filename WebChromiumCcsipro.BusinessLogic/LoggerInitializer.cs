using System;
using System.Globalization;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Destructurama;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Interfaces.IServices;
using Constants = WebChromiumCcsipro.Domain.Constants;
using LoggerExtensions = WebChromiumCcsipro.Domain.Extensions.LoggerExtensions;

namespace WebChromiumCcsipro.BusinessLogic
{
    public static class LoggerInitializer
    {
        public static readonly DateTime DateStart = DateTime.Now;
        public static readonly Guid ApplicationId = Guid.NewGuid();
        public static readonly string RoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.None);

        //        public static EmailConnectionInfo EmailConnectionInfo { get; private set; }
        public static string ApplicationName { get; set; }

        public static string Version { get; private set; }

        public static string VersionDeploy { get; private set; }
        public static ILogger InitializeApplicationLogger(ISettingsService settingsService, LoggingLevelSwitch loggingLevelSwitch, CultureInfo culture, CultureInfo uiCulture, Type applicationType)
        {
            if (string.IsNullOrWhiteSpace(ApplicationName))
            {
                throw new ArgumentNullException(nameof(ApplicationName));
            }
            if (string.IsNullOrWhiteSpace(ApplicationName))
            {
                throw new ArgumentNullException(nameof(Version));
            }
            //            VersionDeploy = applicationDeployment != null ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : Constants.NotAvailable;
            var logPathFoldere = Path.Combine(RoamingPath, "CCSIPRO", ApplicationName);

            if (!Directory.Exists(logPathFoldere))
            {
                Directory.CreateDirectory(logPathFoldere);
            }
            var logPath = Path.Combine(logPathFoldere, ApplicationName + "-{Date}.log");


            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ApplicationId", ApplicationId)
                .Enrich.WithProperty("Version", Version)
                .Enrich.WithProperty("VersionDeploy", VersionDeploy)
                .WriteTo.RollingFile(logPath, LogEventLevel.Verbose, retainedFileCountLimit: 60, outputTemplate: Constants.FileLogFormat)
                //                .WriteTo.Sink(new RollingFileSink(logPath + ".json", new JsonFormatter(false, null, true), 500 * 1024 * 1024, 5, Encoding.UTF8), LogEventLevel.Debug)
                //                .WriteTo.Sink(new StringSink(), LogEventLevel.Error)
                //                .WriteTo.Email(EmailConnectionInfo, Constants.EmailLogFormat, LogEventLevel.Fatal)
                .WriteTo.Console()
                .Destructure.UsingAttributes()
                .Destructure.AsScalar<CultureInfo>()
                .CreateLogger();
            LoggerExtensions.Information(Log.Logger.ForContext(applicationType), ApplicationEvents.ApplicationStarted,
                "Application started at {DateTime}, Version {Version:l}, Deploy {VersionDeploy:l}, Logging level: {level}, CurrentCulture: {CurrentCulture}, CurrentUICulture: {CurrentUICulture}",
                DateStart, Version, VersionDeploy, loggingLevelSwitch.MinimumLevel, culture, uiCulture);
            return Log.Logger;
        }
    }


}
