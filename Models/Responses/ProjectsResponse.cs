



namespace Response.Models;

public class ProjectsResponse
{
    public int id {get; set;}

    public required string Name {get; set;}

    public string Description {get; set;} = string.Empty;

    public required DateTime StartDate {get; set;}

    public required DateTime EstimatedDate {get; set;}

    public  DateTime EndDate {get; set;}


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


}