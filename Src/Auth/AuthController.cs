using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Natlicious.Api.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController(AuthService authService) : Controller
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await authService.LoginAsync(loginDto.Email, loginDto.Password);
        if (token == null)
        {
            return Unauthorized();
        }

        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var user = await authService.RegisterAsync(registerDto.Email, registerDto.Password);
        if (user == null)
        {
            return BadRequest();
        }

        return Ok(user);
    }
}