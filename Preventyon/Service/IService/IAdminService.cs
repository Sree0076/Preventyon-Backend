using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;

namespace Preventyon.Service.IService
{
    public interface IAdminService
    {
        Task<Admin> AddAdminAsync(CreateAdminDTO createAdminDTO);
        Task UpdateAdminAsync(int adminId, int? roleId = null, bool? status = null);
    }
}
