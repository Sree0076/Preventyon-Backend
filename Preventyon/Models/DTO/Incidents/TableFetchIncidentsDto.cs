namespace Preventyon.Models.DTO.Incidents
{
    public class TableFetchIncidentsDto
    {
        public int Id { get; set; }
        public string IncidentTitle { get; set; }
        public string IncidentType { get; set; }
        public string Category { get; set; }

        public string ReportedBy { get; set; }
        public string Priority { get; set; }

        public string IncidentStatus { set; get; }
        public bool IsDraft { get; set; }
    }
}
