using BenchmarkDotNet.Running;
using BenchmarkDotNet.Diagnosers;
using System;
using Newtonsoft.Json;
using Vse.Web.Serialization;
using System.Web.Script.Serialization;

namespace JsonBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //var x = BenchmarkJsonSerializeList.CreateTest();
            //var json = JsonConvert.SerializeObject(x);

            var t = BenchmarkJsonSerializeList.Test;
            var converter = new ControlledSerializationJsonConverter(new[] { t.GetType() });
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new[] { converter });
            var json = jss.Serialize(t);
            if (json == null)
                throw new ApplicationException("Test fails");

            //var summary4 = BenchmarkRunner.Run<BenchmarkJsonDeserializeList>();
            var summary3 = BenchmarkRunner.Run<BenchmarkJsonSerializeList>();
            //BenchmarkRunner.Run<BenchmarkHashset>();
            //var summary1 =BenchmarkRunner.Run<BenchmarkJsonSerialize>();
            //var summary2 = BenchmarkRunner.Run<BenchmarkJsonDeserialize>();
            
            
            Console.ReadLine();
        }
    }
}
