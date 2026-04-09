using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Users;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            throw new NotFoundException("Usuário não encontrado.", request.UserId);

        return _mapper.Map<UserResponse>(user);
    }
}
