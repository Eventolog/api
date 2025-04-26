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
    public class seatsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/seats
        /// <summary>
        /// Gets all seats (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/seats")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getseats()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var seats = await db.seats
                .Select(s => new
                {
                    s.id,
                    s.row_number,
                    s.seat_number
                })
                .ToListAsync();

            return Ok(seats);
        }

        // GET: api/seats/{id}
        /// <summary>
        /// Gets a specific seat by ID (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/seats/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getseats(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var seat = await db.seats
                .Where(s => s.id == id)
                .Select(s => new
                {
                    s.id,
                    s.row_number,
                    s.seat_number
                })
                .FirstOrDefaultAsync();

            if (seat == null)
                return NotFound();

            return Ok(seat);
        }

        // PUT: api/seats/{id}
        /// <summary>
        /// Updates a specific seat by ID.
        /// </summary>
        [HttpPut]
        [Route("api/seats/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putseats(int id, seats seatData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != seatData.id)
                return BadRequest("ID mismatch.");

            var existing = await db.seats.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.row_number = seatData.row_number;
            existing.seat_number = seatData.seat_number;

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

        // POST: api/seats
        /// <summary>
        /// Creates a new seat.
        /// </summary>
        [HttpPost]
        [Route("api/seats")]
        [ResponseType(typeof(seats))]
        public async Task<IHttpActionResult> Postseats(seats seat)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.seats.Add(seat);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = seat.id }, seat);
        }

        // DELETE: api/seats/{id}
        /// <summary>
        /// Deletes a specific seat by ID.
        /// </summary>
        [HttpDelete]
        [Route("api/seats/{id:int}")]
        [ResponseType(typeof(seats))]
        public async Task<IHttpActionResult> Deleteseats(int id)
        {
            var seat = await db.seats.FindAsync(id);
            if (seat == null)
                return NotFound();

            db.seats.Remove(seat);
            await db.SaveChangesAsync();

            return Ok(seat);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}