using Microsoft.AspNetCore.Mvc;
using Preventyon.Models.DTO.AdminDTO;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAllAdminsDto>>> GetAllAdmins()
        {
            return Ok(await _adminService.GetAllAdminsAsync());

        }

        [HttpPost]
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
