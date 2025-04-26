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
    public class roomsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/rooms
        public IQueryable<rooms> Getrooms()
        {
            return db.rooms;
        }

        // GET: api/rooms/5
        [ResponseType(typeof(rooms))]
        public async Task<IHttpActionResult> Getrooms(int id)
        {
            rooms rooms = await db.rooms.FindAsync(id);
            if (rooms == null)
            {
                return NotFound();
            }

            return Ok(rooms);
        }

        // PUT: api/rooms/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putrooms(int id, rooms rooms)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rooms.id)
            {
                return BadRequest();
            }

            db.Entry(rooms).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!roomsExists(id))
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

        // POST: api/rooms
        [ResponseType(typeof(rooms))]
        public async Task<IHttpActionResult> Postrooms(rooms rooms)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.rooms.Add(rooms);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = rooms.id }, rooms);
        }

        // DELETE: api/rooms/5
        [ResponseType(typeof(rooms))]
        public async Task<IHttpActionResult> Deleterooms(int id)
        {
            rooms rooms = await db.rooms.FindAsync(id);
            if (rooms == null)
            {
                return NotFound();
            }

            db.rooms.Remove(rooms);
            await db.SaveChangesAsync();

            return Ok(rooms);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool roomsExists(int id)
        {
            return db.rooms.Count(e => e.id == id) > 0;
        }
    }
}