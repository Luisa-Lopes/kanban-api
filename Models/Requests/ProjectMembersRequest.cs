

namespace Request.Models;



public class ProjectMembersRequest
{
    
    public required int ProjectId {get; set;}

    public required string Email { get; set; }

    public DateTime JoinedAt { get; set; }

    public required ProjectRole Role { get; set; }

}