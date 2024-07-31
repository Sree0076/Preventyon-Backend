using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Preventyon.Models;
using Preventyon.Service.IService;
using Serilog;

namespace Preventyon.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AssignedIncidentController : ControllerBase
    {
        private readonly IAssignedIncidentService _assignedIncidentService;
        private readonly ILogger<AssignedIncidentController> _logger;

        public AssignedIncidentController(IAssignedIncidentService assignedIncidentService, ILogger<AssignedIncidentController> logger)
        {
            _assignedIncidentService = assignedIncidentService;
            _logger = logger;
        }

        [HttpPost("AssignIncidentToEmployees/{incidentId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignIncidentToEmployees(int incidentId, [FromBody] List<int> employeeIds)
        {
            _logger.LogInformation("Attempting to assign incident {IncidentId} to employees: {EmployeeIds}", incidentId, string.Join(", ", employeeIds));

            try
            {
                await _assignedIncidentService.AssignIncidentToEmployeesAsync(incidentId, employeeIds);
                _logger.LogInformation("Successfully assigned incident {IncidentId} to employees: {EmployeeIds}", incidentId, string.Join(", ", employeeIds));
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Incident {IncidentId} not found while assigning to employees: {EmployeeIds}", incidentId, string.Join(", ", employeeIds));
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning incident {IncidentId} to employees: {EmployeeIds}", incidentId, string.Join(", ", employeeIds));
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetAssignedIncidentsForEmployee(int employeeId)
        {
            _logger.LogInformation("Fetching assigned incidents for employee {EmployeeId}", employeeId);

            try
            {
                var incidents = await _assignedIncidentService.GetAssignedIncidentsForEmployeeAsync(employeeId);
                _logger.LogInformation("Successfully fetched {Count} incidents for employee {EmployeeId}", incidents?.Count ?? 0, employeeId);
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching assigned incidents for employee {EmployeeId}", employeeId);
                return BadRequest(ex.Message);
            }
        }
    }
}
