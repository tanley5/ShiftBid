using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;


namespace Shiftbid.Models
{
    public enum Status
    {
        New,
        Working,
        Complete
    }
    public class Report
    {
        public int ReportID { get; set; }
        public string ReportName { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public Status Status { get; set; }
#nullable enable
        public List<Seniority>? Seniorities { get; set; }
        public List<Shift>? Shifts { get; set; }
    }
}