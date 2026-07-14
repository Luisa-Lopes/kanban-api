using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Tables.Models;


[Table("Users")]
public class ApplicationUser : IdentityUser
{
  
    [MaxLength(20)]
    public required string FirstName { get; set; }

    [MaxLength(80)]
    public required string LastName { get; set; }

    public string JobTitle { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;

    public ICollection<ProjectMembers> ProjectMembers { get; set; }
        = new List<ProjectMembers>();

}
