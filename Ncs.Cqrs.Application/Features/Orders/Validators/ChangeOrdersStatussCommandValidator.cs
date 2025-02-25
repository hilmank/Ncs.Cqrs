using Ncs.Cqrs.Application.Features.Orders.Commands;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Orders.Validators
{
    public class ChangeOrdersStatussCommandValidator : AbstractValidator<ChangeOrdersStatussCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeOrdersStatussCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Ids)
                .NotEmpty().WithMessage("Order IDs list cannot be empty.")
                .Must(ids => ids.All(id => id > 0)).WithMessage("All Order IDs must be greater than zero.")
                .MustAsync(OrdersExist).WithMessage("One or more orders do not exist.");

            RuleFor(x => x)
                .MustAsync(IsValidStatusChange).WithMessage("Invalid status change.");
        }

        private async Task<bool> OrdersExist(List<int> orderIds, CancellationToken cancellationToken)
        {
            if (orderIds == null || !orderIds.Any())
                return false;

            var existingOrders = (await _unitOfWork.Orders.GetOrdersByIdsAsync(orderIds)).ToList();
            return existingOrders.Count == orderIds.Count;
        }

        private async Task<bool> IsValidStatusChange(ChangeOrdersStatussCommand command, CancellationToken cancellationToken)
        {
            var orders = (await _unitOfWork.Orders.GetOrdersByIdsAsync(command.Ids)).ToList();

            if (orders.Count != command.Ids.Count)
                return false;

            foreach (var order in orders)
            {
                if (order == null)
                    return false;

                var isValid = order.OrderStatus switch
                {
                    nameof(OrderStatus.Ordered) => command.Status == nameof(OrderStatus.InProcess) || command.Status == nameof(OrderStatus.Canceled),
                    nameof(OrderStatus.InProcess) => command.Status == nameof(OrderStatus.Completed),
                    _ => false
                };

                if (!isValid)
                    return false;
            }

            return true;
        }

    }
}
