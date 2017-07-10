using System;
using EmailParserForCalendar.Google;
using EmailParserForCalendar.Job;
using FluentScheduler;
using Google.Apis.Gmail.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
                Settings settings = LoadSettings(args);

                //setup our DI
                var serviceProvider = new ServiceCollection()
                    .AddSingleton(sp => settings)
                    .AddSingleton(GoogleServiceFactory.CreateGmailService())
                    .AddSingleton<GmailClient>()
                    .AddSingleton<CalendarClient>()
                    .AddSingleton(GoogleServiceFactory.CreateGmailService())
                    .AddSingleton(GoogleServiceFactory.CreateCalendarService())
                    .AddSingleton(sp=> new CalendarSyncJob(sp.GetService<GmailClient>()))
                    .BuildServiceProvider();
               
                JobManager.JobFactory = new JobFactory(serviceProvider);
                JobManager.JobException += (log) => Console.Error.WriteLine("Error: " +
                                                                            $" {log.Exception.Message}\n\n" +
                                                                            $"{log.Exception.StackTrace}");
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
            Console.WriteLine(JsonConvert.SerializeObject(settings, Formatting.Indented));
            return settings;
        }
    }
}