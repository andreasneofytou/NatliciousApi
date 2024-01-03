using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Natlicious.Api.Settings;
using Natlicious.Api.Users.Schema;

namespace Natlicious.Api.Users;

public class UsersService
{
    private readonly IMongoCollection<User> usersCollection;

    public UsersService(IOptions<MainDbSettings> mainDbSettings)
    {
        var mongoClient = new MongoClient(mainDbSettings.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(mainDbSettings.Value.DatabaseName);

        usersCollection = mongoDb.GetCollection<User>("users");
    }

    public async Task<User?> GetUserByEmailAsync(string email) =>
        await usersCollection.FindAsync(u => u.Email == email).Result.FirstOrDefaultAsync();

    public async Task<User?> GetUserByIdAsync(string id) =>
        await usersCollection.FindAsync(u => u.Id == id).Result.FirstOrDefaultAsync();

    public async Task<User> CreateUserAsync(User user)
    {
        await usersCollection.InsertOneAsync(user);
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        await usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
        return user;
    }

    public async Task DeleteUserAsync(string id) => await usersCollection.DeleteOneAsync(u => u.Id == id);
}