using DealBite.Application.DTOs;
using DealBite.Application.Features.Recipes.Queries;
using DealBite.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DealBite.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecipesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("recommended")]
        public async Task<ActionResult<List<RecommendedRecipeDto>>> GetRecommended(
            [FromQuery] string mode="single",
            [FromQuery] Guid? storeId=null,
            [FromQuery] ProductSegment segment=ProductSegment.Standard)
        {
            var query = new GetRecommendedRecipesQuery
            {
                Mode = mode,
                StoreId = storeId,
                Segment = segment
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
