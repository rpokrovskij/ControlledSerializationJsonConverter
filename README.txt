# ControlledSerializationJsonConverter
Classic ASP framework JavaScriptConverter implementation with number of powerful parameters 
            
var item =  ... ;  // object you want to serialize
if (item!=null)
{
        var converter = new ControlledSerializationJsonConverter(
            **supportedTypes**: new[] {item.GetType(), typeof(MyClass1), typeof(MyClass2)},
            simpleTypes: ControlledSerializationJsonConverter.StandardSimpleTypes,
            converters: new Dictionary<Type, Func<object, string>>()
            {
                       { typeof(CultureInfo), (o) => ((CultureInfo)o).ToString()}
            },
            recursionDepth: 50,
            ignoreDuplicates: true);
            
        var jss = new JavaScriptSerializer();
        jss.RegisterConverters(new[] {converter});
        var json2 = jss.Serialize(item);
}
