using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Preventyon.Controllers;
using Preventyon.Models;
using Preventyon.Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PreventyonUnitTest
{
    [TestFixture]
    public class AssignedIncidentControllerUnitTest
    {
        private Mock<IAssignedIncidentService> _mockAssignedIncidentService;
        private Mock<ILogger<AssignedIncidentController>> _mockLogger;
        private AssignedIncidentController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockAssignedIncidentService = new Mock<IAssignedIncidentService>();
            _mockLogger = new Mock<ILogger<AssignedIncidentController>>();
            _controller = new AssignedIncidentController(_mockAssignedIncidentService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task AssignIncidentToEmployees_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            int incidentId = 1;
            List<int> employeeIds = new List<int> { 1, 2, 3 };

            _mockAssignedIncidentService
                .Setup(s => s.AssignIncidentToEmployeesAsync(incidentId, employeeIds))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AssignIncidentToEmployees(incidentId, employeeIds);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task AssignIncidentToEmployees_IncidentNotFound_ReturnsNotFound()
        {
            // Arrange
            int incidentId = 1;
            List<int> employeeIds = new List<int> { 1, 2, 3 };

            _mockAssignedIncidentService
                .Setup(s => s.AssignIncidentToEmployeesAsync(incidentId, employeeIds))
                .Throws(new KeyNotFoundException("Incident not found"));

            // Act
            var result = await _controller.AssignIncidentToEmployees(incidentId, employeeIds);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Incident not found", notFoundResult.Value);
        }

        [Test]
        public async Task AssignIncidentToEmployees_InvalidEmployeeIds_ReturnsBadRequest()
        {
            // Arrange
            var incidentId = 1;
            var employeeIds = new List<int> { -1, -2, -3 }; // Invalid employee IDs
            _mockAssignedIncidentService
                .Setup(s => s.AssignIncidentToEmployeesAsync(incidentId, employeeIds))
                .ThrowsAsync(new ArgumentException("Invalid employee IDs"));

            // Act
            var result = await _controller.AssignIncidentToEmployees(incidentId, employeeIds);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid employee IDs", badRequestResult.Value);
        }

        [Test]
        public async Task AssignIncidentToEmployees_EmptyEmployeeIds_ReturnsBadRequest()
        {
            // Arrange
            var incidentId = 1;
            var employeeIds = new List<int>(); // Empty employee IDs
            _mockAssignedIncidentService
                .Setup(s => s.AssignIncidentToEmployeesAsync(incidentId, employeeIds))
                .ThrowsAsync(new ArgumentException("Employee IDs cannot be empty"));

            // Act
            var result = await _controller.AssignIncidentToEmployees(incidentId, employeeIds);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Employee IDs cannot be empty", badRequestResult.Value);
        }

        [Test]
        public async Task AssignIncidentToEmployees_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var incidentId = 1;
            var employeeIds = new List<int> { 1, 2, 3 };
            _mockAssignedIncidentService
                .Setup(s => s.AssignIncidentToEmployeesAsync(incidentId, employeeIds))
                .ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _controller.AssignIncidentToEmployees(incidentId, employeeIds);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Some error", badRequestResult.Value);
        }

        [Test]
        public async Task GetAssignedIncidentsForEmployee_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var employeeId = 1;
            var mockIncidents = new List<Incident> { new Incident { Id = 1, IncidentTitle = "Test Incident" } };
            _mockAssignedIncidentService
                .Setup(s => s.GetAssignedIncidentsForEmployeeAsync(employeeId))
                .ReturnsAsync(mockIncidents);

            // Act
            var result = await _controller.GetAssignedIncidentsForEmployee(employeeId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(mockIncidents, okResult.Value);
        }

        [Test]
        public async Task GetAssignedIncidentsForEmployee_EmptyResult_ReturnsOkResult()
        {
            // Arrange
            var employeeId = 1;
            var mockIncidents = new List<Incident>();
            _mockAssignedIncidentService
                .Setup(s => s.GetAssignedIncidentsForEmployeeAsync(employeeId))
                .ReturnsAsync(mockIncidents);

            // Act
            var result = await _controller.GetAssignedIncidentsForEmployee(employeeId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(mockIncidents, okResult.Value);
        }

        [Test]
        public async Task GetAssignedIncidentsForEmployee_NonExistentEmployee_ReturnsEmptyList()
        {
            // Arrange
            var employeeId = 9999; // Non-existent employee ID
            var mockIncidents = new List<Incident>();
            _mockAssignedIncidentService
                .Setup(s => s.GetAssignedIncidentsForEmployeeAsync(employeeId))
                .ReturnsAsync(mockIncidents);

            // Act
            var result = await _controller.GetAssignedIncidentsForEmployee(employeeId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(mockIncidents, okResult.Value);
        }
    }
}
