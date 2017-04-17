# Controlled Serialization Json Converter
It extentds JavaScriptConverter (from System.Web.Extensions assembly) with number of powerful parameters to avoid circular references during serialization and to improve json formatting. Those concepts to control serialization are used:

1) **Recursion depth** - number of steps in the object's graph after which serialization will be stoped (avoiding circular references and infinitive serialization process); default `recursionDepth` is 4;
2) **Supported types and Ignore Not Supported Types flag** - the list of reference types you are interested in serialization (usually such list will be constructed witj `Assembly.GetTypes()` call; 
   Supported Types should contains at least one item (the serialized object type). If ***Ignore Not Supported Types flag*** is disabled then every 
by default this is disabled therefore all referenced types will be parsed into deep till ***Max Depth***);
3) **Simple types list** - by default it is a list of CLR standard simple types (like `int`, `datetime`, etc) which will be serialized without parsing into theirs properties; you can add reference types to this list if you are OK with ToString() serialization, other method to prevent "going deep" is to use formatters (see bellow); 
4) **Ignore duplicates** - if this flag is setuped then reference type ojects will appear in json only once (objects are tracked by references);
5) **Formatters** - dictionary `Dictionary<Type, Func<object, string>>` that has a meaning of "for serialization of Type use the function `Func<object, string>`". 
 
Simple usage:
 ```
 var converter1 = new ControlledSerializationJsonConverter(new[] { item.GetType() }); 
 ```
 
 Comprehensive:
 ```
 var converter1 = new ControlledSerializationJsonConverter(
                    supportedTypes:     new[] { typeof(Class1), typeof(Class2), typeof(Class3) },
                    recursionDepth:     10,
                    ignoreDuplicates:   true
                    ignoreNotSupported: true,
                    simpleTypes:        ControlledSerializationJsonConverter.StandardSimpleTypes,
                    ); 
 ```

## About JavaScriptSerializer  
`JavaScriptSerializer` was a Micorsoft default json serialiaztion instrument for ASP platform till MVC6. It seems like now most ASP users preffer `newtonsoft json.net` becasue of its reach serialization customization using attributes, when JavaScriptSerializer supports only ScriptIgnoreAttribute, but as consider using attributes as wrong practice. By default even this attribute interpetation is disabled in the `ControlledSerializationJsonConverter`. To enable it setup `ignoreScriptIgnoreAttribute=false` in constructor.

## USE CASE 1: Safe log object as json string
```
var item =  ... ;  // object you want to log
if (item!=null) {
        var converter = new ControlledSerializationJsonConverter(
            supportedTypes: new[] {item.GetType()},
            recursionDepth: 10);
            
        var jss = new JavaScriptSerializer();
        jss.RegisterConverters(new[] {converter});
        var json = jss.Serialize(item);
}
```

## USE CASE 2: MVC5 Controller throws Circular Reference exception 
```


```

## USE CASE 3: MVC5 WebAPI Controller throws Circular Reference exception 
```

```


