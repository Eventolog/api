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
    public class ticketsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/tickets
        /// <summary>
        /// Gets all tickets (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/tickets")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Gettickets()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var tickets = await db.tickets
                .Select(t => new
                {
                    t.id,
                    t.name,
                    t.reservation,
                    t.status
                })
                .ToListAsync();

            return Ok(tickets);
        }

        // GET: api/tickets/{id}
        /// <summary>
        /// Gets a specific ticket by ID (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/tickets/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getticket(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var ticket = await db.tickets
                .Where(t => t.id == id)
                .Select(t => new
                {
                    t.id,
                    t.name,
                    t.reservation,
                    t.status
                })
                .FirstOrDefaultAsync();

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        // PUT: api/tickets/{id}
        /// <summary>
        /// Updates a specific ticket by ID.
        /// </summary>
        [HttpPut]
        [Route("api/tickets/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Puttickets(int id, tickets ticketData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != ticketData.id)
                return BadRequest("ID mismatch.");

            var existing = await db.tickets.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.name = ticketData.name;
            existing.reservation = ticketData.reservation;
            existing.status = ticketData.status;

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

        // POST: api/tickets
        /// <summary>
        /// Creates a new ticket.
        /// </summary>
        [HttpPost]
        [Route("api/tickets")]
        [ResponseType(typeof(tickets))]
        public async Task<IHttpActionResult> Posttickets(tickets ticket)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.tickets.Add(ticket);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = ticket.id }, ticket);
        }

        // DELETE: api/tickets/{id}
        /// <summary>
        /// Deletes a specific ticket by ID.
        /// </summary>
        [HttpDelete]
        [Route("api/tickets/{id:int}")]
        [ResponseType(typeof(tickets))]
        public async Task<IHttpActionResult> Deletetickets(int id)
        {
            var ticket = await db.tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            db.tickets.Remove(ticket);
            await db.SaveChangesAsync();

            return Ok(ticket);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}