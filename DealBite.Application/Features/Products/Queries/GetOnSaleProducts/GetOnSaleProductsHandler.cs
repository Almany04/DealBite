using AutoMapper;
using DealBite.Application.Common.Models;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Products.Queries.GetOnSaleProducts
{
    public class GetOnSaleProductsHandler : IRequestHandler<GetOnSaleProductsQuery, PaginatedResult<ProductDto>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetOnSaleProductsHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<ProductDto>> Handle(GetOnSaleProductsQuery request, CancellationToken cancellationToken)
        {
            var (products, totalCount) = await _repository.GetOnSaleAsync(
                request.SearchText,
                request.CategoryId,
                request.PageNumber,
                request.PageSize);

            var productDtos = _mapper.Map<List<ProductDto>>(products);

            return new PaginatedResult<ProductDto>
            {
                Items = productDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
