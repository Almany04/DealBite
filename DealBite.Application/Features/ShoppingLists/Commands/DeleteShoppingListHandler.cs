using DealBite.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.ShoppingLists.Commands
{
    public class DeleteShoppingListCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
    public class DeleteShoppingListHandler : IRequestHandler<DeleteShoppingListCommand, Unit>
    {
        private readonly IShoppingListRepository _shoppingListRepository;

        public DeleteShoppingListHandler(IShoppingListRepository shoppingListRepository)
        {
            _shoppingListRepository = shoppingListRepository;
        }

        public async Task<Unit> Handle(DeleteShoppingListCommand request, CancellationToken cancellationToken)
        {
            var shoppingList = await _shoppingListRepository.GetByIdAsync(request.Id);
            if (shoppingList == null)
            {
                throw new KeyNotFoundException($"Törölni kivánt lista nem található: {request.Id}");
            }
            await _shoppingListRepository.DeleteAsync(shoppingList);

            return Unit.Value;
        }
    }
}
