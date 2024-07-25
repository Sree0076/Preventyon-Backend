using Preventyon.Models;

namespace Preventyon.Service.IService
{
    public interface IAssignedIncidentService
    {
        Task AssignIncidentToEmployeesAsync(int incidentId, List<int> employeeIds);
        Task<List<Incident>> GetAssignedIncidentsForEmployeeAsync(int employeeId);
    }
}
