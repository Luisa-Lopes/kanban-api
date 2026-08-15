using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required(ErrorMessage = "O campo nome é obrigatório")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo sobrenome é obrigatório")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail Inválido")]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string? JobTitle { get; set; } = string.Empty;

    public string? Bio { get; set; } = string.Empty;
}