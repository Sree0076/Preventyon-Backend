using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Preventyon.Models.DTO.Employee;
using Preventyon.Repository.IRepository;
using RequestDemoMinimal.models;
using System.Net;

namespace Preventyon.EndPoints
{
    public static class EmployeeEndPoints
    {
        public static void ConfigureEndPoints(this WebApplication app)
        {
            app.MapPut("/api/updateEmployee/{id}", UpdateEmployee)
                .WithName("UpdateEmployee")
                .Accepts<UpdateEmployeeRoleDTO>("application/json")
                .Produces<APIResponse>(200)
                .Produces(400)
                .Produces(404);
        }
        private async static Task<IResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeRoleDTO employeeDTO, IMapper _mapper, IEmployeeRepository _employeeRepo)
        {
            APIResponse response = new APIResponse();
            var existingEmployee = await _employeeRepo.FindAsync(id);

            if (existingEmployee == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.isSuccess = false;
                return Results.NotFound(response);
            }
            _mapper.Map(employeeDTO, existingEmployee);

            await _employeeRepo.UpdateAsync(existingEmployee);

            response.Result = _mapper.Map<UpdateEmployeeRoleDTO>(existingEmployee);
            response.StatusCode = HttpStatusCode.OK;
            response.StatusCode = HttpStatusCode.OK;
            response.isSuccess = true;
            return Results.Ok(response);
        }
    }
}
