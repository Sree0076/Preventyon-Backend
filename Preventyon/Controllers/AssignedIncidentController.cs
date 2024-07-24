using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Preventyon.Data;
using Preventyon.Models;
using Microsoft.EntityFrameworkCore;
using Preventyon.Repository.IRepository;
using System.Text.Json;
using Preventyon.Repository;


namespace Preventyon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignedIncidentController 
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;
        public AssignedIncidentController(ApiContext context, IMapper mapper)
        {
            this._context = context;
            _mapper = mapper;

        }

        [HttpPost("AssignIncidentToEmployees/{incidentId}")]
        public async Task AssignIncidentToEmployees(int incidentId, [FromBody] List<int> employeeIds,IncidentRepository incidentRepository,EmployeeRepository employeeRepository)
        {
         

            var assignedToJson = JsonSerializer.Serialize(employeeIds);

            var assignment = new AssignedIncidents
            {
                IncidentId = incidentId,
                AssignedTo = assignedToJson,
             
            };
            var employeeNames = employeeRepository.GetEmployees().Result.Where(e => employeeIds.Contains(e.Id))
               .Select(e => e.Name)
               .ToList();
            var incident =incidentRepository.GetIncidentById(incidentId).Result;
            incident.IncidentStatus = "progress";
            incident.ActionAssignedTo = JsonSerializer.Serialize(employeeNames);


            _context.AssignedIncidents.Add(assignment);
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<List<Incident>> GetAssignedIncidentsForEmployee(int employeeId)
        {
            var employeeIdString = JsonSerializer.Serialize(employeeId);

            var assignments = await _context.AssignedIncidents
                .Where(a => a.AssignedTo.Contains(employeeIdString))
                .ToListAsync();

            var incidentIds = assignments
                .Select(a => a.IncidentId)
                .Distinct()
                .ToList();

            return await _context.Incident
                .Where(i => incidentIds.Contains(i.Id))
                .ToListAsync();
        }


    }
}
