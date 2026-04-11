using MediatR;

namespace SMCV.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
