namespace Workplace.Tasks.Api.DTOs
{
    public class TaskFilterDto
    {
        public string? Status { get; set; } 
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
