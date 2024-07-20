using System;
using System.Threading.Tasks;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;
using Preventyon.Repository.IRepository;
using Preventyon.Service.IService;

namespace Preventyon.Service
{
    public class AdminService : IAdminService
    {
        private readonly ApiContext _context;
        private readonly IEmployeeRepository _employeeRepository;

        public AdminService(ApiContext context, IEmployeeRepository employeeRepository)
        {
            _context = context;
            _employeeRepository = employeeRepository;
        }

        public async Task<Admin> AddAdminAsync(CreateAdminDTO createAdminDTO)
        {
            var existingEmployee = await _employeeRepository.FindAsync(createAdminDTO.EmployeeId);
            if (existingEmployee == null)
            {
                throw new Exception("Employee not found");
            }

            try
            {
                existingEmployee.RoleId = 1;
                await _employeeRepository.UpdateAsync(existingEmployee);

                var admin = new Admin
                {
                    EmployeeId = createAdminDTO.EmployeeId,
                    AssignedBy = createAdminDTO.AssignedBy,
                    Status = true,
                };

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                return admin;
            }
            catch (Exception ex)
            {
           
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }


        public async Task UpdateAdminAsync(int adminId, int? roleId = null, bool? status = null)
        {
            var admin = await _context.Admins.FindAsync(adminId);
            if (admin == null)
            {
                throw new Exception("Admin not found");
            }

            if (roleId.HasValue)
            {
                var employee = await _context.Employees.FindAsync(admin.EmployeeId);
                if (employee == null)
                {
                    throw new Exception("Employee not found");
                }

                employee.RoleId = roleId.Value;
            }

            if (status.HasValue)
            {
                admin.Status = status.Value;
            }

            await _context.SaveChangesAsync();
        }


    }
}
