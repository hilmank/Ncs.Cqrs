using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Application.Services;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Users.Commands
{
    public class RfidLoginCommand : IRequest<ResponseDto<SigninDto>>
    {
        public string RfidTagId { get; set; }
    }
    public class RfidLoginCommandHandler(IAuthService authService, IValidator<RfidLoginCommand> validator) : IRequestHandler<RfidLoginCommand, ResponseDto<SigninDto>>
    {
        private readonly IAuthService _authService = authService;
        private readonly IValidator<RfidLoginCommand> _validator = validator;
        public async Task<ResponseDto<SigninDto>> Handle(RfidLoginCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<SigninDto>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }
            return await _authService.AuthenticateAsync(request.RfidTagId);
        }
    }

}
