using Microsoft.AspNetCore.Mvc;
using TestAspNetCoreApp.Dtos;
using TestAspNetCoreApp.Models.Data;
using TestAspNetCoreApp.Repositories;

namespace TestAspNetCoreApp.Controllers
{
    //localhost/employee
    [Route("employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        readonly IEmployeeRepository employeeRepository;
        readonly IDepartmentRepository departmentRepository;
        public EmployeeController(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            this.employeeRepository = employeeRepository;
            this.departmentRepository = departmentRepository;
        }
        //
        //Получить список всех сотрудников
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
        {
            List<Employee> employees = (List<Employee>)await employeeRepository.GetAllEmployeesAsync();
            if (employees.Count == 0)
            {
                return NotFound();
            }

            return employees;
        }
        //
        //Получить только одного сотрудника
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetSingleEmployee(Guid id)
        {
            Employee employees = await employeeRepository.FindEmployeeAsync(id);
            return employees;
        }
        //
        //Добавить сотрудника
        //
        [HttpPost]
        public async Task<ActionResult<Employee>> AddEmployee(EmployeeDto employee)
        { 
            Employee newEmployee = await employeeRepository.CreateEmployeeAsync(employee.ToEmployee());
            if (newEmployee is null)
            {
                return BadRequest();
            }
            return newEmployee;
        }
        //
        //Удалить сотрудника
        //
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(Guid id)
        {
            Employee existingEmployee = await employeeRepository.DeleteEmployeeAsync(id);
            if (existingEmployee is null)
            {
                return NotFound();
            }
            return existingEmployee;
        }
        //
        //Изменить информацию о сотруднике
        //
        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(Guid id, EmployeeDto employeeDto)
        {
            
           Employee updatedEmployee = await employeeRepository.UpdateEmployeeAsync(id , employeeDto);

            if (updatedEmployee is null)
            {
                return BadRequest();
            }
           return updatedEmployee;
        }
        //
        //Поиск сотрудников по вхождению строки в атрибуты
        //
        [HttpGet("search")]
        public async Task<IEnumerable<Employee>> SearchEmployees([FromQuery]string query)
        {
            IEnumerable<Employee> employees;

            employees = await employeeRepository.SearchEmployees(query);

            return employees;
        }
    }
}
