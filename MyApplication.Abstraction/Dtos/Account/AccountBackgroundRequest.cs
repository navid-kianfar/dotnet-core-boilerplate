using MyApplication.Abstraction.Enums;

namespace MyApplication.Abstraction.Dtos.Account;

public record AccountBackgroundRequest
{
    public Guid Id { get; set; }
    public AccountActionType Type { get; set; }
}