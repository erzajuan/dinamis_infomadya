using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlazorAuthApi.DTOs.Auth;
using BlazorAuthApi.Interfaces;

namespace BlazorAuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;

        public AuthController(IAuthServices authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result)
            {
                return BadRequest(new { message = "Username already used" });
            }

            return CreatedAtAction(nameof(Register), new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return Ok(new
            {
                message = result.Message,
                token = result.Token
            });
        }

    }

}