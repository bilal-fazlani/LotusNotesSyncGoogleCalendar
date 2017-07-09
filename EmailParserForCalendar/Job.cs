using System;
using System.Collections.Generic;
using EmailParserForCalendar.EmailProcessing;
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
                .GetRecentMessages("abc",30,30);
           
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

                        IEmailProcessor emailProcessor = EmailProcessorFactory.GetEmailProcessor(email.Operation);
                        emailProcessor.Process(email, db);

                        email.Status = Constants.Processed;
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
                        Console.WriteLine($"{Constants.Skipped} - {ex.Message}");
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