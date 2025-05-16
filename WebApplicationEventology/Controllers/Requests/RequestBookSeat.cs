namespace WebApplicationEventology.Controllers.Requests
{
    /// <summary>
    /// Request object to book a seat in an event.
    /// </summary>
    public class RequestBookSeat
    {
        public int EventId { get; set; }
        public int SeatId { get; set; }
    }
}