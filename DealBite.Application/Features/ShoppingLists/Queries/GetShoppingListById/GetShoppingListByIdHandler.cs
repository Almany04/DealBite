using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.ShoppingLists.Queries.GetShoppingListById
{
    public class GetShoppingListByIdQuery : IRequest<ShoppingListDto>
    {
        public Guid Id { get; set; }
    }
    public class GetShoppingListByIdHandler : IRequestHandler<GetShoppingListByIdQuery, ShoppingListDto>
    {
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IMapper _mapper;

        public GetShoppingListByIdHandler(IShoppingListRepository shoppingListRepository, IMapper mapper)
        {
            _shoppingListRepository = shoppingListRepository;
            _mapper = mapper;
        }

        public async Task<ShoppingListDto> Handle(GetShoppingListByIdQuery request, CancellationToken cancellationToken)
        {
            var shoppingList = await _shoppingListRepository.GetByIdWithItemsAsync(request.Id);
            if (shoppingList == null)
            {
                throw new KeyNotFoundException($"Nem található bevásárlólista ezzel az azonosítóval: {request.Id}");
            }
            return _mapper.Map<ShoppingListDto>(shoppingList);
        }
    }
}
