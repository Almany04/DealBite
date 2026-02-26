using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Domain.Services;
using DealBite.Domain.ValueObjects;
using MediatR;
namespace DealBite.Application.Features.ShoppingLists.Commands.ShoppingListItemCommands
{
    public class UpdateShoppingListItemCommand : IRequest<Unit>
    {
        public Guid ShoppingListItemId { get; set; }
        public bool IsChecked { get; set; }
        public double Quantity { get; set; }
    }
    public class UpdateShoppingListItemHandler : IRequestHandler<UpdateShoppingListItemCommand, Unit>
    {
        private readonly IShoppingListItemRepository _shoppingListItemRepository;
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IProductRepository _productRepository;

        public UpdateShoppingListItemHandler(IShoppingListItemRepository shoppingListItemRepository, IProductRepository productRepository, IShoppingListRepository shoppingListRepository)
        {
            _shoppingListItemRepository = shoppingListItemRepository;
            _productRepository = productRepository;
            _shoppingListRepository = shoppingListRepository;
        }

        public async Task<Unit> Handle(UpdateShoppingListItemCommand request, CancellationToken cancellationToken)
        {
            var shoppingListItem = await _shoppingListItemRepository.GetByIdAsync(request.ShoppingListItemId);
            if (shoppingListItem == null)
                throw new KeyNotFoundException("Lista tétel nem található");
            shoppingListItem.IsChecked = request.IsChecked;
            if (shoppingListItem.Quantity != request.Quantity)
            {
                shoppingListItem.Quantity = request.Quantity;

                var cheapestitem= await _productRepository.GetEstimatedPriceMinimumWithDetailsAsync(shoppingListItem.ProductId);

                shoppingListItem.EstimatedPrice = (cheapestitem?.Price ?? Money.Zero) * (decimal)(request.Quantity);
            }

            await _shoppingListItemRepository.UpdateAsync(shoppingListItem);

            var listWithItems = await _shoppingListRepository.GetByIdWithItemsAsync(shoppingListItem.ShoppingListId);

            var totals = ShoppingListCalculator.ShoppingCalculator(
                listWithItems!.ShoppingListItems.ToList().AsReadOnly()
                );

            var list = await _shoppingListRepository.GetByIdAsync(shoppingListItem.ShoppingListId);
            list!.TotalEstimatedPrice = totals.TotalEstimatedPrice;
            list.TotalSaved = totals.TotalSaved;

            await _shoppingListRepository.UpdateAsync(list);
            return Unit.Value;
        }
    }
}
