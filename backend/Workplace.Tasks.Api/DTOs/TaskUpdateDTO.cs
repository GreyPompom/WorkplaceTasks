using System.ComponentModel.DataAnnotations;
using Workplace.Tasks.Api.Models.Enum;

namespace Workplace.Tasks.Api.DTOs
{
    public class TaskUpdateDto
    {
        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public EnumTaskStatus Status { get; set; } = EnumTaskStatus.Pending;
    }
}
