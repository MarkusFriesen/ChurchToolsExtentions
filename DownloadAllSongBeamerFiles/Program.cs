using ChurchToolsExtentions;
using ChurchToolsExtentions.Models;
using DownloadAllSongBeamerFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System.IO.Compression;
using System.Text.Json;

if (ShowHelpRequired(args))
{
    Console.WriteLine(@"Download all sng files from your ChurchTools instance. If a file has already been downloaded, it will not be downloaded again, unless the content has changed.
Requiered Parameters: 
--instance ex: my-church. If your instance is under https://my-church.church.tools then only enter my-church.
--outDir ex: C:\temp. The directory where all files will be downloaded into.
--username ex: username. The e-mail or username used to login.
--password ex: password. The password used to login.
--maxNumberOfLinesPerSngSlide ex: 2. Optional. The number of lines that a sng slide can have
--agendaPath ex: /temp/agenda. Opional, the folder where the agenda needs to be downloaded to.
");
    return;
}

AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

var configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddCommandLine(args)
                        .Build();

Configuration config = VerifyAndReturnConfiguration(configuration);

IOptions<ConnectionSettings> options = Options.Create<ConnectionSettings>(new()
{
    Instance = config.Instance ?? "", // Make the compiler happy. We already checked that the instnaces are not null in VerifyAndReturnConfiguration
    Username = config.Username ?? "",
    Password = config.Password ?? ""
});
var downloader = new FileSynchronizer(options);

Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // reference: https://github.com/serilog/serilog-settings-configuration
                .Enrich.FromLogContext()
                .CreateLogger();

var files = await downloader.TaskGetAllFiles();
var arrangements = files?.SelectMany(s =>
                    s.Arrangements.SelectMany(a =>
                        a.Files.Where(f => Path.GetExtension(f.Name) == ".sng")
                               .Select(f => new FileWithCategory { File = f, Category = s.Category })))
                    ?.ToList();

EnsureUniqueFileNames(arrangements);

await DownloadAllSngFiles(config, downloader, arrangements);

FormatDownloadedFiles(config);

await DownloadTodaysEvents();

Environment.Exit(0);


async Task DownloadTodaysEvents()
{
    if (Directory.Exists(config.AgendaPath))
    {
        Directory.Delete(config.AgendaPath, true);
    }

    Directory.CreateDirectory(config.AgendaPath);

    var eventSync = new EventSynchronizer(options);
    var today = DateTimeOffset.Now;
    var startOfDay = today.AddDays(-1);
    var endOfDay = today.AddDays(1);
    var events = await eventSync.GetEventsAsync(today, endOfDay);

    foreach (var ev in events?.Data?.Where(d => d.StartDate > startOfDay && d.StartDate < endOfDay) ?? [])
    {
        var agenda = await eventSync.GetEventAgendaAsync(ev.Id);
        if (agenda is null) continue;
        var file = await eventSync.DownloadSongBeamerScheduleFromEvent(agenda.Data);
        if (file is null) continue;
        SaveFile(file, agenda.Data);
    }
}

void SaveFile(ZipArchive archive, Agenda agenda)
{
    var schedule = archive.GetEntry("Schedule.col");

    var fileName = Path.Combine(config.AgendaPath, $"{ReplaceInvalidChars(agenda.Name)}.col");
    if (File.Exists(fileName))
    {
        File.Delete(fileName);
    }
    schedule?.ExtractToFile(fileName);
}

string ReplaceInvalidChars(string filename)
{
    return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
}

static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    var exception = e.ExceptionObject as Exception;
    Log.Logger.Fatal(exception, "Unhandeled exception!: {message}", exception?.Message);
    Log.CloseAndFlush();
}

static void EnsureUniqueFileNames(List<FileWithCategory>? arrangements)
{
    arrangements?.OrderBy(a => a.File.Name);
    var previousFileName = string.Empty;
    foreach (var file in arrangements ?? new())
    {
        if (previousFileName == file.File.Name)
        {
            Log.Warning("Duplicate filenames: {0} at {1}", file.File.Name, file.File.FileUrl);
        }

        previousFileName = file.File.Name;
    }
}

static List<FileWithCategory>? GetPreviousFetchResult(string fetchFileName)
{
    List<FileWithCategory>? previousFetch = null;
    if (File.Exists(fetchFileName))
    {
        try
        {
            previousFetch = JsonSerializer.Deserialize<List<FileWithCategory>>(File.ReadAllText(fetchFileName));
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }

    return previousFetch;
}

static bool ShowHelpRequired(IEnumerable<string> args)
{
    return args.Select(s => s.ToLowerInvariant())
        .Intersect(new[] { "help", "/?", "--help", "-help", "-h" }).Any();
}

static Configuration VerifyAndReturnConfiguration(IConfigurationRoot configuration)
{
    var config = new Configuration();
    configuration.Bind(config);

#pragma warning disable CA2208
    if (string.IsNullOrEmpty(config.OutDir))
    {
        throw new ArgumentException("Out dir is null", nameof(config.OutDir));
    }

    if (string.IsNullOrEmpty(config.Instance))
    {
        throw new ArgumentException("Instance is null", nameof(config.Instance));
    }

    if (string.IsNullOrEmpty(config.Username))
    {
        throw new ArgumentException("USername is null", nameof(config.Username));
    }

    if (string.IsNullOrEmpty(config.Password))
    {
        throw new ArgumentException("Password is null", nameof(config.Password));
    }
#pragma warning restore CA2208

    return config;
}

static async Task DownloadAllFiles(Configuration config, FileSynchronizer downloader, List<FileWithCategory>? arrangements, List<FileWithCategory>? previousFetch)
{
    var folder = config.OutDir;
    if (!Directory.Exists(folder))
    {
        Directory.CreateDirectory(folder);
    }

    foreach (var arrangement in arrangements ?? new())
    {

        var fileName = Path.Combine(folder, arrangement.File.Name);
        if (File.Exists(fileName))
        {
            var info = previousFetch?.FirstOrDefault(p => p.File.Name == arrangement.File.Name);
            if (info?.File.Meta?.ModifiedDate == arrangement.File.Meta.ModifiedDate)
            {
                Log.Information("Skipped {0}", arrangement.File.Name);
                continue;
            }
        }
        await downloader.DownloadFile(arrangement.File, folder);
        Log.Information("Downloaded {0}", arrangement.File.Name);
    }
}

static void FormatDownloadedFiles(Configuration config)
{
    Log.Information("Formatting");

    var formatter = new FileFormatter(new FormatSettings { MaxNumberOfLines = config.MaxNumberOfLinesPerSngSlide });
    foreach (var file in Directory.GetFiles(config.OutDir, "*.sng"))
    {
        Log.Information("Formatting {name}", file);
        File.WriteAllText(file, formatter.Format(File.ReadAllText(file, System.Text.Encoding.UTF8)), System.Text.Encoding.UTF8);
    }

    Log.Information("Finished Formatting");
}

static async Task DownloadAllSngFiles(Configuration config, FileSynchronizer downloader, List<FileWithCategory>? arrangements)
{
    var fetchFileName = Path.Combine(config.OutDir, "lastFetch.json");

    List<FileWithCategory>? previousFetch = GetPreviousFetchResult(fetchFileName);

    await DownloadAllFiles(config, downloader, arrangements, previousFetch);

    File.WriteAllText(fetchFileName, JsonSerializer.Serialize(arrangements));

    Log.Information("Finished Downloading.");
}

internal class FileWithCategory
{
    public ArrangementFile File { get; set; } = new ArrangementFile();
    public Category Category { get; set; } = new Category();
}
