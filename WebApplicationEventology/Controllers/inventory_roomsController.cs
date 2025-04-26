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
    public class inventory_roomsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/inventory_rooms
        public IQueryable<inventory_rooms> Getinventory_rooms()
        {
            return db.inventory_rooms;
        }

        // GET: api/inventory_rooms/5
        [ResponseType(typeof(inventory_rooms))]
        public async Task<IHttpActionResult> Getinventory_rooms(int id)
        {
            inventory_rooms inventory_rooms = await db.inventory_rooms.FindAsync(id);
            if (inventory_rooms == null)
            {
                return NotFound();
            }

            return Ok(inventory_rooms);
        }

        // PUT: api/inventory_rooms/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putinventory_rooms(int id, inventory_rooms inventory_rooms)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != inventory_rooms.room_id)
            {
                return BadRequest();
            }

            db.Entry(inventory_rooms).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!inventory_roomsExists(id))
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

        // POST: api/inventory_rooms
        [ResponseType(typeof(inventory_rooms))]
        public async Task<IHttpActionResult> Postinventory_rooms(inventory_rooms inventory_rooms)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.inventory_rooms.Add(inventory_rooms);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (inventory_roomsExists(inventory_rooms.room_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = inventory_rooms.room_id }, inventory_rooms);
        }

        // DELETE: api/inventory_rooms/5
        [ResponseType(typeof(inventory_rooms))]
        public async Task<IHttpActionResult> Deleteinventory_rooms(int id)
        {
            inventory_rooms inventory_rooms = await db.inventory_rooms.FindAsync(id);
            if (inventory_rooms == null)
            {
                return NotFound();
            }

            db.inventory_rooms.Remove(inventory_rooms);
            await db.SaveChangesAsync();

            return Ok(inventory_rooms);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool inventory_roomsExists(int id)
        {
            return db.inventory_rooms.Count(e => e.room_id == id) > 0;
        }
    }
}