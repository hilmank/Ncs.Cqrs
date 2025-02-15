using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Application.Services;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Users.Commands
{
    public class UserLoginCommand : IRequest<ResponseDto<SigninDto>>
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, ResponseDto<SigninDto>>
    {
        private readonly IAuthService _authService;
        private readonly IValidator<UserLoginCommand> _validator;
        public UserLoginCommandHandler(IAuthService authService, IValidator<UserLoginCommand> validator)
        {
            _authService = authService;
            _validator = validator;
        }
        public async Task<ResponseDto<SigninDto>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<SigninDto>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }
            return await _authService.AuthenticateAsync(request.UsernameOrEmail, request.Password);
        }
    }

}
