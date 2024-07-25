using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Repository.IRepository;

namespace Preventyon.Repository
{
    public class AssignedIncidentRepository : IAssignedIncidentRepository
    {
        private readonly ApiContext _context;

        public AssignedIncidentRepository(ApiContext context)
        {
            _context = context;
        }

        public async Task AddAssignmentAsync(AssignedIncidents assignment)
        {
            _context.AssignedIncidents.Add(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AssignedIncidents>> GetAssignmentsByEmployeeIdAsync(int employeeId)
        {
            var employeeIdString = employeeId.ToString(); // Serialize if needed

            return await _context.AssignedIncidents
                .Where(a => a.AssignedTo.Contains(employeeIdString))
                .ToListAsync();
        }

        public async Task<Incident> GetIncidentByIdAsync(int id)
        {
            return await _context.Incident.FindAsync(id);
        }

        public async Task<List<Incident>> GetIncidentsByIdsAsync(List<int> ids)
        {
            return await _context.Incident
                .Where(i => ids.Contains(i.Id))
                .ToListAsync();
        }
    }
}
