using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Shiftbid.Data;
using Shiftbid.Models;
using Shiftbid.Helper;

using MailKit;
using MimeKit;

namespace Shiftbid.Background
{
    public class Background
    {
        public Background(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, EmailService email)
        {
            this.context = context;
            this._httpContextAccessor = httpContextAccessor;
            this._email = email;
        }
        public ApplicationDbContext context { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly EmailService _email;

        public DbContext GetDbContext()
        {
            return context;
        }
        public void ProcessBGJob(int reportId)
        {
            var report = this.context.Reports.Find(reportId);
            int i = 0;
            //while (report.Status != Status.Complete)
            while (i < 5)
            {
                //var Seniorities = context.Seniorities.Where(sen => sen.ReportID == a.ReportID);
                var Seniorities = context.Seniorities.Where(sen => sen.SeniorityState != SeniorityState.Received && sen.Report == report);

                // setting up email stuff
                EmailAddress toEmail = new EmailAddress { Name = "Test", Address = "Address@email.com" };
                List<EmailAddress> toEmails = new List<EmailAddress>() { toEmail };

                EmailAddress fromEmail = new EmailAddress { Name = "admin", Address = "admin@email.com" };
                List<EmailAddress> fromEmails = new List<EmailAddress>() { fromEmail };

                EmailMessage message = new EmailMessage { ToAddresses = toEmails, FromAddresses = fromEmails, Subject = "Shiftbid Stuff", Content = "ShiftbidContent" };

                // Check if any of the seniority stat is "sent". If there is, we skip this round
                if (Seniorities.Any(sen => sen.SeniorityState == SeniorityState.Sent))
                {
                    Console.WriteLine("Still Waiting Reply");
                    Task.Delay(10000).Wait();
                }
                // if all Seniorities are null, change report status to complete
                else if (Seniorities == null)
                {
                    //report.Status = Status.Complete;
                    //context.SaveChanges();
                    Console.WriteLine("No Seniorities Left");
                    Task.Delay(10000).Wait();
                }
                // If none of the seniority state is "sent" (All state is new), we get the first seniority of the list and send an email to them with the link
                else
                {
                    // Seniority NextSeniority = Seniorities.First();
                    // string host = _httpContextAccessor.HttpContext.Request.Host.Value;
                    // string link = $"{host}/Shiftbid/Responses/{report.ReportID}";
                    // NextSeniority.SeniorityState = SeniorityState.Sent;
                    // context.SaveChanges();
                    Console.WriteLine("Sending the next seniority an email");
                    //_email.Send(message);

                    Task.Delay(10000).Wait();
                }
                i++;

            }


        }
    }
}