using System;
using Workplace.Tasks.Api.Models.Enum;

namespace Workplace.Tasks.Api.DTOs
{
    public class TaskResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EnumTaskStatus Status { get; set; } = EnumTaskStatus.Pending;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? CreatedById { get; set; }
    }
}
