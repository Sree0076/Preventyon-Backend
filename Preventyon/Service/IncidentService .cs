using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;
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

        public IncidentService(IIncidentRepository incidentRepository, IEmployeeRepository employeeRepository, IAssignedIncidentRepository assignedIncidentRepository, IMapper mapper)
        {
            _incidentRepository = incidentRepository;
            _employeeRepository = employeeRepository;
            _assignedIncidentRepository = assignedIncidentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Incident>> GetAllIncidents()
        {
            try
            {
                var incidents = await _incidentRepository.GetAllIncidents();
                return incidents ?? Enumerable.Empty<Incident>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<Incident>();
            }
        }

        public async Task<GetIncidentsByEmployeeID> GetIncidentsByEmployeeId(int employeeId)
        {
          
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


            var allincidents = await GetAllIncidents();
            var lastEntry = allincidents.OrderByDescending(i => i.Id).FirstOrDefault();
            if (string.IsNullOrEmpty(lastEntry.IncidentNo) || !lastEntry.IncidentNo.StartsWith("EXPINC"))
            {
                incident.IncidentNo = "EXPINC1";
            }
            else
            {
                var numberPart = lastEntry.IncidentNo.Substring(6);
                if (int.TryParse(numberPart, out int numericValue))
                {
                    incident.IncidentNo = $"EXPINC{numericValue + 1}";
                }
            }

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

        public async Task UserUpdateIncident(int id, UpdateIncidentUserDto updateIncidentDto)
        {
            var incident = await _incidentRepository.GetIncidentById(id);
            if (incident == null)
            {
                throw new ArgumentException("Invalid incident ID");
            }


            List<string> UserGivenOldDocumentUrls = updateIncidentDto.OldDocumentUrls ?? new List<string>();
            List<string> NewUploadedDocuments = new List<string>();

            if (updateIncidentDto.NewDocumentUrls != null)
            {
                foreach (IFormFile document in updateIncidentDto.NewDocumentUrls)
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

                    NewUploadedDocuments.Add($"/images/{fileName}");
                }
            }


            List<string> finalDocumentUrls = incident.DocumentUrls.Intersect(UserGivenOldDocumentUrls).Concat(NewUploadedDocuments).Distinct().ToList();
            Console.WriteLine(finalDocumentUrls);

            List<string> documentsToDelete = UserGivenOldDocumentUrls.Except(finalDocumentUrls).ToList();

            if (documentsToDelete.Count > 0)
            {
                foreach (string urlToDelete in documentsToDelete)
                {
                    var filePathToDelete = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", urlToDelete.TrimStart('/'));
                    if (File.Exists(filePathToDelete))
                    {
                        File.Delete(filePathToDelete);
                    }
                }

            }



            incident.DocumentUrls = finalDocumentUrls;

            updateIncidentDto.IncidentOccuredDate = DateTime.SpecifyKind(updateIncidentDto.IncidentOccuredDate, DateTimeKind.Utc);
            updateIncidentDto.EmployeeId = incident.EmployeeId;
            if (updateIncidentDto.IsDraft == true)
            {
                incident.IncidentStatus = "draft";
            }
            else
            {
                incident.IncidentStatus = "pending";
            }

            await _incidentRepository.UserUpdateIncident(incident, updateIncidentDto);
        }

        public async Task<GetUserUpdateIncidentDTO> GetUserUpdateIncident(int id)
        {

            var incident = await _incidentRepository.GetIncidentById(id);

            if (incident == null)
            {
                return null;
            }

            return _mapper.Map<UpdateIncidentUserDto>(incident);

        }

        public async Task<GetIncidentsByEmployeeID> GetIncidentsAdmins()
        {

            var incidentsByEmployee = await _incidentRepository.GetAllIncidentsWithBarChart();

            return incidentsByEmployee ?? new GetIncidentsByEmployeeID();

        }
    }
}
