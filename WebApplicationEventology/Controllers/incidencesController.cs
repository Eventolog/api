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
    public class incidencesController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/incidences
        public IQueryable<incidences> Getincidences()
        {
            return db.incidences;
        }

        // GET: api/incidences/5
        [ResponseType(typeof(incidences))]
        public async Task<IHttpActionResult> Getincidences(int id)
        {
            incidences incidences = await db.incidences.FindAsync(id);
            if (incidences == null)
            {
                return NotFound();
            }

            return Ok(incidences);
        }

        // PUT: api/incidences/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putincidences(int id, incidences incidences)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != incidences.id)
            {
                return BadRequest();
            }

            db.Entry(incidences).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!incidencesExists(id))
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

        // POST: api/incidences
        [ResponseType(typeof(incidences))]
        public async Task<IHttpActionResult> Postincidences(incidences incidences)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.incidences.Add(incidences);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = incidences.id }, incidences);
        }

        // DELETE: api/incidences/5
        [ResponseType(typeof(incidences))]
        public async Task<IHttpActionResult> Deleteincidences(int id)
        {
            incidences incidences = await db.incidences.FindAsync(id);
            if (incidences == null)
            {
                return NotFound();
            }

            db.incidences.Remove(incidences);
            await db.SaveChangesAsync();

            return Ok(incidences);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool incidencesExists(int id)
        {
            return db.incidences.Count(e => e.id == id) > 0;
        }
    }
}