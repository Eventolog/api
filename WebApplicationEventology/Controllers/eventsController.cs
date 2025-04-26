using System.Net;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;
using WebApplicationEventology.Models;
using System.Data.Entity.Infrastructure;

namespace WebApplicationEventology.Controllers
{
    public class eventsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/events
        public IQueryable<events> Getevents()
        {
            return db.events;
        }

        // GET: api/events/5
        [ResponseType(typeof(events))]
        public async Task<IHttpActionResult> Getevents(int id)
        {
            events events = await db.events.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }

            return Ok(events);
        }

        // PUT: api/events/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putevents(int id, events events)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != events.id)
            {
                return BadRequest();
            }

            db.Entry(events).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!eventsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/events
        [ResponseType(typeof(events))]
        public async Task<IHttpActionResult> Postevents(events events)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.events.Add(events);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = events.id }, events);
        }

        // DELETE: api/events/5
        [ResponseType(typeof(events))]
        public async Task<IHttpActionResult> Deleteevents(int id)
        {
            events events = await db.events.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }

            db.events.Remove(events);
            await db.SaveChangesAsync();

            return Ok(events);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool eventsExists(int id)
        {
            return db.events.Count(e => e.id == id) > 0;
        }
    }
}