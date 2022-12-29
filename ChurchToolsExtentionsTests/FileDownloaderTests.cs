using ChurchToolsExtentions;
using ChurchToolsExtentions.Models;
using Microsoft.Extensions.Options;

namespace ChurchToolsSongExtentionTests;

public class FileDownloaderTests
{
    //Todo: To run the test, set the correct instancename, username and password.
    [Fact]
    public async Task TestAllFileDownload()
    {
        IOptions<ConnectionSettings> options = Options.Create<ConnectionSettings>(new ()
        {
            Instance = "test",
            Username = "test",
            Password = "test"
        });

        var downloader = new FileSynchronizer(options);

        var files = await downloader.TaskGetAllFiles();

        var arrangements = files?.SelectMany(f => f.Arrangements.SelectMany(f => f.Files.Where(f => Path.GetExtension(f.Name) == ".sng")));

        Assert.NotNull(arrangements);

        foreach(var arrangement in arrangements)
        {
            await downloader.DownloadFile(arrangement, Directory.GetCurrentDirectory());
        }
    }
}