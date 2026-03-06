using System.Text.Json;
using BlazorAuthApp.DTOs;
using static System.Net.WebRequestMethods;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<(LoginResponse? Data, ApiErrorDto? Error)> Login(string username, string password)
    {
        var request = new
        {
            username = username,
            passwordHash = password
        };

        var response = await _http.PostAsJsonAsync("Auth/login", request);

        var raw = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var data = JsonSerializer.Deserialize<LoginResponse>(
                raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return (data, null);
        }
        else
        {
            var error = JsonSerializer.Deserialize<ApiErrorDto>(
                raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return (null, error);
        }
    }

    public async Task<RegisterResponseDto?> Register(LoginRequestDto request)
{
    Console.WriteLine("REGISTER REQUEST:");
    Console.WriteLine($"USERNAME: {request.Username}");

    var response = await _http.PostAsJsonAsync("Auth/register", request);

    Console.WriteLine($"STATUS CODE: {response.StatusCode}");

    var raw = await response.Content.ReadAsStringAsync();

    Console.WriteLine($"RAW RESPONSE: {raw}");

    try
    {
        return JsonSerializer.Deserialize<RegisterResponseDto>(
            raw,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
    catch
    {
        return new RegisterResponseDto
        {
            Message = "Gagal memproses response API"
        };
    }
}
}