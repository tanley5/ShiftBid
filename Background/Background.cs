using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Shiftbid.Data;
using Shiftbid.Models;

namespace Shiftbid.Background
{
    public class Background
    {
        public Background(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this._httpContextAccessor = httpContextAccessor;
        }
        public ApplicationDbContext context { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;

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
                    Task.Delay(10000).Wait();
                }
                i++;

            }


        }
    }
}