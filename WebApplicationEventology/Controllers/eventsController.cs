using System;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Http.Description;
using WebApplicationEventology.Models;
using System.Data.Entity.Infrastructure;
using WebApplicationEventology.Attributes;
using WebApplicationEventology.Controllers.Requests;
using WebApplicationEventology.Utils;

namespace WebApplicationEventology.Controllers
{
    /// <summary>
    /// Controller that manages event-related operations including listing events, retrieving free seats, and booking them.
    /// </summary>
    public class eventsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        /// <summary>
        /// Retrieves all events with relevant room details (excluding navigation properties like users or tickets).
        /// </summary>
        /// <returns>Returns a list of event objects with basic and room information.</returns>
        [HttpGet]
        [Route("api/events")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getevents()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var events = await db.events
                .Select(e => new
                {
                    e.id,
                    e.name,
                    e.description,
                    e.if_full_day,
                    e.start_time,
                    e.end_time,
                    e.status,
                    e.created_at,
                    roomName = e.rooms.name,
                    roomDescription = e.rooms.description,
                    roomDistribution = e.rooms.roomLayout,
                    roomHasSeatDistribution = e.rooms.hasSeatingDistribution
                })
                .ToListAsync();

            return Ok(events);
        }

        /// <summary>
        /// Gets all available (not reserved) seats for a given event.
        /// </summary>
        /// <param name="eventId">The ID of the event.</param>
        /// <returns>Returns a list of available seat objects.</returns>
        [HttpGet]
        [Route("api/events/getFreeSeats")]
        [ProtectedUser]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> GetFreeSeats([FromUri] int eventId)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var eventEntity = await db.events.FindAsync(eventId);
            if (eventEntity == null)
                return NotFound();

            var allSeats = await db.seats
                .Where(s => s.room_id == eventEntity.room_id)
                .ToListAsync();

            var takenSeatIds = await db.tickets
                .Where(t => t.event_id == eventId && t.seat_id != null)
                .Select(t => t.seat_id.Value)
                .ToListAsync();

            var freeSeats = allSeats
                .Where(s => !takenSeatIds.Contains(s.id))
                .Select(s => new
                {
                    s.id,
                    s.row_number,
                    s.seat_number
                })
                .ToList();

            return Ok(freeSeats);
        }

        /// <summary>
        /// Books a specific seat for the authenticated user.
        /// </summary>
        /// <param name="booking">Object with eventId and seatId.</param>
        /// <returns>Returns ticket ID if successful.</returns>
        [HttpPost]
        [Route("api/events/bookSeat")]
        [ProtectedUser]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> BookSeat([FromBody] RequestBookSeat booking)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var user = UserUtils.GetCurrentUser();

            var eventEntity = await db.events.FindAsync(booking.EventId);
            if (eventEntity == null)
                return NotFound();

            var seat = await db.seats.FindAsync(booking.SeatId);
            if (seat == null || seat.room_id != eventEntity.room_id)
                return BadRequest("Invalid seat for this event.");

            var isTaken = await db.tickets.AnyAsync(t => t.event_id == booking.EventId && t.seat_id == booking.SeatId);
            if (isTaken)
                return BadRequest("Seat already taken.");

            var ticket = new tickets
            {
                name = "Reserva amb seient",
                reservation = DateTime.Now,
                status = "reserved",
                buyer_id = user.id,
                event_id = booking.EventId,
                seat_id = booking.SeatId
            };

            db.tickets.Add(ticket);

            try
            {
                await db.SaveChangesAsync();
                return Ok(new { success = true, ticketId = ticket.id });
            }
            catch (DbUpdateException e)
            {
                return BadRequest("Error reservant: " + (e.InnerException?.Message ?? e.Message));
            }
        }
    }
}