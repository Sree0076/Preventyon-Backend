using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Repository.IRepository;

namespace Preventyon.Repository
{
    public class IncidentRepository: IIncidentRepository
    {


        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public IncidentRepository(ApiContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Incident>> GetAllIncidents()
        {
            return await _context.Incident.ToListAsync();
        }

        public async Task<IEnumerable<Incident>> GetDraftIncidentsByEmployeeId(int employeeId)
        {
            return await _context.Incident.Where(i => i.IsDraft && i.EmployeeId == employeeId).ToListAsync();
               
        }
        public async Task<GetIncidentsByEmployeeID> GetIncidentsByEmployeeId(int employeeId)
        {
           var incidents = await _context.Incident.Where(i => i.EmployeeId == employeeId && i.IsDraft==false).ToListAsync();
            var privacyIncidents = incidents.Where(i => i.IncidentType.Contains("Privacy Incidents")).ToList();
            var qualityIncidents = incidents.Where(i => i.IncidentType.Contains("Quality Incidents")).ToList();
            var securityIncidents = incidents.Where(i => i.IncidentType.Contains("Security Incidents")).ToList();

            int totalPrivacyIncidents = privacyIncidents.Count;
            int closedPrivacyIncidents = privacyIncidents.Count(i => i.IncidentStatus == "Completed");
            int pendingPrivacyIncidents = totalPrivacyIncidents - closedPrivacyIncidents;

            int totalQualityIncidents = qualityIncidents.Count;
            int closedQualityIncidents = qualityIncidents.Count(i => i.IncidentStatus == "Completed");
            int pendingQualityIncidents = totalQualityIncidents - closedQualityIncidents;

            int totalSecurityIncidents = securityIncidents.Count;
            int closedSecurityIncidents = securityIncidents.Count(i => i.IncidentStatus == "Completed");
            int pendingSecurityIncidents = totalSecurityIncidents - closedSecurityIncidents;
        

            var incidentStats = new GetIncidentsByEmployeeID
            {
         
                PrivacyTotalIncidents = totalPrivacyIncidents,
                PrivacyPendingIncidents = pendingPrivacyIncidents,
                PrivacyClosedIncidents = closedPrivacyIncidents,
                QualityTotalIncidents = totalQualityIncidents,
                QualityPendingIncidents = pendingQualityIncidents,
                QualityClosedIncidents = closedQualityIncidents,
                SecurityTotalIncidents = totalSecurityIncidents,
                SecurityPendingIncidents = pendingSecurityIncidents,
                SecurityClosedIncidents = closedSecurityIncidents,
                Incidents = await _context.Incident.Where(i => i.EmployeeId == employeeId ).ToListAsync()
            };

            return incidentStats;
        }

        public async Task<Incident> GetIncidentById(int id)
        {
            return await _context.Incident.FindAsync(id);
        }
  
        public async Task<Incident> AddIncident(Incident incident)
        {
            _context.Incident.Add(incident);
            await _context.SaveChangesAsync();
            return incident;
        }

        public async Task<Incident> UpdateIncident(Incident incident, UpdateIncidentDTO updateIncidentDto)
        {
            _mapper.Map(updateIncidentDto, incident);

            _context.Entry(incident).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return incident;
        }

        public async Task<Incident> UserUpdateIncident(Incident incident, CreateIncidentDTO updateIncidentDto)
        {
            _mapper.Map(updateIncidentDto, incident);

            _context.Entry(incident).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return incident;
        }
        public async Task DeleteIncident(int id)
        {
            var incident = await _context.Incident.FindAsync(id);
            if (incident != null)
            {
                _context.Incident.Remove(incident);
                await _context.SaveChangesAsync();
            }
        }
    }
}
