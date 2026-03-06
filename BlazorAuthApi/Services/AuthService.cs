using BlazorAuthApi.Interfaces;
using BlazorAuthApi.DTOs.Auth;
using BlazorAuthApi.Data;
using Microsoft.EntityFrameworkCore;
using BlazorAuthApi.Hellpers;

namespace BlazorAuthApi.Services
{
    public class AuthService : IAuthServices

    {

        private readonly AppDbContext _context;
        private readonly Authentication _authentication;

        private readonly RedisCacheService _cacheService;
        private readonly RedisRateLimit _rateLimitService;

        public AuthService(AppDbContext context, Authentication authentication, RedisCacheService cacheService, RedisRateLimit rateLimitService)
        {
            _context = context;
            _authentication = authentication;
            _cacheService = cacheService;
            _rateLimitService = rateLimitService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            await _rateLimitService.CheckLoginRateLimitAsync(dto.Username);


            var user = await _context.Users.SingleOrDefaultAsync(
                u => u.Username == dto.Username
            );

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.PasswordHash, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials User not found or password mismatch");
            }

            var token = _authentication.GenerateToken(user);



            await _cacheService.SetAsync(
                $"redis:refresh_token:{user.Id}",
                token,
                TimeSpan.FromDays(30)
            );

            return new LoginResponseDto
            {
                Message = "Login successful",
                Token = token
            };
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
            {
                return false;
            }

            var newUser = new Models.User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash),
                Role = "User"
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}