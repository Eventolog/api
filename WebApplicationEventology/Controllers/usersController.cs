    using System;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;
using WebApplicationEventology.Models;
using WebApplicationEventology.Controllers.Requests;
using WebApplicationEventology.Utils;
using WebApplicationEventology.Constants;
using WebApplicationEventology.Attributes;

namespace WebApplicationEventology.Controllers
{
    public class usersController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();


        // POST: api/user/signup
        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <remarks>
        /// This endpoint accepts user details and creates a new user account with a hashed password.
        /// Upon successful creation, a JWT is generated for the new user.
        /// </remarks>
        /// <param name="request">
        /// An object containing the details for the new user, including Name, Email, and Password.
        /// The password will be securely hashed before storage.
        /// </param>
        /// <returns>
        /// <para>
        /// If the user is created successfully, returns an OK (200) with the generated JWT string in the response body.
        /// </para>
        /// <para>
        /// If the provided <paramref name="request"/> fails model validation, returns a BadRequest (400)
        /// with details about the validation errors in the ModelState.
        /// </para>
        /// <para>
        /// If an unexpected error occurs during the process (e.g., database error, hashing failure),
        /// returns a BadRequest (400) with a string representation of the exception details.
        /// </para>
        /// </returns>
        [HttpPost]
        [Route("api/user/signup")]
        public async Task<IHttpActionResult> CreateUser([FromBody] RequestCreateUser request)
        {
            try {
                // Check if the incoming request data is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Encrypt the password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create the user instance
                users user = new users
                {
                    name = request.Name,
                    email = request.Email,
                    password = hashedPassword,
                    type = UserTypes.NORMAL
                };

                db.users.Add(user);
                await db.SaveChangesAsync();

                // Generate JWT for the created uer
                var jwt = JwtUtils.GenerateUserJwt(user);

                // Return the JWT in an OK response
                return Ok(jwt);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        // POST: api/user/login
        /// <summary>
        /// Authenticates a user and provides a JSON Web Token (JWT) upon successful login.
        /// </summary>
        /// <remarks>
        /// This endpoint accepts user credentials (email and password) and verifies them
        /// against stored user data. If the credentials are valid, a JWT is generated
        /// and returned for subsequent authenticated requests.
        /// </remarks>
        /// <param name="request">
        /// An object containing the user's login credentials: Email and Password.
        /// </param>
        /// <returns>
        /// <para>
        /// If authentication is successful, returns an OK (200) with the generated JWT string
        /// in the response body.
        /// </para>
        /// <para>
        /// If the provided <paramref name="request"/> fails model validation, returns a BadRequest (400)
        /// with details about the validation errors in the ModelState.
        /// </para>
        /// <para>
        /// If the email and password do not match a registered user, returns an Unauthorized (401)
        /// with a message indicating invalid credentials.
        /// </para>
        /// <para>
        /// If an unexpected error occurs during the process (e.g., database connection issues),
        /// returns a BadRequest (400) with a string representation of the exception details.
        /// </para>
        /// </returns>
        [HttpPost]
        [Route("api/user/login")]
        public async Task<IHttpActionResult> Login([FromBody] RequestLogin request)
        {
            try
            {
                // Check if the incoming request data is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Find the user by email
                var user = await db.users.FirstOrDefaultAsync(u => u.email == request.Email);

                // If user is not found or password doesn't match
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.password))
                {
                    // Return Unauthorized for invalid credentials
                    return Unauthorized();
                }

                // Generate JWT for the authenticated user
                var jwt = JwtUtils.GenerateUserJwt(user);

                // Return the JWT in an OK response
                return Ok(jwt);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        // GET: api/user/whoami
        /// <summary>
        /// Gets the details of the currently authenticated user.
        /// </summary>
        /// <remarks>
        /// This endpoint requires authentication via the [ProtectedUser] attribute.
        /// </remarks>
        /// <returns>
        /// An IHttpActionResult containing an OK response with an anonymous object
        /// representing the authenticated user's essential information (id, name, type).
        /// </returns>
        [HttpGet]
        [Route("api/user/whoami")]
        [ProtectedUser]
        public async Task<IHttpActionResult> WhoAmI()
        {
            users user = UserUtils.GetCurrentUser();

            return Ok(new
            {
                id = user.id,
                name = user.name,
                type = user.type,
            });
        }


        // GET: api/users/{id}
        /// <summary>
        /// Gets a specific user by ID (only main fields).
        /// </summary>
        /// <returns>
        /// An IHttpActionResult containing an OK response with an anonymous object
        /// representing the authenticated user's essential information (id, name, type).
        /// </returns>
        [HttpGet]
        [Route("api/users/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getuser(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var user = await db.users
                .Where(u => u.id == id)
                .Select(u => new
                {
                    u.id,
                    u.name,
                    u.email,
                    u.type
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

    }
}