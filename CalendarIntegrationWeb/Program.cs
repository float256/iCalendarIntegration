using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.ServiceFabric.Services.Runtime;
using NLog;
using NLog.Web;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace CalendarIntegrationWeb
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main(string[] args)
        {
            string appWorkingMode = Environment.GetEnvironmentVariable("APP_WORKING_MODE");
            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            IConfigurationRoot config;
            if (File.Exists("appsettings.prod.json"))
            {
                config = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.{envName}.json")
                    .AddJsonFile($"appsettings.prod.json")
                    .Build();
            }
            else
            {
                config = new ConfigurationBuilder().AddJsonFile(path: $"appsettings.{envName}.json").Build();
            }

            NLog.Extensions.Logging.ConfigSettingLayoutRenderer.DefaultConfiguration = config;
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Starting application");
                if (appWorkingMode == "Console")
                {
                    StartWebHost(args);
                }
                else
                {
                    StartService();
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        private static void StartService()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("CalendarIntegrationWebType",
                    context => new CalendarIntegrationWeb(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(CalendarIntegrationWeb).Name);

                // Prevents this host process from terminating so services keeps running. 
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static void StartWebHost(string[] args)
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logger =>
                {
                    logger.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
            hostBuilder.Build().Run();
        }
    }
}
