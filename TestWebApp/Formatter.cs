using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using TestWebApp.Controllers.Models;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Globalization;
using Vse.Web.Serialization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using System.Net.Http;
using System.Net;

namespace TestWebApp
{
    public class JavaScriptSerializerFormatter : MediaTypeFormatter
    {
        public JavaScriptSerializerFormatter()
        {
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        }

        public override bool CanWriteType(Type type)
        {
            if (type == typeof(SampleModel))
                return true;
            return false;
        }

        public override bool CanReadType(Type type)
        {
            if (type == typeof(SampleModel))
                return true;

            return false;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            //System.Net.Http.Headers.HttpContentHeaders contentHeaders, FormatterContext formatterContext
            var task = Task.Factory.StartNew(() =>
            {

                var ser = new JavaScriptSerializer();
                var json = ser.Serialize(value);

                byte[] buf = System.Text.Encoding.Default.GetBytes(json);
                stream.Write(buf, 0, buf.Length);
                stream.Flush();
            });

            return task;
        }
    }
}