using System;
using FluentScheduler;

namespace EmailParserForCalendar
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json

        static void Main(string[] args)
        {
            var registry = new Registry();
            registry.Schedule<Job>().ToRunNow().AndEvery(1).Hours();
            JobManager.Initialize(registry);

//            ConsoleKeyInfo key;
//            do
//            {
//                key = Console.ReadKey();
//            } while (key.Key != ConsoleKey.Escape);

            Console.ReadLine();
        }
    }
}