using System;
using FluentValidation;
using Ncs.Cqrs.Application.Features.Users.Commands;

namespace Ncs.Cqrs.Application.Features.Users.Validators;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{

    public RefreshTokenValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Access token is required.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
