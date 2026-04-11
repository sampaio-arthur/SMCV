using MediatR;
using SMCV.Application.DTOs.Users;

namespace SMCV.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string Name, string Email) : IRequest<UserResponse>;
