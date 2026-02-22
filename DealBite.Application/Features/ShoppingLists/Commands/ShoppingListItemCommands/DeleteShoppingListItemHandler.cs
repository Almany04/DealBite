using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.ShoppingLists.Commands.ShoppingListItemCommands
{
    public class DeleteShoppingListItemCommand : IRequest<Unit>
    {
        public Guid ShoppingListItemId { get; set; }
    }
    public class DeleteShoppingListItemHandler : IRequestHandler<DeleteShoppingListItemCommand, Unit>
    {
        private readonly IShoppingListItemRepository _shoppingListItemRepository;
        private readonly IShoppingListRepository _shoppingListRepository;

        public DeleteShoppingListItemHandler(IShoppingListItemRepository shoppingListItemRepository, IShoppingListRepository shoppingListRepository)
        {
            _shoppingListItemRepository = shoppingListItemRepository;
            _shoppingListRepository = shoppingListRepository;
        }

        public async Task<Unit> Handle(DeleteShoppingListItemCommand request, CancellationToken cancellationToken)
        {
            var shoppingListItem = await _shoppingListItemRepository.GetByIdAsync(request.ShoppingListItemId);
            if (shoppingListItem == null)
                throw new KeyNotFoundException($"Törölni kivánt lista tétel nem található: {request.ShoppingListItemId}");
            
            await _shoppingListItemRepository.DeleteAsync(shoppingListItem);

            var listWithItems = await _shoppingListRepository.GetByIdWithItemsAsync(shoppingListItem.ShoppingListId);

            var totals = ShoppingListCalculator.ShoppingCalculator(
                listWithItems!.ShoppingListItems.ToList().AsReadOnly()
                );

            listWithItems.TotalEstimatedPrice = totals.TotalEstimatedPrice;
            listWithItems.TotalSaved = totals.TotalSaved;

            await _shoppingListRepository.UpdateAsync(listWithItems);
            return Unit.Value;
        }
    }
}
