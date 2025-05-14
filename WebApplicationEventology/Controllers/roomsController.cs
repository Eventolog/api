using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;
using WebApplicationEventology.Models;
using System.Collections.Generic;

namespace WebApplicationEventology.Controllers
{
    public class roomsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/rooms
        /// <summary>
        /// Gets all rooms (only main fields).
        /// </summary>
        [HttpGet]
        [Route("api/rooms")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getrooms()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var rooms = await db.rooms
                .Select(r => new
                {
                    r.id,
                    r.name,
                    r.capacity,
                    r.description
                })
                .ToListAsync();

            return Ok(rooms);
        }

    }
}