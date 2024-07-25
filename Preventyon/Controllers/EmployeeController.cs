using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Preventyon.Data;
using Preventyon.Repository;
using System.Text.Json;
using Preventyon.Models.DTO.Employee;
using Preventyon.Models;

namespace Preventyon.Controllers
{
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class EmployeeController : ControllerBase
    {
        private  EmployeeRepository _repository;
        private ApiContext _context;
        public EmployeeController(EmployeeRepository _repository, ApiContext _context)
        {
            this._repository = _repository;
            this._context = _context; 
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

                Employee employee = await _repository.AddEmployee(employeedto);

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
            List<GetEmployeesDTO> getEmployees = await _repository.GetEmployees();

            return Ok(getEmployees);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeByIdAsync(int id)
        {
            GetEmployeeRoleWithIDDTO employee = await _repository.GetEmployeeByIdAsync(id);
            return Ok(employee);
        }

/*        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeWithIncidents(int id)
        {
            Employee employee = await _repository.GetEmployeeWithIncidents(id);
           
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }*/
    }
}
