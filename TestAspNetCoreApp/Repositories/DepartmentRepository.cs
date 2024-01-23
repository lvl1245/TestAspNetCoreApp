using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System.Reflection.Emit;
using TestAspNetCoreApp.Dtos;
using TestAspNetCoreApp.Models.Data;
using TestAspNetCoreApp.Settings;

namespace TestAspNetCoreApp.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {

        private readonly IDepartmentBuilder departmentBuilder;


        private readonly NpgsqlDataSource dataSource;

        public  DepartmentRepository(IDatabaseSettings Settings, IDepartmentBuilder departmentBuilder, NpgsqlDataSource dataSource)
        {
           
            this.departmentBuilder = departmentBuilder;
            this.dataSource = dataSource;
        }
        public async Task<Department> CreateDepartmentAsync(Department newDepartment)
        {
            await using NpgsqlCommand command = dataSource.CreateCommand($"INSERT INTO departments VALUES (" +
                $" @id," +
                $" @name," +
                $" @phone_number," +
                $" @email," +
                $" @employee_count," +
                $" @department_id," +
                $" @level" +
                $")");

            command.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, newDepartment.id);
            command.Parameters.AddWithValue("name", newDepartment.name);
            command.Parameters.AddWithValue("phone_number", newDepartment.phoneNumber);
            command.Parameters.AddWithValue("email", newDepartment.email);
            command.Parameters.AddWithValue("employee_count", newDepartment.employeeCount);
            command.Parameters.AddWithValue("department_id", newDepartment.departmentId);
            command.Parameters.AddWithValue("level", newDepartment.level);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            return await FindDepartmentAsync(newDepartment.id);
        }
        public async Task<Department> CreateDepartmentAsync(DepartmentDto NewDepartmentDto)
        {
            Department newDepartment = await departmentBuilder.BuilDepartmentAsync(NewDepartmentDto);

            return await CreateDepartmentAsync(newDepartment);  
        }
        public async Task<Department> DeleteDepartmentAsync(Guid id)
        {
            Department existingDepartment = await FindDepartmentAsync(id);
            if (existingDepartment is null)
            {
                return null;
            }

            await using NpgsqlCommand command = dataSource.CreateCommand($"DELETE FROM departments WHERE id = '{id}'");
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            return existingDepartment;
        }
        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            List<Department> departments = new List<Department>();

            await using NpgsqlCommand command = dataSource.CreateCommand("SELECT * FROM departments");
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Department tmpDepartment = new Department()
                {
                    id = reader.GetGuid(reader.GetOrdinal("Id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    phoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                    email = reader.GetString(reader.GetOrdinal("email")),
                    employeeCount = reader.GetInt32(reader.GetOrdinal("employee_count")),
                    level = reader.GetInt32(reader.GetOrdinal("level")),
                    departmentId = reader.GetGuid(reader.GetOrdinal("department_id"))
                };
                departments.Add(tmpDepartment);
            }

            return departments;
        }

        private  async Task<List<Department>> GetChildrensAsync (Guid id)
        {
            List<Department> result = new List<Department>();

            await using NpgsqlCommand command = dataSource.CreateCommand($"SELECT * FROM departments WHERE department_id = '{id}'");
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Department tmpDepartment = new Department()
                {
                    id = reader.GetGuid(reader.GetOrdinal("id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    phoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                    email = reader.GetString(reader.GetOrdinal("email")),
                    employeeCount = reader.GetInt32(reader.GetOrdinal("employee_count")),
                    level = reader.GetInt32(reader.GetOrdinal("level")),
                    departmentId = reader.GetGuid(reader.GetOrdinal("department_id"))
                };

                if (tmpDepartment.level != 1)
                {
                    result.Add(tmpDepartment);
                }
            }

            return result;

        }
        public async Task<IEnumerable<Department>> GetChildrenDepartmentsAsync(Guid id)
        {
            List<Department> result = new List<Department>();
            
            Department tmpDepartment;
            
            Stack<Department> stack = new Stack<Department>();

            stack.Push(await FindDepartmentAsync(id));

            while (stack.Count > 0) 
            {
                tmpDepartment = stack.Pop();
                result.Add(tmpDepartment);
                foreach (Department childDepartment in await GetChildrensAsync(tmpDepartment.id))
                {
                    stack.Push(childDepartment);    
                }
            }
            

            return result;
        }
        public async Task<Department> FindDepartmentAsync(Guid DepartmentId)
        {
            Department tmpDepartment;


            await using NpgsqlCommand command = dataSource.CreateCommand($"SELECT * FROM departments WHERE id::uuid = '{DepartmentId}'::uuid;");
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                tmpDepartment = new Department()
                {
                    id = reader.GetGuid(reader.GetOrdinal("Id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    phoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                    email = reader.GetString(reader.GetOrdinal("email")),
                    employeeCount = reader.GetInt32(reader.GetOrdinal("employee_count")),
                    level = reader.GetInt32(reader.GetOrdinal("level")),
                    departmentId = reader.GetGuid(reader.GetOrdinal("department_id"))
                };
                return tmpDepartment;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Employee>> GetDepartmentsEmployeeAsync(Guid id)
        {
            List<Employee> employees = new List<Employee>();

            await using NpgsqlCommand command = dataSource.CreateCommand($"SELECT * FROM employees WHERE employee_department_id::uuid = '{id}'::uuid;");
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Employee tmpEmployee = new Employee()
                {
                    id = reader.GetGuid(reader.GetOrdinal("id")),
                    employeeName = reader.GetString(reader.GetOrdinal("employee_name")),
                    employeeSurname = reader.GetString(reader.GetOrdinal("employee_surname")),
                    employeePhoneNumber = reader.GetString(reader.GetOrdinal("employee_phone_number")),
                    employeeRole = reader.GetString(reader.GetOrdinal("employee_role")),
                    employeeEmail = reader.GetString(reader.GetOrdinal("employee_email")),
                    employeeDepartmentId = reader.GetGuid(reader.GetOrdinal("employee_department_id"))
                };
                employees.Add(tmpEmployee);
            }
            return employees;
        } 
        public async Task<Department> UpdateDepartmentAsync(Guid id, DepartmentDto editedDepartment)
        {
            Department existingDepartment = await FindDepartmentAsync(id);

            if (existingDepartment is null) 
            {
                return null;  
            }

            await using var command = dataSource.CreateCommand($"UPDATE departments SET " +
                  $"name ='{editedDepartment.name}'," +
                  $"email ='{editedDepartment.email}'," +
                  $"phone_number ='{editedDepartment.phoneNumber}' " +
                  $"WHERE id = '{existingDepartment.id}'");

            await command.ExecuteNonQueryAsync();

            return await FindDepartmentAsync(existingDepartment.id);
            
        }

        public async Task<IEnumerable<Employee>> GetChildrenEmployeeAsync(Guid id)
        {
            List<Employee> result = new List<Employee>();

            List<Department> departments = (await GetChildrenDepartmentsAsync(id)).ToList();

            foreach (var department in departments)
            {
                result.AddRange(await GetDepartmentsEmployeeAsync(department.id));
            }
            return result;
        }
    }
}
