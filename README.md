# Controlled Serialization Json Converter
JavaScriptConverter implementation with number of powerful parameters to avoid circular references during serialization and to improve json formatting. JavaScriptConverter realize serialization that uses those concepts:

1) **Depth** - max depth after which serialization will be stoped (avoiding circular references)
2) **Ignore duplicates** - 

 
## USE CASE 1: Safe log object as json string

```
var item =  ... ;  // object you want to log
if (item!=null)
{
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

