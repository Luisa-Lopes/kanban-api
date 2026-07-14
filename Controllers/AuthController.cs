
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Register(RegisterDto register)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new ApplicationUser
        {
            UserName = register.Email,
            Email = register.Email,
            FirstName = register.FirstName,
            LastName = register.LastName,
            JobTitle = register.JobTitle,
            Bio = register.Bio
        };

        var result = await _userManager.CreateAsync(
            user,
            register.Password
        );

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        return Ok(new
        {
            Message = "Usuário criado com sucesso."
        });
    }

    [HttpPost("login")]
    public async Task<Response<string>> Login(LoginDto login)
    {

        Response<string> response = new Response<string>();

        try
        {
            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user == null)
            {
                response.Message = "Usuário não encontrado.";
                response.Status = false;
                return response;
            }

             var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                login.Password,
                false
            );

            
            if (!result.Succeeded)
            {
                response.Message = "Email ou senha inválidos!";
                response.Status = false;
                return response;
            }


            var token =  _tokenServices.CreateToken(user);

            response.Dados = token;

            return response;
            

        }
        catch(Exception ex){
            response.Message = ex.Message;
            response.Status = false;

            return response;
        }

    }


}

