using BlazorAuthApi.DTOs.Auth;

namespace BlazorAuthApi.Interfaces
{
    public interface IAuthServices
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto Dto);
        Task<bool> RegisterAsync(RegisterDto dto);


    }

}