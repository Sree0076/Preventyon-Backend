using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Preventyon.Controllers;
using Preventyon.Models;
using Preventyon.Models.DTO.Employee;
using Preventyon.Models.DTO;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PreventyonUnitTest
{
    [TestFixture]
    public class IncidentControllerUnitTest
    {
        private Mock<IIncidentService> _mockIncidentService;
        private Mock<IEmployeeService> _mockEmployeeService;
        private Mock<IEmailService> _mockEmailService;
        private IncidentController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockIncidentService = new Mock<IIncidentService>();
            _mockEmployeeService = new Mock<IEmployeeService>();
            _mockEmailService = new Mock<IEmailService>();
            _controller = new IncidentController(_mockIncidentService.Object, _mockEmployeeService.Object, _mockEmailService.Object);
        }

        [Test]
        public async Task GetIncidents_ShouldReturnOkWithIncidents()
        {
            // Arrange
            var incidents = new List<Incident> { new Incident { Id = 1, IncidentNo = "INC001" } };
            _mockIncidentService.Setup(service => service.GetAllIncidents()).ReturnsAsync(incidents);

            // Act
            var result = await _controller.GetIncidents();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(incidents, okResult.Value);
        }

        [Test]
        public async Task GetIncident_ExistingId_ShouldReturnOkWithIncident()
        {
            // Arrange
            var incident = new Incident { Id = 1, IncidentNo = "INC001" };
            _mockIncidentService.Setup(service => service.GetIncidentById(1)).ReturnsAsync(incident);

            // Act
            var result = await _controller.GetIncident(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(incident, okResult.Value);
        }

        [Test]
        public async Task GetIncident_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            _mockIncidentService.Setup(service => service.GetIncidentById(1)).ReturnsAsync((Incident)null);

            // Act
            var result = await _controller.GetIncident(1);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task CreateIncident_ValidData_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createIncidentDto = new CreateIncidentDTO { EmployeeId = 1, IncidentType = "Test", IncidentDescription = "Test Description" };
            var incident = new Incident { Id = 1, IncidentNo = "INC001" };
            _mockIncidentService.Setup(service => service.CreateIncident(createIncidentDto)).ReturnsAsync(incident);
            _mockEmailService.Setup(service => service.SendNotificationAsync(createIncidentDto.EmployeeId, incident)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateIncident(createIncidentDto);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            Assert.AreEqual(incident, createdAtActionResult.Value);
        }

        [Test]
        public async Task CreateIncident_InvalidData_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.CreateIncident(null);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task UpdateIncident_ValidData_ShouldReturnNoContent()
        {
            // Arrange
            var updateIncidentDto = new UpdateIncidentDTO { IncidentDescription = "Updated Description" };
            _mockIncidentService.Setup(service => service.UpdateIncident(1, updateIncidentDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateIncident(1, updateIncidentDto);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        [Test]
        public async Task UpdateIncident_InvalidId_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.UpdateIncident(0, new UpdateIncidentDTO());

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task UpdateIncident_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var updateIncidentDto = new UpdateIncidentDTO { IncidentDescription = "Updated Description" };
            _mockIncidentService.Setup(service => service.UpdateIncident(1, updateIncidentDto)).Throws<ArgumentException>();

            // Act
            var result = await _controller.UpdateIncident(1, updateIncidentDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetIncidentsByEmployeeId_ReturnsOkResult_WithIncidents()
        {
            // Arrange
            int employeeId = 1;
            var incidentsByEmployee = new GetIncidentsByEmployeeID();
            _mockEmployeeService.Setup(s => s.GetEmployeeByIdAsync(employeeId)).ReturnsAsync(new GetEmployeeRoleWithIDDTO { Role = new RoleDTO { Name = "Employee" } });
            _mockIncidentService.Setup(s => s.GetIncidentsByEmployeeId(employeeId)).ReturnsAsync(incidentsByEmployee);

            // Act
            var result = await _controller.GetIncidentsByEmployeeId(employeeId, true);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOf<GetIncidentsByEmployeeID>(okResult.Value);
        }

        [Test]
        public async Task UserUpdateIncident_ReturnsNoContent_WhenValidInput()
        {
            // Arrange
            int incidentId = 1;
            var updateIncidentDto = new UpdateIncidentUserDto();
            _mockIncidentService.Setup(s => s.UserUpdateIncident(incidentId, updateIncidentDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UserUpdateIncident(incidentId, updateIncidentDto);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

       


        [Test]
        public async Task GetAdminIncidentsWithBarChart_ReturnsOkResult()
        {
            // Arrange
            var incidentsWithBarChart = new GetIncidentsByEmployeeID();
            _mockIncidentService.Setup(s => s.GetIncidentsAdmins()).ReturnsAsync(incidentsWithBarChart);

            // Act
            var result = await _controller.GetAdminIncidentsWithBarChart();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(incidentsWithBarChart, okResult.Value);
        }

        [Test]
        public async Task CreateIncident_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("EmployeeId", "EmployeeId is required");
            var createIncidentDto = new CreateIncidentDTO();

            // Act
            var result = await _controller.CreateIncident(createIncidentDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task CreateIncident_ReturnsBadRequest_WhenEmailNotSent()
        {
            // Arrange
            var createIncidentDto = new CreateIncidentDTO { EmployeeId = 1 };
            var createdIncident = new Incident { Id = 1 };
            _mockIncidentService.Setup(s => s.CreateIncident(createIncidentDto)).ReturnsAsync(createdIncident);
            _mockEmailService.Setup(s => s.SendNotificationAsync(It.IsAny<int>(), It.IsAny<Incident>())).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateIncident(createIncidentDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }
    }
}
