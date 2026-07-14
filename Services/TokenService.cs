

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectManager.Data;
using Tables.Models;

namespace Service;


public class TokenServices
{
    
    private readonly AppDbContext _dbContext;

    private readonly IConfiguration _config;

    public TokenServices (AppDbContext dbContext, IConfiguration configuration)
    {
         _dbContext = dbContext;
         _config = configuration;
    }

    public string CreateToken (ApplicationUser user)
    {

        var userEmail = user.Email ?? "";

        List<Claim> claims = new List<Claim>()
        {
            new Claim("Email", userEmail),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

        var configToken = _config.GetSection("AppSettings:Token").Value
            ?? throw new InvalidOperationException("Token não configurado.");

        var key = new SymmetricSecurityKey(System.Text.
            Encoding.UTF8.GetBytes(configToken)
        );

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(    
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: cred
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;

    }




}

