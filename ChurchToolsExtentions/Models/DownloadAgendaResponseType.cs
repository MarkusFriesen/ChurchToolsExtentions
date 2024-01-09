namespace ChurchToolsExtentions.Models;

public class DownloadAgendaResponseType
{
    public DataResponse? Data { get; set; }
}

public class DataResponse
{
    public string Url { get; set; } = "";
}
