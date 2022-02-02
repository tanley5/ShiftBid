namespace Shiftbid.Models
{
    public class Seniority
    {
        public int SeniorityID { get; set; }
        public int SeniorityNumber { get; set; }
        public string AgentName { get; set; }
        public string AgentEmail { get; set; }

        public int ReportID { get; set; }
        public Report Report { get; set; }
    }
}