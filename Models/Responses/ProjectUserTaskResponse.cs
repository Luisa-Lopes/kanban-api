namespace Response.Models;

public class ProjectUserTaskResponse
{
    public int Id {get; set;}

    public int ProjectSprintId { get; set; }

    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public required TaskStatus Status { get; set; } 

    public string AssigneeId { get; set; } = string.Empty;

    public  DateTime EndDate { get; set; }

    public int ProjectId {get; set;}

    public required string ProjectName {get; set;}

}