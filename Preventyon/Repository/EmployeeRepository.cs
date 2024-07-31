using Microsoft.AspNetCore.Mvc;
using Preventyon.Data;
using Microsoft.EntityFrameworkCore;
using System;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Preventyon.Models.DTO.Employee;
using Preventyon.Repository.IRepository;
using Preventyon.Models;


namespace Preventyon.Repository
{


    public class EmployeeRepository:IEmployeeRepository

    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;
        public EmployeeRepository(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
           
        }

        public async Task<Employee> AddEmployee(CreateEmployeeDTO employeedto)
        {
            Employee employee = _mapper.Map<Employee>(employeedto);
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

         public async Task<List<GetEmployeesDTO>> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return _mapper.Map<List<GetEmployeesDTO>>(employees);
        }

        public async Task<Employee> FindEmployeeAsync(int id)
        {

            var employee = await _context.Employees
             .FirstOrDefaultAsync(e => e.Id == id);

            return employee;
        }
        public async Task<GetEmployeeRoleWithIDDTO> GetEmployeeByIdAsync(int id)
        {

            var employee = await _context.Employees
             .Include(e => e.Role)
             .ThenInclude(r => r.Permission)
             .FirstOrDefaultAsync(e => e.Id == id);

            return _mapper.Map<GetEmployeeRoleWithIDDTO>(employee);
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }
        public async Task<Employee> FindAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

    }
}
