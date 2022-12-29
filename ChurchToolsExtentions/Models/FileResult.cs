namespace ChurchToolsExtentions.Models;

public class SongResult
{
    public List<Song> Data { get; set; } = new List<Song>();
    public ResultMeta Meta { get; set; } = new ResultMeta();
}

public class ResultMeta
{
    public int Count { get; set; }
    public Pagination Pagination { get; set; } = new Pagination();
}

public class Pagination
{
    public int Total { get; set; }
    public int Limit { get; set; }
    public int Current { get; set; }
    public int LastPage { get; set; }
}

public class Song
{
    public string Name { get; set; } = string.Empty;
    public int Id { get; set; }
    public List<Arrangement> Arrangements { get; set; } = new List<Arrangement>();
    public Category Category { get; set; } = new Category();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class Arrangement
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ArrangementFile> Files { get; set; } = new List<ArrangementFile>();
}

public class ArrangementFile
{
    public string Name { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public Meta Meta { get; set; } = new Meta();
}

public class Meta
{
    public DateTimeOffset ModifiedDate { get; set; }
    public int ModifiedPid { get; set; }
}
