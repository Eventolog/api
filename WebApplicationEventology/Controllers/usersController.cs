using System;
using System.Net;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;
using WebApplicationEventology.Models;
using System.Data.Entity.Infrastructure;
using WebApplicationEventology.Controllers.Requests;
using WebApplicationEventology.Utils;
using WebApplicationEventology.Constants;

namespace WebApplicationEventology.Controllers
{
    public class usersController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // POST: api/users
        /// <summary>
        /// Creates a new user.
        /// </summary>
        [HttpPost]
        [Route("api/user/add")]
        public async Task<IHttpActionResult> CreateUser([FromBody] RequestCreateUser request)
        {
            try { 
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);


                users user = new users
                {
                    name = request.Name,
                    email = request.Email,
                    password = hashedPassword,
                    type = UserTypes.NORMAL
                };

                db.users.Add(user);
                await db.SaveChangesAsync();

                var jwt = JwtUtils.GenerateUserJwt(user);

                return Ok(jwt);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        //// GET: api/users
        ///// <summary>
        ///// Gets all users (only main fields).
        ///// </summary>
        //[HttpGet]
        //[Route("api/users")]
        //[ResponseType(typeof(IEnumerable<object>))]
        //public async Task<IHttpActionResult> Getusers()
        //{
        //    db.Configuration.LazyLoadingEnabled = false;

        //    var users = await db.users
        //        .Select(u => new
        //        {
        //            u.id,
        //            u.name,
        //            u.email,
        //            u.password,
        //            u.type
        //        })
        //        .ToListAsync();

        //    return Ok(users);
        //}

        // GET: api/users/{id}
        /// <summary>
        /// Gets a specific user by ID (only main fields).
        /// </summary>
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
                    u.password,
                    u.type
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // PUT: api/users/{id}
        /// <summary>
        /// Updates a specific user by ID.
        /// </summary>
        [HttpPut]
        [Route("api/users/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putusers(int id, users userData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != userData.id)
                return BadRequest("ID mismatch.");

            var existing = await db.users.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.name = userData.name;
            existing.email = userData.email;
            existing.password = userData.password;
            existing.type = userData.type;

            try
            {
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException)
            {
                return InternalServerError();
            }
        }

        // POST: api/users
        /// <summary>
        /// Creates a new user.
        /// </summary>
        [HttpPost]
        [Route("api/users")]
        [ResponseType(typeof(users))]
        public async Task<IHttpActionResult> Postusers(users user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.users.Add(user);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = user.id }, user);
        }

        // DELETE: api/users/{id}
        /// <summary>
        /// Deletes a specific user by ID.
        /// </summary>
        [HttpDelete]
        [Route("api/users/{id:int}")]
        [ResponseType(typeof(users))]
        public async Task<IHttpActionResult> Deleteusers(int id)
        {
            var user = await db.users.FindAsync(id);
            if (user == null)
                return NotFound();

            db.users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}