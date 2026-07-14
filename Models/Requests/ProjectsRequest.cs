


namespace Request.Models;


public class ProjectsRequest
{
    public int Id {get; set;}

    public required string Name {get; set;}

    public string Description {get; set;} = string.Empty;

    public required DateTime StartDate {get; set;}

    public required DateTime EstimatedDate {get; set;}

    public DateTime EndDate {get; set;}

}