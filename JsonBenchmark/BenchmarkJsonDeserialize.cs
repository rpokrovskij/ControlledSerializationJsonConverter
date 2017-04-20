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
using ServiceStack;
using System.Runtime.Serialization;

namespace JsonBenchmark
{
    //[Config(typeof(Config))]
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn,  ]
    [ClrJob]//, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser, InliningDiagnoser]
    public class BenchmarkJsonDeserialize
    {
        static string GetJson()
        {
            return "{\"Text1\":\"dsdfsdf,\",\"Text2\":\"ds sdfsdf\",\"Text3\":\"fsd\",\"DateTime\":\"1975-01-01T00:00:00\",\"StartActivity\":true,\"FinishActivity\":true,\"Input\":true,\"Output\":false,\"Verbose\":true,\"UseBufferForVerbose\":true,\"VerboseWithStackTrace\":true}";
        }
        static string GetJsonInline()
        {
            return "{StartActivity:true, FinishActivity:true, Input:true, Output:false, Verbose:true, UseBufferForVerbose:true, VerboseWithStackTrace:true }";
        }
        [Benchmark]
        public void DataContractJsonSerializer()
        {
            var json = GetJson();
            var t = default(TestStructure1);
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var deserializer = new DataContractJsonSerializer(
                    typeof(TestStructure1),
                    new DataContractJsonSerializerSettings() { DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ss") });
                t = (TestStructure1)deserializer.ReadObject(ms);
            }
            if (t == null)
                throw new ApplicationException("Test fails");
        }
        [Benchmark]
        public void JavaScriptSerializer()
        {
            var json = GetJson();
            var jss = new JavaScriptSerializer();
            var t = jss.Deserialize<TestStructure1>(json);
            if (t == null)
                throw new ApplicationException("Test fails");
        }
        [Benchmark]
        public void NewtonsoftJson()
        {
            var json = GetJson();
            var t = JsonConvert.DeserializeObject<TestStructure1>(json);
            if (t == null)
                throw new ApplicationException("Test fails");
        }
        
        [Benchmark]
        public void ServiceStackDynamicJson()
        {
            var json = GetJson();
            dynamic t = DynamicJson.Deserialize(json);
            if (t == null)
                throw new ApplicationException("Test fails");

        }
        [Benchmark]
        public void ServiceStackDynamicJsonWithParsing()
        {
            var json = GetJson();
            dynamic d = DynamicJson.Deserialize(json);
            var t = new TestStructure1()
            {
                StartActivity = bool.Parse(d.StartActivity),
                FinishActivity = bool.Parse(d.FinishActivity),
                Input = bool.Parse(d.Input),
                Output = bool.Parse(d.Output),
                Verbose = bool.Parse(d.Verbose),
                UseBufferForVerbose = bool.Parse(d.UseBufferForVerbose),
                VerboseWithStackTrace = bool.Parse(d.VerboseWithStackTrace)
            };
            if (t == null)
                throw new ApplicationException("Test fails");

        }
        [Benchmark]
        public void ServiceStackJson()
        {
            var json = GetJson();
            var t = json.FromJson<TestStructure1>();
            if (t == null)
                throw new ApplicationException("Test fails");

        }


        //private class Config : ManualConfig
        //{
        //    public Config()
        //    {
        //        Add(
        //            new Job(EnvMode.Clr, RunMode.Dry)
        //            {
        //                Run = { LaunchCount = 3, WarmupCount = 5, TargetCount = 10 }

        //                //,
        //                //Accuracy = { MaxStdErrRelative = 0.01 }
        //            }
        //        );
        //    }
        //}
        //private readonly byte[] data = new byte[10000];
        //private readonly SHA256 sha256 = SHA256.Create();
        //private readonly MD5 md5 = MD5.Create();

        public BenchmarkJsonDeserialize()
        {
          //  new Random(42).NextBytes(data);
        }

    }
}
