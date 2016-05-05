using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using TennisChallenge.Dal;
using TennisWeb.Utils;

using IOFile = System.IO.File;

namespace TennisWeb.Controllers
{
  public class PictureCleanupController : Controller
  {
    //
    // GET: /PictureCleanup/

    /// <summary>
    /// Deletes files that are not profile pictures and
    /// haven't been used for 24 hours
    /// </summary>
    public ActionResult Index()
    {
      var timestampLimit = DateTime.Today.AddDays(-1);
      var picturePath = Server.MapPath(Picture.UploadImagePath);
      var profilePictures = new MemberAccessor()
        .GetAllWhere(m => !String.IsNullOrWhiteSpace(m.PictureUrl))
        .Select(m => picturePath + m.PictureUrl);

      var allPictures = new DirectoryInfo(picturePath)
      .GetFiles();

      var toDelete = allPictures
        .Where(fi => fi.LastAccessTimeUtc < timestampLimit)
        .Select(fi => fi.FullName)
        .Except(profilePictures)
        .ToList();

      toDelete.ForEach(f => IOFile.Delete(f));

      return RedirectToAction("Index", "Home");
    }
  }
}
