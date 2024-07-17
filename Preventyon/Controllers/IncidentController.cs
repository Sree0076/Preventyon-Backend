using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Repository;
using Preventyon.Repository.IRepository;
using System.ComponentModel.DataAnnotations;

namespace Preventyon.Controllers
{
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class IncidentController : ControllerBase
    {
        private readonly IncidentRepository _incidentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public IncidentController(IncidentRepository incidentRepository, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _incidentRepository = incidentRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidents()
        {
            return Ok(await _incidentRepository.GetAllIncidents());
        }




        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incident>>> GetDraftIncidentsByEmployeeId(int employeeId)
        {
            var draftIncidents = await _incidentRepository.GetDraftIncidentsByEmployeeId(employeeId);
            return Ok(draftIncidents);
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidentsByEmployeeId(int employeeId)
        {
            var Incidents = await _incidentRepository.GetIncidentsByEmployeeId(employeeId);
            return Ok(Incidents);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Incident>> GetIncident(int id)
        {
            var incident = await _incidentRepository.GetIncidentById(id);

            if (incident == null)
            {
                return NotFound();
            }

            return Ok(incident);
        }



        [HttpPost]
        public async Task<ActionResult<Incident>> CreateIncident([FromBody] CreateIncidentDTO createIncidentDto)
        {
            createIncidentDto.EmployeeId = 2;
            var employee = await _employeeRepository.GetEmployeeByIdAsync(createIncidentDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest("invalid employee id");
            }
            try
            {
                DateTime utcDate = createIncidentDto.IncidentOccuredDate.ToUniversalTime();
                createIncidentDto.IncidentOccuredDate = utcDate;
                Incident incident = _mapper.Map<Incident>(createIncidentDto);
                incident.ReportedBy = employee.Name;
                await _incidentRepository.AddIncident(incident);

                return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incident);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncident(int id, Incident incident)
        {
            if (id != incident.Id)
            {
                return BadRequest();
            }

            await _incidentRepository.UpdateIncident(incident);

            return NoContent();
        }


    }
}
