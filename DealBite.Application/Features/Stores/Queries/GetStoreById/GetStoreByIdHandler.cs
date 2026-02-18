using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Stores.Queries.GetStoreById
{
    public class GetStoreByIdQuery : IRequest<StoreDto>
    {
        public Guid Id { get; set; }
    }
    public class GetStoreByIdHandler : IRequestHandler<GetStoreByIdQuery, StoreDto>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public GetStoreByIdHandler(IStoreRepository storeRepository, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public async Task<StoreDto> Handle(GetStoreByIdQuery request, CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByIdWithLocationsAsync(request.Id);
            if (store == null)
            {
                throw new KeyNotFoundException($"Nem található bolt ezzel az azonosítóval: {request.Id}");
            }
            return _mapper.Map<StoreDto>(store);
        }
    }
}
