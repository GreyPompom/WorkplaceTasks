using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Workplace.Tasks.Api.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))] //aceita string sem precisar 0, 1, 2
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Done
    }

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
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid? CreatedById { get; set; }
    }
}
