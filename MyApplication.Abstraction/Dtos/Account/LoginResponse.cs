namespace MyApplication.Abstraction.Dtos.Account;

public record LoginResponse
{
    public string Token { get; set; } = string.Empty;
}