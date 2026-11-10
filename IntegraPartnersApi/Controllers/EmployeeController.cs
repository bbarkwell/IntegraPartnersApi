using IntegraPartnersApi.Data;
using IntegraPartnersApi.Models;
using IntegraPartnersApi.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntegraPartnersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeController(IEmployeeRepository repo) => _repo = repo;

        // GET: api/<EmployeeController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll() =>
            Ok(await _repo.GetAllAsync());

        // GET api/<EmployeeController>/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _repo.GetByIdAsync(id);
            return employee is null ? NotFound() : Ok(employee);
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public async Task<ActionResult<Employee>> Create(Employee employee)
        {
            if (await _repo.GetByEmailAsync(employee.Email!) is not null)
                return Conflict(new { message = "Email already exists" });

            var created = await _repo.AddAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Employee employee)
        {
            employee.Id = id;
            var sameEmail = await _repo.GetByEmailAsync(employee.Email!);
            if (sameEmail is not null && sameEmail.Id != id)
                return Conflict(new { message = "Email already exists" });

            return await _repo.UpdateAsync(employee) ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            await _repo.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
