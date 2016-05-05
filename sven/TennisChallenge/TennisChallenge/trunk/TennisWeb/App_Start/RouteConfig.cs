using System.Web.Mvc;
using System.Web.Routing;

namespace TennisWeb
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      
      routes.MapRoute("cache.manifest", "cache.manifest", new { controller = "Resources", action = "Manifest" });
      routes.MapRoute(
          "Default", // Route name
          "{controller}/{action}/{id}", // URL with parameters
          new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
      );
    }
  }
}