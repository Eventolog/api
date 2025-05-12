using System.ComponentModel.DataAnnotations;

namespace WebApplicationEventology.Controllers.Requests
{
    public class RequestCreateUser
    {
        [Required(ErrorMessage = "Name is required.")]
        [MinLength(1, ErrorMessage = "Name must be at least 1 character.")]
        [MaxLength(100, ErrorMessage = "Name must be maxium 100 character.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(255, ErrorMessage = "Email must be maxium 100 character.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(1, ErrorMessage = "Password must be at least 1 character.")]
        [MaxLength(255, ErrorMessage = "Name must be maxium 100 character.")]
        public string Password { get; set; }

    }
}