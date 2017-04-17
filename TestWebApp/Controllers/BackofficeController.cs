using System.Web.Http;
using TestWebApp.Controllers.Models;

namespace TestWebApp.Controllers
{
    public class BackofficeController : ApiController
    {
        [Route("api/Test1/{param1}"), HttpGet]
        public SampleModel Test1(int param1) 
        {
            var item = SampleModel.CreateSampleWithCultureInfo();
            return item;
        }
    }
}