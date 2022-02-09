using Shiftbid.Data;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Shiftbid.Background
{
    public class Background
    {
        public Background(ApplicationDbContext context)
        {
            this.context = context;

        }
        public ApplicationDbContext context { get; set; }

        public DbContext GetDbContext()
        {
            return context;
        }
        public void ProcessBGJob(int reportId)
        {
            var report = this.context.Reports.Find(reportId);
            Console.WriteLine($"{report.ReportName}");

        }
    }
}