using DealBite.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Products.Queries.GetPriceHistory
{
    public class GetPriceHistoryQuery:IRequest<List<PriceHistoryDto>>
    {
        public Guid ProductId { get; set; }
    }
}
