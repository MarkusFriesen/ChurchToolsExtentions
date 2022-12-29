using ChurchToolsExtentions.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace ChurchToolsExtentions
{
    public class EventSynchronizer : AuthorizedClient
    {
        public EventSynchronizer(IOptions<ConnectionSettings> settings) : base(settings) { }
        
        public async Task<AgendaResult?> GetEventAgendaAsync(int eventId)
        {
            await GetLatestCookie();
            return await Client.GetFromJsonAsync<AgendaResult>($"events/{eventId}/agenda");
        }

        public async Task<EventResult?> GetEventsAsync(DateTimeOffset from, DateTimeOffset to)
        {
            await GetLatestCookie();
            var result = new EventResult();
            int current = 1;
            int totalEvents;
            do
            {
                var query = $"from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&page={current++}&limit=250&direction=forward";

                var batch = await Client.GetFromJsonAsync<EventResult>($"events?{query}");
                result.Meta = batch?.Meta ?? new MetaEvent();
                result.Data.AddRange(batch?.Data ?? new List<Event>());
                totalEvents = batch?.Meta.Count ?? 0;
            } while (result.Data.Count != totalEvents);

            return result;
        }

        public async Task<SongResult?> GetEventSongs(int eventId)
        {
            await GetLatestCookie();
            return await Client.GetFromJsonAsync<SongResult>($"events/{eventId}/agenda/songs");
        }
    }
}
