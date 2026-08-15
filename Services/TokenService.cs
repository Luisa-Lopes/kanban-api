

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

    public string CreateToken(ApplicationUser user)
            {
                var configToken = _config["AppSettings:Token"]
                    ?? throw new InvalidOperationException("Token não configurado.");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName)
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configToken));

                var credentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha512Signature);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

}