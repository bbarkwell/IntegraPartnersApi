using IntegraPartnersApi.Models;
using IntegraPartnersApi.Utility;
using Microsoft.AspNetCore.Mvc;

namespace IntegraPartnersApi.Data
{
    public class EmployeeDataAccess
    {

        public IEnumerable<Employee> GetEmployees()
        {
            return JsonHelper.ReadFromJsonFile<Employee>();
        }

        
        public IEnumerable<Employee> AddEmployee(Employee value)
        {
            var employees = JsonHelper.ReadFromJsonFile<Employee>();

            //get max Id from employees list and increment by 1 for new employee
            var maxId = employees.Max(e => e.Id);
            value.Id = maxId + 1;

            //add new employee to the file
            employees.Add(value);
            JsonHelper.WriteToJsonFile(employees);

            return employees;
        }

        
        public IEnumerable<Employee> UpdateEmployee(Employee value)
        {
            var employees = JsonHelper.ReadFromJsonFile<Employee>();
            var employee = employees.FirstOrDefault(e => e.Id == value.Id);

            if (employee == null)
            {
                return null;
            }
            //update employee values
            employee.FirstName = value.FirstName;
            employee.LastName = value.LastName;
            employee.Email = value.Email;
            JsonHelper.WriteToJsonFile(employees);

            return employees;
        }

        public IEnumerable<Employee> DeleteEmployee(Employee value)
        {
            var employees = JsonHelper.ReadFromJsonFile<Employee>();
            var employee = employees.FirstOrDefault(e => e.Id == value.Id);

            if (employee == null)
            {
                return null;
            }
            employees.Remove(employee);
            JsonHelper.WriteToJsonFile(employees);

            return employees;
        }
    }
}
