using IntegraPartnersApi.Models;

namespace IntegraPartnersApi.Data
{
    public interface IEmployeeRepository
    {
        Task<IReadOnlyList<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByEmailAsync(string email);
        Task<Employee> AddAsync(Employee employee);
        Task<bool> UpdateAsync(Employee employee);   // false = not found
        Task<bool> DeleteAsync(int id);              // false = not found
    }
}
