using Preventyon.Models;

namespace Preventyon.Repository.IRepository
{
    public interface IIncidentRepository
    {
        Task<IEnumerable<Incident>> GetAllIncidents();
        Task<IEnumerable<Incident>> GetDraftIncidentsByEmployeeId(int employeeId);
        Task<Incident> GetIncidentById(int id);
        Task<Incident> AddIncident(Incident incident);
        Task<Incident> UpdateIncident(Incident incident);
    }
}
