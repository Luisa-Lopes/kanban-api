using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tables.Models;

[Table("Projects")]

public class Projects
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public required string Name { get; set; }

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("start_date")]
        public required DateTime StartDate { get; set; }

        [Column("estimated_date")]
        public required DateTime EstimatedDate { get; set; }

        [Column("end_date")]
        public  DateTime EndDate { get; set; }

        public ICollection<ProjectMembers> Members { get; set; } = new List<ProjectMembers>();

        public ICollection<ProjectSprint> Sprints { get; set; } = new List<ProjectSprint>();

    }
