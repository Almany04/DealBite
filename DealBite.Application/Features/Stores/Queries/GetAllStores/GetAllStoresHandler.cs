using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Stores.Queries.GetAllStores
{
    public class GetAllStoresQuery : IRequest<List<StoreDto>>
    {

    }
    public class GetAllStoresHandler : IRequestHandler<GetAllStoresQuery, List<StoreDto>>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public GetAllStoresHandler(IStoreRepository storeRepository, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public async Task<List<StoreDto>> Handle(GetAllStoresQuery request, CancellationToken cancellationToken)
        {
            var stores = await _storeRepository.GetAllWithLocationsAsync();

            return _mapper.Map<List<StoreDto>>(stores);
        }
    }
}
