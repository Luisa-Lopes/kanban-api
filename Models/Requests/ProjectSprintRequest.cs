
namespace Request.Models;


public class ProjectSprintRequest
{
    public int id {get; set;}

    public required int ProjectId { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public required DateTime StartDate { get; set; }

    public  DateTime EndDate { get; set; }

}