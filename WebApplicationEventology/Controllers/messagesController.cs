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
    public class messagesController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/messages
        /// <summary>
        /// Gets all messages (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/messages")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getmessages()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var messages = await db.messages
                .Select(m => new
                {
                    m.id,
                    m.content,
                    m.date,
                    m.status
                })
                .ToListAsync();

            return Ok(messages);
        }

        // GET: api/messages/{id}
        /// <summary>
        /// Gets a specific message by ID.
        /// </summary>
        [HttpGet]
        [Route("api/messages/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getmessages(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var message = await db.messages
                .Where(m => m.id == id)
                .Select(m => new
                {
                    m.id,
                    m.content,
                    m.date,
                    m.status
                })
                .FirstOrDefaultAsync();

            if (message == null)
                return NotFound();

            return Ok(message);
        }

        // PUT: api/messages/{id}
        /// <summary>
        /// Updates a specific message by ID.
        /// </summary>
        [HttpPut]
        [Route("api/messages/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmessages(int id, messages messageData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != messageData.id)
                return BadRequest("ID mismatch.");

            var existing = await db.messages.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.content = messageData.content;
            existing.date = messageData.date;
            existing.status = messageData.status;

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

        // POST: api/messages
        /// <summary>
        /// Creates a new message.
        /// </summary>
        [HttpPost]
        [Route("api/messages")]
        [ResponseType(typeof(messages))]
        public async Task<IHttpActionResult> Postmessages(messages message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.messages.Add(message);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = message.id }, message);
        }

        // DELETE: api/messages/{id}
        /// <summary>
        /// Deletes a specific message by ID.
        /// </summary>
        [HttpDelete]
        [Route("api/messages/{id:int}")]
        [ResponseType(typeof(messages))]
        public async Task<IHttpActionResult> Deletemessages(int id)
        {
            var message = await db.messages.FindAsync(id);
            if (message == null)
                return NotFound();

            db.messages.Remove(message);
            await db.SaveChangesAsync();

            return Ok(message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}