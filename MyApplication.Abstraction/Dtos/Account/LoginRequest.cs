namespace MyApplication.Abstraction.Dtos.Account;

public record LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}