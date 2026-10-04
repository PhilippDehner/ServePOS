using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServePos.Infrastructure;

namespace ServePOS.API.Auth;

public class AuthService(PosDbContext db, IOptions<JwtOptions> jwtOptions)
{
    public async Task<LoginResponse?> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Include(x => x.Role)
            .SingleOrDefaultAsync(
                x => x.Username == request.Username && x.IsActive,
                cancellationToken);

        if (user is null || !PinHasher.Verify(request.Pin, user.PinHash))
        {
            return null;
        }

        var options = jwtOptions.Value;
        var expiresAt = DateTime.UtcNow.AddMinutes(options.ExpirationMinutes);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }
}
