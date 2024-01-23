using TestAspNetCoreApp.Dtos;
using TestAspNetCoreApp.Models.Data;

namespace TestAspNetCoreApp
{
    public static class Extensions
    {
        public static Employee ToEmployee(this EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return null; 
            }

            return new Employee
            {
               id = Guid.NewGuid(),
               employeeName = employeeDto.employeeName,
               employeeSurname = employeeDto.employeeSurname,
               employeeEmail = employeeDto.employeeEmail,
               employeePhoneNumber = employeeDto.employeePhoneNumber,
               employeeDepartmentId = employeeDto.employeeDepartmentId,
               employeeRole = employeeDto.employeeRole
            };
        }
    }
}
