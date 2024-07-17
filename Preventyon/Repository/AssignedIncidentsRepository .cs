using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Repository.IRepository;

namespace Preventyon.Repository
{
    public class AssignedIncidentsRepository 
    {
        private readonly ApiContext _context;

        public AssignedIncidentsRepository(ApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssignedIncidents>> GetAllAsync()
        {
            return await _context.AssignedIncidents.Include(ai => ai.Incident).ToListAsync();
        }

        public async Task<AssignedIncidents> GetByIdAsync(int id)
        {
            return await _context.AssignedIncidents.Include(ai => ai.Incident)
                                                   .FirstOrDefaultAsync(ai => ai.Id == id);
        }

        public async Task AddAsync(AssignedIncidents assignedIncident)
        {
            await _context.AssignedIncidents.AddAsync(assignedIncident);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AssignedIncidents assignedIncident)
        {
            _context.AssignedIncidents.Update(assignedIncident);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var assignedIncident = await GetByIdAsync(id);
            if (assignedIncident != null)
            {
                _context.AssignedIncidents.Remove(assignedIncident);
                await _context.SaveChangesAsync();
            }
        }

    }
}
