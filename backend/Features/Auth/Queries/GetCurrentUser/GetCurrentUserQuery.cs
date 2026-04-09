using MediatR;
using SMCV.Application.DTOs.Users;

namespace SMCV.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<UserResponse>;
