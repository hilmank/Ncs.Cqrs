using System;
using Ncs.Cqrs.Application.Features.Menu.Commands;
using Ncs.Cqrs.Application.Interfaces;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Menu.Vlidators;

public class UpdateMenuItemsCommandValidator : AbstractValidator<UpdateMenuItemsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateMenuItemsCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;


        RuleFor(x => x.Name)
            .MaximumLength(255).WithMessage("Menu name must be less than 255 characters.")
            .MustAsync((command, name, cancellationToken) => BeUniqueMenuName(command.Id, name, cancellationToken))
                    .WithMessage("Menu name already exists or is too similar.")
            .When(v => !string.IsNullOrEmpty(v.Name));


        RuleFor(x => x.ImageUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Invalid Image URL format.")
            .When(v => !string.IsNullOrEmpty(v.ImageUrl));
        ;
    }
    private async Task<bool> BeUniqueMenuName(int id, string name, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Menus.IsMenuNameUniqueAsync(id, name);
    }
}