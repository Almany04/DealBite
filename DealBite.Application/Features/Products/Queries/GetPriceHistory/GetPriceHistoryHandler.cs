using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using MediatR;


namespace DealBite.Application.Features.Products.Queries.GetPriceHistory
{
    public class GetPriceHistoryQuery : IRequest<List<PriceHistoryDto>>
    {
        public Guid ProductId { get; set; }
    }
    public class GetPriceHistoryHandler : IRequestHandler<GetPriceHistoryQuery, List<PriceHistoryDto>>
    {
        private readonly IPriceHistoryRepository _priceHistory;
        private readonly IMapper _mapper;

        public GetPriceHistoryHandler(IPriceHistoryRepository priceHistory, IMapper mapper)
        {
            _priceHistory = priceHistory;
            _mapper = mapper;
        }

        public async Task<List<PriceHistoryDto>> Handle(GetPriceHistoryQuery request, CancellationToken cancellationToken)
        {
            var history = await _priceHistory.GetByProductIdAsync(request.ProductId);
            if (history == null)
            {
                throw new KeyNotFoundException($"Nem található termék ezzel az azonosítóval: {request.ProductId}");
            }

            return _mapper.Map<List<PriceHistoryDto>>(history);
        }
    }
}
