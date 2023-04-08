using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyApplication.Abstraction.Dtos.Account;

namespace MyApplication.DataAccess.Tables;

[Table("User", Schema = "Account")]
[Index(nameof(Username), IsUnique = true, Name = "Unique_Username")]
internal class User
{
    [Key] public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? BlockedAt { get; set; }
    [Required] [MaxLength(128)] public string Username { get; set; } = string.Empty;
    [Required] [MaxLength(128)] public string Hash { get; set; } = string.Empty;
    [Required] [MaxLength(64)] public string Salt { get; set; } = string.Empty;
    public int FailedAttempt { get; set; }

    public UserDto ToDto()
    {
        return new UserDto
        {
            Hash = Hash,
            CreatedAt = CreatedAt,
            Username = Username,
            Id = Id,
            BlockedAt = BlockedAt,
            Salt = Salt,
            DeletedAt = DeletedAt,
            FailedAttempt = FailedAttempt
        };
    }

    public static User FromDto(UserDto user)
    {
        return new User
        {
            Hash = user.Hash,
            CreatedAt = user.CreatedAt,
            Username = user.Username,
            Id = user.Id,
            BlockedAt = user.BlockedAt,
            Salt = user.Salt,
            DeletedAt = user.DeletedAt,
            FailedAttempt = user.FailedAttempt
        };
    }
}