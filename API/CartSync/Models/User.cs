using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(UserId))]
public record User
{
    public Ulid UserId { get; init; } = Ulid.NewUlid();
    
    [StringLength(64, MinimumLength = 1)]
    public required string Username { get; init; }

    public required byte[] Hash { get; init; }
    public required byte[] Salt { get; init; }
}

public record UserLoginRequest(string Username, string Password);
public record UserLoginSuccessResponse(string Token);