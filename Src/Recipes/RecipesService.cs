using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Natlicious.Api.Recipes.Schema;
using Natlicious.Api.Settings;

namespace Natlicious.Api.Recipes;

public class RecipesService
{
    private readonly IMongoCollection<Recipe> recipesCollection;

    public RecipesService(IOptions<MainDbSettings> mainDbSettings)
    {
        var mongoClient = new MongoClient(mainDbSettings.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(mainDbSettings.Value.DatabaseName);

        recipesCollection = mongoDb.GetCollection<Recipe>("recipes");
    }

    public async Task<List<Recipe>> GetAllAsync() => await recipesCollection.FindAsync(_ => true).Result.ToListAsync();

    public async Task<Recipe?> GetByIdAsync(string id) =>
        await recipesCollection.FindAsync(r => r.Id == id).Result.FirstOrDefaultAsync();

    public async Task<Recipe> CreateAsync(Recipe recipe)
    {
        await recipesCollection.InsertOneAsync(recipe);
        return recipe;
    }

    public async Task<Recipe> UpdateRecipe(Recipe recipe)
    {
        await recipesCollection.ReplaceOneAsync(r => r.Id == recipe.Id, recipe);
        return recipe;
    }

    public async Task DeleteAsync(string id) => await recipesCollection.DeleteOneAsync(r => r.Id == id);
}