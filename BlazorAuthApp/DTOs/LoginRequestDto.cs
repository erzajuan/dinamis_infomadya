using System.ComponentModel.DataAnnotations;

namespace BlazorAuthApp.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username wajib diisi")]
        [MinLength(4, ErrorMessage = "Username minimal 4 karakter")]
        [MaxLength(20, ErrorMessage = "Username maksimal 20 karakter")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password wajib diisi")]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}