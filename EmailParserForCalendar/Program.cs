using System;
using EmailParserForCalendar.EmailProcessing;
using EmailParserForCalendar.Google;
using EmailParserForCalendar.Job;
using FluentScheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EmailParserForCalendar
{
   class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json

        static void Main(string[] args)
        {
            try
            {
                ILogger logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.LiterateConsole()
                    .WriteTo.RollingFile("logs/log.txt")
                    .CreateLogger();
                
                Settings settings = LoadSettings(args);
                
                logger.Verbose("{@settings}", settings);
                
                //setup our DI
                var serviceProvider = new ServiceCollection()
                    .AddSingleton<EmailProcessorFactory>()
                    .AddSingleton(sp => settings)
                    .AddSingleton(GoogleServiceFactory.CreateGmailService())
                    .AddSingleton<GmailClient>()
                    .AddSingleton<CalendarClient>()
                    .AddSingleton(GoogleServiceFactory.CreateGmailService())
                    .AddSingleton(GoogleServiceFactory.CreateCalendarService())
                    .AddSingleton(logger)
                    .AddSingleton(sp=> new CalendarSyncJob(
                        sp.GetService<GmailClient>(), 
                        sp.GetService<EmailProcessorFactory>(),
                        sp.GetService<ILogger>()))
                    .BuildServiceProvider();
               
                JobManager.JobFactory = new JobFactory(serviceProvider);
                JobManager.JobException += (log) => logger.Error(log.Exception, "{message} \n\n\n {stackTrace}",
                    log.Exception.Message, log.Exception.StackTrace);
                
                JobManager.Initialize(new JobRegistry(settings.FetchIntervalInHours));                 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{ex.Message}\n\n{ex.StackTrace}");
                throw;
            }
            
            Console.ReadLine();
        }

        private static Settings LoadSettings(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("configuration.json", optional: true, reloadOnChange: false)
                .AddCommandLine(args)
                .Build();

            Settings settings = new Settings();
            config.Bind(settings);
            return settings;
        }
    }
}