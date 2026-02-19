using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.ShoppingLists.Queries.GetUserShoppingLists
{
    public class GetUserShoppingListsQuery : IRequest<List<ShoppingListDto>>
    {
        public Guid UserId { get; set; }
    }
    public class GetUserShoppingListsHandler : IRequestHandler<GetUserShoppingListsQuery, List<ShoppingListDto>>
    {
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IMapper _mapper;

        public GetUserShoppingListsHandler(IShoppingListRepository shoppingListRepository, IMapper mapper)
        {
            _shoppingListRepository = shoppingListRepository;
            _mapper = mapper;
        }

        public async Task<List<ShoppingListDto>> Handle(GetUserShoppingListsQuery request, CancellationToken cancellationToken)
        {
            var shoppingLists = await _shoppingListRepository.GetByUserIdAsync(request.UserId);

            return _mapper.Map<List<ShoppingListDto>>(shoppingLists);
        }
    }
}
