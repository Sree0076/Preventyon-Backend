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
            await _context.AssignedIncidents.AddAsync(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AssignedIncidents>> GetAssignmentsByEmployeeIdAsync(int employeeId)
        {
            var employeeIdString = employeeId.ToString(); 

            return await _context.AssignedIncidents
                .Where(a => a.AssignedTo.Contains(employeeIdString))
                .ToListAsync();
        }

        public async Task<Incident> GetIncidentByIdAsync(int id)
        {
            return await _context.Incident.FindAsync(id);
        }

        public async Task<List<Incident>> GetIncidentsByIdsAsync(int employeeId,List<int> ids)
        {
            /*            return await _context.Incident
                            .Where(i => ids.Contains(i.Id))
                            .ToListAsync();*/

            /*            var result = await (from incident in _context.Incident
                                            join assignment in _context.AssignedIncidents
                                            on incident.Id equals assignment.IncidentId into incidentAssignments
                                            from assignment in incidentAssignments.DefaultIfEmpty()
                                            where ids.Contains(incident.Id)
                                            select new Incident
                                            {
                                                Id = incident.Id,
                                                // Map other incident fields
                                                Accepted = assignment.Accepted
                                            }).ToListAsync();*/


            var result = await (from incident in _context.Incident
                                join assignment in _context.AssignedIncidents
                                on incident.Id equals assignment.IncidentId into incidentAssignments
                                from assignment in incidentAssignments.DefaultIfEmpty()
                                where ids.Contains(incident.Id) &&
                                      (assignment == null ||
                                       assignment.Accepted == null ||
                                       assignment.Accepted == employeeId)
                                select new Incident
                                {
                                    Id = incident.Id,
                                    IncidentTitle = incident.IncidentTitle,
                                    IncidentType = incident.IncidentType,
                                    Category = incident.Category,
                                    IncidentOccuredDate = incident.IncidentOccuredDate,
                                    ReportedBy = incident.ReportedBy,
                                    Priority = incident.Priority,
                                    IncidentStatus = incident.IncidentStatus,
                                    IsSubmittedForReview = incident.IsSubmittedForReview,
                                    IsCorrectionFilled = (!string.IsNullOrEmpty(incident.Correction)) && (!string.IsNullOrEmpty(incident.CorrectiveAction)),
                                    IsDraft = incident.IsDraft,
                                    Accepted = assignment.Accepted
                                }).ToListAsync();

            return result;
        }
    }
}
