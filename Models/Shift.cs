namespace Shiftbid.Models
{
    public class Shift
    {
        public int ShiftID { get; set; }
        public string ShiftName { get; set; }
#nullable enable
        public string? Email { get; set; }
#nullable disable

        public int ReportID { get; set; }
        public Report Report { get; set; }
    }
}