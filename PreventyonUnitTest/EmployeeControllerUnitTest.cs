using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Preventyon.Controllers;
using Preventyon.Data;
using Preventyon.Models.DTO.Employee;
using Preventyon.Models;
using Preventyon.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Preventyon.Models.DTO;

namespace PreventyonUnitTest
{
    [TestFixture]
    public class EmployeeControllerUnitTest
    {
        private EmployeeController _controller;
        private IEmployeeService _employeeService;
        private ApiContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApiContext(options);
            _employeeService = new Mock<IEmployeeService>().Object;
            _controller = new EmployeeController(_employeeService, _context);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose of _context to release resources
            _context.Dispose();
        }

        [Test]
        public async Task PostEmployee_ValidEmployee_ReturnsOk()
        {
            // Arrange
            var newEmployee = new CreateEmployeeDTO { Name = "John Doe", Department = "IT", RoleId = 1 };
            var createdEmployee = new Employee { Id = 1, Name = "John Doe", Department = "IT", RoleId = 1, Designation = "Developer", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock.Setup(s => s.AddEmployee(It.IsAny<CreateEmployeeDTO>())).ReturnsAsync(createdEmployee);
            _controller = new EmployeeController(employeeServiceMock.Object, _context);

            // Act
            var result = await _controller.PostEmployee(newEmployee) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(createdEmployee, result.Value);
        }


        [Test]
        public async Task GetEmployees_ReturnsOkWithEmployees()
        {
            // Arrange
            var employees = new List<GetEmployeesDTO> { new GetEmployeesDTO { Id = 1, Name = "John Doe", Email = "john.doe@example.com", Department = "IT", Designation = "Developer" } };
            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock.Setup(s => s.GetEmployees()).ReturnsAsync(employees);
            _controller = new EmployeeController(employeeServiceMock.Object, _context);

            // Act
            var result = await _controller.GetEmployees() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(employees, result.Value);
        }

        [Test]
        public async Task GetEmployeeByIdAsync_ValidId_ReturnsOkWithEmployee()
        {
            // Arrange
            var employeeId = 1;
            var employee = new GetEmployeeRoleWithIDDTO { Id = 1, Name = "John Doe", Email = "john.doe@example.com", Department = "IT", Designation = "Developer", Role = new RoleDTO { Id = 1, Name = "Admin" } };
            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(employeeId)).ReturnsAsync(employee);
            _controller = new EmployeeController(employeeServiceMock.Object, _context);

            // Act
            var result = await _controller.GetEmployeeByIdAsync(employeeId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(employee, result.Value);
        }

        [Test]
        public async Task GetEmployeeByTokenAsync_ValidToken_ReturnsOkWithEmployee()
        {
            // Arrange
            var token = "valid.jwt.token";
            var employee = new GetEmployeeRoleWithIDDTO { Id = 1, Name = "John Doe", Email = "john.doe@example.com", Department = "IT", Designation = "Developer", Role = new RoleDTO { Id = 1, Name = "Admin" } };
            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock.Setup(s => s.GetEmployeeByTokenAsync(It.IsAny<string>(), It.IsAny<ApiContext>())).ReturnsAsync(employee);
            _controller = new EmployeeController(employeeServiceMock.Object, _context);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            // Act
            var result = await _controller.GetEmployeeByTokenAsync() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(employee, result.Value);
        }

        [Test]
        public async Task GetEmployeeByTokenAsync_MissingToken_ReturnsBadRequest()
        {
            // Arrange
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.GetEmployeeByTokenAsync() as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.AreEqual("Authorization header missing or invalid", result.Value);
        }

        [Test]
        public async Task PostEmployee_IncompleteDTO_ReturnsBadRequest()
        {
            // Arrange
            var newEmployee = new CreateEmployeeDTO { Name = "John Doe" }; // Missing required fields
            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock.Setup(s => s.AddEmployee(It.IsAny<CreateEmployeeDTO>())).ThrowsAsync(new ArgumentException("Invalid data"));
            _controller = new EmployeeController(employeeServiceMock.Object, _context);

            // Act
            var result = await _controller.PostEmployee(newEmployee) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PostEmployee_InvalidRoleId_ReturnsBadRequest()
        {
            // Arrange
            var newEmployee = new CreateEmployeeDTO { Name = "John Doe", Department = "IT", RoleId = -1 }; // Invalid RoleId
            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock.Setup(s => s.AddEmployee(It.IsAny<CreateEmployeeDTO>())).ThrowsAsync(new ArgumentException("Invalid RoleId"));
            _controller = new EmployeeController(employeeServiceMock.Object, _context);

            // Act
            var result = await _controller.PostEmployee(newEmployee) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Test]
        public async Task GetEmployees_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock.Setup(s => s.GetEmployees()).ThrowsAsync(new Exception("Service error"));
            _controller = new EmployeeController(employeeServiceMock.Object, _context);

            // Act
            var result = await _controller.GetEmployees() as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.AreEqual("Service error", (result.Value as Exception).Message);
        }


        [Test]
        public async Task GetEmployeeByIdAsync_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var employeeId = 1;
            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(employeeId)).ThrowsAsync(new Exception("Service error"));
            _controller = new EmployeeController(employeeServiceMock.Object, _context);

            // Act
            var result = await _controller.GetEmployeeByIdAsync(employeeId) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.AreEqual("Service error", (result.Value as Exception).Message);
        }

    }
}
