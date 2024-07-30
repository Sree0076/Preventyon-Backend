using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;

namespace Preventyon.Service.IService
{
    public interface IAdminService
    {

        Task<IEnumerable<GetAllAdminsDto>> GetAllAdminsAsync();
        Task<Admin> AddAdminAsync(CreateAdminDTO createAdminDTO);
        Task UpdateAdminAsync(UpdateAdminDTO updateAdmin);
    }
}
