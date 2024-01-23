using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAspNetCoreApp.Dtos;
using TestAspNetCoreApp.Models.Data;
using TestAspNetCoreApp.Repositories;

namespace TestAspNetCoreApp.Controllers
{
    //localhost/department
    [Route("department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository departmentRepository;

        private readonly IDepartmentBuilder departmentBuilder;

        public DepartmentController(IDepartmentRepository departmentRepository, IDepartmentBuilder departmentBuilder)
        {
            this.departmentRepository = departmentRepository;
            this.departmentBuilder = departmentBuilder;
        }
        //
        //Получить все отделы
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetAllDepartments()
        {
            IEnumerable<Department> deps = await departmentRepository.GetAllDepartmentsAsync();
            if (deps.Count() == 0)
            {
                return NotFound();
            }
            return deps.ToList();
        }
        //
        //Получить сотрудников только из выбранного отдела
        //Или из всех подотделов
        //
        [HttpGet("{id}/employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(Guid id, [FromQuery]string param)
        {
            List<Employee> employees = new List<Employee>();

            switch (param)
            {
                case "all":
                    {
                        employees = (await departmentRepository.GetChildrenEmployeeAsync(id)).ToList();

                        return employees.ToList(); ;
                    }
                case "current":
                    {
                        employees = await departmentRepository.GetDepartmentsEmployeeAsync(id);

                        return employees;
                    }
                default:
                    return BadRequest();         
            }
        }
        //
        //Получить все подотделы из выбранного отдела
        //
        [HttpGet("{id}/subdepartments")]
        public async Task<IEnumerable<Department>> GetChildrenDepartment(Guid id)
        {
            IEnumerable<Department> departments;

            departments = await departmentRepository.GetChildrenDepartmentsAsync(id);

            return departments;
        }
        //
        //Получить отдел по id
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetSingleDepartment(Guid id)
        {

            var existingDepartment = await departmentRepository.FindDepartmentAsync(id);
            if (existingDepartment is null)
            {
                return NotFound();
            }


            return existingDepartment;
        }
        //
        //Создать новый отдел
        //
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(DepartmentDto department)
        {
            if (department is null)
            {
                return BadRequest();
            }
            var newDepartment = await departmentBuilder.BuilDepartmentAsync(department);

            await departmentRepository.CreateDepartmentAsync(newDepartment);

            return CreatedAtAction(nameof(GetSingleDepartment), new { id = newDepartment.id }, department);
        }
        //
        //Удалить отдел, всех сотрудников и подотделы
        //
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(Guid id)
        {
            Department existingDepartment = await departmentRepository.DeleteDepartmentAsync(id);
            if (existingDepartment is null)
            {
                return NotFound();
            }
            return existingDepartment;
        }
        //
        //Заменить данные в отделе
        //
        [HttpPut("{id}")]
        public async Task<ActionResult<Department>> UpdateDepartment(Guid id, DepartmentDto departmentDto)
        {
            Department existingDepartment = await departmentRepository.FindDepartmentAsync(id);
            if (existingDepartment is null)
            {
                return NotFound();
            }
            Department result = await departmentRepository.UpdateDepartmentAsync(id, departmentDto);
            return result;
        }
    }
}
