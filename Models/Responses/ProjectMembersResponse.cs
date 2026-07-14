namespace Response.Models;


public class  ProjectMembersResponse
{
    public int Id {get; set;}

    public required int ProjectId {get; set;}

    public required UserResponse User { get; set; }

    public DateTime? JoinedAt { get; set; }

    public required ProjectRole Role { get; set; }

}