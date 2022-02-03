using System.Collections.Generic;

namespace Shiftbid.Models.ViewModels
{
    public class ShiftbidIndexViewModel
    {
        public IEnumerable<Report> NewReport { get; set; }
        public IEnumerable<Report> WorkingReport { get; set; }
        public IEnumerable<Report> CompletedReport { get; set; }
    }
}