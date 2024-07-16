using Preventyon.Models;
using Preventyon.Models.DTO.Employee;

namespace Preventyon.Repository.IRepository
{
    public interface IEmployeeRepository
    {
        Task<List<GetEmployeesDTO>> GetEmployees();
        Task<GetEmployeeRoleWithIDDTO> GetEmployeeByIdAsync(int employeeId);
        Task UpdateAsync(Employee employee);
        Task<Employee> FindAsync(int id);
    }
}
