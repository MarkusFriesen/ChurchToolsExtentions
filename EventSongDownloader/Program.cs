using ChurchToolsExtentions;
using ChurchToolsExtentions.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ConnectionSettings>(builder.Configuration.GetSection("ChurchToolsConnection"));

// Add services to the container.
builder.Services.AddSingleton<FileSynchronizer>();
builder.Services.AddSingleton<EventSynchronizer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
