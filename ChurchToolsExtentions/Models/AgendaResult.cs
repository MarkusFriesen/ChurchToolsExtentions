namespace ChurchToolsExtentions.Models;
public class AgendaResult
{
    public Agenda Data { get; set; } = new Agenda();
}

public class Agenda
{
    public string Name { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public int Id { get; set; }
    public int Total { get; set; }
    public List<Item> Items { get; set; } = new List<Item>();
}

public class Item
{
    public string Note { get; set; } = string.Empty;
    public int Position { get; set; }
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ItemSong Song { get; set; } = new ItemSong();
}

public class ItemSong
{
    public string Arrangement { get; set; } = string.Empty;
    public int ArrangementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SongId { get; set; }
}
