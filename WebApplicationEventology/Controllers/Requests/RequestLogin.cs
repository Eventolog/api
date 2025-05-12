using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationEventology.Controllers.Requests
{
    public class RequestLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}