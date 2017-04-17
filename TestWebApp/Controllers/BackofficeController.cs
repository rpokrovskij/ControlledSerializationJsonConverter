using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace TestWebApp.Controllers
{
    public class BackofficeController : ApiController
    {
        [Route("api/Test1/{param1}"), HttpGet]
        public int Test1(int param1)
        {
            return 100;
        }
    }
}