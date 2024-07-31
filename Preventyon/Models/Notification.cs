namespace Preventyon.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int IncidentId { get; set; }
        
        public string Type { get; set; }    
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public int EmployeeId { get; set; }

        public Incident Incident { get; set; }
        public Employee Employee { get; set; }

        public Notification()
        {
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
        }
    }
}
