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
                    e.created_at
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/{id}
        /// <summary>
        /// Gets a specific event by ID, showing only own fields (no foreign keys).
        /// </summary>
        [HttpGet]
        [Route("api/events/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getevent(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var ev = await db.events
                .Where(e => e.id == id)
                .Select(e => new
                {
                    e.id,
                    e.name,
                    e.description,
                    e.if_full_day,
                    e.start_time,
                    e.end_time,
                    e.status,
                    e.created_at
                })
                .FirstOrDefaultAsync();

            if (ev == null)
                return NotFound();

            return Ok(ev);
        }

        // PUT: api/events/{id}
        /// <summary>
        /// Updates an existing event by ID.
        /// </summary>
        [HttpPut]
        [Route("api/events/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putevent(int id, events eventUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != eventUpdate.id)
                return BadRequest("ID mismatch");

            var existing = await db.events.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.name = eventUpdate.name;
            existing.description = eventUpdate.description;
            existing.if_full_day = eventUpdate.if_full_day;
            existing.start_time = eventUpdate.start_time;
            existing.end_time = eventUpdate.end_time;
            existing.status = eventUpdate.status;
            existing.created_at = eventUpdate.created_at;
            existing.organizer_id = eventUpdate.organizer_id;
            existing.room_id = eventUpdate.room_id;

            try
            {
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException)
            {
                return InternalServerError();
            }
        }

        // POST: api/events
        /// <summary>
        /// Creates a new event.
        /// </summary>
        [HttpPost]
        [Route("api/events")]
        [ResponseType(typeof(events))]
        public async Task<IHttpActionResult> Postevent(events newEvent)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.events.Add(newEvent);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = newEvent.id }, newEvent);
        }

        // DELETE: api/events/{id}
        /// <summary>
        /// Deletes an event by ID.
        /// </summary>
        [HttpDelete]
        [Route("api/events/{id}")]
        [ResponseType(typeof(events))]
        public async Task<IHttpActionResult> Deleteevent(int id)
        {
            var ev = await db.events.FindAsync(id);
            if (ev == null)
                return NotFound();

            db.events.Remove(ev);
            await db.SaveChangesAsync();

            return Ok(ev);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}