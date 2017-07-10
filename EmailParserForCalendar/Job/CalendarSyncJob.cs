using System;
using System.Collections.Generic;
using EmailParserForCalendar.EmailProcessing;
using EmailParserForCalendar.Exceptions;
using EmailParserForCalendar.Google;
using EmailParserForCalendar.Persistance;
using FluentScheduler;
using Google.Apis.Gmail.v1.Data;

namespace EmailParserForCalendar.Job
{
    public class CalendarSyncJob : IJob
    {
        private readonly GmailClient _gmailClient;

        public CalendarSyncJob(GmailClient gmailClient)
        {
            _gmailClient = gmailClient;
        }
        
        public void Execute()
        {
            IEnumerable<Message> messages = _gmailClient
                .GetRecentMessages();
           
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

                        if(email.Status == Constants.Error)
                            throw new ParsingFailedException(email.Subject);
                        
                        IEmailProcessor emailProcessor = EmailProcessorFactory.GetEmailProcessor(email.Operation);
                        emailProcessor.Process(email, db);

                        email.Status = Constants.Processed;
                        db.SaveChanges();
                        Console.WriteLine($"{Constants.Processed} - {email.GoodleId} sucessfully");
                    }
                    catch (ParsingFailedException)
                    {
                        Console.Error.WriteLine("An error occued while parsing subject: "+ email.Subject);
                    }
                    catch (RecordSkippedException ex)
                    {
                        email.Status = $"{Constants.Skipped}";
                        Console.WriteLine($"{Constants.Skipped} - {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        email.Status = $"{Constants.Error} - {ex.Message}";
                        Console.Error.WriteLine($"{Constants.Error} - {ex.Message} - {email.Subject}");
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