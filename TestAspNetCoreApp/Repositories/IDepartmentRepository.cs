using TestAspNetCoreApp.Dtos;
using TestAspNetCoreApp.Models.Data;

namespace TestAspNetCoreApp.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();

        Task<Department> FindDepartmentAsync(Guid DepartmentId);

        Task<Department> UpdateDepartmentAsync(Guid id, DepartmentDto editedDepartment);
        Task<Department> DeleteDepartmentAsync(Guid DepartmentId);
    
        Task<Department> CreateDepartmentAsync(Department NewDepartment);

        Task<List<Employee>> GetDepartmentsEmployeeAsync(Guid id);

        Task<IEnumerable<Department>> GetChildrenDepartmentsAsync(Guid id);

        Task<IEnumerable<Employee>> GetChildrenEmployeeAsync(Guid id);
    
    }
}