using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tables.Models;

[Table("ProjectInvitations")]

public class ProjectInvitations
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("project_Id")]
        public required int ProjectId { get; set; }

        [Column("email")]
        public required string Email { get; set; }

        [Column("role")]
        public required ProjectRole Role { get; set; }

        [Column("token")]
        public Guid Token { get; set; }

        [Column("status")]
        public required InvitationStatus Status { get; set; }

        [Column("invites_by")]
        public required string InvitesBy { get; set; }

        [Column("created_At")]
        public DateTime CreatedAt { get; set; }

    }
