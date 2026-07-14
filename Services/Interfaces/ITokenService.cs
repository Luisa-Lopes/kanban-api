
using Tables.Models;

public interface ITokenService
{

    string GenerateToken(ApplicationUser user);
};