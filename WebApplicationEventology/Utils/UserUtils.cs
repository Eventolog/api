using WebApplicationEventology.Models;
using System.Web;

namespace WebApplicationEventology.Utils
{
    /// <summary>
    /// Provides an utility method for retrieving the current user from the HTTP context.
    /// </summary>
    /// <remarks>
    /// This static class encapsulates common functionalities for interacting with user data
    /// within the context of web requests.
    /// </remarks>
    public static class UserUtils
    {

        /// <summary>
        /// Retrieves the currently authenticated user from the current HTTP context.
        /// </summary>
        /// <remarks>
        /// This method expects that an authenticated user object has been
        /// added to the <see cref="HttpContext.Current"/>.Items collection
        /// under the key "User".
        /// <para>
        /// The <see cref="WebApplicationEventology.Attributes.ProtectedUserAttribute"/> is responsible for authenticating
        /// the request and populating the HTTP context with the user object
        /// before the controller action is executed.
        /// </para>
        /// </remarks>
        /// <returns>
        /// The <see cref="users"/> object representing the currently authenticated user,
        /// or <see langword="null"/> if no user object is found in the context
        /// (e.g., the request was not authenticated by <see cref="WebApplicationEventology.Attributes.ProtectedUserAttribute"/>).
        /// </returns>
        public static users GetCurrentUser()
        {
            return HttpContext.Current?.Items["User"] as users;
        }
    }
}