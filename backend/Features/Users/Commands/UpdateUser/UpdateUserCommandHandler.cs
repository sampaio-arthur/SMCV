using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Users;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("User", request.Id);

        user.Name = request.Name;
        user.Email = request.Email;

        await _userRepository.UpdateAsync(user);
        return _mapper.Map<UserResponse>(user);
    }
}
