# Controlled Serialization Json Converter
It extentds JavaScriptConverter (from System.Web.Extensions assembly) with number of powerful parameters to avoid circular references during serialization and to improve json formatting. Those concepts are used:

1) **Max Depth** - number of steps into the object graph after which serialization will be stoped (avoiding circular references and infinitive serialization process);
2) **Ignore not supported types** - the list of referenced types you are interested in serialization (usually such list will be constructed witj `Assembly.GetType()` call; by default this is disabled therefore all referenced types will be parsed into deep till ***Max Depth***);
3) **Simple Types List** - by default it is a list of CLR standard simple types (`int`, `datetime`, etc) which will be serialized calling ToString() method; you can add reference types to this list if you are OK with ToString() serialization, otherwice use formatters (see bellow); 
4) **Ignore duplicates** - reference type ojects will appear in json only once (objects are tracked by references);
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

