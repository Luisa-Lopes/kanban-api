namespace Response.Models;

public class NotificationResponse
{
    public int InvitationId { get; set; }

    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public ProjectRole Role { get; set; }

    public DateTime CreatedAt { get; set; }
}
