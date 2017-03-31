﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

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
            var item = Item.CreateSample();

            var jss1 = new JavaScriptSerializer();
            jss1.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new[] { typeof(Item) }, 50, false, false, ControlledSerializationJsonConverter.StandardSimpleTypes, null) });

            var json1 = jss1.Serialize(item);
            if (json1.Length < 1000)
                throw new ApplicationException("History doesn't work. Case 0");

            var jss2 = new JavaScriptSerializer();
            jss2.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new[] { typeof(Item) }, 50, true, false) });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b"",""Child"":{""Number"":3,""Name"":""c""}}}")
                throw new ApplicationException("History doesn't work. Case 1");
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerWithHistory()
        {
            var item = Item.CreateSample();
            var jss2 = new JavaScriptSerializer();
            jss2.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new[] { typeof(Item) }, 50, true, false) });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b"",""Child"":{""Number"":3,""Name"":""c""}}}")
                throw new ApplicationException("History doesn't work. Case 1");
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerWithHistoryAndNotSupported()
        {
            var item = Item.CreateSampleWithCultureInfo();
            var jss2 = new JavaScriptSerializer();
            var converter = new ControlledSerializationJsonConverter(
                    supportedTypes: new[] { typeof(Item) },
                    simpleTypes: ControlledSerializationJsonConverter.StandardSimpleTypes,
                    ignoreNotSupported: true,
                    recursionDepth: 3,
                    ignoreDuplicates: true);
            jss2.RegisterConverters(new[] { converter });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b"",""Child"":{""Number"":3,""Name"":""c"",""Child"":{""Number"":1,""Name"":""a""}}}}")
                throw new ApplicationException("History doesn't work. Case 1");
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerConfigurationException()
        {
            var item = Item.CreateSample();

            var jss1 = new JavaScriptSerializer();
            try
            {
                jss1.RegisterConverters(new[] { new ControlledSerializationJsonConverter(null, 50, false, false, ControlledSerializationJsonConverter.StandardSimpleTypes, null) });
                var json1 = jss1.Serialize(item);
            }
            catch (ArgumentException)
            {
            }

            var jss2 = new JavaScriptSerializer();
            
            try
            {
                jss2.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new List<Type>(), 50, false, false, ControlledSerializationJsonConverter.StandardSimpleTypes, null) });
                var json1 = jss2.Serialize(item);
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void RecursiveJavaScriptSerializerWithHistoryAndCustomConverters()
        {
            var item = Item.CreateSampleWithCultureInfo();
            var jss2 = new JavaScriptSerializer();
            var converter = new ControlledSerializationJsonConverter(
                    supportedTypes: new[] { typeof(Item)},
                    simpleTypes: ControlledSerializationJsonConverter.StandardSimpleTypes,
                    converters: new Dictionary<Type, Func<object, string>>()
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
            var item = Item.CreateSample();
            var jss2 = new JavaScriptSerializer();
            jss2.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new[] { typeof(Item) }) });
            var json2 = jss2.Serialize(item);
            if (json2 != @"{""Number"":1,""Name"":""a"",""Child"":{""Number"":2,""Name"":""b""}}")
                throw new ApplicationException("History doesn't work. Case 1");

            try
            {
                var o = jss2.Deserialize<Item>("{Number:9}");
            }
            catch (Exception ex)
            {
                if (!(ex is NotImplementedException))
                    throw;
            }
        }

        class Item
        {
            public static Item CreateSample()
            {
                var item1 = new Item { Name = "a", Number = 1 };
                var item2 = new Item { Name = "b", Number = 2 };
                item1.Child = item2; // circular reference 
                var item3 = new Item { Name = "c", Number = 3 };
                item2.Child = item3;
                item3.Child = item1;
                return item1;
            }
            public static Item CreateSampleWithCultureInfo()
            {
                var item1 = CreateSample();
                item1.CultureInfo = CultureInfo.CurrentCulture;
                return item1;
            }
            public int Number { get; set; }
            public string Name { get; set; }
            public Item Child { get; set; }
            public CultureInfo CultureInfo { get; set; }

            private string Name2 { get; set; } = "default";

            public string this[int index]
            {
                get
                {
                    return Name2[index].ToString();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        }

        public class CircularScriptConverter : JavaScriptConverter
        {
            private readonly int recursionDepth = 1;
            private readonly int currentRecursionDepth = 1;
            private readonly bool ignoreDuplicates;
            private readonly List<object> history = new List<object>();


            readonly IEnumerable<Type> supportedTypes;

            public CircularScriptConverter(IEnumerable<Type> supportedTypes, int recursionDepth = 1, bool ignoreDuplicates = false)
            {
                this.recursionDepth = recursionDepth;
                this.supportedTypes = supportedTypes;
                this.ignoreDuplicates = ignoreDuplicates;
            }

            private CircularScriptConverter(IEnumerable<Type> supportedTypes, int recursionDepth, bool ignoreDuplicates, int currentRecursionDepth, List<object> history)
            {
                this.recursionDepth = recursionDepth;
                this.ignoreDuplicates = ignoreDuplicates;
                this.supportedTypes = supportedTypes;
                this.currentRecursionDepth = currentRecursionDepth;
                this.history = history;
            }

            public override IDictionary<string, object> Serialize(object o, JavaScriptSerializer serializer)
            {
                history.Add(o);
                var type = o.GetType();
                var standardTypesValues = new Dictionary<string, object>();
                var properties = type.GetProperties();

                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
                    {
                        if (ControlledSerializationJsonConverter.StandardSimpleTypes.Contains(propertyInfo.PropertyType))
                        {
                            string propertyName = propertyInfo.Name;
                            var value = propertyInfo.GetValue(o, null);
                            standardTypesValues.Add(propertyName, value);
                        }
                        else
                        {
                            if (currentRecursionDepth <= recursionDepth)
                            {
                                string propertyName = propertyInfo.Name;
                                var value = propertyInfo.GetValue(o, null);
                                if (value != null)
                                {
                                    if (!ignoreDuplicates)
                                    {
                                        var dictionaryProperties = LayerUp(propertyName, value);
                                        standardTypesValues.Add(propertyName, dictionaryProperties);
                                    }
                                    else if (!history.Contains(value))
                                    {
                                        var dictionaryProperties = LayerUp(propertyName, value);
                                        standardTypesValues.Add(propertyName, dictionaryProperties);

                                    }

                                }
                            }
                        }
                    }
                }
                return standardTypesValues;
            }

            private IDictionary<string, object> LayerUp(string propertyName, object value)
            {
                var js = new CircularScriptConverter(supportedTypes, recursionDepth - currentRecursionDepth, ignoreDuplicates, currentRecursionDepth, history);
                var jss = new JavaScriptSerializer();
                jss.RegisterConverters(new[] { new CircularScriptConverter(supportedTypes) });
                var dictionary = js.Serialize(value, jss);
                return dictionary;
            }

            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException("This json serializer is used only for serialization");
            }

            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    return supportedTypes;
                }
            }


        }
    }
}
