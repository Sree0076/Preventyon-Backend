using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Models.DTO.Employee;
using Preventyon.Service.IService;

namespace Preventyon.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")] 
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly ApiContext _context;

        public EmployeeController(IEmployeeService service, ApiContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostEmployee([FromBody] CreateEmployeeDTO employeedto)
        {
            try
            {
                Employee employee = await _service.AddEmployee(employeedto);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                List<GetEmployeesDTO> getEmployees = await _service.GetEmployees();
                return Ok(getEmployees);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeByIdAsync(int id)
        {
            try
            {
                GetEmployeeRoleWithIDDTO employee = await _service.GetEmployeeByIdAsync(id);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getUserRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeByTokenAsync()
        {
            var jwtStream = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(jwtStream))
                return BadRequest("Authorization header missing or invalid");

            try
            {
                var employee = await _service.GetEmployeeByTokenAsync(jwtStream, _context);
                return Ok(employee);
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
