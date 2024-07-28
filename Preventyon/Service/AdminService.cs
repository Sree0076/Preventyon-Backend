using System;
using System.Threading.Tasks;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;
using Preventyon.Models.DTO.Employee;
using Preventyon.Repository;
using Preventyon.Repository.IRepository;
using Preventyon.Service.IService;

namespace Preventyon.Service
{
    public class AdminService : IAdminService
    {
       
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAdminRepository _adminRepository;

        public AdminService(ApiContext context, IEmployeeRepository employeeRepository,IAdminRepository adminRepository)
        {
            
            _employeeRepository = employeeRepository;
            _adminRepository = adminRepository;
           
        }
        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            return await _adminRepository.GetAllAdminsAsync();
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
                if (createAdminDTO.isIncidentMangenet == true && createAdminDTO.isUserMangenet == true)
                {
                    existingEmployee.RoleId = 1;
                }
                else if(createAdminDTO.isIncidentMangenet == true && createAdminDTO.isUserMangenet == false)
                {
                    existingEmployee.RoleId = 3;
                }
                else if (createAdminDTO.isIncidentMangenet == false && createAdminDTO.isUserMangenet == true)
                {
                    existingEmployee.RoleId = 4;
                }
                else 
                {
                    existingEmployee.RoleId = 5;
                }
                await _employeeRepository.UpdateAsync(existingEmployee);

                var admin = new Admin
                {
                    EmployeeId = createAdminDTO.EmployeeId,
                    AssignedBy = createAdminDTO.AssignedBy,
                    Status = createAdminDTO.Status,
                };

               await  _adminRepository.AddAdminAsync(admin);

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
            var admin = await _adminRepository.GetAdminByIdAsync(adminId);
            if (admin == null)
            {
                throw new Exception("Admin not found");
            }

            if (roleId.HasValue)
            {
                var employee = await _employeeRepository.FindEmployeeAsync(admin.EmployeeId);
                if (employee == null)
                {
                    throw new Exception("Employee not found");
                }
                await _employeeRepository.UpdateAsync(employee);
               employee.RoleId = roleId.Value;  
            }

            if (status.HasValue)
            {
                admin.Status = status.Value;
            }

            await _adminRepository.UpdateAdminAsync(admin);
        }


    }
}
