using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using System.Net.Http;
using System.Net;
using Vse.Web.Serialization;

namespace TestWebApp
{
    public class JavaScriptSerializerFormatter : MediaTypeFormatter
    {
        IEnumerable<Type> types;
        public JavaScriptSerializerFormatter(IEnumerable<Type> types)
        {
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            this.types = types;
        }

        public override bool CanWriteType(Type type)
        {
            return types.Contains(type);
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var converter = new ControlledSerializationJsonConverter(
                supportedTypes: types,
                recursionDepth: 10,
                converters: new Dictionary<Type, Func<object, string>>() {
                     {typeof(CultureInfo), (o) => ((CultureInfo)o).ToString()}
                });

                var jss = new JavaScriptSerializer(); // is no thread safe and should be recreated
                jss.RegisterConverters(new[] { converter });
                var json = jss.Serialize(value);

                byte[] buf = System.Text.Encoding.Default.GetBytes(json);
                stream.Write(buf, 0, buf.Length);
                stream.Flush();
            });

            return task;
        }
    }
}