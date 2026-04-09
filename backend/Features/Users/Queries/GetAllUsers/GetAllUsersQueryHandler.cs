using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Users;
using SMCV.Application.Interfaces;

namespace SMCV.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }
}
