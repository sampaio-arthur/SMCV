using MediatR;
using SMCV.Application.DTOs.Users;

namespace SMCV.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<IEnumerable<UserResponse>>;
