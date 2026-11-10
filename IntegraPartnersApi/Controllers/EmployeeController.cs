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
        EmployeeDataAccess eda = new EmployeeDataAccess();
        // GET: api/<EmployeeController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                //get all employees
                return Ok(eda.GetEmployees());
            }
            catch(Exception ex)
            {
                //TODO: write exception to error log in DB or EventViewer and send generic error message to client
                return BadRequest(ex);
            }
        }

        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                //get one employee based on Id
                var employees = eda.GetEmployees();
                return Ok(employees.FirstOrDefault(e => e.Id == id));
            }
            catch (Exception ex)
            {
                //TODO: write exception to error log in DB or EventViewer and send generic error message to client
                return BadRequest(ex);
            }
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public IActionResult Post([FromBody] Employee value)
        {
            try
            {
                if (value.Id == 0)
                {
                    var employees = eda.GetEmployees();

                    //check if email exists when adding new employees so that we don't add duplicate employees
                    var emailExists = employees.FirstOrDefault(e => e.Email == value.Email);
                    if(emailExists == null)
                    {
                        eda.AddEmployee(value);
                        return Ok(new { result = "Employee Added", employees = eda.GetEmployees() });
                    }
                    else
                    {
                        return BadRequest(new { result = "Email already exists", employees = eda.GetEmployees() });
                    }
                }
                else
                {
                    //update employee
                    eda.UpdateEmployee(value);
                    return Ok(new { result = "Employee Updated", employees = eda.GetEmployees() });
                }
            }
            catch (Exception ex)
            {
                //TODO: write exception to error log in DB or EventViewer and send generic error message to client
                return BadRequest(ex);
            }
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                //delete employee from list
                var employees = eda.GetEmployees();
                var employee = employees.FirstOrDefault(e => e.Id == id);
                if (employee != null)
                {
                    eda.DeleteEmployee(employee);
                }
                return Ok(new { result = "Employee Deleted", employees = eda.GetEmployees() });
            }
            catch (Exception ex)
            {
                //TODO: write exception to error log in DB or EventViewer and send generic error message to client
                return BadRequest(ex);
            }
        }
    }
}
