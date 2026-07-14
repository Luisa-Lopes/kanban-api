using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tables.Models;


[Table("ProjectTasks")]


public class ProjectTasks
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("project_sprint_id")]
        public int ProjectSprintId { get; set; }

        [Column("title")]
        public required string Title { get; set; }

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("status")]
        public required TaskStatus Status { get; set; } 

        //Criar relacionamento entre as tabelas
        public ProjectSprint ProjectSprint { get; set; } = null!;

    }
