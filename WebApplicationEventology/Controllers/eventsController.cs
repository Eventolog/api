using System.Net;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;
using WebApplicationEventology.Models;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using WebApplicationEventology.Attributes;
using System;

namespace WebApplicationEventology.Controllers
{
    public class eventsController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/events
        /// <summary>
        /// Gets all events with only local table fields (no foreign keys).
        /// </summary>
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

        [HttpGet]
        [Route("api/{eventId}/getFreeSeats")]
        [ProtectedUser]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> GetFreeSeatsById(int eventId)
        {
            db.Configuration.LazyLoadingEnabled = false;

            // Obtenir totes les butaques de la sala de l’esdeveniment
            var eventEntity = await db.events.FindAsync(eventId);
            if (eventEntity == null)
                return NotFound();

            var allSeats = await db.seats
                .Where(s => s.room_id == eventEntity.room_id)
                .ToListAsync();

            // Obtenir IDs de les butaques ja reservades per aquest event
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

        [HttpPost]
        [Route("api/{eventId}/bookSeat")]
        [ProtectedUser]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> PostSeat(int eventId, [FromBody] int seatId)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var user = WebApplicationEventology.Utils.UserUtils.GetCurrentUser();

            // Validacions bàsiques
            var eventEntity = await db.events.FindAsync(eventId);
            if (eventEntity == null)
                return NotFound();

            var seat = await db.seats.FindAsync(seatId);
            if (seat == null || seat.room_id != eventEntity.room_id)
                return BadRequest("Invalid seat for this event.");

            // Comprovar si la butaca ja està agafada
            var isTaken = await db.tickets.AnyAsync(t => t.event_id == eventId && t.seat_id == seatId);
            if (isTaken)
                return BadRequest("Seat already taken.");

            // Crear la reserva
            var ticket = new tickets
            {
                name = "Reserva amb seient",
                reservation = DateTime.Now,
                status = "reserved",
                buyer_id = user.id,
                event_id = eventId,
                seat_id = seatId
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