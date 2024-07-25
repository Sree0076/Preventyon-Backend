using AutoMapper;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Models;
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
        {
            return await _incidentRepository.GetAllIncidents();
        }

        public async Task<GetIncidentsByEmployeeID> GetIncidentsByEmployeeId(int employeeId)

        {
            var assignedIncidentsEntities = await _assignedIncidentRepository.GetAssignmentsByEmployeeIdAsync(employeeId);
            var allIncidentsEntities = await _incidentRepository.GetIncidentsByEmployeeId(employeeId);
            var assignedIncidentTasks = assignedIncidentsEntities
                .Select(async assignment => await _incidentRepository.GetIncidentById(assignment.IncidentId))
                .ToList();
            var assignedIncidents = (await Task.WhenAll(assignedIncidentTasks))
                .Where(incident => incident != null)
                .ToList();
            var allIncidents = await _incidentRepository.GetIncidentsByEmployeeId(employeeId);
            allIncidents.AssignedIncidents = _mapper.Map<List<TableFetchIncidentsDto>>(assignedIncidents);
            GetIncidentsByEmployeeID getIncidentsByEmployeeID = allIncidents;
           return  getIncidentsByEmployeeID;
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
            incident.DocumentUrls = documentUrls;
            incident.IncidentStatus = "pending";

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

        public async Task UserUpdateIncident(int id, UpdateIncidentUserDto updateIncidentDto)
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
         
            await _incidentRepository.UserUpdateIncident(incident, updateIncidentDto);
        }
    }
}
