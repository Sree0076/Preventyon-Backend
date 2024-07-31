using Preventyon.Models;
using Preventyon.Repository.IRepository;
using Preventyon.Service.IService;
using System.Text.Json;

namespace Preventyon.Service
{
    public class AssignedIncidentService : IAssignedIncidentService
    {
        private readonly IAssignedIncidentRepository _assignedIncidentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IIncidentRepository _incidentRepository;

        public AssignedIncidentService(
            IAssignedIncidentRepository assignedIncidentRepository,
            IEmployeeRepository employeeRepository,
            IIncidentRepository incidentRepository)
        {
            _assignedIncidentRepository = assignedIncidentRepository;
            _employeeRepository = employeeRepository;
            _incidentRepository = incidentRepository;
        }

        public async Task AssignIncidentToEmployeesAsync(int incidentId, List<int> employeeIds)
        {
            var incident = await _incidentRepository.GetIncidentById(incidentId);
            if (incident == null)
            {
                throw new KeyNotFoundException("Incident not found");
            }

/*            var employeeNames = (await _employeeRepository.GetEmployees())
                .Where(e => employeeIds.Contains(e.Id))
                .Select(e => e.Name)
                .ToList();*/

            incident.IncidentStatus = "progress";

          //  incident.ActionAssignedTo = JsonSerializer.Serialize(employeeNames);

            var assignment = new AssignedIncidents
            {
                IncidentId = incidentId,
                AssignedTo = JsonSerializer.Serialize(employeeIds),
            };

            await _assignedIncidentRepository.AddAssignmentAsync(assignment);
            await _incidentRepository.UpdateIncidentAsync(incident);
        }

        public async Task<List<Incident>> GetAssignedIncidentsForEmployeeAsync(int employeeId)
        {
            var assignments = await _assignedIncidentRepository.GetAssignmentsByEmployeeIdAsync(employeeId);
            var incidentIds = assignments
                .Select(a => a.IncidentId)
                .Distinct()
                .ToList();

            return await _assignedIncidentRepository.GetIncidentsByIdsAsync(employeeId,incidentIds);
        }
    }
}
