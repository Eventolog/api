using WebApplicationEventology.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationEventology.Utils
{
    public static class UserUtils
    {
        public static users GetCurrentUser()
        {
            return HttpContext.Current?.Items["User"] as users;
        }
    }
}