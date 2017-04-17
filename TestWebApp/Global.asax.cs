using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Linq;

namespace TestWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register); // IMPORTANT: GlobalConfiguration should go after FilterConfig (opposite to default order)
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var types = typeof(MvcApplication).Assembly.GetTypes()
                     .Where(t => t.IsClass && t.Namespace == "TestWebApp.Controllers.Models");
            GlobalConfiguration.Configuration.Formatters.Insert(0, new JavaScriptSerializerFormatter(types));
        }
    }
}
