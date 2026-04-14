using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.Users;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;
using SMCV.Domain.Entities;

namespace SMCV.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new BusinessException("Email já cadastrado.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email
        };

        await _userRepository.AddAsync(user);
        return _mapper.Map<UserResponse>(user);
    }
}
