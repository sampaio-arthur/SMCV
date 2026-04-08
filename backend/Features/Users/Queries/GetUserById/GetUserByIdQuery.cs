using MediatR;
using SMCV.Application.DTOs.Users;

namespace SMCV.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserResponse>;
