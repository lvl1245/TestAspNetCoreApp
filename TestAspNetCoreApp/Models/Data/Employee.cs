using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618

namespace TestAspNetCoreApp.Models.Data
{
    public record Employee
    {
        [Required]
        public Guid id { get; set; }

        [Required]
        public string employeeName { get; set; }

        [Required]
        public string employeeSurname { get; set; }

        [Required]
        public string employeePhoneNumber { get; set; }

        [Required]
        public string employeeRole { get; set; }

        [Required]
        public string employeeEmail { get; set;}

        [Required]
        public Guid employeeDepartmentId { get; set; }

    }
}
