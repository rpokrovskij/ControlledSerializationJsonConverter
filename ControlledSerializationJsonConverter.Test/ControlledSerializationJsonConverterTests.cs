using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vse.Web.Serialization.Test
{
    [TestClass]
    public class ControlledSerializationJsonConverterTests
    {
        /// <summary>
        /// Note: it is still much more quicker then serialization using History (even when it produces huge json output and uses deeper recursion)
        /// </summary>
        [TestMethod]
        public void RecursiveJavaScriptSerializer()
        {
            var item = SampleModel.CreateSampleWithCircularReference();

            var jss1 = new JavaScriptSerializer();
            jss1.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new[] { typeof(SampleModel) }, 50, false, false, null, false) });

            var json1 = jss1.Serialize(item);
            if (json1.Length < 1000)
                throw new ApplicationException("History doesn't work. Case 0");

            var jss2 = new JavaScriptSerializer();
            jss2.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new[] { typeof(SampleModel) }, 50, true, false) });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b"",""Child"":{""Number"":3,""Name"":""c""}}}")
                throw new ApplicationException("History doesn't work. Case 1");
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerWithHistory()
        {
            var item = SampleModel.CreateSampleWithCircularReference();
            var jss2 = new JavaScriptSerializer();
            jss2.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new[] { typeof(SampleModel) }, 50, true, false) });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b"",""Child"":{""Number"":3,""Name"":""c""}}}")
                throw new ApplicationException("History doesn't work. Case 1");
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerWithHistoryAndNotSupported()
        {
            var item = SampleModel.CreateSampleWithCultureInfo();
            var jss2 = new JavaScriptSerializer();
            var converter = new ControlledSerializationJsonConverter2(
                    supportedTypes: new[] { typeof(SampleModel) },
                    ignoreNotSupported: true,
                    recursionDepth: 3,
                    ignoreDuplicates: true,
                    supremeTypes: ControlledSerializationJsonConverter.StandardSimpleTypes);
            jss2.RegisterConverters(new[] { converter });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b"",""Child"":{""Number"":3,""Name"":""c"",""Child"":{""Number"":1,""Name"":""a""}}}}")
                throw new ApplicationException("History doesn't work. Case 1");
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerConfigurationException()
        {
            var item = SampleModel.CreateSampleWithCircularReference();

            var jss1 = new JavaScriptSerializer();
            try
            {
                jss1.RegisterConverters(new[] { new ControlledSerializationJsonConverter(null, 50, false, false, null, false) });
                var json1 = jss1.Serialize(item);
            }
            catch (ArgumentException)
            {
            }

            var jss2 = new JavaScriptSerializer();

            try
            {
                jss2.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new List<Type>(), 50, false, false) });
                var json1 = jss2.Serialize(item);
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerWithHistoryAndCustomConverters()
        {
            var item = SampleModel.CreateSampleWithCultureInfo();
            var jss2 = new JavaScriptSerializer();
            var converter = new ControlledSerializationJsonConverter(
                    supportedTypes: new[] { typeof(SampleModel) },
                    formatters: new Dictionary<Type, Func<object, string>>()
                    {
                       { typeof(CultureInfo), (o) => ((CultureInfo)o).ToString()}
                    },
                    recursionDepth: 50,
                    ignoreDuplicates: true);
            jss2.RegisterConverters(new[] { converter });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b"",""Child"":{""Number"":3,""Name"":""c"",""CultureInfo"":null},""CultureInfo"":null},""CultureInfo"":""en-US""}")
                throw new ApplicationException("History doesn't work. Case 1");
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerDefault()
        {
            var item = SampleModel.CreateSampleWithCircularReference();
            var jss2 = new JavaScriptSerializer();
            jss2.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new[] { typeof(SampleModel) }) });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b"",""Child"":{""Number"":3,""Name"":""c"",""Child"":{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b""}}}}}")
                throw new ApplicationException("History doesn't work. Case 1");

            try
            {
                var o = jss2.Deserialize<SampleModel>("{Number:9}");
            }
            catch (Exception ex)
            {
                if (!(ex is NotImplementedException))
                    throw;
            }
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerWithHistoryAndCustomConverters2()
        {
            var item = new ComplexModel()
            {
                SampleModel = SampleModel.CreateSampleWithCultureInfo(),
                DateTime = DateTime.Now
            };

            var jss2 = new JavaScriptSerializer();
            var converter = new ControlledSerializationJsonConverter(
                    supportedTypes: new[] { item.GetType() },
                    formatters: new Dictionary<Type, Func<object, string>>()
                    {
                       { typeof(DateTime), (o) => ((DateTime)(o)).ToLongDateString()},
                       { typeof(CultureInfo), null}
                    },
                    recursionDepth: 4,
                    ignoreScriptIgnoreAttribute:false,
                    ignoreDuplicates: true);
            jss2.RegisterConverters(new[] { converter });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""SampleModel"":{""Number"":1,""Child"":{""Number"":2,""Child"":{""Number"":3,""CultureInfo"":null},""CultureInfo"":null},""CultureInfo"":""en-US""},""DateTime"":""Tuesday, April 18, 2017""}")
                throw new ApplicationException("History doesn't work. Case 1");

        }
    }

    class ControlledSerializationJsonConverter2: ControlledSerializationJsonConverter
    {
        public ControlledSerializationJsonConverter2(
            IEnumerable<Type> supportedTypes,
            int recursionDepth = 4,
            bool ignoreDuplicates = false,
            bool ignoreNotSupported = false,
            Dictionary<Type, Func<object, string>> formatters = null,
            bool ignoreScriptIgnoreAttribute = true,
            IEnumerable<Type> supremeTypes = null
            ) : base(supportedTypes, recursionDepth, ignoreDuplicates, ignoreNotSupported, formatters, ignoreScriptIgnoreAttribute, supremeTypes)
            
        {

        }
    }
}