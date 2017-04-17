# Controlled Serialization Json Converter
`ControlledSerializationJsonConverter` extentds `JavaScriptConverter` (from `System.Web.Extensions` assembly) with number of powerful parameters to avoid carshes by circular references during serialization and to improve json formatting. 

## About JavaScriptSerializer  
`JavaScriptSerializer` was a Micorsoft default json serialiaztion instrument for ASP platform till MVC6. Now it seems like most ASP users preffer `newtonsoft json.net` becasue of its reach serialization customization using attributes, when `JavaScriptSerializer` supports only `ScriptIgnoreAttribute`. I consider using attributes as a wrong practice, when DTO class generation is a best approach. Practice shows that DTO approach should be complemented with the flexible tool that can "serialize everithing". That what is `ControlledSerializationJsonConverter`. 

Simple configuration:
 ```
 var converter = new ControlledSerializationJsonConverter(new[] { item.GetType() }); 
 ```
 
 Comprehensive configuration:
 ```
 var converter = new ControlledSerializationJsonConverter(
                    supportedTypes:     new[] { typeof(Class1), typeof(Class2), typeof(Class3) },
                    recursionDepth:     10,
                    ignoreDuplicates:   true
                    ignoreNotSupported: true,
                    simpleTypes:        ControlledSerializationJsonConverter.StandardSimpleTypes,
                    ); 
 ```
 Serialization
 
```
var jss = new JavaScriptSerializer();
jss2.RegisterConverters(new[] { converter });
var json2 = jss2.Serialize(item);
``` 

Note: `ControlledSerializationJsonConverter` as well as `JavaScriptSerializer` are not thread safe.
 
## Control serialization with ControlledSerializationJsonConverter
Those concepts and parameters are used
1) **recursionDepth** - number of steps in the object's graph after which serialization will be stoped (avoiding circular references and infinitive serialization process); default `recursionDepth` is 4;
2) **simpleTypes** list - by default it is a list of CLR standard simple types (like `int`, `datetime`, etc) which will be serialized without going deep into theirs properties therefore "simple types" as a leafs in serialized object's graph; you can add custom types to this list if you are OK with ToString() serialization; 
3) **supportedTypes list and ignoreNotSupported flag** - the list of reference types you are interested in serialization (usually such list will be constructed with `Assembly.GetTypes()` call; 
   Supported Types should contains at least one item (the serialized object's type), otherwise `ControlledSerializationJsonConverter` will be not hooked by JavaScriptSerializer and not used in serialization. Once `ControlledSerializationJsonConverter` was hooked it will return (instead of converters for system simple types).  
   If ***Ignore Not Supported Types flag*** is enabled then types only from this list will be sinchronized;
4) **Ignore duplicates** - if this flag is setuped then reference type ojects will appear in json only once (objects are tracked by references);
5) **Formatters** - dictionary `Dictionary<Type, Func<object, string>>` that has a meaning of "for serialization of Type use the function `Func<object, string>`". 
 


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


