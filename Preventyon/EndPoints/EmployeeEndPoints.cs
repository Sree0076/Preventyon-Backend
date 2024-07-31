using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Preventyon.Data;
using Preventyon.Models.DTO.Employee;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Repository.IRepository;
using RequestDemoMinimal.models;
using System.Net;

namespace Preventyon.EndPoints
{
    public static class EmployeeEndPoints
    {

        private static readonly IIncidentRepository _incidentRepository;
        private static readonly IMapper _mapper;
        public static void ConfigureEndPoints(this WebApplication app)
        {
            app.MapPut("/api/updateEmployee/{id}", UpdateEmployee)
                .WithName("UpdateEmployee")
                .Accepts<UpdateEmployeeRoleDTO>("application/json")
                .Produces<APIResponse>(200)
                .Produces(400)
                .Produces(404);


            app.MapPut("/api/updateIncidentByReview/{id}", updateIncidentByReview)
                .WithName("updateIncidentByReview")
                .Accepts<UpdateIncidentByReviewDto>("application/json")
                .Produces<APIResponse>(200)
                .Produces(400)
                .Produces(404);


            app.MapPut("/api/acceptIncidents/{incidentId}", acceptIncidents)
                .WithName("acceptIncidents")
                .Accepts<UpdateIncidentByReviewDto>("application/json")
                .Produces<APIResponse>(200)
                .Produces(400)
                .Produces(404);

            app.MapGet("/api/incidentApproval/{id}", incidentApproval)
                .WithName("incidentApproval")
                .Produces<APIResponse>(200)
                .Produces(400)
                .Produces(404);
        }
        public async static Task<IResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeRoleDTO employeeDTO, IMapper _mapper, IEmployeeRepository _employeeRepo)
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



        public async static Task<IResult> updateIncidentByReview(int id, [FromBody] UpdateIncidentByReviewDto incidentByReviewDto, IMapper _mapper, IIncidentRepository incidentRepository)
        {
            APIResponse response = new APIResponse();
            var existingIncident = await incidentRepository.GetIncidentById(id);

            if (existingIncident == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.isSuccess = false;
                return Results.NotFound(response);
            }
            if (existingIncident.IsSubmittedForReview)
            {
                incidentByReviewDto.IsSubmittedForReview = false;
            }
            _mapper.Map(incidentByReviewDto, existingIncident);
            existingIncident.IncidentStatus = "review";
            await incidentRepository.UpdateIncidentAsync(existingIncident);

            response.Result = _mapper.Map<UpdateIncidentByReviewDto>(existingIncident);
            response.StatusCode = HttpStatusCode.OK;
            response.StatusCode = HttpStatusCode.OK;
            response.isSuccess = true;
            return Results.Ok(response);
        }


        public async static Task<IResult> acceptIncidents(int incidentId, [FromBody] int employeeId,ApiContext apiContext)
        {
            APIResponse response = new APIResponse();

            var assignedIncidents = await apiContext.AssignedIncidents
                .Where(a => a.IncidentId == incidentId)
                .ToListAsync();

            var employee = await apiContext.Employees.FindAsync(employeeId);

            if (employee == null)
            {
               response.StatusCode = HttpStatusCode.NotFound;
                return Results.NotFound(response);
            }
            var incident = await apiContext.Incident.FindAsync(incidentId);
            if (incident == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return Results.NotFound(response);
            }
            incident.ActionAssignedTo = employee.Name;
            if (assignedIncidents == null || !assignedIncidents.Any())
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.isSuccess = false;
                return Results.NotFound(response);
            }
            
            foreach (var assignedIncident in assignedIncidents)
            {
                assignedIncident.Accepted = employeeId;
            }
             

            await apiContext.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.OK;
            response.isSuccess = true;
            return Results.Ok(response);
        }

        public async static Task<IResult> incidentApproval(int id, [FromServices] IIncidentRepository _incidentRepository)
        {
            APIResponse response = new APIResponse();
            var existingIncident = await _incidentRepository.GetIncidentById(id);

            if (existingIncident == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.isSuccess = false;
                return Results.NotFound(response);
            }

            if (existingIncident.IsSubmittedForReview && string.IsNullOrEmpty(existingIncident.Correction))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                return Results.BadRequest(response);
            }

            existingIncident.IncidentStatus = "closed";
            await _incidentRepository.UpdateIncidentAsync(existingIncident);

            response.Result = existingIncident;
            response.StatusCode = HttpStatusCode.OK;
            response.isSuccess = true;
            return Results.Ok(response);
        }
    }
}
