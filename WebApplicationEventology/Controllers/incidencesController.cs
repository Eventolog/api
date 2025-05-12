using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;
using WebApplicationEventology.Models;
using System.Collections.Generic;
using WebApplicationEventology.Attributes;
using WebApplicationEventology.Utils;
using System;
using WebApplicationEventology.Controllers.Requests;

namespace WebApplicationEventology.Controllers
{
    public class incidencesController : ApiController
    {
        private eventologyEntities db = new eventologyEntities();

        // GET: api/incidences
        /// <summary>
        /// Gets all incidences of the current authenticated user.
        /// </summary>
        /// <returns>
        /// An IHttpActionResult containing an OK response with
        /// an IEnumerable(object) containing all the incidences given an authenticated user
        /// </returns>
        [HttpGet]
        [Route("api/incidences")]
        [ProtectedUser]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> GetUserincidences()
        {
            db.Configuration.LazyLoadingEnabled = false;

            users user = UserUtils.GetCurrentUser();

            var incidences = await db.incidences
                .Select(i => new
                {
                    i.id,
                    i.reason,
                    i.status,
                    i.normal_user_id
                })
                .Where(i => i.normal_user_id == user.id)
                .ToListAsync();

            return Ok(incidences);
        }

      
        // POST: api/incidences
        /// <summary>
        /// Creates a new incidence for the authenticated user.
        /// </summary>
        [HttpPost]
        [Route("api/incidences")]
        [ProtectedUser]
        public async Task<IHttpActionResult> Postincidence([FromBody] RequestCreateNewIncidence request)
        {
            try
            {
                var initialStatus = "open";

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                users user = UserUtils.GetCurrentUser();
                incidences newIncidence = new incidences
                {
                    normal_user_id = user.id,
                    solver_user_id = user.id,
                    reason = request.Reason,
                    status = initialStatus
                };

                db.incidences.Add(newIncidence);
                await db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

        }
    }
}