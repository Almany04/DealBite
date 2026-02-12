using DealBite.Application.Common.Models;
using DealBite.Application.DTOs;
using DealBite.Application.Features.Products.Queries.GetAllProducts;
using DealBite.Application.Features.Products.Queries.GetProductById;
using DealBite.Application.Features.Products.Queries.SearchProducts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DealBite.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllProductsQuery());

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            try
            {
                var query = new GetProductByIdQuery { Id = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
               
        }
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResult<ProductDto>>> Search(
            [FromQuery] string? query,
            [FromQuery] Guid? categoryId,
            [FromQuery] int page=1,
            [FromQuery] int pageSize = 10)
        {
            var searchQuery = new SearchProductsQuery
            {
                SearchText = query,
                CategoryId = categoryId,
                PageNumber=page,
                PageSize=pageSize
            };

            var result = await _mediator.Send(searchQuery);
            return Ok(result);
        }
    }
}
