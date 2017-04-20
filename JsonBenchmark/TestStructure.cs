using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using ServiceStack;

namespace JsonBenchmark
{
    /// <summary>
    /// JSON has been described in sever standards and differs from standard to standard (scalars, braces, comments)
    /// http://seriot.ch/parsing_json.php
    /// </summary>

    public class JsonTest
    {
        /// <summary>
        /// It is a best deserializer not only because of speed (2nd) but also because of supported fluent
        /// syntax. It supports also single quotes what is important mixing xml and json, c# and json.
        /// E.g it is possible to deserialize <code>var serialized = "[['asd', 'pok'],['asd2', 'pok2']]";</code>
        /// </summary>
        
    }

    public class TestStructure1
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public DateTime DateTime { get; set; }
        public bool StartActivity { get; set; }
        public bool FinishActivity { get; set; }
        public bool Input { get; set; }
        public bool Output { get; set; }
        public bool Verbose { get; set; }
        public bool UseBufferForVerbose { get; set; }
        public bool VerboseWithStackTrace { get; set; }
    }
}
