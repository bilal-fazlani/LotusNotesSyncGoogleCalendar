using System;
using System.Collections.Generic;
using EmailParserForCalendar.EmailProcessing;
using EmailParserForCalendar.Exceptions;
using EmailParserForCalendar.Google;
using EmailParserForCalendar.Persistance;
using FluentScheduler;
using Google.Apis.Gmail.v1.Data;
using Serilog;

namespace EmailParserForCalendar.Job
{
    public class CalendarSyncJob : IJob
    {
        private readonly GmailClient _gmailClient;
        private readonly EmailProcessorFactory _emailProcessorFactory;
        private readonly ILogger _logger;

        public CalendarSyncJob(GmailClient gmailClient, EmailProcessorFactory emailProcessorFactory,
            ILogger logger)
        {
            _gmailClient = gmailClient;
            _emailProcessorFactory = emailProcessorFactory;
            _logger = logger;
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
                        
                        IEmailProcessor emailProcessor = _emailProcessorFactory.GetEmailProcessor(email.Operation);
                        emailProcessor.Process(email, db);

                        email.Status = Constants.Processed;
                        db.SaveChanges();
                        _logger.Information($"{Constants.Processed} - {{GoodleId}} sucessfully", email.GoodleId);
                    }
                    catch (ParsingFailedException ex)
                    {
                        _logger.Error(ex, "An error occued while parsing subject: {subject}", email.Subject);
                    }
                    catch (RecordSkippedException ex)
                    {
                        email.Status = $"{Constants.Skipped}";
                        _logger.Warning(ex, $"{Constants.Skipped} - {{message}}", ex.Message);
                        _logger.Verbose("{subject}", email.Subject);
                    }
                    catch (Exception ex)
                    {
                        email.Status = $"{Constants.Error} - {ex.Message}";
                        _logger.Error(ex, "{message} - {subject}", ex.Message, email.Subject);
                    }
                    finally
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch
                        {
                            _logger.Error("Could not save to database");
                        }
                    }
                }
            }
        }
    }
}