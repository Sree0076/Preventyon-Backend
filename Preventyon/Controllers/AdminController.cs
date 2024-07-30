using Microsoft.AspNetCore.Mvc;
using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Repository;
using Preventyon.Service.IService;

namespace Preventyon.Controllers
{
    [ApiController]
    [Route("api/[controller]/[Action]")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
         
        }
        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<GetAllAdminsDto>>> GetAllAdmins(int Id)
        {
            return Ok(await _adminService.GetAllAdminsAsync(Id));

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddAdmin(CreateAdminDTO createAdminDTO)
        {
            try
            {
                var admin = await _adminService.AddAdminAsync(createAdminDTO);
                return Ok(admin);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{adminId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAdmin(int adminId,UpdateAdminDTO updateAdmin)
        {
            try
            {
                await _adminService.UpdateAdminAsync(updateAdmin);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
