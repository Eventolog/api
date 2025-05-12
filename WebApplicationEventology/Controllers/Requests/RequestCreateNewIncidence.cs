using System.ComponentModel.DataAnnotations;

namespace WebApplicationEventology.Controllers.Requests
{

    /// <summary>
    /// Represents the data expected in the body of a create incidence request.
    /// </summary>
    /// <remarks>
    /// This class is used to deserialize the JSON payload sent to the create incidence endpoint,
    /// containing the incidence details.
    /// </remarks>
    public class RequestCreateNewIncidence
    {
        [Required(ErrorMessage = "Reason is required.")]
        [MinLength(1, ErrorMessage = "Reason must be at least 1 character.")]
        [MaxLength(500, ErrorMessage = "Reason must be maxium 500 character.")]
        public string Reason { get; set; }
    }
}