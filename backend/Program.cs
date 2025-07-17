using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using PortfolioApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB with environment variable fallback
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING")
                           ?? builder.Configuration.GetConnectionString("MongoDb");

var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase("PortfolioDb");
builder.Services.AddSingleton(mongoDatabase);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "https://portfolio-hub-frontend.vercel.app",
            "https://portfoliohub.vercel.app" 
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Configure Kestrel to listen on port 80 (for Docker/Render)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // Use port 80 inside Docker container
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API V1");
    c.RoutePrefix = "swagger";
});

//app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
