namespace WebApplicationEventology.Controllers.Requests
{

    /// <summary>
    /// Represents the data expected in the body of a user login request.
    /// </summary>
    /// <remarks>
    /// This class is used to deserialize the JSON payload sent to the user login endpoint,
    /// containing the credentials required for authentication.
    /// </remarks>
    public class RequestLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}