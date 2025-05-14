using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplicationEventology.Attributes;
using WebApplicationEventology.Models;
using WebApplicationEventology.Utils;
using System.Collections.Generic;

namespace WebApplicationEventology.Controllers
{
    public class ticketsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        /// <summary>
        /// Gets all tickets of the current authenticated user.
        /// </summary>
        /// <returns>
        /// An IHttpActionResult containing a list of tickets purchased by the authenticated user.
        /// </returns>
        [HttpGet]
        [Route("api/getMyTickets")]
        [ProtectedUser]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> GetTickets()
        {
            db.Configuration.LazyLoadingEnabled = false;

            users user = UserUtils.GetCurrentUser();

            var tickets = await Task.Run(() =>
                db.tickets
                    .Where(t => t.buyer_id == user.id)
                    .Select(t => new
                    {
                        t.id,
                        t.name,
                        t.status,
                        t.reservation,
                        Event = new
                        {
                            t.events.id,
                            t.events.name,
                            t.events.start_time,
                            t.events.end_time,
                            t.events.status
                        },
                        Seat = t.seat_id != null ? new
                        {
                            t.seats.row_number,
                            t.seats.seat_number
                        } : null
                    })
                    .ToList()
            );

            return Ok(tickets);
        }
    }
}