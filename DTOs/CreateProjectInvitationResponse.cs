public class CreateProjectInvitationResponse
{
    public int Id { get; set; }

    public Guid Token { get; set; }

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}