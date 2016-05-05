using Newtonsoft.Json.Serialization;
using System;
using System.Web.Http;

namespace TennisWeb
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new LowercaseFirstContractResolver();

      config.Routes.MapHttpRoute(
        name: "EventFeedApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional },
        constraints: new { controller = "EventFeed" }
      );

      config.Routes.MapHttpRoute(
        name: "UserApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional },
        constraints: new { controller = "User" }
      );

      config.Routes.MapHttpRoute(
        name: "CourtLockingApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional },
        constraints: new { controller = "CourtLocking" }
      );

      config.Routes.MapHttpRoute(
        name: "StandbyApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: null,
        constraints: new { controller = "Standby" }
      );
    }

    private class LowercaseFirstContractResolver : DefaultContractResolver
    {
      protected override string ResolvePropertyName(string propertyName)
      {
        return String.Concat(Char.ToLower(propertyName[0]), propertyName.Substring(1));
      }
    }
  }
}