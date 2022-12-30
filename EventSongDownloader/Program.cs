using ChurchToolsExtentions;
using ChurchToolsExtentions.Models;
using EventSongDownloader;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration) // reference: https://github.com/serilog/serilog-settings-configuration
                .Enrich.FromLogContext()
                .CreateLogger();

AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

builder.Services.AddLogging(l => l.AddSerilog(Log.Logger, true));
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


if (!Directory.Exists(Constants.AgendaDirectory))
    Directory.CreateDirectory(Constants.AgendaDirectory);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Constants.AgendaDirectory),
    DefaultContentType= "application/pdf",
    RequestPath = "/api/songs/download"
});
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();

static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    var exception = e.ExceptionObject as Exception;
    Log.Logger.Fatal(exception, "Unhandeled exception!: {message}", exception?.Message);
    Log.CloseAndFlush();
}
