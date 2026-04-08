using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Users;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("User", request.Id);

        return _mapper.Map<UserResponse>(user);
    }
}
