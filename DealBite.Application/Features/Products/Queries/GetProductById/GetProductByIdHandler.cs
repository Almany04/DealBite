using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using MediatR;


namespace DealBite.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdHandler: IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IProductRepository productRepository, IMapper mapper)
        {
            _repository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdWithDetailsAsync(request.Id);
            if (product==null)
            {
                throw new KeyNotFoundException($"Nem található termék ezzel az azonosítóval: {request.Id}");
            }

            return _mapper.Map<ProductDto>(product);
        }

        
    }
}
