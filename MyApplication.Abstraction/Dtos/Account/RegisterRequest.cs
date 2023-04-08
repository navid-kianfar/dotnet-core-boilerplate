namespace MyApplication.Abstraction.Dtos.Account;

public record RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}