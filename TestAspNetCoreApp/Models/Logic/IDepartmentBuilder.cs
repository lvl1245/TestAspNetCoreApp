using TestAspNetCoreApp.Dtos;

namespace TestAspNetCoreApp.Models.Data
{
    public interface IDepartmentBuilder
    {
        Task<Department> BuilDepartmentAsync(DepartmentDto departmentDto);
    }
}