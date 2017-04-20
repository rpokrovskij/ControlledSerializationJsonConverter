using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Diagnostics.Windows;
using System;
using System.Text;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Vse.Web.Serialization;

namespace JsonBenchmark
{
    //[Config(typeof(Config))]
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn,  ]
    [ClrJob]//, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser, InliningDiagnoser]
    public class BenchmarkJsonSerialize
    {
        public static TestStructure1 CreateTest()
        {
            var t = new TestStructure1() {
                Text1 ="dsdfsdf,",
                Text2 ="ds sdfsdf",
                Text3 = "fsd",
                DateTime = new DateTime(1975,01,01),
                StartActivity = true, FinishActivity = true, Input = true, Output = false, Verbose = true, UseBufferForVerbose = true, VerboseWithStackTrace = true };
            return t;
        }

        [Benchmark]
        public void NewtonsoftJson()
        {
            var t = CreateTest();
            var json = JsonConvert.SerializeObject(t);
            if (json == null)
                throw new ApplicationException("Test fails");
        }
        [Benchmark]
        public void DataContractJsonSerializer()
        {
            var t = CreateTest();
            var json = default(string);
            var serializer = new DataContractJsonSerializer(typeof(TestStructure1), new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true });
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, t);
                json = Encoding.Default.GetString(stream.ToArray());
            }
            if (json == null)
                throw new ApplicationException("Test fails");
        }
        [Benchmark]
        public void JavaScriptSerializer()
        {
            var t = CreateTest();
            var jss = new JavaScriptSerializer();
            var json = jss.Serialize(t);
            if (json == null)
                throw new ApplicationException("Test fails");
        }
        [Benchmark]
        public void JavaScriptSerializerCustom()
        {
            var t = CreateTest();
            var converter = new ControlledSerializationJsonConverter(new[] { t.GetType() });
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new[] { converter });
            var json = jss.Serialize(t);
            if (json == null)
                throw new ApplicationException("Test fails");
        }
        [Benchmark]
        public void ServiceStackToString()
        {
            var t = CreateTest();
            var json = ServiceStack.Text.JsonSerializer.SerializeToString(t);
            if (json == null)
                throw new ApplicationException("Test fails");
        }


        public BenchmarkJsonSerialize()
        {
          //  new Random(42).NextBytes(data);
        }

    }
}
