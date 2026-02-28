using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Services;
using DealBite.Domain.ValueObjects;
using MediatR;


namespace DealBite.Application.Features.ShoppingLists.Commands.ShoppingListItemCommands
{
    public class AddShoppingListItemCommand : IRequest<Guid>
    {
        public Guid ShoppingListId { get; set; }
        public Guid? ProductId { get; set; }
        public double Quantity { get; set; }
    }
    public class AddShoppingListItemHandler : IRequestHandler<AddShoppingListItemCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IShoppingListItemRepository _shoppingListItemRepository;
        public AddShoppingListItemHandler(IProductRepository productRepository, IShoppingListRepository shoppingListRepository, IShoppingListItemRepository shoppingListItemRepository)
        {
            _productRepository = productRepository;
            _shoppingListRepository = shoppingListRepository;
            _shoppingListItemRepository = shoppingListItemRepository;
        }

        public async Task<Guid> Handle(AddShoppingListItemCommand request, CancellationToken cancellationToken)
        {
            var shoppingList = await _shoppingListRepository.GetByIdAsync(request.ShoppingListId);

            if (shoppingList == null)
                throw new KeyNotFoundException("Nem létezik ez a bevásárló lista...");

            string productName = "Névtelen hozzávaló";
            Money estimatedPrice = Money.Zero;

            if (request.ProductId.HasValue)
            {
                var product = await _productRepository.GetByIdAsync(request.ProductId.Value);
                if (product == null)
                    throw new KeyNotFoundException("Nem létezik ez a termék...");

                productName = product.Name;
                var cheapestPrice = await _productRepository.GetEstimatedPriceMinimumWithDetailsAsync(request.ProductId.Value);
                estimatedPrice = (cheapestPrice?.Price ?? Money.Zero) * (decimal)request.Quantity;
            }
        
            var item = new Domain.Entities.ShoppingListItem
            {
                ProductName = productName,
                Quantity = request.Quantity,
                IsChecked = false,
                EstimatedPrice = estimatedPrice,
                ProductId = request.ProductId,
                StoreId = null,
                ShoppingListId=request.ShoppingListId
            };

            await _shoppingListItemRepository.AddAsync(item);

            var listWithItems = await _shoppingListRepository.GetByIdWithItemsAsync(request.ShoppingListId);

            var totals = ShoppingListCalculator.ShoppingCalculator(
                listWithItems!.ShoppingListItems.ToList().AsReadOnly()
                );

            shoppingList.TotalEstimatedPrice = totals.TotalEstimatedPrice;
            shoppingList.TotalSaved = totals.TotalSaved;

            await _shoppingListRepository.UpdateAsync(shoppingList);

            return item.Id;
        }
    }
}
