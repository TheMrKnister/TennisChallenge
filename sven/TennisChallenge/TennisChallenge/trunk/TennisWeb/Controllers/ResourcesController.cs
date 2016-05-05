using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TennisWeb.Utils;

namespace TennisWeb.Controllers
{
    public class ResourcesController : Controller
    {
        //
        // GET: /Resources/

        public ActionResult Manifest()
        {
            //Create a list for the cache resources
            var cacheResources = new List<string>();
            //Get the content URLs for each of the directories that will cache content files
            var contentUrls = GetContentUrlsFromDirectory("Content");
            var scriptUrls = GetContentUrlsFromDirectory("Scripts");

            //Add each of the content URLs to the list of cache resources
            cacheResources.AddRange(contentUrls);
            cacheResources.AddRange(scriptUrls);
            //Add controller actions that can be cached
            cacheResources.Add(Url.Action("Index", "InfoBoard"));
            cacheResources.Add(Url.Action("Standby", "InfoBoard"));

            var manifestResult = new ManifestResult("1.0")
            {
                //Set the CacheResources of the ManifestResult
                CacheResources = cacheResources,
                //Set the NetworkResources of the ManifestResult to *
                //This will default any resource not listed in the cache to always require network connection
                NetworkResources = new [] { "*" }
            };
            return manifestResult;
        }

        /// <summary>
        /// Get the content URLS for the files in a directory
        /// </summary>
        /// <param name="directoryName">The directory name</param>
        /// <returns>The content URLs for all of the files in a directory</returns>
        private IEnumerable<string> GetContentUrlsFromDirectory(string directoryName)
        {
            return Directory.GetFiles(
                Server.MapPath($"~/{directoryName}"), "*.*", SearchOption.AllDirectories)
                .Where(fileName => !fileName.EndsWith(".db"))
                .Select(fileName => Url.Content(
                    $"~/{fileName.Substring(fileName.IndexOf(directoryName, StringComparison.Ordinal))}".Replace(@"\", "/")))
                .ToList();
        }

    }
}
