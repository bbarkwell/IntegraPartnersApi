using IntegraPartnersApi.Data;
using IntegraPartnersApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegraPartnersApi.Tests
{
    [TestClass]
    public class EmployeeTests
    {
        [TestMethod]
        public void AddNewEmployee()
        {
            var employee = new Employee { FirstName = "Johnny", LastName = "Appleseed", Email = "johnny.appleseed@example.com" };

            EmployeeDataAccess eda = new EmployeeDataAccess();
            eda.AddEmployee(employee);

        }

        [TestMethod]
        public void UpdateEmployee()
        {
            EmployeeDataAccess eda = new EmployeeDataAccess();
            var employees = eda.GetEmployees();

            var employee = employees.FirstOrDefault(e => e.Id == 10);
            
            if(employee != null)
            {
                eda.UpdateEmployee(employee);
            }
            

        }
    }
}
