using DealBite.Application.Common.Models;
using DealBite.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Products.Queries.GetProductsByCategory
{
    public class GetProductsByCategoryQuery:IRequest<PaginatedResult<ProductDto>>
    {
        public string Slug { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}
