using AutoMapper;
using DealBite.Application.Interfaces.Repositories;

using DealBite.Domain.ValueObjects;
using MediatR;

namespace DealBite.Application.Features.ShoppingLists.Commands.ShoppingListCommands
{
    public class CreateShoppingListCommand : IRequest<Guid>
    {
        public required string Name { get; set; }
        public Guid UserId { get; set; }
    }
    public class CreateShoppingListHandler : IRequestHandler<CreateShoppingListCommand, Guid>
    {
        private readonly IShoppingListRepository _shoppingListRepository;

        public CreateShoppingListHandler(IShoppingListRepository shoppingListRepository, IMapper mapper)
        {
            _shoppingListRepository = shoppingListRepository;
        }

        public async Task<Guid> Handle(CreateShoppingListCommand request, CancellationToken cancellationToken)
        {
            var shoppingList = new Domain.Entities.ShoppingList
            {
                Name = request.Name,
                UserId = request.UserId,
                TotalEstimatedPrice = Money.Zero,
                TotalSaved = Money.Zero,
                IsCompleted=false
            };

            await _shoppingListRepository.AddAsync(shoppingList);

            return shoppingList.Id;
        }
    }
}
