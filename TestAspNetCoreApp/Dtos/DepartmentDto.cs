using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618

namespace TestAspNetCoreApp.Dtos
{
    public record DepartmentDto
    {
        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        public string phoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string email { get; set; }

        public Guid? departmentId { get; set; }
    }
}
