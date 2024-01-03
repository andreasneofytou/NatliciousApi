using Microsoft.Extensions.Options;
using Natlicious.Api.Users;
using Natlicious.Api.Users.Schema;

namespace Natlicious.Api.Auth;

public class AuthService
{
    private readonly UsersService usersService;
    private readonly JwtSettings jwtSettings;

    public AuthService(IOptions<JwtSettings> jwtSettings, UsersService usersService)
    {
        this.usersService = usersService;
        this.jwtSettings = jwtSettings.Value;
    }

    public async Task<TokenDto?> LoginAsync(string email, string password)
    {
        var user = await usersService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        return PasswordHasher.Verify(password, user.PasswordHash) ?
          GenerateToken(user) : null;
    }



    public async Task<TokenDto> RegisterAsync(string email, string password)
    {
        var hashedPassword = PasswordHasher.Hash(password);

        var user = await usersService.CreateUserAsync(new User { Email = email, PasswordHash = hashedPassword });
        return GenerateToken(user);
    }

    private TokenDto GenerateToken(User user)
    {
        var token = TokenUtils.GenerateAccessToken(user, jwtSettings.Secret, jwtSettings.AccessTokenExpiration);
        var refreshToken = TokenUtils.GenerateRefreshToken();

        return new TokenDto { AccessToken = token, RefreshToken = refreshToken };
    }
}