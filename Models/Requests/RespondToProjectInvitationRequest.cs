using System.ComponentModel.DataAnnotations;

namespace Request.Models;

public class RespondToProjectInvitationRequest
{
    [Required]
    public InvitationStatus Status { get; set; }
}
