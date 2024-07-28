using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Preventyon.Data;
using Preventyon.Models;
using Microsoft.EntityFrameworkCore;
using Preventyon.Repository.IRepository;
using System.Text.Json;
using Preventyon.Repository;
using Preventyon.Service.IService;


namespace Preventyon.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AssignedIncidentController : ControllerBase
    {
        private readonly IAssignedIncidentService _assignedIncidentService;

        public AssignedIncidentController(IAssignedIncidentService assignedIncidentService)
        {
            _assignedIncidentService = assignedIncidentService;
        }

        [HttpPost("AssignIncidentToEmployees/{incidentId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignIncidentToEmployees(int incidentId, [FromBody] List<int> employeeIds)
        {
            try
            {
                await _assignedIncidentService.AssignIncidentToEmployeesAsync(incidentId, employeeIds);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetAssignedIncidentsForEmployee(int employeeId)
        {
           /* null check*/
            var incidents = await _assignedIncidentService.GetAssignedIncidentsForEmployeeAsync(employeeId);
            return Ok(incidents);
        }
    }
}
