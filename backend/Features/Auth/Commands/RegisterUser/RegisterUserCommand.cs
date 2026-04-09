using MediatR;
using SMCV.Application.DTOs.Users;

namespace SMCV.Features.Auth.Commands.RegisterUser;

public record RegisterUserCommand(string Name, string Email, string Password) : IRequest<UserResponse>;
