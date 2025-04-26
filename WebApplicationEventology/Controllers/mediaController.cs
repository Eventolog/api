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
    public class mediaController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/media
        public IQueryable<media> Getmedia()
        {
            return db.media;
        }

        // GET: api/media/5
        [ResponseType(typeof(media))]
        public async Task<IHttpActionResult> Getmedia(int id)
        {
            media media = await db.media.FindAsync(id);
            if (media == null)
            {
                return NotFound();
            }

            return Ok(media);
        }

        // PUT: api/media/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmedia(int id, media media)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != media.id)
            {
                return BadRequest();
            }

            db.Entry(media).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!mediaExists(id))
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

        // POST: api/media
        [ResponseType(typeof(media))]
        public async Task<IHttpActionResult> Postmedia(media media)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.media.Add(media);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = media.id }, media);
        }

        // DELETE: api/media/5
        [ResponseType(typeof(media))]
        public async Task<IHttpActionResult> Deletemedia(int id)
        {
            media media = await db.media.FindAsync(id);
            if (media == null)
            {
                return NotFound();
            }

            db.media.Remove(media);
            await db.SaveChangesAsync();

            return Ok(media);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool mediaExists(int id)
        {
            return db.media.Count(e => e.id == id) > 0;
        }
    }
}