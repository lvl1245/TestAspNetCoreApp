using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618

namespace TestAspNetCoreApp.Dtos
{
    public record EmployeeDto
    {
        [Required]
        [StringLength(50)]
        public string employeeName { get; set; }

        [Required]
        [StringLength(50)]
        public string employeeSurname { get; set; }

        [Required]
        public string employeePhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string employeeRole { get; set; }

        [Required]
        [StringLength(50)]
        public string employeeEmail { get; set; }

        [Required]
        public Guid employeeDepartmentId { get; set; }
    }
}
