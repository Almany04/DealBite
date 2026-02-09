using DealBite.Application.DTOs;
using DealBite.Application.Features.Stores.Queries.GetNearbyStores;
using MediatR;
using Microsoft.AspNetCore.Http;
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
    }
}
