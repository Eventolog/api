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
    public class inventory_itemsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/inventory_items
        /// <summary>
        /// Gets all inventory items (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/inventory_items")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getinventory_items()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var items = await db.inventory_items
                .Select(i => new
                {
                    i.id,
                    i.name,
                    i.description
                })
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/inventory_items/{id}
        /// <summary>
        /// Gets a specific inventory item by ID (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/inventory_items/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getinventory_item(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var item = await db.inventory_items
                .Where(i => i.id == id)
                .Select(i => new
                {
                    i.id,
                    i.name,
                    i.description
                })
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // PUT: api/inventory_items/{id}
        /// <summary>
        /// Updates a specific inventory item by ID.
        /// </summary>
        [HttpPut]
        [Route("api/inventory_items/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putinventory_item(int id, inventory_items inventoryItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != inventoryItem.id)
                return BadRequest("ID mismatch.");

            var existing = await db.inventory_items.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.name = inventoryItem.name;
            existing.description = inventoryItem.description;

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

        // POST: api/inventory_items
        /// <summary>
        /// Creates a new inventory item.
        /// </summary>
        [HttpPost]
        [Route("api/inventory_items")]
        [ResponseType(typeof(inventory_items))]
        public async Task<IHttpActionResult> Postinventory_item(inventory_items newItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.inventory_items.Add(newItem);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = newItem.id }, newItem);
        }

        // DELETE: api/inventory_items/{id}
        /// <summary>
        /// Deletes a specific inventory item by ID.
        /// </summary>
        [HttpDelete]
        [Route("api/inventory_items/{id:int}")]
        [ResponseType(typeof(inventory_items))]
        public async Task<IHttpActionResult> Deleteinventory_item(int id)
        {
            var inventoryItem = await db.inventory_items.FindAsync(id);
            if (inventoryItem == null)
                return NotFound();

            db.inventory_items.Remove(inventoryItem);
            await db.SaveChangesAsync();

            return Ok(inventoryItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}