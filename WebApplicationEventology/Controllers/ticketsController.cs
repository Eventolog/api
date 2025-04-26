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
    public class ticketsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/tickets
        public IQueryable<tickets> Gettickets()
        {
            return db.tickets;
        }

        // GET: api/tickets/5
        [ResponseType(typeof(tickets))]
        public async Task<IHttpActionResult> Gettickets(int id)
        {
            tickets tickets = await db.tickets.FindAsync(id);
            if (tickets == null)
            {
                return NotFound();
            }

            return Ok(tickets);
        }

        // PUT: api/tickets/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Puttickets(int id, tickets tickets)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tickets.id)
            {
                return BadRequest();
            }

            db.Entry(tickets).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ticketsExists(id))
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

        // POST: api/tickets
        [ResponseType(typeof(tickets))]
        public async Task<IHttpActionResult> Posttickets(tickets tickets)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tickets.Add(tickets);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = tickets.id }, tickets);
        }

        // DELETE: api/tickets/5
        [ResponseType(typeof(tickets))]
        public async Task<IHttpActionResult> Deletetickets(int id)
        {
            tickets tickets = await db.tickets.FindAsync(id);
            if (tickets == null)
            {
                return NotFound();
            }

            db.tickets.Remove(tickets);
            await db.SaveChangesAsync();

            return Ok(tickets);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ticketsExists(int id)
        {
            return db.tickets.Count(e => e.id == id) > 0;
        }
    }
}