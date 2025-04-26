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
    public class chatroomsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/chatrooms
        public IQueryable<chatrooms> Getchatrooms()
        {
            return db.chatrooms;
        }

        // GET: api/chatrooms/5
        [ResponseType(typeof(chatrooms))]
        public async Task<IHttpActionResult> Getchatrooms(int id)
        {
            chatrooms chatrooms = await db.chatrooms.FindAsync(id);
            if (chatrooms == null)
            {
                return NotFound();
            }

            return Ok(chatrooms);
        }

        // PUT: api/chatrooms/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putchatrooms(int id, chatrooms chatrooms)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chatrooms.id)
            {
                return BadRequest();
            }

            db.Entry(chatrooms).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!chatroomsExists(id))
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

        // POST: api/chatrooms
        [ResponseType(typeof(chatrooms))]
        public async Task<IHttpActionResult> Postchatrooms(chatrooms chatrooms)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.chatrooms.Add(chatrooms);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = chatrooms.id }, chatrooms);
        }

        // DELETE: api/chatrooms/5
        [ResponseType(typeof(chatrooms))]
        public async Task<IHttpActionResult> Deletechatrooms(int id)
        {
            chatrooms chatrooms = await db.chatrooms.FindAsync(id);
            if (chatrooms == null)
            {
                return NotFound();
            }

            db.chatrooms.Remove(chatrooms);
            await db.SaveChangesAsync();

            return Ok(chatrooms);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool chatroomsExists(int id)
        {
            return db.chatrooms.Count(e => e.id == id) > 0;
        }
    }
}