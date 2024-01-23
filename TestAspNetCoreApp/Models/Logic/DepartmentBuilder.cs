using Npgsql;
using TestAspNetCoreApp.Dtos;
using TestAspNetCoreApp.Repositories;
using TestAspNetCoreApp.Settings;

namespace TestAspNetCoreApp.Models.Data
{
    public class DepartmentBuilder : IDepartmentBuilder
    {
        private readonly IDatabaseSettings settings;

        private readonly string connectionString;

        public DepartmentBuilder(IDatabaseSettings settings)
        {
            this.settings = settings;
            connectionString = settings.ConnectionString;
        }

        public async Task<Department> BuilDepartmentAsync(DepartmentDto departmentDto)
        {
            Department newDepartment = new Department()
            {
                id = Guid.NewGuid(),
                phoneNumber = departmentDto.phoneNumber,
                departmentId = departmentDto.departmentId,
                name = departmentDto.name,
                email = departmentDto.email,
                employeeCount = 0
            };
            if (departmentDto.departmentId is null)// если не указан id главного отедал то считаем отдел одним из главных
            {
                newDepartment.departmentId = newDepartment.id;
                newDepartment.level = 1;
                return newDepartment;
            }
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (NpgsqlCommand selectParentComand = new NpgsqlCommand($"SELECT * FROM departments WHERE id::uuid = '{departmentDto.departmentId}'::uuid;", connection))
                // извлекаем родительский отдел что бы получить глубину
                using (NpgsqlDataReader selectParentComandReader = await selectParentComand.ExecuteReaderAsync())
                {
                    if (await selectParentComandReader.ReadAsync())
                    {
                        newDepartment.level = selectParentComandReader.GetInt32(selectParentComandReader.GetOrdinal("level")) + 1;
                    }
                }
            }

            return newDepartment;
        }

    }
}
