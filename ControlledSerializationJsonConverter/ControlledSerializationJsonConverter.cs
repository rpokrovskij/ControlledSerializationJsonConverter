using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Vse.Web.Serialization
{
    public class ControlledSerializationJsonConverter : JavaScriptConverter
    {
        public List<object> history;
        public int currentRecursionDepth;
        readonly ControlledSerializationJsonConverter parent;

        #region StandardSimpleTypes
        public static readonly IEnumerable<Type> StandardSimpleTypes = new[] {
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
        #endregion

        private readonly int recursionDepth = 1;
        private readonly bool ignoreDuplicates;
        private readonly bool ignoreNotSupported;
        private readonly bool ignoreScriptIgnoreAttribute;
        private readonly IEnumerable<Type> supportedTypes;
        private readonly IEnumerable<Type> supremeTypes;
        private readonly Dictionary<Type, Func<object, string>> formatters;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supportedTypes">Referenced types, types that can be parsed into</param>
        /// <param name="simpleTypes">Value types that are not tracked in history (for duplicates). Serialized with ToString()</param>
        /// <param name="formatters">Some referenced types on complex types can be configured as simple with custome serializers</param>
        /// <param name="recursionDepth">Control recursion</param>
        /// <param name="ignoreDuplicates"></param>
        /// <param name="ignoreNotSupported"></param>
        public ControlledSerializationJsonConverter(
            IEnumerable<Type> supportedTypes,
            int recursionDepth = 4,
            bool ignoreDuplicates = false,
            bool ignoreNotSupported = false,
            Dictionary<Type, Func<object, string>> formatters = null,
            bool ignoreScriptIgnoreAttribute = true
            ) :
            this(supportedTypes, recursionDepth, ignoreDuplicates, ignoreNotSupported, formatters, ignoreScriptIgnoreAttribute, null)
        {

        }

        protected ControlledSerializationJsonConverter(
            IEnumerable<Type> supportedTypes,
            int recursionDepth = 4,
            bool ignoreDuplicates = false,
            bool ignoreNotSupported = false,
            Dictionary<Type, Func<object, string>> formatters = null,
            bool ignoreScriptIgnoreAttribute = true,
            IEnumerable<Type> supremeTypes = null
            ) :
            this(supportedTypes, recursionDepth, ignoreDuplicates, ignoreNotSupported, formatters, ignoreScriptIgnoreAttribute, supremeTypes,   1, new List<object>(), null)
        {

        }
        private static IEnumerable<Type> ExtractTypes(IEnumerable<Type> source)
        {
            var destination = new List<Type>();
            foreach(var t in source)
            {
                if (t.IsArray)
                {
                    var x = t.GetElementType();
                    destination.Add(x);
                }
                else 
                {
                    var interfaces = t.GetInterfaces();
                    if (interfaces.Length > 0 && interfaces.Contains(typeof(IEnumerable)))
                    {
                        if (interfaces.Contains(typeof(IList)))
                        {
                            var x = t.GetProperty("Item").PropertyType;
                            destination.Add(x);
                        }
                        else if (interfaces.Any(t2 =>
                                    t2.IsGenericType &&
                                    t2.GetGenericTypeDefinition() == typeof(ISet<>)))
                        {
                            var x = t.GetGenericArguments()[0];
                            destination.Add(x);
                        }
                        else
                        {
                            destination.Add(t);
                        }
                    }
                    else
                    {
                        destination.Add(t);
                    }
                }
            }
            return destination;
        }
        private ControlledSerializationJsonConverter(
            IEnumerable<Type> supportedTypes,
            int recursionDepth,
            bool ignoreDuplicates,
            bool ignoreNotSupported,
            Dictionary<Type, Func<object, string>> formatters,
            bool ignoreScriptIgnoreAttribute,
            IEnumerable<Type> supremeTypes,
            int currentRecursionDepth, 
            List<object> history, 
            ControlledSerializationJsonConverter parent)
        {
            if (supportedTypes == null || supportedTypes.Count() == 0)
                throw new ArgumentException("SupportedTypes can't be null or empty", nameof(supportedTypes));
            this.recursionDepth     = recursionDepth;
            this.ignoreDuplicates   = ignoreDuplicates;
            this.ignoreNotSupported = ignoreNotSupported;
            this.ignoreScriptIgnoreAttribute = ignoreScriptIgnoreAttribute;
            this.supportedTypes     = ExtractTypes(supportedTypes);
            this.formatters         = formatters;
            this.supremeTypes        = supremeTypes ?? StandardSimpleTypes;
            this.currentRecursionDepth = currentRecursionDepth;
            this.history = history;
            this.parent = parent;
        }

        public override IDictionary<string, object> Serialize(object o, JavaScriptSerializer serializer)
        {
            var type = o.GetType();
            var standardTypesValues = new Dictionary<string, object>();
            history.Add(o);
            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
                {
                    if (ignoreScriptIgnoreAttribute || (!ignoreScriptIgnoreAttribute && !Attribute.IsDefined(propertyInfo, typeof(ScriptIgnoreAttribute))))
                    {
                        if (formatters != null && formatters.TryGetValue(propertyInfo.PropertyType, out Func<object, string> func))
                        {
                            string propertyName = propertyInfo.Name;
                            var value = propertyInfo.GetValue(o, null);
                            var stringValue = (value == null) ? null : (func == null ? value.ToString():func(value));
                            standardTypesValues.Add(propertyName, stringValue);
                        }
                        else if (supremeTypes.Contains(propertyInfo.PropertyType))
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
                                    if (ignoreNotSupported)
                                    {
                                        if (supportedTypes.Contains(value.GetType()))
                                        {
                                            var dictionaryProperties = LayerUp(propertyName, value);
                                            standardTypesValues.Add(propertyName, dictionaryProperties);
                                        }
                                    }
                                    else if (ignoreDuplicates)
                                    {
                                        if (!history.Contains(value))
                                        {
                                            var dictionaryProperties = LayerUp(propertyName, value);
                                            standardTypesValues.Add(propertyName, dictionaryProperties);
                                        }
                                    }
                                    else
                                    {
                                        var dictionaryProperties = LayerUp(propertyName, value);
                                        standardTypesValues.Add(propertyName, dictionaryProperties);
                                    }
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
            var js = new ControlledSerializationJsonConverter(supportedTypes, recursionDepth - currentRecursionDepth, ignoreDuplicates, ignoreNotSupported, formatters, ignoreScriptIgnoreAttribute, supremeTypes, 
                currentRecursionDepth, history, this);
            var jss = new JavaScriptSerializer();
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
