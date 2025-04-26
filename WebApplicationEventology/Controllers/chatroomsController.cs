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
    public class chatroomsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/chatrooms
        /// <summary>
        /// Gets all chatrooms (only local fields, no foreign keys expanded).
        /// </summary>
        [HttpGet]
        [Route("api/chatrooms")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getchatrooms()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var chatrooms = await db.chatrooms
                .Select(c => new
                {
                    c.id
                })
                .ToListAsync();

            return Ok(chatrooms);
        }

        // GET: api/chatrooms/{id}
        /// <summary>
        /// Gets a specific chatroom by ID (only local fields, no foreign keys expanded).
        /// </summary>
        [HttpGet]
        [Route("api/chatrooms/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getchatroom(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var chatroom = await db.chatrooms
                .Where(c => c.id == id)
                .Select(c => new
                {
                    c.id
                })
                .FirstOrDefaultAsync();

            if (chatroom == null)
                return NotFound();

            return Ok(chatroom);
        }

        // PUT: api/chatrooms/{id}
        /// <summary>
        /// Updates a specific chatroom by ID.
        /// </summary>
        [HttpPut]
        [Route("api/chatrooms/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putchatroom(int id, chatrooms chatroomUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != chatroomUpdate.id)
                return BadRequest("ID mismatch.");

            var existing = await db.chatrooms.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.user1_id = chatroomUpdate.user1_id;
            existing.user2_id = chatroomUpdate.user2_id;

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

        // POST: api/chatrooms
        /// <summary>
        /// Creates a new chatroom.
        /// </summary>
        [HttpPost]
        [Route("api/chatrooms")]
        [ResponseType(typeof(chatrooms))]
        public async Task<IHttpActionResult> Postchatroom(chatrooms newChatroom)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.chatrooms.Add(newChatroom);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = newChatroom.id }, newChatroom);
        }

        // DELETE: api/chatrooms/{id}
        /// <summary>
        /// Deletes a specific chatroom by ID.
        /// </summary>
        [HttpDelete]
        [Route("api/chatrooms/{id:int}")]
        [ResponseType(typeof(chatrooms))]
        public async Task<IHttpActionResult> Deletechatroom(int id)
        {
            var chatroom = await db.chatrooms.FindAsync(id);
            if (chatroom == null)
                return NotFound();

            db.chatrooms.Remove(chatroom);
            await db.SaveChangesAsync();

            return Ok(chatroom);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}