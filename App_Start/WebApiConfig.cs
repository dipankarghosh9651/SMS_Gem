using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SMS_Gem
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // 1. Enable Attribute Routing & Default Routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // 2. Remove the XML Formatter completely
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // 3. Configure JSON Formatter Defaults (CamelCase, Ignore Nulls/Loops)
            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsonFormatter.SerializerSettings.Formatting = Formatting.None;

            // 4. Force JSON header response type
            jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }
    }
}


//using System.Web.Http;

//namespace SMS_Gem
//{
//    public static class WebApiConfig
//    {
//        [SMS.Security.JwtAuth]
//        public class StudentApi_CLController : ApiController
//        {
//            // All endpoints in this controller now require a valid, non-expired JWT Bearer token
//        }


//        public static void Register(HttpConfiguration config)
//        {
//            // Enable Attribute Routing
//            config.MapHttpAttributeRoutes();

//            // Default Convention Route
//            config.Routes.MapHttpRoute(
//                name: "DefaultApi",
//                routeTemplate: "api/{controller}/{action}/{id}",
//                defaults: new { id = RouteParameter.Optional }
//            );
//        }
//    }
//}

//using System.Web.Http;

//namespace SMS
//{
//    public static class WebApiConfig
//    {
//        public static void Register(HttpConfiguration config)
//        {
//            // 1. Enable attribute routing
//            config.MapHttpAttributeRoutes();

//            // 2. Map Action-based routing (api/{controller}/{action}/{id})
//            config.Routes.MapHttpRoute(
//                name: "ActionApi",
//                routeTemplate: "api/{controller}/{action}/{id}",
//                defaults: new { id = RouteParameter.Optional }
//            );

//            // 3. Ensure JSON is returned instead of XML
//            config.Formatters.Remove(config.Formatters.XmlFormatter);
//        }
//    }
//}
