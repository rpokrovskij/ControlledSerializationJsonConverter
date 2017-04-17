using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vse.Web.Serialization;
using TestWebApp.Controllers.Models;

namespace TestWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 0)]
        public ActionResult GetItem1(int param1)
        {
            var item =  SampleModel.CreateSampleWithCultureInfo();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0)]
        public ActionResult GetItem2(int param1)
        {
            var item = SampleModel.CreateSampleWithCultureInfo();

            var converter = new ControlledSerializationJsonConverter(
                supportedTypes: new[] {item.GetType()},
                recursionDepth: 10,
                converters: new Dictionary<Type, Func<object, string>>() {
                     {typeof(CultureInfo), (o) => ((CultureInfo)o).ToString()}
                });

            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new[] { converter });
            var json = jss.Serialize(item);
            return this.Content(json, "application/json");
        }
    }
}