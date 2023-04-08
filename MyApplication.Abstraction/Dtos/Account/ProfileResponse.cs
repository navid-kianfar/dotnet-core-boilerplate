namespace MyApplication.Abstraction.Dtos.Account;

public record ProfileResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}