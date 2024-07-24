using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Repository;
using Preventyon.Service.IService;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Preventyon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly AdminRepository _adminRepository;

        public AdminsController(IAdminService adminService,AdminRepository adminRepository)
        {
            _adminService = adminService;
            _adminRepository = adminRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAllAdminsDto>>> GetAllAdmins()
        {
            return Ok(await _adminRepository.GetAllAdminsAsync());

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
