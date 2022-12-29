using ChurchToolsExtentions;
using EventSongDownloader.Controllers.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace EventSongDownloader.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly FileSynchronizer _synchronizer;
    private readonly string directory = Path.Combine(Path.GetTempPath(), "agendas");

    public SongsController(FileSynchronizer synchronizer)
    {
        _synchronizer = synchronizer;

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    [HttpPost("merge")]
    public async Task<string> Merge([FromBody] MergeParameters parameters)
    {
        var randomFileName = $"{Path.GetRandomFileName()}.pdf";
        await _synchronizer.MergeFiles(parameters.FileUrls, Path.Combine(directory, randomFileName));
        return randomFileName;
    }

    [HttpGet("download/{filename}")]
    public IActionResult DownloadPdfFile(string filename, [FromQuery] string? downloadedName)
    {
        var fullPath = Path.Combine(directory, filename);

        if (!new FileInfo(fullPath).Exists || !filename.EndsWith(".pdf"))
            return new NotFoundResult();

        var stream =  System.IO.File.OpenRead(fullPath);
        string mimeType = "application/pdf";
        return new FileStreamResult(stream, mimeType)
        {
            FileDownloadName = downloadedName ?? filename
        };
    }
}

