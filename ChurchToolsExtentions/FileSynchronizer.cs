using ChurchToolsExtentions.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
namespace ChurchToolsExtentions;

public class FileSynchronizer : AuthorizedClient
{
    public FileSynchronizer(IOptions<ConnectionSettings> settings) : base(settings) { }

    public async Task<List<Song>?> TaskGetAllFiles()
    {
        await GetLatestCookie();
        List<Song> allSongs = new();
        int current = 1;
        int lastPage;
        do
        {
            var result = await Client.GetFromJsonAsync<SongResult>($"songs?limit=250&page={current}", options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            });

            if (result is null)
            {
                return null;
            }

            allSongs.AddRange(result.Data);

            lastPage = result.Meta.Pagination.LastPage;

        } while (current++ != lastPage);

        return allSongs;
    }

    public async Task DownloadFile(ArrangementFile arrangement, string directory)
    {
        var array = await GetBytes(arrangement.FileUrl);
        await File.WriteAllBytesAsync(Path.Combine(directory, arrangement.Name), array);
    }

    public async Task<byte[]> GetBytes(string url)
    {
        await GetLatestCookie();
        return await Client.GetByteArrayAsync(url);
    }

    public async Task MergeFiles(IEnumerable<string> FileUrls, string targetFile)
    {
        using var pdf = new PdfDocument(new PdfWriter(new FileStream(targetFile, FileMode.Create, FileAccess.Write)));
        var merger = new PdfMerger(pdf);
        foreach (var fileUrl in FileUrls)
        {
            var bytes = await GetBytes(fileUrl);
            using var stream = new MemoryStream(bytes);

            using var firstSourcePdf = new PdfDocument(new PdfReader(stream));
            merger.Merge(firstSourcePdf, 1, firstSourcePdf.GetNumberOfPages());
        }

        pdf.Close();
        DeleteFile(targetFile, TimeSpan.FromMinutes(15));
    }

    public static void DeleteFile(string fileName, TimeSpan deleteIn)
    {
        Task.Run(async () =>
        {
            await Task.Delay(deleteIn);
            File.Delete(fileName);
        });
    }

}