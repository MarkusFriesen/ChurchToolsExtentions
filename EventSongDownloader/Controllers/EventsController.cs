using ChurchToolsExtentions;
using ChurchToolsExtentions.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventSongDownloader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly EventSynchronizer _synchronizer;

        public EventsController(ILogger<EventsController> logger, EventSynchronizer synchronizer)
        {
            _logger = logger;
            _synchronizer = synchronizer;
        }

        [HttpGet("month")]
        public async Task<IEnumerable<Event>?> GetMonth([FromQuery] DateTimeOffset month)
        {
            if (month == DateTimeOffset.MinValue)
            {
                return null;
            }

            var beginningMonth = new DateTimeOffset(month.Year, month.Month,1,0,0,0, new TimeSpan()).LocalDateTime;
            var endMonth = beginningMonth.AddMonths(1);

            var result = await _synchronizer.GetEventsAsync(beginningMonth, endMonth);
            return result?.Data.Where(d => $"{d.StartDate:yyyy-MM}" == $"{month:yyyy-MM}");
        }


        [HttpGet("day")]
        public async Task<IEnumerable<Event>?> GetEventsOnDay([FromQuery] DateTimeOffset day)
        {
            if (day == DateTimeOffset.MinValue)
            {
                return null;
            }

            var result = await _synchronizer.GetEventsAsync(day, day);
            return result?.Data.Where(d => $"{d.StartDate:yyyy-MM-dd}" ==  $"{day:yyyy-MM-dd}" );
        }

        [HttpGet("{eventId}/songs")]
        public async Task<IEnumerable<Song>?> GetSongsAsync(int eventId)
        {
            if (eventId == 0) return null;

            var result = await _synchronizer.GetEventSongs(eventId);
            return result?.Data;
        }

        [HttpGet("{eventId}/agenda")]
        public async Task<Agenda?> GetAgendaAsync(int eventId)
        {
            if (eventId == 0) return null;

            var result = await _synchronizer.GetEventAgendaAsync(eventId);
            return result?.Data;
        }
    }
}