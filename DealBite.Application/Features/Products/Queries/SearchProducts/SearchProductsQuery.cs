using DealBite.Application.Common.Models;
using DealBite.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Products.Queries.SearchProducts
{
    public class SearchProductsQuery:IRequest<PaginatedResult<ProductDto>>
    {
        public string? SearchText { get; set; }
        public Guid? CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
