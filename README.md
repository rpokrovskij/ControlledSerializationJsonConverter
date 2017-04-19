# Controlled Serialization Json Converter
[![NuGet](https://img.shields.io/nuget/v/Vse.Web.Serialization.ControlledSerializationJsonConverter.svg)](https://www.nuget.org/packages/Vse.Web.Serialization.ControlledSerializationJsonConverter)
[![Coverage Status](https://s3.amazonaws.com/assets.coveralls.io/badges/coveralls_100.svg)](https://coveralls.io/github/rpokrovskij/ControlledSerializationJsonConverter?branch=)

`ControlledSerializationJsonConverter` extentds `JavaScriptConverter` (customizable part of Microsoft `JavaScriptSerializer`, namespace `System.Web.Script.Serialization`, assembly `System.Web.Extensions`) with number of powerful parameters to avoid crashes by walking through circular references, to limit serialization to certain types or unique objects and to easy configure json formatting. 

## About JavaScriptSerializer  
`JavaScriptSerializer` was a Microsoft default json serialization tool for ASP platform till MVC5. MVC6 released at 12 August 2016 use `newtonsoft json.net` as default serialization tool. It seems like most ASP developers prefer `newtonsoft json.net` because of its reach serialization customization possibilities using attributes, when `JavaScriptSerializer` supports only `ScriptIgnoreAttribute`. I consider using attributes there as a controversional practice, when DTO class generation is a correct approach. Altough the practice shows that DTO approach should be complemented with the flexible tool that can serialize "everything" in case you need to do it quick without creating DTO. That what is the `ControlledSerializationJsonConverter`. 

## Usage
Simple configuration:
 ```
 var converter = new ControlledSerializationJsonConverter(new[] { item.GetType() }); 
 ```
 
 Comprehensive configuration:
 ```
  var converter = new ControlledSerializationJsonConverter(
                    supportedTypes:     typeof(MyModel).Assembly.GetTypes(), // all form models assembly
                    ignoreNotSupported: true,            // default false
                    recursionDepth:     10,              // default 4
                    ignoreDuplicates:   true,            // default false
                    ignoreScriptIgnoreAttribute : false, // default true
                    // serialize CultureInfo with to string
                    supremeTypes:        ControlledSerializationJsonConverter.StandardSimpleTypes.Union(new[] { typeof(CultureInfo) }),
                    // custom date formatter  and CultureInfo Formatter
                    formatters:         new Dictionary<Type, Func<object, string>>(){
                                               { typeof(DateTime), (o) => ((DateTime)(o)).ToLongDateString()},
                                               { typeof(CultureInfo), null} // for null - toString() will be used
                                        }
                    ); 
 ```
Then serialization looks as:
 
```
var jss = new JavaScriptSerializer();
jss.RegisterConverters(new[] { converter });
var json = jss.Serialize(item);
``` 

Note: `ControlledSerializationJsonConverter` as well as `JavaScriptSerializer` are not thread safe.
 
## Control serialization with ControlledSerializationJsonConverter
Those concepts and parameters to control the serialization process  used in ControlledSerializationJsonConverter:
1) **recursionDepth** - number of steps in the object's graph after which serialization will be stoped (avoiding circular references and infinitive serialization process); default `recursionDepth` is 4;
2) **supremeTypes list** - by default it is a list of CLR standard simple types (like `int`, `datetime`, etc) which will be serialized without going deep into theirs properties therefore "supreme types" are leafs in serialized object's graph;  you can add custom types to this list (referenced as well) using singnificant side effect: to process supreme type serializing process exit ControlledSerializationJsonConverter code and returns to `JavaScriptSerializer` code with all consiquences, which you should understand very good, therefore `supremeTypes` accessible only in protected constructor; 
3) **supportedTypes list and ignoreNotSupported flag** - in simplest configuration it should contain at least one item (type of the object you are going to serialize), otherwise `ControlledSerializationJsonConverter` will be not hooked by `JavaScriptSerializer` process and will not get a chance to start a work. This is the `JavaScriptSerializer` specific that can't be avoided. Once  converter was hooked, it not return serialization to parent process till of serializing *suprem types* (leafs). If `ignoreNotSupported` setuped to `true` then all *not supreme types* you are interested in serialization should be in this list otherwice they will be ignored. In that case  list of types usually will be constructed with `Assembly.GetTypes()` call; 
4) **Ignore duplicates** - when this flag is setuped, then reference types objects will be tracked by theirs references and one and the same object will be serialized only once;
5) **Formatters** - `Dictionary<Type, Func<object, string>>` that has a meaning of "for serialization that type use the function `Func<object, string>`". Func<object, string> can be null, then .ToString() will be used;
6) **ignoreScriptIgnoreAttribute** - default value is `false` that means all *ScriptIgnoreAttribute* attributes will be ignotred; with `ControlledSerializationJsonConverter` there are no sense to use *ScriptIgnoreAttribute* widely. 

## USE CASE 1: do the safe log of an object as json string
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

## USE CASE 2: to avoid Circular Reference exception in MVC5 Controller
```
public ActionResult GetItem2(int param1)
{
            var item = ...;
            // return Json(item, JsonRequestBehavior.AllowGet);
            
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new[] { new ControlledSerializationJsonConverter(new[] {item.GetType()}) });
            var json = jss.Serialize(item);
            return this.Content(json, "application/json");
}

```

## USE CASE 3: to avoid Circular Reference exception in MVC5 Web Api Controller
For this case you need to create ASP.MVC JavaScriptSerializerFormatter and register it in the Global.asax.cs

```
   public class JavaScriptSerializerFormatter : MediaTypeFormatter
    {
        IEnumerable<Type> types;
        public JavaScriptSerializerFormatter(IEnumerable<Type> types)
        {
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            this.types = types;
        }

        public override bool CanWriteType(Type type)
        {
            return types.Contains(type);
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var converter = new ControlledSerializationJsonConverter(
                supportedTypes: types,
                recursionDepth: 10,
                converters: new Dictionary<Type, Func<object, string>>() {
                     {typeof(CultureInfo), (o) => ((CultureInfo)o).ToString()}
                });

                var jss = new JavaScriptSerializer(); // is no thread safe and should be recreated
                jss.RegisterConverters(new[] { converter });
                var json = jss.Serialize(value);

                byte[] buf = System.Text.Encoding.Default.GetBytes(json);
                stream.Write(buf, 0, buf.Length);
                stream.Flush();
            });

            return task;
        }
    }
    
    // .............
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register); // IMPORTANT: GlobalConfiguration should go after FilterConfig (opposite to default order)
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var types = typeof(MvcApplication).Assembly.GetTypes()
                     .Where(t => t.IsClass && t.Namespace == "TestWebApp.Controllers.Models");
            GlobalConfiguration.Configuration.Formatters.Insert(0, new JavaScriptSerializerFormatter(types));
        }
    }
```



