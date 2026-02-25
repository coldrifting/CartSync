using CartSyncBackend.Database;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Recipes")]
[Route("/api/recipes/[action]")]
public class RecipeController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult All()
    {
        return Ok();
    }
}