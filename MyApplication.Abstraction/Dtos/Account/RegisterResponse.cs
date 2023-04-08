namespace MyApplication.Abstraction.Dtos.Account;

public record RegisterResponse
{
    public string Token { get; set; } = string.Empty;
}