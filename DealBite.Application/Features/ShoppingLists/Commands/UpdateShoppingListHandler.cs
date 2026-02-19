using DealBite.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.ShoppingLists.Commands
{
    public class UpdateShoppingListCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
    public class UpdateShoppingListHandler : IRequestHandler<UpdateShoppingListCommand, Unit>
    {
        private readonly IShoppingListRepository _shoppingListRepository;

        public UpdateShoppingListHandler(IShoppingListRepository shoppingListRepository)
        {
            _shoppingListRepository = shoppingListRepository;
        }

        public async Task<Unit> Handle(UpdateShoppingListCommand request, CancellationToken cancellationToken)
        {
            var shoppingList = await _shoppingListRepository.GetByIdAsync(request.Id);
            if (shoppingList == null)
            {
                throw new KeyNotFoundException("Módosítani kívánt lista nem található...");
            }

            shoppingList.Name = request.Name;
            shoppingList.IsCompleted = request.IsCompleted;

            await _shoppingListRepository.UpdateAsync(shoppingList);

            return Unit.Value;
        }
    }
}
