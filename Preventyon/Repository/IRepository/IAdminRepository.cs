using Preventyon.Models;

namespace Preventyon.Repository.IRepository
{
    public interface IAdminRepository
    {
        Task<Admin> GetAdminByIdAsync(int id);
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task AddAdminAsync(Admin admin);
        Task UpdateAdminAsync(Admin admin);
        Task DeleteAdminAsync(int id);
    }
}
