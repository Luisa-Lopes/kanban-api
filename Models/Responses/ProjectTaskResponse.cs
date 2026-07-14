
namespace Response.Models;

public class ProjectTaskResponse
{
    public int Id {get; set;}

    public int ProjectSprintId { get; set; }

    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public required TaskStatus Status { get; set; } 

}