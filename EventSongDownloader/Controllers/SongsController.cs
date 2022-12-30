using ChurchToolsExtentions;
using EventSongDownloader.Controllers.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace EventSongDownloader.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly FileSynchronizer _synchronizer;

    public SongsController(FileSynchronizer synchronizer)
    {
        _synchronizer = synchronizer;
    }

    [HttpPost("merge")]
    public async Task<string> Merge([FromBody] MergeParameters parameters)
    {
        var randomFileName = $"{Path.GetRandomFileName()}.pdf";
        await _synchronizer.MergeFiles(parameters.FileUrls, Path.Combine(Constants.AgendaDirectory, randomFileName));
        return randomFileName;
    }

    [HttpDelete("download/{filename}")]
    public bool DeletePdfFile(string filename)
    {
        var fullPath = Path.Combine(Constants.AgendaDirectory, filename);
        if (!new FileInfo(fullPath).Exists || !filename.EndsWith(".pdf"))
            return false;

        System.IO.File.Delete(fullPath);
        return true;
    }
}

