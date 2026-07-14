using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tables.Models;

[Table("ProjectSprint")]

public class ProjectSprint
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("project_Id")]
        public required int ProjectId { get; set; }

        [Column("title")]
        public required string Title { get; set; }

        [Column("description")]
        public required string Description { get; set; }

        [Column("start_date")]
        public required DateTime StartDate { get; set; }

        [Column("end_date")]
        public  DateTime EndDate { get; set; }

        //Criar relacionamento entre as tabelas
        public Projects Project { get; set; } = null!;

        public ICollection<ProjectTasks> Tasks { get; set; } = new List<ProjectTasks>();

    }
