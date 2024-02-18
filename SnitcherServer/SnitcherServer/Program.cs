using SnitcherServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();
app.Urls.Add("http://0.0.0.0:8080");

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

await SnitcherServer.Services.AppDomain.Instance.CreateSignalRConnection();
app.Run();
