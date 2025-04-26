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
    public class seatsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/seats
        public IQueryable<seats> Getseats()
        {
            return db.seats;
        }

        // GET: api/seats/5
        [ResponseType(typeof(seats))]
        public async Task<IHttpActionResult> Getseats(int id)
        {
            seats seats = await db.seats.FindAsync(id);
            if (seats == null)
            {
                return NotFound();
            }

            return Ok(seats);
        }

        // PUT: api/seats/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putseats(int id, seats seats)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != seats.id)
            {
                return BadRequest();
            }

            db.Entry(seats).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!seatsExists(id))
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

        // POST: api/seats
        [ResponseType(typeof(seats))]
        public async Task<IHttpActionResult> Postseats(seats seats)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.seats.Add(seats);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = seats.id }, seats);
        }

        // DELETE: api/seats/5
        [ResponseType(typeof(seats))]
        public async Task<IHttpActionResult> Deleteseats(int id)
        {
            seats seats = await db.seats.FindAsync(id);
            if (seats == null)
            {
                return NotFound();
            }

            db.seats.Remove(seats);
            await db.SaveChangesAsync();

            return Ok(seats);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool seatsExists(int id)
        {
            return db.seats.Count(e => e.id == id) > 0;
        }
    }
}