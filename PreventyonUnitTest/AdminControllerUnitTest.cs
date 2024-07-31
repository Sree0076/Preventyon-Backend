using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Preventyon.Controllers;
using Preventyon.Models;
using Preventyon.Models.DTO.AdminDTO;
using Preventyon.Service.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Preventyon.Tests
{
    [TestFixture]
    public class AdminsControllerTests
    {
        private Mock<IAdminService> _mockAdminService;
        private Mock<ILogger<AdminsController>> _mockLogger;
        private AdminsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockAdminService = new Mock<IAdminService>();
            _mockLogger = new Mock<ILogger<AdminsController>>();
            _controller = new AdminsController(_mockAdminService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllAdmins_ValidId_ReturnsOkResult_WithListOfAdmins()
        {
            // Arrange
            int id = 1;
            var adminList = new List<GetAllAdminsDto> { new GetAllAdminsDto() }; // Sample admin list
            _mockAdminService.Setup(service => service.GetAllAdminsAsync(id))
                             .ReturnsAsync(adminList);

            // Act
            var result = await _controller.GetAllAdmins(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<IEnumerable<GetAllAdminsDto>>(okResult.Value);
            Assert.AreEqual(adminList, okResult.Value);
        }

        [Test]
        public async Task GetAllAdmins_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            int id = 1;
            var adminList = new List<GetAllAdminsDto>();
            _mockAdminService.Setup(service => service.GetAllAdminsAsync(id))
                             .ReturnsAsync(adminList);

            // Act
            var result = await _controller.GetAllAdmins(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<IEnumerable<GetAllAdminsDto>>(okResult.Value);
            Assert.AreEqual(adminList, okResult.Value);
        }

        [Test]
        public async Task GetAllAdmins_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            int id = 1;
            _mockAdminService.Setup(service => service.GetAllAdminsAsync(id))
                             .ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _controller.GetAllAdmins(id);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Error", badRequestResult.Value);
        }

        [Test]
        public async Task AddAdmin_ValidAdmin_ReturnsOkResult()
        {
            // Arrange
            var createAdminDto = new CreateAdminDTO
            {
                EmployeeId = 1,
                AssignedBy = 2,
                isIncidentMangenet = true,
                isUserMangenet = false,
                Status = true
            };
            var admin = new Admin
            {
                AdminId = 1,
                EmployeeId = createAdminDto.EmployeeId,
                AssignedBy = createAdminDto.AssignedBy,
                Status = createAdminDto.Status
            };
            _mockAdminService.Setup(s => s.AddAdminAsync(createAdminDto)).ReturnsAsync(admin);

            // Act
            var result = await _controller.AddAdmin(createAdminDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(admin, okResult.Value);
        }

        [Test]
        public async Task AddAdmin_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var createAdminDto = new CreateAdminDTO();
            _mockAdminService.Setup(s => s.AddAdminAsync(createAdminDto)).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _controller.AddAdmin(createAdminDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Error", badRequestResult.Value);
        }

        /*[Test]
        public async Task AddAdmin_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Invalid model state");

            var invalidCreateAdminDTO = new CreateAdminDTO(); // Missing required fields

            // Act
            var result = await _controller.AddAdmin(invalidCreateAdminDTO);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Invalid model state", badRequestResult.Value);
        }
*/
        /*[Test]
        public async Task UpdateAdmin_ValidUpdate_ReturnsNoContent()
        {
            // Arrange
            int adminId = 1;
            var updateAdminDto = new UpdateAdminDTO { *//* populate with valid values *//* };
            _mockAdminService.Setup(s => s.UpdateAdminAsync(adminId, updateAdminDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateAdmin(adminId, updateAdminDto);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }
*/
        /*[Test]
        public async Task UpdateAdmin_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            int adminId = 1;
            var updateAdminDto = new UpdateAdminDTO { *//* populate with valid values *//* };
            _mockAdminService.Setup(s => s.UpdateAdminAsync(adminId, updateAdminDto)).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _controller.UpdateAdmin(adminId, updateAdminDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Error", badRequestResult.Value);
        }*/

        /*[Test]
        public async Task UpdateAdmin_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Invalid model state");

            // Act
            var result = await _controller.UpdateAdmin(1, new UpdateAdminDTO { *//* invalid data *//* });

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Invalid model state", badRequestResult.Value);
        }*/

       /* [Test]
        public async Task AddAdmin_NullCreateAdminDto_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.AddAdmin(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Invalid admin data", badRequestResult.Value);
        }*/

        [Test]
        public async Task AddAdmin_InvalidAssignedBy_ReturnsBadRequest()
        {
            // Arrange
            var createAdminDto = new CreateAdminDTO { EmployeeId = 1, AssignedBy = -1, Status = true };
            _mockAdminService.Setup(service => service.AddAdminAsync(createAdminDto))
                             .ThrowsAsync(new Exception("Invalid AssignedBy"));

            // Act
            var result = await _controller.AddAdmin(createAdminDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Invalid AssignedBy", badRequestResult.Value);
        }
    }
}
