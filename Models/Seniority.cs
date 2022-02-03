namespace Shiftbid.Models
{
    public enum SeniorityState
    {
        New,
        Sent,
        Received
    }
    public class Seniority
    {
        public int SeniorityID { get; set; }
        public int SeniorityNumber { get; set; }
        public string AgentName { get; set; }
        public string AgentEmail { get; set; }
        public SeniorityState SeniorityState { get; set; }

        public int ReportID { get; set; }
        public Report Report { get; set; }
    }
}