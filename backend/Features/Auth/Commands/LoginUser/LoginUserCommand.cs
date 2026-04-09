using MediatR;
using SMCV.Application.DTOs.Users;

namespace SMCV.Features.Auth.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : IRequest<UserResponse>;
