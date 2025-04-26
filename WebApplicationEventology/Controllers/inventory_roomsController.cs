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
    public class inventory_roomsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/inventory_rooms
        /// <summary>
        /// Gets all inventory-room associations (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/inventory_rooms")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getinventory_rooms()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var inventoryRooms = await db.inventory_rooms
                .Select(ir => new
                {
                    ir.room_id,
                    ir.inventory_id,
                    ir.quantity
                })
                .ToListAsync();

            return Ok(inventoryRooms);
        }

        // GET: api/inventory_rooms/{roomId}/{inventoryId}
        /// <summary>
        /// Gets a specific inventory-room association by composite keys.
        /// </summary>
        [HttpGet]
        [Route("api/inventory_rooms/{roomId:int}/{inventoryId:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getinventory_room(int roomId, int inventoryId)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var inventoryRoom = await db.inventory_rooms
                .Where(ir => ir.room_id == roomId && ir.inventory_id == inventoryId)
                .Select(ir => new
                {
                    ir.room_id,
                    ir.inventory_id,
                    ir.quantity
                })
                .FirstOrDefaultAsync();

            if (inventoryRoom == null)
                return NotFound();

            return Ok(inventoryRoom);
        }

        // PUT: api/inventory_rooms/{roomId}/{inventoryId}
        /// <summary>
        /// Updates a specific inventory-room association by composite keys.
        /// </summary>
        [HttpPut]
        [Route("api/inventory_rooms/{roomId:int}/{inventoryId:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putinventory_room(int roomId, int inventoryId, inventory_rooms inventoryRoomData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (roomId != inventoryRoomData.room_id || inventoryId != inventoryRoomData.inventory_id)
                return BadRequest("Composite ID mismatch.");

            var existing = await db.inventory_rooms.FindAsync(roomId, inventoryId);
            if (existing == null)
                return NotFound();

            existing.quantity = inventoryRoomData.quantity;

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

        // POST: api/inventory_rooms
        /// <summary>
        /// Creates a new inventory-room association.
        /// </summary>
        [HttpPost]
        [Route("api/inventory_rooms")]
        [ResponseType(typeof(inventory_rooms))]
        public async Task<IHttpActionResult> Postinventory_room(inventory_rooms inventoryRoom)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await db.inventory_rooms.FindAsync(inventoryRoom.room_id, inventoryRoom.inventory_id);
            if (exists != null)
                return Conflict();

            db.inventory_rooms.Add(inventoryRoom);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { room_id = inventoryRoom.room_id, inventory_id = inventoryRoom.inventory_id }, inventoryRoom);
        }

        // DELETE: api/inventory_rooms/{roomId}/{inventoryId}
        /// <summary>
        /// Deletes a specific inventory-room association by composite keys.
        /// </summary>
        [HttpDelete]
        [Route("api/inventory_rooms/{roomId:int}/{inventoryId:int}")]
        [ResponseType(typeof(inventory_rooms))]
        public async Task<IHttpActionResult> Deleteinventory_room(int roomId, int inventoryId)
        {
            var inventoryRoom = await db.inventory_rooms.FindAsync(roomId, inventoryId);
            if (inventoryRoom == null)
                return NotFound();

            db.inventory_rooms.Remove(inventoryRoom);
            await db.SaveChangesAsync();

            return Ok(inventoryRoom);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}