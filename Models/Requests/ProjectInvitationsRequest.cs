
namespace Request.Models;



public class ProjectInvitationsRequest
{

    public required int ProjectId {get; set;}

    public required string Email { get; set; }

    public required ProjectRole Role { get; set; }

    public required string InvitesBy { get; set; }

}