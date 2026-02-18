using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Products.Queries.GetByStoreId
{
    public class GetByStoreIdQuery : IRequest<List<ProductDto>>
    {
        public Guid StoreId { get; init; }
        public bool OnlyActive { get; set; }
    }
    public class GetByStoreIdHandler : IRequestHandler<GetByStoreIdQuery, List<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetByStoreIdHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> Handle(GetByStoreIdQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetByStoreIdAsync(request.StoreId, request.OnlyActive);

            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
