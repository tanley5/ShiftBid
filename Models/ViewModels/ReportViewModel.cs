using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;


namespace Shiftbid.Models.ViewModels
{
    public class ReportViewModel
    {
        public int ReportID { get; set; }
        public string ReportName { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public IFormFile SeniorityFile { get; set; }
        public IFormFile ShiftFile { get; set; }
    }
}