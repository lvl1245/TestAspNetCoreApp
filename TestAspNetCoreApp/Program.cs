
using TestAspNetCoreApp.Repositories;
using TestAspNetCoreApp.Settings;
using Microsoft.Extensions.Configuration;
using TestAspNetCoreApp.Models.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data.Common;


namespace TestAspNetCoreApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IConfiguration configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();




            PostgresSettings settings =  new PostgresSettings()
            {
                Host = configuration["PosgreSQL:Host"],
                Username = configuration["PosgreSQL:Username"],
                Database = configuration["PosgreSQL:Database"],
                Port = configuration["PosgreSQL:Port"],
                Password = configuration["PosgreSQL:Password"],
            };
          
            builder.Services.AddSingleton<IDatabaseSettings>(settings);

            LoggerFactory loggerFactory = new LoggerFactory();
            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(settings.ConnectionString);
            dataSourceBuilder
                .UseLoggerFactory(loggerFactory);
            NpgsqlDataSource dataSource = dataSourceBuilder.Build();

            builder.Services.AddSingleton<NpgsqlDataSource>(dataSource);

            builder.Services.AddSingleton<IDepartmentBuilder, DepartmentBuilder>();

            builder.Services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddSingleton<IDepartmentRepository, DepartmentRepository>();

            builder.Services.AddControllers();
          
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
               app.UseSwagger();
               app.UseSwaggerUI();
            }
           
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}