using MediatR;
using SMCV.Application.DTOs.Users;

namespace SMCV.Features.Users.Commands.CreateUser;

public record CreateUserCommand(string Name, string Email) : IRequest<UserResponse>;
