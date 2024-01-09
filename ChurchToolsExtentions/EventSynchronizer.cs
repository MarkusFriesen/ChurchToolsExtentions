using ChurchToolsExtentions.Models;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.Net.Http.Json;

namespace ChurchToolsExtentions;

public class EventSynchronizer(IOptions<ConnectionSettings> settings) : AuthorizedClient(settings)
{
    public async Task<AgendaResult?> GetEventAgendaAsync(int eventId)
    {
        await GetLatestCookie();
        return await Client.GetFromJsonAsync<AgendaResult>($"api/events/{eventId}/agenda");
    }

    public async Task<EventResult?> GetEventsAsync(DateTimeOffset from, DateTimeOffset to)
    {
        await GetLatestCookie();
        var result = new EventResult();
        int current = 1;
        int totalEvents;
        do
        {
            var query = $"from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&page={current++}&limit=100&direction=forward";

            var batch = await Client.GetFromJsonAsync<EventResult>($"api/events?{query}");
            result.Meta = batch?.Meta ?? new MetaEvent();
            result.Data.AddRange(batch?.Data ?? new List<Event>());
            totalEvents = batch?.Meta.Count ?? 0;
        } while (result.Data.Count != totalEvents);

        return result;
    }

    public async Task<SongResult?> GetEventSongs(int eventId)
    {
        await GetLatestCookie();
        return await Client.GetFromJsonAsync<SongResult>($"api/events/{eventId}/agenda/songs");
    }

    public async Task<ZipArchive?> DownloadSongBeamerScheduleFromEvent(Agenda agenda)
    {
        await GetLatestCookie();
        var response = await Client.PostAsJsonAsync($"api/agendas/{agenda.Id}/export?target=SONG_BEAMER", new
        {
            appendArrangement = false,
            exportSongs = true,
            withCategory = false,
        });

        if (response is null) return null;

        var url = (await response.Content.ReadFromJsonAsync<DownloadAgendaResponseType>())?.Data?.Url;

        var download = await Client.GetStreamAsync(url);
        return new ZipArchive(download);
    }
}
