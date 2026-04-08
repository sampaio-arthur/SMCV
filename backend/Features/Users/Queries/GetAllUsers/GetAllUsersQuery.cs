using MediatR;
using SMCV.Application.DTOs.Users;

namespace SMCV.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery() : IRequest<IEnumerable<UserResponse>>;
