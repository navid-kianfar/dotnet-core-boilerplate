namespace MyApplication.Abstraction.Dtos.Account;

public record UserDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? BlockedAt { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public int FailedAttempt { get; set; }
}