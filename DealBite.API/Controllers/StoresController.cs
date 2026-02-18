using DealBite.Application.Common.Models;
using DealBite.Application.DTOs;
using DealBite.Application.Features.Products.Queries.GetByStoreId;
using DealBite.Application.Features.Products.Queries.GetProductsByCategory;
using DealBite.Application.Features.Stores.Queries.GetAllStores;
using DealBite.Application.Features.Stores.Queries.GetNearbyStores;
using DealBite.Application.Features.Stores.Queries.GetStoreById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DealBite.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoresController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StoresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("nearby")]
        public async Task<ActionResult<List<StoreLocationDto>>> GetNearby(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radius=5000)
        {
            var query = new GetNearbyStoresQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusInMeters = radius
            };
            var result=await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<StoreDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllStoresQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StoreDto>> GetById(Guid id)
        {
            try
            {
                var query = new GetStoreByIdQuery { Id = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        [HttpGet("{id}/products")]
        public async Task<ActionResult<List<ProductDto>>> GetByStoreId(Guid id, bool OnlyActive=false)
        {

            var query = new GetByStoreIdQuery { StoreId = id, OnlyActive = OnlyActive};
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
