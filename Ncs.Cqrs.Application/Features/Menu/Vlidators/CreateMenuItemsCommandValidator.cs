using FluentValidation;
using Ncs.Cqrs.Application.Features.Menu.Commands;
using Ncs.Cqrs.Application.Interfaces;

namespace Ncs.Cqrs.Application.Features.Menu.Validators
{
    /// <summary>
    /// Validator for CreateMenuItemsCommand.
    /// </summary>
    public class CreateMenuItemsCommandValidator : AbstractValidator<CreateMenuItemsCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateMenuItemsCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.VendorId)
                .GreaterThan(0).WithMessage("Vendor ID must be greater than zero.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Menu name is required.")
                .MaximumLength(255).WithMessage("Menu name must be less than 255 characters.")
                .MustAsync((name, cancellationToken) => BeUniqueMenuName(0, name, cancellationToken))
                    .WithMessage("Menu name already exists or is too similar.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Calories)
                .GreaterThan(0).WithMessage("Calories must be greater than zero.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Image URL is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Invalid Image URL format.");
        }
        private async Task<bool> BeUniqueMenuName(int id, string name, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Menus.IsMenuNameUniqueAsync(id, name);
        }

    }
}
