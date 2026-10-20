



namespace Response.Models;

public class ProjectsResponse
{
    public int id {get; set;}

    public required string Name {get; set;}

    public string Description {get; set;} = string.Empty;

    public required DateTime StartDate {get; set;}

    public required DateTime EstimatedDate {get; set;}

    public  DateTime EndDate {get; set;}

    public required int progress {get; set;}

    public required ProjectStatus status {get; set;}
    public List<ProjectTaskResponse> Tasks { get; set; } = [];
}

public class ProjectsResumedResponse
{
    public int id {get; set;}

    public required string Name {get; set;}

    public string Description {get; set;} = string.Empty;

    public required DateTime StartDate {get; set;}

    public required DateTime EstimatedDate {get; set;}

    public  DateTime EndDate {get; set;}

   
}


public class ProjectMetricsResponse
{

    public int totalTasks {get; set;}
    public int completedTasks {get; set;}

    public int inProgressTasks {get; set;}

    public int overdueTasks {get; set;}
}


public class ProjectResponse
{
    public int id {get; set;}

    public required string Name {get; set;}

    public string Description {get; set;} = string.Empty;

    public required DateTime StartDate {get; set;}

    public required DateTime EstimatedDate {get; set;}

    public  DateTime EndDate {get; set;}

    public List<ProjectMembersResponse> Members { get; set; } = [];

    public List<ProjectSprintResponse> Sprints { get; set; } = [];

    public List<ProjectInvitationResponse> Invitations { get; set; } = [];

    public ProjectMetricsResponse Metrics { get; set; } = new();


}
