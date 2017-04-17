# Controlled Serialization Json Converter
`ControlledSerializationJsonConverter` extentds `JavaScriptConverter` (from assembly `System.Web.Extensions`) with number of powerful parameters to avoid carshes by circular references during serialization and to improve json formatting. 

## About JavaScriptSerializer  
`JavaScriptSerializer` was a Micorsoft default json serialiaztion instrument for ASP platform till MVC6. Now it seems like most ASP users preffer `newtonsoft json.net` becasue of its reach serialization customization using attributes, when `JavaScriptSerializer` supports only `ScriptIgnoreAttribute`. I consider using attributes as a wrong practice, when DTO class generation is a best approach. Practice shows that DTO approach should be complemented with the flexible tool that can serialize "everithing" in case you need to do it rapidly without DTO. That what is `ControlledSerializationJsonConverter`. 

Simple configuration:
 ```
 var converter = new ControlledSerializationJsonConverter(new[] { item.GetType() }); 
 ```
 
 Comprehensive configuration:
 ```
  var converter = new ControlledSerializationJsonConverter(
                    supportedTypes:     typeof(MyModel).GetAssembly().GetTypes(), // all form models assembly
                    ignoreNotSupported: true,            // default false
                    recursionDepth:     10,              // default 4
                    ignoreDuplicates:   true,            // default false
                    ignoreScriptIgnoreAttribute : false, // default true
                    // serialize CultureInfo with to string
                    simpleTypes:        ControlledSerializationJsonConverter.StandardSimpleTypes.Union(new[] { typeof(CultureInfo) }),
                    // custom date formatter
                    formatters:         new Dictionary<Type, Func<object, string>>(){
                                               { typeof(DateTime), (o) => ((DateTime)(o)).ToLongDateString()} 
                                        }
                    ); 
 ```
 Call serialization
 
```
var jss = new JavaScriptSerializer();
jss2.RegisterConverters(new[] { converter });
var json2 = jss2.Serialize(item);
``` 

Note: `ControlledSerializationJsonConverter` as well as `JavaScriptSerializer` are not thread safe.
 
## Control serialization with ControlledSerializationJsonConverter
Those concepts and parameters to controle the serialization are used
1) **recursionDepth** - number of steps in the object's graph after which serialization will be stoped (avoiding circular references and infinitive serialization process); default `recursionDepth` is 4;
2) **simpleTypes** list - by default it is a list of CLR standard simple types (like `int`, `datetime`, etc) which will be serialized without going deep into theirs properties therefore "simple types" as a leafs in serialized object's graph; you can add custom types to this list if you are OK with ToString() serialization; 
3) **supportedTypes list and ignoreNotSupported flag** - in simplest configuration it should contain at least one item (serialized object's type), otherwise `ControlledSerializationJsonConverter` will be not hooked by `JavaScriptSerializer` process and not have a chance to start a work. Once this converter was hooked it not return serialization to parent process except of serializing simple system types (leafs). If `ignoreNotSupported` setuped to `true` then all reference types you are interested in serialization should be in this list. In that case usually list of types will be constructed with `Assembly.GetTypes()` call; 
4) **Ignore duplicates** - if this flag is setuped then reference types objects will tracked by reference, one and the same object will be serialized only once;
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


