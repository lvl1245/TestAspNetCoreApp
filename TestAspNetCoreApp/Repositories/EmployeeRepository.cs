using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;
using NpgsqlTypes;
using System.Transactions;
using TestAspNetCoreApp.Dtos;
using TestAspNetCoreApp.Models.Data;

namespace TestAspNetCoreApp.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly NpgsqlDataSource dataSource;

        private readonly IDepartmentRepository departmentRepository;
        public EmployeeRepository(NpgsqlDataSource dataSource, IDepartmentRepository departmentRepository)
        {
            this.dataSource = dataSource;
            this.departmentRepository = departmentRepository;
        }

        public async Task<Employee> CreateEmployeeAsync(Employee newEmployee)
        {
            await using NpgsqlConnection conn = await dataSource.OpenConnectionAsync();
            await using NpgsqlTransaction transaction = await conn.BeginTransactionAsync();

          
            try
            {
                if (departmentRepository.FindDepartmentAsync(newEmployee.employeeDepartmentId) is null)
                {
                    return null;
                } 

                await using NpgsqlCommand addEmployeeTodepartmentCommand = dataSource.CreateCommand($"UPDATE departments SET employee_count = employee_count + 1 WHERE id = '{newEmployee.employeeDepartmentId}'");
                await using NpgsqlCommand createEmployeeCommand = dataSource.CreateCommand("INSERT INTO Employees VALUES (" +
                "@id," +
                "@employee_name," +
                "@employee_surname," +
                "@employee_phone_number," +
                "@employee_email," +
                "@employee_role," +
                "@employee_department_id" +
                ")");

                createEmployeeCommand.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, newEmployee.id);
                createEmployeeCommand.Parameters.AddWithValue("employee_name", NpgsqlDbType.Char, newEmployee.employeeName);
                createEmployeeCommand.Parameters.AddWithValue("employee_surname", NpgsqlDbType.Char, newEmployee.employeeSurname);
                createEmployeeCommand.Parameters.AddWithValue("employee_phone_number", NpgsqlDbType.Text, newEmployee.employeePhoneNumber);
                createEmployeeCommand.Parameters.AddWithValue("employee_email", NpgsqlDbType.Char, newEmployee.employeeEmail);
                createEmployeeCommand.Parameters.AddWithValue("employee_role", NpgsqlDbType.Char, newEmployee.employeeRole);
                createEmployeeCommand.Parameters.AddWithValue("employee_department_id", NpgsqlDbType.Uuid, newEmployee.employeeDepartmentId);

                await createEmployeeCommand.ExecuteNonQueryAsync();

                await addEmployeeTodepartmentCommand.ExecuteNonQueryAsync();

                transaction.Commit();

                return await FindEmployeeAsync(newEmployee.id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<Employee> DeleteEmployeeAsync(Guid id)
        {
            
            await using NpgsqlConnection conn = await dataSource.OpenConnectionAsync();
            await using NpgsqlTransaction transaction = await conn.BeginTransactionAsync();

            try
            {
                Employee employee = await FindEmployeeAsync(id);
                if (employee is null)
                {
                    return null;
                }

                await using NpgsqlCommand deleteEmployeeCommand = dataSource.CreateCommand($"DELETE  FROM employees WHERE id = '{id}'");

                await using NpgsqlCommand updateDepartmentCommand = dataSource.CreateCommand($"UPDATE departments SET employee_count = employee_count - 1 " +
                                                                                             $"WHERE id = '{employee.employeeDepartmentId}'");

                await updateDepartmentCommand.ExecuteNonQueryAsync();

                await deleteEmployeeCommand.ExecuteNonQueryAsync();

                

                transaction.Commit();
                return employee;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            List<Employee> employees = new List<Employee>();

            await using NpgsqlCommand command = dataSource.CreateCommand("SELECT * FROM Employees");
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

        public async Task<Employee> FindEmployeeAsync(Guid id)
        {
            Employee singleEmployee;

            await using var command = dataSource.CreateCommand($"SELECT * FROM employees WHERE id = '{id}' LIMIT 1");
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                singleEmployee = new Employee()
                {
                    id = reader.GetGuid(reader.GetOrdinal("id")),
                    employeeName = reader.GetString(reader.GetOrdinal("employee_name")),
                    employeeSurname = reader.GetString(reader.GetOrdinal("employee_surname")),
                    employeePhoneNumber = reader.GetString(reader.GetOrdinal("employee_phone_number")),
                    employeeRole = reader.GetString(reader.GetOrdinal("employee_role")),
                    employeeEmail = reader.GetString(reader.GetOrdinal("employee_email")),
                    employeeDepartmentId = reader.GetGuid(reader.GetOrdinal("employee_department_id"))
                };
            }
            else
            {
               return null;
            }
           return singleEmployee;
        }

        public async Task<Employee> UpdateEmployeeAsync(Guid id ,EmployeeDto editedEmployee)
        {
            await using NpgsqlConnection conn = await dataSource.OpenConnectionAsync();
            await using NpgsqlTransaction transaction = await conn.BeginTransactionAsync();

            try
            {
                Employee existingEmployee = await FindEmployeeAsync(id);

                if (existingEmployee is null)
                {
                    return null;
                }
                Department oldDepartment = await departmentRepository.FindDepartmentAsync(existingEmployee.employeeDepartmentId);
                Department newDepartment = await departmentRepository.FindDepartmentAsync(editedEmployee.employeeDepartmentId);

                if (newDepartment is null)
                {
                    return null;
                }

                await using NpgsqlCommand updateOldDepartmentCommand = dataSource.CreateCommand($"UPDATE departments SET employee_count = employee_count - 1 " +
                                                                                                $"WHERE id = '{oldDepartment.id}'");

                await using NpgsqlCommand updateNewDepartmentCommand = dataSource.CreateCommand($"UPDATE departments SET employee_count = employee_count + 1 " +
                                                                                                $"WHERE id = '{newDepartment.id}'");

                await using var command = dataSource.CreateCommand($"UPDATE employees SET " +
                    $"employee_name ='{editedEmployee.employeeName}'," +
                    $"employee_surname ='{editedEmployee.employeeSurname}'," +
                    $"employee_phone_number ='{editedEmployee.employeePhoneNumber}'," +
                    $"employee_email ='{editedEmployee.employeeEmail}'," +
                    $"employee_role ='{editedEmployee.employeeRole}'," +
                    $"employee_department_id ='{editedEmployee.employeeDepartmentId}' " +
                    $"WHERE id = '{id}'");

                await using var reader = await command.ExecuteReaderAsync();

                await updateNewDepartmentCommand.ExecuteNonQueryAsync();
                await updateOldDepartmentCommand.ExecuteNonQueryAsync();

                transaction.Commit();
                return existingEmployee;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                transaction.Rollback();
                throw;
            }
        }


        public async Task<IEnumerable<Employee>> SearchEmployees(string searchString)
        {
            List<Employee> employees= new List<Employee>();

            await using NpgsqlCommand command = dataSource.CreateCommand($"SELECT* FROM employees WHERE employee_name " +
                $"ILIKE '%{searchString}%' " +
                $"OR employee_surname ILIKE '%{searchString}%' " +
                $"OR employee_phone_number ILIKE '%{searchString}%' " +
                $"OR employee_email ILIKE '%{searchString}%' " +
                $"OR employee_role ILIKE '%{searchString}%'");

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
    }
}
