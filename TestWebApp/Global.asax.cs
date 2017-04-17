using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Vse.Web.Serialization;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using TestWebApp.Controllers.Models;

namespace TestWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //var types = typeof(MvcApplication).Assembly.GetTypes()
              //         .Where(t => t.IsClass && t.Namespace == "TestWebApp.Controllers.Models");

            //var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            
            //serializer.RegisterConverters(new JavaScriptConverter[] { new DateTimeJavaScriptConverter() });


            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
            //    new ControlledSerializationJsonConverter(
            //        supportedTypes: types,
            //        recursionDepth: 10,
            //        converters: new Dictionary<Type, Func<object, string>>() {
            //             {typeof(CultureInfo), (o) => ((CultureInfo)o).ToString()}
            //    }));
        }
    }

    

    
}
