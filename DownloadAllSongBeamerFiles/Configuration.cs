namespace DownloadAllSongBeamerFiles;

public class Configuration
{
    public string OutDir { get; set; } = Directory.GetCurrentDirectory();
    public string? Instance { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int? MaxNumberOfLinesPerSngSlide { get; set; }
    public string AgendaPath { get; set; } = "/tmp";
}
