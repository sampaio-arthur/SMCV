namespace SMCV.Application.DTOs.Users;

public record CreateUserRequest(
    string Name,
    string Email
);
