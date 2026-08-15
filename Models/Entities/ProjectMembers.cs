using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tables.Models;

[Table("ProjectMembers")]

public class ProjectMembers
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("project_Id")]
        public required int ProjectId { get; set; }

        [Column("user_Id")]
        public required string UserId { get; set; }

        [Column("invitation_sent")]
        public required DateTime InvitationSent { get; set; }

        [Column("joinedAt")]
        public DateTime? JoinedAt { get; set; }

        [Column("role")]
        public required ProjectRole Role { get; set; }

        //Criar relacionamento entre as tabelas

        public Projects Project { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;


    }
