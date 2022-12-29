namespace ChurchToolsExtentions.Models;
public class EventResult
{
    public List<Event> Data { get; set; } = new List<Event>();
    public MetaEvent Meta { get; set; } = new MetaEvent();
}

public class MetaEvent
{
    public int Count { get; set; }
}

public class Event
{
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset EndDate { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public DateTimeOffset StartDate { get; set; }
    public List<EventService> EventServices { get; set; } = new List<EventService>();
}

public class EventService
{

    public string Comment { get; set; } = string.Empty;
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
}
