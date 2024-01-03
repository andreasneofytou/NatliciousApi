using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Natlicious.Api.Recipes.Schema;

public class Recipe
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required]
    [MinLength(2)]
    public required string Title { get; set; }

    [Required]
    public required string Slug { get; set; }

    [Required]
    public required string Body { get; set; }

    [Required]
    public required string Description { get; set; }

    public bool IsPublished { get; set; }

    public bool IsFeatured { set; get; }

    public List<string>? Tags { get; set; }

    public required Category Category { get; set; }

}