using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618

namespace TestAspNetCoreApp.Models.Data
{
    public record Department
    {

        [Required]
        public Guid id { get; set; }
        [Required]
        public string name { get; set; }

        public string phoneNumber { get; set; }

        public int employeeCount { get; set; }

        public Guid? departmentId { get; set; }

        public string email { get; set; }

        [Required]
        public int level { get; set; }
    }
}
