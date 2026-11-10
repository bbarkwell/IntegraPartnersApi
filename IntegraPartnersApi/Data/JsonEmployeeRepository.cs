using IntegraPartnersApi.Models;
using System.Text.Json;

namespace IntegraPartnersApi.Data
{
    public class JsonEmployeeRepository : IEmployeeRepository
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _lock = new(1, 1);

        public JsonEmployeeRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "employees.json");
        }

        public async Task<IReadOnlyList<Employee>> GetAllAsync()
        {
            await _lock.WaitAsync();
            try { return await ReadAsync(); }
            finally { _lock.Release(); }
        }

        public async Task<Employee?> GetByIdAsync(int id) =>
            (await GetAllAsync()).FirstOrDefault(e => e.Id == id);

        public async Task<Employee?> GetByEmailAsync(string email) =>
            (await GetAllAsync()).FirstOrDefault(e =>
                string.Equals(e.Email, email, StringComparison.OrdinalIgnoreCase));

        public async Task<Employee> AddAsync(Employee employee)
        {
            await _lock.WaitAsync();
            try
            {
                var employees = await ReadAsync();
                employee.Id = employees.Count == 0 ? 1 : employees.Max(e => e.Id) + 1;
                employees.Add(employee);
                await WriteAsync(employees);
                return employee;
            }
            finally { _lock.Release(); }
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            await _lock.WaitAsync();
            try
            {
                var employees = await ReadAsync();
                var existing = employees.FirstOrDefault(e => e.Id == employee.Id);
                if (existing is null) return false;

                existing.FirstName = employee.FirstName;
                existing.LastName = employee.LastName;
                existing.Email = employee.Email;
                await WriteAsync(employees);
                return true;
            }
            finally { _lock.Release(); }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _lock.WaitAsync();
            try
            {
                var employees = await ReadAsync();
                if (employees.RemoveAll(e => e.Id == id) == 0) return false;
                await WriteAsync(employees);
                return true;
            }
            finally { _lock.Release(); }
        }

        private async Task<List<Employee>> ReadAsync()
        {
            if (!File.Exists(_filePath)) return new List<Employee>();
            await using var stream = File.OpenRead(_filePath);
            return await JsonSerializer.DeserializeAsync<List<Employee>>(stream) ?? new List<Employee>();
        }

        private async Task WriteAsync(List<Employee> employees)
        {
            await using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, employees,
                new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
