using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Repository;
using Preventyon.Repository.IRepository;

namespace Preventyon.Controllers
{
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class IncidentController : ControllerBase
    {
        private readonly IncidentRepository _incidentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public IncidentController(IncidentRepository incidentRepository, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _incidentRepository = incidentRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Incident>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidents()
        {
            return Ok(await _incidentRepository.GetAllIncidents());
        }



        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Incident>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Incident>>> GetDraftIncidentsByEmployeeId(int employeeId)
        {
            var draftIncidents = await _incidentRepository.GetDraftIncidentsByEmployeeId(employeeId);
            return Ok(draftIncidents);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Incident>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidentsByEmployeeId(int employeeId)
        {
            var Incidents = await _incidentRepository.GetIncidentsByEmployeeId(employeeId);
            return Ok(Incidents);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Incident))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Incident>> GetIncident(int id)
        {
            var incident = await _incidentRepository.GetIncidentById(id);

            if (incident == null)
            {
                return NotFound();
            }

            return Ok(incident);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Incident))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Incident>> GetUserUpdateIncident(int id)
        {
            var incident = await _incidentRepository.GetIncidentById(id);

            if (incident == null)
            {
                return NotFound();
            }
            CreateIncidentDTO userUpdateIncident = _mapper.Map<CreateIncidentDTO>(incident);
            return Ok(userUpdateIncident);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Incident))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Incident>> CreateIncident([FromForm] CreateIncidentDTO createIncidentDto)
        {
            if (createIncidentDto == null)
            {
                return BadRequest("Incident data is required");
            }

            if (string.IsNullOrEmpty(createIncidentDto.IncidentTitle) || createIncidentDto.IncidentOccuredDate == default || string.IsNullOrEmpty(createIncidentDto.IncidentDescription))
            {
                return BadRequest("Title, Incident Occurred Date,Incident Description are required");
            }
            createIncidentDto.EmployeeId = 2;//todo
            var employee = await _employeeRepository.GetEmployeeByIdAsync(createIncidentDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest("invalid employee id");
            }
            try
            {

                List<string> documentUrls = [];
                if (createIncidentDto.DocumentFiles != null)
                {
                    foreach (IFormFile document in createIncidentDto.DocumentFiles)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(document.FileName);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await createIncidentDto.DocumentFiles[0].CopyToAsync(stream);
                        }

                        var request = HttpContext.Request;
                        var baseUrl = $"{request.Scheme}://{request.Host}";
                        documentUrls.Add($"{baseUrl}/uploads/{fileName}");
                    }

                }

                DateTime utcDate = createIncidentDto.IncidentOccuredDate.ToUniversalTime();
                createIncidentDto.IncidentOccuredDate = utcDate;
                Incident incident = _mapper.Map<Incident>(createIncidentDto);
                incident.DocumentUrls = documentUrls;
                incident.ReportedBy = employee.Name;
                await _incidentRepository.AddIncident(incident);



                return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incident);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateIncident(int id, [FromBody] UpdateIncidentDTO updateIncidentDto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid incident ID");
            }

            if (updateIncidentDto == null)
            {
                return BadRequest("Incident update data is required");
            }

            var incident = await _incidentRepository.GetIncidentById(id);
            if (incident == null)
            {
                return NotFound();
            }

            try
            {
                await _incidentRepository.UpdateIncident(incident, updateIncidentDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UserUpdateIncident(int id, [FromBody] CreateIncidentDTO createIncidentDto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid incident ID");
            }

            if (createIncidentDto == null)
            {
                return BadRequest("Incident update data is required");
            }

            var incident = await _incidentRepository.GetIncidentById(id);
            if (incident == null)
            {
                return NotFound();
            }

            try
            {
                await _incidentRepository.UserUpdateIncident(incident, createIncidentDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
