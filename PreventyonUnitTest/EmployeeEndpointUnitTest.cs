using AutoMapper;
using Moq;
using Preventyon.Models.DTO.Employee;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Models;
using Preventyon.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Preventyon.EndPoints;
using RequestDemoMinimal.models;
using Microsoft.AspNetCore.Builder;
using System.Net;
using Preventyon.Data;

namespace PreventyonUnitTest
{
    [TestFixture]
    public class EmployeeEndpointUnitTest
    {
        private Mock<IEmployeeRepository> _employeeRepoMock;
        private Mock<IIncidentRepository> _incidentRepoMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ApiContext> _apiContextMock;
        private UpdateEmployeeRoleDTO _employeeDto;
        private Employee _employee;
        private UpdateIncidentByReviewDto _incidentDto;
        private Incident _incident;

        [SetUp]
        public void Setup()
        {
            _employeeRepoMock = new Mock<IEmployeeRepository>();
            _incidentRepoMock = new Mock<IIncidentRepository>();
            _mapperMock = new Mock<IMapper>();
            _apiContextMock = new Mock<ApiContext>();

            _employeeDto = new UpdateEmployeeRoleDTO { Id = 1, RoleID = 1 };
            _employee = new Employee { Id = 1, Name = "John Doe", Email = "john.doe@example.com", Department = "IT", RoleId = 1 };

            _incidentDto = new UpdateIncidentByReviewDto { Id = 1, IsSubmittedForReview = true };
            _incident = new Incident { Id = 1, IncidentTitle = "Incident Title", IsSubmittedForReview = true };
        }

        [Test]
        public async Task UpdateEmployee_ValidId_ReturnsOk()
        {
            // Arrange
            _employeeRepoMock.Setup(repo => repo.FindAsync(It.IsAny<int>())).ReturnsAsync(_employee);
            _employeeRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map(It.IsAny<UpdateEmployeeRoleDTO>(), It.IsAny<Employee>()));
            _mapperMock.Setup(m => m.Map<UpdateEmployeeRoleDTO>(It.IsAny<Employee>())).Returns(_employeeDto);

            // Act
            var result = await EmployeeEndPoints.UpdateEmployee(1, _employeeDto, _mapperMock.Object, _employeeRepoMock.Object);

            // Assert
            var okResult = result as IResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, ((Microsoft.AspNetCore.Http.HttpResults.Ok<APIResponse>)okResult).StatusCode);
        }

        [Test]
        public async Task UpdateEmployee_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _employeeRepoMock.Setup(repo => repo.FindAsync(It.IsAny<int>())).ReturnsAsync((Employee)null);

            // Act
            var result = await EmployeeEndPoints.UpdateEmployee(1, _employeeDto, _mapperMock.Object, _employeeRepoMock.Object);

            // Assert
            var notFoundResult = result as IResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, ((Microsoft.AspNetCore.Http.HttpResults.NotFound<APIResponse>)notFoundResult).StatusCode);
        }

        [Test]
        public async Task IncidentApproval_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _incidentRepoMock.Setup(repo => repo.GetIncidentById(It.IsAny<int>())).ReturnsAsync((Incident)null);

            // Act
            var result = await EmployeeEndPoints.incidentApproval(1, _incidentRepoMock.Object);

            // Assert
            var notFoundResult = result as IResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, ((Microsoft.AspNetCore.Http.HttpResults.NotFound<APIResponse>)notFoundResult).StatusCode);
        }

        [Test]
        public async Task IncidentApproval_AlreadyClosed_ReturnsBadRequest()
        {
            // Arrange
            _incident.IncidentStatus = "closed";
            _incidentRepoMock.Setup(repo => repo.GetIncidentById(It.IsAny<int>())).ReturnsAsync(_incident);

            // Act
            var result = await EmployeeEndPoints.incidentApproval(1, _incidentRepoMock.Object);

            // Assert
            var badRequestResult = result as IResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, ((Microsoft.AspNetCore.Http.HttpResults.BadRequest<APIResponse>)badRequestResult).StatusCode);
        }

        [Test]
        public async Task IncidentApproval_SubmittedForReviewWithoutCorrection_ReturnsBadRequest()
        {
            // Arrange
            _incident.IsSubmittedForReview = true;
            _incident.Correction = null; // No corrective action provided
            _incidentRepoMock.Setup(repo => repo.GetIncidentById(It.IsAny<int>())).ReturnsAsync(_incident);

            // Act
            var result = await EmployeeEndPoints.incidentApproval(1, _incidentRepoMock.Object);

            // Assert
            var badRequestResult = result as IResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, ((Microsoft.AspNetCore.Http.HttpResults.BadRequest<APIResponse>)badRequestResult).StatusCode);
        }

        [Test]
        public async Task UpdateIncidentByReview_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _incidentRepoMock.Setup(repo => repo.GetIncidentById(It.IsAny<int>())).ReturnsAsync((Incident)null);

            // Act
            var result = await EmployeeEndPoints.updateIncidentByReview(1, _incidentDto, _mapperMock.Object, _incidentRepoMock.Object);

            // Assert
            var notFoundResult = result as IResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, ((Microsoft.AspNetCore.Http.HttpResults.NotFound<APIResponse>)notFoundResult).StatusCode);
        }

        /* [Test]
         public async Task UpdateIncidentByReview_ValidId_ReturnsOk()
         {
             // Arrange
             _incidentRepoMock.Setup(repo => repo.GetIncidentById(It.IsAny<int>())).ReturnsAsync(_incident);
             _incidentRepoMock.Setup(repo => repo.UpdateIncidentAsync(It.IsAny<Incident>())).Returns(Task.CompletedTask);
             _mapperMock.Setup(m => m.Map(It.IsAny<UpdateIncidentByReviewDto>(), It.IsAny<Incident>()));
             _mapperMock.Setup(m => m.Map<UpdateIncidentByReviewDto>(It.IsAny<Incident>())).Returns(_incidentDto);

             // Act
             var result = await EmployeeEndPoints.updateIncidentByReview(1, _incidentDto, _mapperMock.Object, _incidentRepoMock.Object);

             // Assert
             var okResult = result as IResult;
             Assert.IsNotNull(okResult);
             Assert.AreEqual(200, ((Microsoft.AspNetCore.Http.HttpResults.Ok<APIResponse>)okResult).StatusCode);
         }*/



        /*[Test]
        public async Task AcceptIncidents_ValidIds_ReturnsOk()
        {
            // Arrange
            var assignedIncidents = new List<AssignedIncident>
            {
                new AssignedIncident { IncidentId = 1, Accepted = 0 }
            };
            _apiContextMock.Setup(c => c.AssignedIncidents.Where(a => a.IncidentId == 1).ToListAsync()).ReturnsAsync(assignedIncidents);
            _apiContextMock.Setup(c => c.Employees.FindAsync(It.IsAny<int>())).ReturnsAsync(_employee);
            _apiContextMock.Setup(c => c.Incident.FindAsync(It.IsAny<int>())).ReturnsAsync(_incident);
            _apiContextMock.Setup(c => c.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await EmployeeEndPoints.acceptIncidents(1, 1, _apiContextMock.Object);

            // Assert
            var okResult = result as IResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, ((Microsoft.AspNetCore.Http.HttpResults.Ok<APIResponse>)okResult).StatusCode);
            Assert.AreEqual(_employee.Name, _incident.ActionAssignedTo);
        }*/

        /*[Test]
        public async Task AcceptIncidents_InvalidEmployeeId_ReturnsNotFound()
        {
            // Arrange
            _apiContextMock.Setup(c => c.Employees.FindAsync(It.IsAny<int>())).ReturnsAsync((Employee)null);

            // Act
            var result = await EmployeeEndPoints.acceptIncidents(1, 1, _apiContextMock.Object);

            // Assert
            var notFoundResult = result as IResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, ((Microsoft.AspNetCore.Http.HttpResults.NotFound<APIResponse>)notFoundResult).StatusCode);
        }*/

        /*[Test]
        public async Task AcceptIncidents_InvalidIncidentId_ReturnsNotFound()
        {
            // Arrange
            _apiContextMock.Setup(c => c.Incident.FindAsync(It.IsAny<int>())).ReturnsAsync((Incident)null);

            // Act
            var result = await EmployeeEndPoints.acceptIncidents(1, 1, _apiContextMock.Object);

            // Assert
            var notFoundResult = result as IResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, ((Microsoft.AspNetCore.Http.HttpResults.NotFound<APIResponse>)notFoundResult).StatusCode);
        }*/


    }
}



