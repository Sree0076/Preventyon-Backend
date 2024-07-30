using Microsoft.AspNetCore.Mvc;
using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Service.IService;


namespace Preventyon.Controllers
{

    [ApiController]
    [Route("api/[Controller]/[Action]")]

    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly IEmployeeService _employeeService;
        private readonly IEmailService _emailService;

        public IncidentController(IIncidentService incidentService, IEmployeeService employeeService, IEmailService emailService)
        {
            _incidentService = incidentService;
            _employeeService = employeeService;
            _emailService = emailService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Incident>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidents()
        {
            var incidents = await _incidentService.GetAllIncidents();
            return Ok(incidents);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetIncidentsByEmployeeID))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetIncidentsByEmployeeId(int employeeId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee.Role.Name == "SuperAdmin" || employee.Role.Name == "Admins-User" || employee.Role.Name == "Admin-Incidents")
            {
                var Adminincidents = await _incidentService.GetIncidentsAdmins();
                return Ok(Adminincidents);
            }
            var incidents = await _incidentService.GetIncidentsByEmployeeId(employeeId);
            return Ok(incidents);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Incident))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Incident>> GetIncident(int id)
        {
            var incident = await _incidentService.GetIncidentById(id);

            if (incident == null)
            {
                return NotFound();
            }

            return Ok(incident);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Incident))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Incident>> CreateIncident([FromForm] CreateIncidentDTO createIncidentDto)
        {
            if (createIncidentDto == null)
            {
                return BadRequest("Incident data is required");
            }

            try
            {
                var incident = await _incidentService.CreateIncident(createIncidentDto);
                var emailResult = await _emailService.SendNotificationAsync(createIncidentDto.EmployeeId, incident);

                if (emailResult)
                {
                    return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incident);
                }

                return BadRequest("Email Not Send");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateIncident(int id, [FromBody] UpdateIncidentDTO updateIncidentDto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid incident ID");
            }

            if (updateIncidentDto == null)
            {
                return BadRequest("Incident update data is required");
            }

            try
            {
                await _incidentService.UpdateIncident(id, updateIncidentDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UserUpdateIncident(int id, [FromForm] UpdateIncidentUserDto updateIncidentDto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid incident ID");
            }

            if (updateIncidentDto == null)
            {
                return BadRequest("Incident update data is required");
            }

            try
            {
                await _incidentService.UserUpdateIncident(id, updateIncidentDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Incident))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetUserUpdateIncidentDTO>> GetUserUpdateIncident(int id)
        {
            var incident = await _incidentService.GetUserUpdateIncident(id);

            if (incident == null)
            {
                return NotFound();
            }

            return Ok(incident);
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminIncidentsWithBarChart()
        {
            var result = await _incidentService.GetIncidentsAdmins();
            return Ok(result); // Automatically serializes the DTO to JSON
        }

    }
}
