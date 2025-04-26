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
    public class roomsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/rooms
        /// <summary>
        /// Gets all rooms (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/rooms")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getrooms()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var rooms = await db.rooms
                .Select(r => new
                {
                    r.id,
                    r.name,
                    r.capacity,
                    r.description
                })
                .ToListAsync();

            return Ok(rooms);
        }

        // GET: api/rooms/{id}
        /// <summary>
        /// Gets a specific room by ID (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/rooms/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getrooms(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var room = await db.rooms
                .Where(r => r.id == id)
                .Select(r => new
                {
                    r.id,
                    r.name,
                    r.capacity,
                    r.description
                })
                .FirstOrDefaultAsync();

            if (room == null)
                return NotFound();

            return Ok(room);
        }

        // PUT: api/rooms/{id}
        /// <summary>
        /// Updates a specific room by ID.
        /// </summary>
        [HttpPut]
        [Route("api/rooms/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putrooms(int id, rooms roomData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != roomData.id)
                return BadRequest("ID mismatch.");

            var existing = await db.rooms.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.name = roomData.name;
            existing.capacity = roomData.capacity;
            existing.description = roomData.description;

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

        // POST: api/rooms
        /// <summary>
        /// Creates a new room.
        /// </summary>
        [HttpPost]
        [Route("api/rooms")]
        [ResponseType(typeof(rooms))]
        public async Task<IHttpActionResult> Postrooms(rooms room)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.rooms.Add(room);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = room.id }, room);
        }

        // DELETE: api/rooms/{id}
        /// <summary>
        /// Deletes a specific room by ID.
        /// </summary>
        [HttpDelete]
        [Route("api/rooms/{id:int}")]
        [ResponseType(typeof(rooms))]
        public async Task<IHttpActionResult> Deleterooms(int id)
        {
            var room = await db.rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            db.rooms.Remove(room);
            await db.SaveChangesAsync();

            return Ok(room);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}