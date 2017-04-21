using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Diagnostics.Windows;
using System;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using System.Collections.Generic;
using System.Linq;

namespace JsonBenchmark
{
    //[Config(typeof(Config))]
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn]
    [ClrJob/*, CoreJob*/]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser, InliningDiagnoser]
    public class BenchmarkHashset
    {
        static IEnumerable<Type> GetHashSet()
        {
            return new HashSet<Type>(new[] {
                typeof(string),
                typeof(bool),
                typeof(int),
                typeof(DateTime),
                typeof(bool?),
                typeof(int?),
                typeof(DateTime?),
                typeof(Guid),
                typeof(Guid?),
                typeof(TimeSpan),
                typeof(TimeSpan?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(float),
                typeof(float?),
                typeof(byte),
                typeof(byte?),
                typeof(char),
                typeof(char?),
                typeof(long),
                typeof(long?),
                typeof(sbyte),
                typeof(sbyte?),
                typeof(short),
                typeof(short?),
                typeof(uint),
                typeof(uint?),
                typeof(ulong),
                typeof(ulong?),
                typeof(ushort),
                typeof(ushort?),
                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),
            });
        }

        static IEnumerable<Type> GetArray()
        {
            return new[] {
                typeof(string),
                typeof(bool),
                typeof(int),
                typeof(DateTime),
                typeof(bool?),
                typeof(int?),
                typeof(DateTime?),
                typeof(Guid),
                typeof(Guid?),
                typeof(TimeSpan),
                typeof(TimeSpan?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(float),
                typeof(float?),
                typeof(byte),
                typeof(byte?),
                typeof(char),
                typeof(char?),
                typeof(long),
                typeof(long?),
                typeof(sbyte),
                typeof(sbyte?),
                typeof(short),
                typeof(short?),
                typeof(uint),
                typeof(uint?),
                typeof(ulong),
                typeof(ulong?),
                typeof(ushort),
                typeof(ushort?),
                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),
            };
        }

        static List<Type> GetList()
        {
            return new List<Type>(new[] {
                typeof(string),
                typeof(bool),
                typeof(int),
                typeof(DateTime),
                typeof(bool?),
                typeof(int?),
                typeof(DateTime?),
                typeof(Guid),
                typeof(Guid?),
                typeof(TimeSpan),
                typeof(TimeSpan?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(float),
                typeof(float?),
                typeof(byte),
                typeof(byte?),
                typeof(char),
                typeof(char?),
                typeof(long),
                typeof(long?),
                typeof(sbyte),
                typeof(sbyte?),
                typeof(short),
                typeof(short?),
                typeof(uint),
                typeof(uint?),
                typeof(ulong),
                typeof(ulong?),
                typeof(ushort),
                typeof(ushort?),
                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),
            });
        }

        [Benchmark]
        public void Array()
        {
            var l = GetArray();
            l.Contains(typeof(string));
            l.Contains(typeof(DateTime));
            l.Contains(typeof(bool));
            l.Contains(typeof(bool?));
            l.Contains(typeof(int));
            l.Contains(typeof(int?));
            l.Contains(typeof(long));
            l.Contains(typeof(double));

        }
        [Benchmark]
        public void HashSet()
        {
            var l = GetHashSet();
            l.Contains(typeof(string));
            l.Contains(typeof(DateTime));
            l.Contains(typeof(bool));
            l.Contains(typeof(bool?));
            l.Contains(typeof(int));
            l.Contains(typeof(int?));
            l.Contains(typeof(long));
            l.Contains(typeof(double));
        }

        [Benchmark]
        public void Null()
        {
        }

        [Benchmark]
        public void Concantent()
        {
            var a = "asdasd1";
            var b = "asdasd2";
            var c = a + b;
        }
    }
}
