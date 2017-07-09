using System;
using System.Collections.Generic;
using EmailParserForCalendar.EventProcessors;
using EmailParserForCalendar.Exceptions;
using EmailParserForCalendar.Persistance;
using FluentScheduler;
using Google.Apis.Gmail.v1.Data;

namespace EmailParserForCalendar
{
    public class Job : IJob
    {
        public void Execute()
        {
            IEnumerable<Message> messages = GmailClient
                .GetRecentMessages(null,null,null);
           
            using (Database db = new Database())
            {
                foreach (var message in messages)
                {
                    ForwardedEmail email = null;

                    try
                    {
                        email = new ForwardedEmail(message);
                        
                        db.ForwardedEmails.Add(email);
                        db.SaveChanges();

                        CalendarEvent calendarEvent = new CalendarEvent(email);
                        email.Status = Constants.Parsed;
                        db.SaveChanges();

                        IEventProcessor eventProcessor = EventProcessorFactory.GetEventProcessor(email.Operation);

                        eventProcessor.Process(calendarEvent);
                        Console.WriteLine($"Successfully processed - {calendarEvent.Title} from {calendarEvent.From}");
                        email.Status = Constants.Processed;
                        db.SaveChanges();

                        db.CalendarEvents.Add(calendarEvent);
                        db.SaveChanges();
                    }
                    catch (ParsingFailedException)
                    {
                        email.Status = "Error - Subject Parsing failed";
                        Console.Error.WriteLine("An error occued while parsing subject: "+ email.Subject);
                    }
                    catch (RecordSkippedException ex)
                    {
                        email.Status = $"{Constants.Skipped}";
                        Console.WriteLine($"{Constants.Skipped}  - {ex.Message} - {email.Subject}");
                    }
                    catch (Exception ex)
                    {
                        if (email != null)
                        {
                            email.Status = $"{Constants.Error} - {ex.Message}";
                            Console.Error.WriteLine($"{Constants.Error} - {ex.Message} - {email.Subject}");
                        }
                        else
                        {
                            Console.Error.WriteLine($"Error: Could not create an instance of type: {typeof(ForwardedEmail).Name}");
                        }
                    }
                    finally
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch
                        {
                            Console.Error.WriteLine($"Could not save to database");
                        }
                    }
                }
            }
        }
    }
}