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
    public class incidencesController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/incidences
        /// <summary>
        /// Gets all incidences (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/incidences")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getincidences()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var incidences = await db.incidences
                .Select(i => new
                {
                    i.id,
                    i.reason,
                    i.status
                })
                .ToListAsync();

            return Ok(incidences);
        }

        // GET: api/incidences/{id}
        /// <summary>
        /// Gets a specific incidence by ID (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/incidences/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getincidence(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var incidence = await db.incidences
                .Where(i => i.id == id)
                .Select(i => new
                {
                    i.id,
                    i.reason,
                    i.status
                })
                .FirstOrDefaultAsync();

            if (incidence == null)
                return NotFound();

            return Ok(incidence);
        }

        // PUT: api/incidences/{id}
        /// <summary>
        /// Updates a specific incidence by ID.
        /// </summary>
        [HttpPut]
        [Route("api/incidences/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putincidence(int id, incidences incidenceUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != incidenceUpdate.id)
                return BadRequest("ID mismatch.");

            var existing = await db.incidences.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.reason = incidenceUpdate.reason;
            existing.status = incidenceUpdate.status;
            existing.normal_user_id = incidenceUpdate.normal_user_id;
            existing.solver_user_id = incidenceUpdate.solver_user_id;

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

        // POST: api/incidences
        /// <summary>
        /// Creates a new incidence.
        /// </summary>
        [HttpPost]
        [Route("api/incidences")]
        [ResponseType(typeof(incidences))]
        public async Task<IHttpActionResult> Postincidence(incidences newIncidence)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.incidences.Add(newIncidence);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = newIncidence.id }, newIncidence);
        }

        // DELETE: api/incidences/{id}
        /// <summary>
        /// Deletes a specific incidence by ID.
        /// </summary>
        [HttpDelete]
        [Route("api/incidences/{id:int}")]
        [ResponseType(typeof(incidences))]
        public async Task<IHttpActionResult> Deleteincidence(int id)
        {
            var incidence = await db.incidences.FindAsync(id);
            if (incidence == null)
                return NotFound();

            db.incidences.Remove(incidence);
            await db.SaveChangesAsync();

            return Ok(incidence);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}