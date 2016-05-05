using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TennisWeb
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }

    protected void Application_BeginRequest()
    {
#if DEBUG
#else
      switch (Request.Url.Scheme)
      {
        case "https":
        Response.AddHeader("Strict-Transport-Security", "max-age=300");
        break;
        case "http":
        var path = "https://" + Request.Url.Host + Request.Url.PathAndQuery;
        Response.Status = "301 Moved Permanently";
        Response.AddHeader("Location", path);
        break;
      }
#endif
      CheckForContentPath();
    }

    private void CheckForContentPath()
    {
      const string contentKeyword = "_CONTENT_PATH";
      var requestUrl = Request.Url.ToString();
      if (!Regex.IsMatch(requestUrl, contentKeyword))
      {
        return;
      }

      const string relativeContentPath = "~/Content/";
      var relativePath = Regex.Replace(requestUrl, ".*" + contentKeyword, relativeContentPath);
      var redirectLocation = VirtualPathUtility.ToAbsolute(relativePath);
      Response.RedirectPermanent(redirectLocation);
    }
  }
}