namespace Preventyon.Models
{
    public class AssignedIncidents
    {
        public int Id { get; set; }
        public int IncidentId { get; set; }
        public string AssignedTo { get; set; }

        public Incident Incident { get; set; }
    }
}
