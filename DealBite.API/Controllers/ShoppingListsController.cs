using DealBite.Application.DTOs;
using DealBite.Application.Features.ShoppingLists.Commands;
using DealBite.Application.Features.ShoppingLists.Queries.GetShoppingListById;
using DealBite.Application.Features.ShoppingLists.Queries.GetUserShoppingLists;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DealBite.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingListsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShoppingListsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShoppingListDto>>> GetMyLists()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(userIdString)||!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Érvénytelen felhasználói azonosító.");
            }
            var query = new GetUserShoppingListsQuery() { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingListDto>> GetById(Guid Id)
        {
            try
            {
                var query = new GetShoppingListByIdQuery() { Id = Id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        public record CreateListRequest(string Name);

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateListRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Érvénytelen felhasználói azonosító.");
            }
            var command = new CreateShoppingListCommand
            {
                Name = request.Name,
                UserId = userId
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateShoppingListCommand command)
        {
            command.Id = id;

            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var command = new DeleteShoppingListCommand { Id = id};

            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

        }
    }
}
