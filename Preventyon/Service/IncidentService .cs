using AutoMapper;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Models;
using Microsoft.EntityFrameworkCore;
using Preventyon.Repository.IRepository;
using Preventyon.Service.IService;

namespace Preventyon.Service
{
    public class IncidentService : IIncidentService
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly IAssignedIncidentRepository _assignedIncidentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public IncidentService(IIncidentRepository incidentRepository, IEmployeeRepository employeeRepository,IAssignedIncidentRepository assignedIncidentRepository, IMapper mapper)
        {
            _incidentRepository = incidentRepository;
            _employeeRepository = employeeRepository;
            _assignedIncidentRepository= assignedIncidentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Incident>> GetAllIncidents()
        {/*
             null check*/
            return await _incidentRepository.GetAllIncidents();
        }

        public async Task<GetIncidentsByEmployeeID> GetIncidentsByEmployeeId(int employeeId)
        {
            // Fetch assigned incidents and all incidents for the employee
            /*  var assignedIncidentsEntities = await _assignedIncidentRepository.GetAssignmentsByEmployeeIdAsync(employeeId);
                var allIncidentsEntities =;*/
            /*            var uniqueIncidentIds = new HashSet<int>();
                        var assignedIncidentTasks = assignedIncidentsEntities
                            .Select(async assignment =>
                            {
                                if (uniqueIncidentIds.Add(assignment.IncidentId))
                                {
                                    return await _incidentRepository.GetIncidentById(assignment.IncidentId);
                                }
                                return null;
                            })
                            .ToList();

                        var assignedIncidents = (await Task.WhenAll(assignedIncidentTasks))
                            .Where(incident => incident != null)
                            .ToList();

                        allIncidentsEntities.AssignedIncidents = _mapper.Map<List<TableFetchIncidentsDto>>(assignedIncidents);

                        GetIncidentsByEmployeeID getIncidentsByEmployeeID = allIncidentsEntities;*/
            var assignments = await _assignedIncidentRepository.GetAssignmentsByEmployeeIdAsync(employeeId);
            var incidentIds = assignments
                .Select(a => a.IncidentId)
                .Distinct()
                .ToList();
            var assignedIncidentsEntities = await _assignedIncidentRepository.GetIncidentsByIdsAsync(incidentIds);

            var incidents = await _incidentRepository.GetIncidentsByEmployeeId(employeeId);

            incidents.AssignedIncidents = _mapper.Map<List<TableFetchIncidentsDto>>(assignedIncidentsEntities);

            return incidents;
        }


        public async Task<Incident> GetIncidentById(int id)
        {
            return await _incidentRepository.GetIncidentById(id);
        }

        public async Task<Incident> CreateIncident(CreateIncidentDTO createIncidentDto)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(createIncidentDto.EmployeeId);
            if (employee == null)
            {
                throw new ArgumentException("Invalid employee ID");
            }

            List<string> documentUrls = new List<string>();
            if (createIncidentDto.DocumentUrls != null)
            {
                foreach (IFormFile document in createIncidentDto.DocumentUrls)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(document.FileName);
                    //change the upload path
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    documentUrls.Add($"/images/{fileName}");
                }
            }

             createIncidentDto.IncidentOccuredDate = createIncidentDto.IncidentOccuredDate.ToUniversalTime();
            var incident = _mapper.Map<Incident>(createIncidentDto);
            incident.ReportedBy = employee.Name;
            incident.RoleOfReporter = employee.Designation;
            incident.DocumentUrls = documentUrls;

/*            var lastEntity = await _incidentRepository.GetAllIncidents()
                                     .OrderByDescending(e => e.Id)
                                     .FirstOrDefault();*/
            if (createIncidentDto.IsDraft)
            {
                incident.IncidentStatus = "draft";
            }
            else
            {
                incident.IncidentStatus = "pending";
            }
           

            await _incidentRepository.AddIncident(incident);

            return incident;
        }

        public async Task UpdateIncident(int id, UpdateIncidentDTO updateIncidentDto)
        {
            var incident = await _incidentRepository.GetIncidentById(id);
            if (incident == null)
            {
                throw new ArgumentException("Invalid incident ID");
            }

            await _incidentRepository.UpdateIncident(incident, updateIncidentDto);
        }

        public async Task UserUpdateIncident(int id, CreateIncidentDTO updateIncidentDto)
        {
            var incident = await _incidentRepository.GetIncidentById(id);
            if (incident == null)
            {
                throw new ArgumentException("Invalid incident ID");
            }

            List<string> documentUrls = new List<string>();
            if (updateIncidentDto.DocumentUrls != null)
            {
                foreach (IFormFile document in updateIncidentDto.DocumentUrls)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(document.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    documentUrls.Add($"/images/{fileName}");
                }
            }

            updateIncidentDto.IncidentOccuredDate = DateTime.SpecifyKind(updateIncidentDto.IncidentOccuredDate, DateTimeKind.Utc);
            updateIncidentDto.EmployeeId = incident.EmployeeId;
            if (updateIncidentDto.IsDraft ==true)
            {
                incident.IncidentStatus = "draft";
            }
            else
            {
                incident.IncidentStatus = "pending";
            }

            await _incidentRepository.UserUpdateIncident(incident, updateIncidentDto);
        }

        public async Task<UpdateIncidentUserDto> GetUserUpdateIncident(int id)
        {
           //null checks

           var incident= await _incidentRepository.GetIncidentById(id);

            return _mapper.Map<UpdateIncidentUserDto>(incident);

        }

        public async Task<GetIncidentsByEmployeeID> GetIncidentsAdmins()
        {
            // Null check or any additional logic
            return  await _incidentRepository.GetAllIncidentsWithBarChart();
        }
    }
}
