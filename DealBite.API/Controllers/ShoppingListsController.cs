using DealBite.Application.DTOs;
using DealBite.Application.Features.Recipes.Commands;
using DealBite.Application.Features.ShoppingLists.Commands.ShoppingListCommands;
using DealBite.Application.Features.ShoppingLists.Commands.ShoppingListItemCommands;
using DealBite.Application.Features.ShoppingLists.Queries.GetMultiStoreOptimization;
using DealBite.Application.Features.ShoppingLists.Queries.GetShoppingListById;
using DealBite.Application.Features.ShoppingLists.Queries.GetSingleStoreOptimization;
using DealBite.Application.Features.ShoppingLists.Queries.GetUserShoppingLists;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            if (!TryGetUserId(out var userId))
                return Unauthorized("Érvénytelen felhasználói azonosító.");

            var query = new GetUserShoppingListsQuery() { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingListDto>> GetById(Guid id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            try
            {
                var result = await _mediator.Send(new GetShoppingListByIdQuery { Id = id });

                if (result.UserId != userId)
                    return Forbid();

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
            if (!TryGetUserId(out var userId))
                return Unauthorized("Érvénytelen felhasználói azonosító.");

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
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!await IsOwner(id, userId))
                return Forbid();

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
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!await IsOwner(id, userId))
                return Forbid();

            var command = new DeleteShoppingListCommand { Id = id };

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

        public record CreateListItemRequest(Guid? ProductId, double Quantity);

        [HttpPost("{shoppingListId}/items")]
        public async Task<ActionResult<Guid>> CreateItem(Guid shoppingListId, [FromBody] CreateListItemRequest request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!await IsOwner(shoppingListId, userId))
                return Forbid();

            var command = new AddShoppingListItemCommand
            {
                Quantity = request.Quantity,
                ProductId = request.ProductId,
                ShoppingListId = shoppingListId
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}/items/{shoppingListItemId}")]
        public async Task<ActionResult> UpdateItem(Guid id, Guid shoppingListItemId, [FromBody] UpdateShoppingListItemCommand command)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!await IsOwner(id, userId))
                return Forbid();

            command.ShoppingListItemId = shoppingListItemId;

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

        [HttpDelete("{id}/items/{shoppingListItemId}")]
        public async Task<ActionResult> DeleteItem(Guid id, Guid shoppingListItemId)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!await IsOwner(id, userId))
                return Forbid();

            var command = new DeleteShoppingListItemCommand { ShoppingListItemId = shoppingListItemId };

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

        [HttpGet("{id}/optimize")]
        public async Task<IActionResult> OptimizeShoppingList(Guid id, [FromQuery] string mode = "single", [FromQuery] List<Guid>? storeIds = null)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized("Érvénytelen felhasználói azonosító.");

            if (!await IsOwner(id, userId))
                return Forbid();

            if (string.Equals(mode, "single", StringComparison.OrdinalIgnoreCase))
            {
                var query = new GetSingleStoreOptimizationQuery { Id = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }

            if (string.Equals(mode, "multi", StringComparison.OrdinalIgnoreCase))
            {
                var query = new GetMultiStoreOptimizationQuery { Id = id, StoreIds = storeIds };
                var result = await _mediator.Send(query);
                return Ok(result);
            }

            return BadRequest("Támogatott módok: 'single', 'multi'.");
        }
        public record AddRecipeRequest(List<Guid>? SelectedIngredientsIds);

        [HttpPost("{shoppingListId}/recipes/{recipeId}")]
        public async Task<ActionResult> AddRecipe(Guid shoppingListId, Guid recipeId, [FromBody] AddRecipeRequest? request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!await IsOwner(shoppingListId, userId))
                return Forbid();

            var command = new AddRecipeToShoppingListCommand
            {
                RecipeId = recipeId,
                ShoppingListId = shoppingListId,
                SelectedIngredientsIds = request?.SelectedIngredientsIds
            };

            await _mediator.Send(command);
            return NoContent();
        }


        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return !string.IsNullOrEmpty(claim) && Guid.TryParse(claim, out userId);
        }

        private async Task<bool> IsOwner(Guid shoppingListId, Guid userId)
        {
            try
            {
                var list = await _mediator.Send(new GetShoppingListByIdQuery { Id = shoppingListId });
                return list.UserId == userId;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
    }
}