using System.Net;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;
using WebApplicationEventology.Models;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;

namespace WebApplicationEventology.Controllers
{
    public class eventsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/events
        /// <summary>
        /// Gets all events with only local table fields (no foreign keys).
        /// </summary>
        [HttpGet]
        [Route("api/events")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getevents()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var events = await db.events
                .Select(e => new
                {
                    e.id,
                    e.name,
                    e.description,
                    e.if_full_day,
                    e.start_time,
                    e.end_time,
                    e.status,
                    e.created_at,
                    roomName = e.rooms.name,
                    roomDescription = e.rooms.description,
                    roomDistribution = e.rooms.roomLayout,
                    roomHasSeatDistribution = e.rooms.hasSeatingDistribution
                })
                .ToListAsync();

            return Ok(events);
        }

    }
}