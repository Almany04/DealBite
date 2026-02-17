using AutoMapper;
using DealBite.Application.Common.Models;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Products.Queries.GetProductsByCategory
{
    public class GetProductsByCategoryQuery : IRequest<PaginatedResult<ProductDto>>
    {
        public string Slug { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
    public class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategoryQuery, PaginatedResult<ProductDto>>
    {
        private ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productrepository;
        private readonly IMapper _mapper;

        public GetProductsByCategoryHandler(ICategoryRepository categoryRepository ,IProductRepository repository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _productrepository = repository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<ProductDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetBySlugAsync(request.Slug);
            if (category == null)
            {
                throw new KeyNotFoundException($"Nincs ilyen kategória: {request.Slug}");
            }
            var (products, totalCount) = await _productrepository.SearchAsync(
                searchText: null,
                categoryId: category.Id,
                page: request.PageNumber,
                pageSize: request.PageSize
                );

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
