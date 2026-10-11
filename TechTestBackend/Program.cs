using Microsoft.EntityFrameworkCore;
using TechTestBackend;
using TechTestBackend.Database;
using TechTestBackend.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddLogging();
builder.Services.AddTransient<SpotifyAuthHandler>();
builder.Services.AddHttpClient<IApiMarker, ApiMarker>(client =>
{
    client.BaseAddress = new Uri("https://api.spotify.com/");
})
.AddHttpMessageHandler<SpotifyAuthHandler>();

builder.Services.AddDbContextFactory<SongStorageDbContext>(options => options.UseInMemoryDatabase("Songstorage"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();