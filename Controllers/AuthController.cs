
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Exceptions;
using Response.Models;
using Service;
using Tables.Models;

namespace Function.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{ 

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenServices _tokenServices;

    public AuthController (UserManager<ApplicationUser> userManager,
       SignInManager<ApplicationUser> signInManager,
       TokenServices tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenServices = tokenService;
    }



    [HttpPost("register")]
    public async Task<Response<string>> Register(RegisterDto register)
    {
        var userExists = await _userManager.FindByEmailAsync(register.Email);

        if (userExists != null)
        {
            throw new ConflictException("Já existe um usuário cadastrado com este e-mail.");
        }

        var user = new ApplicationUser
        {
            UserName = register.Email,
            Email = register.Email,
            FirstName = register.FirstName,
            LastName = register.LastName,
            JobTitle = register.JobTitle ?? "",
            Bio = register.Bio ?? ""
        };

        var result = await _userManager.CreateAsync(
            user,
            register.Password);

        if (!result.Succeeded)
        {
            throw new BadRequestException(
                string.Join(" ", result.Errors.Select(e => e.Description)));
        }

        return new Response<string>
        {
            Message = "Usuário criado com sucesso.",
            StatusCode = StatusCodes.Status200OK
        };
    }

    [HttpPost("login")]
    public async Task<Response<string>> Login(LoginDto login)
    {
        var user = await _userManager.FindByEmailAsync(login.Email);

        if (user == null)
            throw new UnauthorizedException("Email ou senha inválidos.");

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            login.Password,
            false);

        if (!result.Succeeded)
            throw new UnauthorizedException("Email ou senha inválidos.");

        return new Response<string>
        {
            Dados = _tokenServices.CreateToken(user),
            Message = "Login realizado com sucesso.",
            StatusCode = StatusCodes.Status200OK
        };
    }

    [Authorize]
    [HttpGet("token")]
    public IActionResult validadeToken()
    {
        return Ok(new
        {
            Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            FirstName = User.FindFirst(ClaimTypes.GivenName)?.Value,
            LastName = User.FindFirst(ClaimTypes.Surname)?.Value
        });
    }


}

