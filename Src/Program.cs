using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using Natlicious.Api.Auth;
using Natlicious.Api.Recipes;
using Natlicious.Api.Settings;
using Natlicious.Api.Users;
using Natlicious.Api.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
}).AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MainDbSettings>(builder.Configuration.GetSection("MainDb"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Authentication:Schemes:Bearer"));
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://natlicious.com/api/modelvalidationproblem",
            Title = "One or more model validation errors occurred.",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = "See the errors property for details.",
            Instance = context.HttpContext.Request.Path,

        };

        problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

        return new UnprocessableEntityObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json", "application/problem+xml" }
        };
    };
});

builder.Services.AddScoped<RecipesService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsersService>();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Schemes:Bearer:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Schemes:Bearer:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Schemes:Bearer:Secret"]!))
    };
});

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();