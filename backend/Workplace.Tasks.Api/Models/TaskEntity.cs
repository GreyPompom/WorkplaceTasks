using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Workplace.Tasks.Api.Models.Enum;

namespace Workplace.Tasks.Api.Models
{
    public class TaskEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public EnumTaskStatus Status { get; set; } = EnumTaskStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid? CreatedById { get; set; }
    }
}
