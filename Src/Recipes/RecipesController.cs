using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Natlicious.Api.Recipes.Schema;

namespace Natlicious.Api.Recipes;

[ApiController]
[Route("[controller]")]
public class RecipesController(RecipesService recipesService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var recipes = await recipesService.GetAllAsync();
        return Ok(recipes);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Recipe recipe)
    {
        var createdRecipe = await recipesService.CreateAsync(recipe);
        return Ok(createdRecipe);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] Recipe recipe)
    {
        if (await recipesService.GetByIdAsync(id) == null)
        {
            return NotFound();
        }

        var updatedRecipe = await recipesService.UpdateRecipe(recipe);
        return Ok(updatedRecipe);
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (await recipesService.GetByIdAsync(id) == null)
        {
            return NotFound();
        }
        await recipesService.DeleteAsync(id);
        return NoContent();
    }
}