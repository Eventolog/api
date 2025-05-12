using WebApplicationEventology.Utils;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.IdentityModel.Tokens;
using WebApplicationEventology.Models;

namespace WebApplicationEventology.Attributes
{

    /// <summary>
    /// An authorization filter attribute used to protect Web API controller actions and controllers.
    /// </summary>
    /// <remarks>
    /// This attribute verifies the presence and validity of a JWT in the 'Authorization' header
    /// (using the 'Bearer' scheme). It authenticates the user based on the token's claims,
    /// retrieves the user from the database, and optionally enforces role-based authorization.
    /// If authorization is successful, the authenticated user object is stored in
    /// <see cref="HttpContext.Current"/>.Items["User"] for easy access in controllers
    /// via <see cref="UserUtils.GetCurrentUser"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ProtectedUserAttribute : AuthorizationFilterAttribute
    {

        // <summary>
        // Stores the role string required for the decorated action or controller, if specified.
        // </summary>
        private readonly string requiredRole = null;
        // <summary>
        // Database context for retrieving user information during authorization.
        // </summary>
        // <remarks>
        // Note: Using a class-level DbContext in an attribute can sometimes lead to
        // lifetime management issues in complex scenarios. Consider dependency injection
        // or creating the context within OnAuthorization if appropriate for your architecture.
        // Ensure the DbContext is properly disposed of (e.g., via the controller's Dispose method).
        // </remarks>
        private eventologyEntities db = new eventologyEntities();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedUserAttribute"/>.
        /// </summary>
        /// <param name="role">
        /// An optional role string. If provided, the authenticated user must
        /// have this role to access the resource. Defaults to null, meaning
        /// only authentication is required.
        /// </param>
        public ProtectedUserAttribute(string role = null)
        {
            requiredRole = role;
        }

        /// <summary>
        /// Called when a request is being authorized.
        /// </summary>
        /// <remarks>
        /// This method implements the core authentication and authorization logic:
        /// <list type="bullet">
        /// <item>Checks for the 'Bearer' token in the Authorization header.</item>
        /// <item>Validates the JWT using predefined parameters.</item>
        /// <item>Extracts the user ID from the token claims.</item>
        /// <item>Retrieves the user from the database based on the user ID.</item>
        /// <item>Checks if a required role is specified and matches the user's role.</item>
        /// <item>If authorization succeeds, stores the user object in <see cref="HttpContext.Current"/>.Items.</item>
        /// <item>If authorization fails at any step, sets the response to Unauthorized (401).</item>
        /// </list>
        /// </remarks>
        /// <param name="actionContext">The context for the action.</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var authHeader = request.Headers.Authorization;

            // 1. Check for Authorization header and Bearer scheme
            if (authHeader == null || authHeader.Scheme != "Bearer")
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "Unauthorized: Invalid or missing token",
                    Content = new StringContent("Unauthorized: Invalid or missing token")
                };
                return;
            }

            var token = authHeader.Parameter;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(JwtUtils.PRIVATE_KEY); // Use your static JWT secret key

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "eventology",
                    ValidAudience = "eventology",
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                // 2. Validate the token
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // 3. Extract user ID claim
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "user_id");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        ReasonPhrase = "Unauthorized: Invalid or missing token",
                        Content = new StringContent("Unauthorized: Invalid or missing token")
                    };
                    return;
                }

                // 4. Retrieve user from database
                var user = db.users.Find(userId);
                if (user == null)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        ReasonPhrase = "Unauthorized: Invalid or missing token",
                        Content = new StringContent("Unauthorized: Invalid or missing token")
                    };
                    return;
                }

                // 5. Perform role-based authorization if a role is required
                if (requiredRole != null)
                {
                    if(user.type != requiredRole)
                    {
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                        {
                            ReasonPhrase = "Unauthorized role",
                            Content = new StringContent("Unauthorized role")
                        };
                        return;
                    }
                }

                // 6. If authorization is successful, store the user in HttpContext for later access
                HttpContext.Current.Items["User"] = user;
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }
    }
}
