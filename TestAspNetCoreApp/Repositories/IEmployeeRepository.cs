using TestAspNetCoreApp.Dtos;
using TestAspNetCoreApp.Models.Data;

namespace TestAspNetCoreApp.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

        Task<Employee> FindEmployeeAsync(Guid EmployeeId);

        Task<Employee> UpdateEmployeeAsync(Guid id,EmployeeDto EditedEmployee);

        Task<Employee> DeleteEmployeeAsync(Guid EmployeeId);

        Task<Employee> CreateEmployeeAsync(Employee NewEmployee);

        Task<IEnumerable<Employee>> SearchEmployees(string searchString);
    }
}