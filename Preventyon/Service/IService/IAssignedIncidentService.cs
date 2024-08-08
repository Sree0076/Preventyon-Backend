using Preventyon.Models;

namespace Preventyon.Service.IService
{
    public interface IAssignedIncidentService
    {
        Task AssignIncidentToEmployeesAsync(int incidentId, AssignIncidentRequest request);
        Task<List<Incident>> GetAssignedIncidentsForEmployeeAsync(int employeeId);
    }
}
