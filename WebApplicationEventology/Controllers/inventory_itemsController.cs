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
    public class inventory_itemsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/inventory_items
        public IQueryable<inventory_items> Getinventory_items()
        {
            return db.inventory_items;
        }

        // GET: api/inventory_items/5
        [ResponseType(typeof(inventory_items))]
        public async Task<IHttpActionResult> Getinventory_items(int id)
        {
            inventory_items inventory_items = await db.inventory_items.FindAsync(id);
            if (inventory_items == null)
            {
                return NotFound();
            }

            return Ok(inventory_items);
        }

        // PUT: api/inventory_items/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putinventory_items(int id, inventory_items inventory_items)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != inventory_items.id)
            {
                return BadRequest();
            }

            db.Entry(inventory_items).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!inventory_itemsExists(id))
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

        // POST: api/inventory_items
        [ResponseType(typeof(inventory_items))]
        public async Task<IHttpActionResult> Postinventory_items(inventory_items inventory_items)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.inventory_items.Add(inventory_items);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = inventory_items.id }, inventory_items);
        }

        // DELETE: api/inventory_items/5
        [ResponseType(typeof(inventory_items))]
        public async Task<IHttpActionResult> Deleteinventory_items(int id)
        {
            inventory_items inventory_items = await db.inventory_items.FindAsync(id);
            if (inventory_items == null)
            {
                return NotFound();
            }

            db.inventory_items.Remove(inventory_items);
            await db.SaveChangesAsync();

            return Ok(inventory_items);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool inventory_itemsExists(int id)
        {
            return db.inventory_items.Count(e => e.id == id) > 0;
        }
    }
}