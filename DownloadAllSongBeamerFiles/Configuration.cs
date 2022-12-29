using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadAllSongBeamerFiles;

public class Configuration
{
    public string OutDir { get; set; } = Directory.GetCurrentDirectory();
    public string? Instance { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}
