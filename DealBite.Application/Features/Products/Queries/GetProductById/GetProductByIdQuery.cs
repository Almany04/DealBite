using DealBite.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQuery: IRequest<ProductDto>
    {
        public Guid Id { get; init; }
    }
}
