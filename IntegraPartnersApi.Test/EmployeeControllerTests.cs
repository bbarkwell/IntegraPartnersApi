using IntegraPartnersApi.Controllers;
using IntegraPartnersApi.Data;
using IntegraPartnersApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace IntegraPartnersApi.Test
{
    public class EmployeesControllerTests
    {
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var repo = new Mock<IEmployeeRepository>();
            repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Employee?)null);

            var controller = new EmployeesController(repo.Object);
            var result = await controller.GetById(99);

            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var employee = new Employee { Id = 1, FirstName = "James", LastName = "Anderson", Email = "james.ander@example.com" };
            var repo = new Mock<IEmployeeRepository>();
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);

            var controller = new EmployeesController(repo.Object);
            var result = await controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            
        }
    }
}
