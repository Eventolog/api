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
    public class mediaController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/media
        /// <summary>
        /// Gets all media files (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/media")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getmedia()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var medias = await db.media
                .Select(m => new
                {
                    m.id,
                    m.path
                })
                .ToListAsync();

            return Ok(medias);
        }

        // GET: api/media/{id}
        /// <summary>
        /// Gets a specific media file by ID.
        /// </summary>
        [HttpGet]
        [Route("api/media/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getmedia(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var media = await db.media
                .Where(m => m.id == id)
                .Select(m => new
                {
                    m.id,
                    m.path
                })
                .FirstOrDefaultAsync();

            if (media == null)
                return NotFound();

            return Ok(media);
        }

        // PUT: api/media/{id}
        /// <summary>
        /// Updates a specific media file by ID.
        /// </summary>
        [HttpPut]
        [Route("api/media/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmedia(int id, media mediaData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != mediaData.id)
                return BadRequest("ID mismatch.");

            var existing = await db.media.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.path = mediaData.path;

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

        // POST: api/media
        /// <summary>
        /// Creates a new media file.
        /// </summary>
        [HttpPost]
        [Route("api/media")]
        [ResponseType(typeof(media))]
        public async Task<IHttpActionResult> Postmedia(media media)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.media.Add(media);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = media.id }, media);
        }

        // DELETE: api/media/{id}
        /// <summary>
        /// Deletes a specific media file by ID.
        /// </summary>
        [HttpDelete]
        [Route("api/media/{id:int}")]
        [ResponseType(typeof(media))]
        public async Task<IHttpActionResult> Deletemedia(int id)
        {
            var media = await db.media.FindAsync(id);
            if (media == null)
                return NotFound();

            db.media.Remove(media);
            await db.SaveChangesAsync();

            return Ok(media);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}