using Ncs.Cqrs.Application.Features.Orders.Commands;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Orders.Validators
{
    public class ChangeOrdersStatusCommandValidator : AbstractValidator<ChangeOrdersStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeOrdersStatusCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Order ID must be greater than zero.")
                .MustAsync(OrdersExists).WithMessage("Order does not exist.");
            RuleFor(x => x)
                .MustAsync(IsValidStatusChange).WithMessage("Invalid status changed.");

        }

        private async Task<bool> OrdersExists(int orderId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Orders.GetOrderByIdAsync(orderId) != null;
        }

        private async Task<bool> IsValidStatusChange(ChangeOrdersStatusCommand command, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetOrderByIdAsync(command.Id);

            if (order == null)
                return false;

            return order.OrderStatus switch
            {
                nameof(OrderStatus.Ordered) => command.Status == nameof(OrderStatus.InProcess) || command.Status == nameof(OrderStatus.Canceled),
                nameof(OrderStatus.InProcess) => command.Status == nameof(OrderStatus.Completed),
                _ => false
            };
        }
    }
}
