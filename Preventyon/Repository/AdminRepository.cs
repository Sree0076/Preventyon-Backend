using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Repository.IRepository;

namespace Preventyon.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApiContext _context;

        public AdminRepository(ApiContext context)
        {
            _context = context;
        }

        public async Task<Admin> GetAdminByIdAsync(int id)
        {
            return await _context.Admins
                .Include(a => a.Employee)
                .ThenInclude(e => e.Role)
                .FirstOrDefaultAsync(a => a.AdminId == id);
        }

        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            return await _context.Admins
                .Include(a => a.Employee)
                .ThenInclude(e => e.Role)
                .ToListAsync();
        }

        public async Task AddAdminAsync(Admin admin)
        {
           await  _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAdminAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAdminAsync(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
        }

    }
}
