﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Vse.Web.Serialization
{
    public class ControlledSerializationJsonConverter : JavaScriptConverter
    {
        #region StandardSimpleTypes
        public static readonly IEnumerable<Type> StandardSimpleTypes = new[]
            {
                typeof(bool),
                typeof(bool?),
                typeof(byte),
                typeof(byte?),
                typeof(char),
                typeof(char?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(float),
                typeof(float?),
                typeof(int),
                typeof(int?),
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
                typeof(string),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),
                typeof(Guid),
                typeof(Guid?),
                typeof(TimeSpan),
                typeof(TimeSpan?)
            };
        #endregion

        private readonly int recursionDepth = 1;
        private readonly int currentRecursionDepth = 1;
        private readonly bool ignoreDuplicates;
        private readonly List<object> history;
        private readonly IEnumerable<Type> supportedTypes;
        private readonly IEnumerable<Type> simpleTypes;
        private readonly Dictionary<Type, Func<object, string>> converters;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supportedTypes">Referenced types, types that can be parsed into</param>
        /// <param name="simpleTypes">Value types that are not tracked in history (for duplicates). Serialized with ToString()</param>
        /// <param name="converters">Some referenced types on complex types can be configured as simple with custome serializers</param>
        /// <param name="recursionDepth">Control recursion</param>
        /// <param name="ignoreDuplicates"></param>
        public ControlledSerializationJsonConverter(
            IEnumerable<Type> supportedTypes,
            IEnumerable<Type> simpleTypes = null,
            Dictionary<Type, Func<object, string>> converters = null,
            int recursionDepth = 1,
            bool ignoreDuplicates = false) :
            this(supportedTypes, simpleTypes, converters, recursionDepth, ignoreDuplicates, 1, new List<object>())
        {
        }

        private ControlledSerializationJsonConverter(
            IEnumerable<Type> supportedTypes,
            IEnumerable<Type> simpleTypes,
            Dictionary<Type, Func<object, string>> converters,
            int recursionDepth,
            bool ignoreDuplicates, int currentRecursionDepth, List<object> history)
        {
            this.recursionDepth = recursionDepth;
            this.ignoreDuplicates = ignoreDuplicates;
            this.supportedTypes = supportedTypes;
            this.converters = converters;
            this.simpleTypes = simpleTypes ?? StandardSimpleTypes;
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
                    if (simpleTypes.Contains(propertyInfo.PropertyType))
                    {
                        string propertyName = propertyInfo.Name;
                        var value = propertyInfo.GetValue(o, null);
                        standardTypesValues.Add(propertyName, value);
                    }
                    else if (converters != null && converters.TryGetValue(propertyInfo.PropertyType, out Func<object, string> func))
                    {
                        if (o != null)
                        {
                            string propertyName = propertyInfo.Name;
                            var value = propertyInfo.GetValue(o, null);
                            var stringValue = (value == null) ? null : func(value);
                            standardTypesValues.Add(propertyName, stringValue);
                        }
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
            var js = new ControlledSerializationJsonConverter(supportedTypes, simpleTypes, converters, recursionDepth - currentRecursionDepth, ignoreDuplicates, currentRecursionDepth, history);
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new[] { new ControlledSerializationJsonConverter(supportedTypes, simpleTypes) });
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
