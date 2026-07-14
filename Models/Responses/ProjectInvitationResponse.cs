namespace Response.Models;


public class ProjectInvitationResponse
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Email { get; set; } = string.Empty;

    public ProjectRole Role { get; set; }

    public InvitationStatus Status { get; set; }

    public string InvitesBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}