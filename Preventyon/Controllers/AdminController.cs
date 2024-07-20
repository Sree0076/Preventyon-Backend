using Microsoft.AspNetCore.Mvc;
using Preventyon.Models.DTO.AdminDTO;
using Preventyon.Service.IService;
using System;
using System.Threading.Tasks;

namespace Preventyon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
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
        public async Task<IActionResult> UpdateAdmin(int adminId, int? roleId, bool? status)
        {
            try
            {
                await _adminService.UpdateAdminAsync(adminId, roleId, status);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
