using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using TennisWeb.Models;
using TennisWeb.Services;
using TennisWeb.Services.Member;

namespace TennisWeb.Controllers
{
  public class FileUploadController : Controller
  {

    public ActionResult Uploader()
    {
      return View();
    }

    [System.Web.Http.HttpPost]
    public ActionResult UploadFile(Guid? clubFk)
    {
      //var memberModels = new List<MemberModel>();

      try
      {
        var memberParser = new CsvMemberParser();

        //Create File limitation on client
        using (var stream = this.Request.Files[0]?.InputStream)
        {
          if (stream == null)
            throw new Exception("No file has been selected");

          var memoryStream = new MemoryStream();
          stream.CopyTo(memoryStream);
          memoryStream.Position = 0;
          var csvReader = new CsvFileReadService();
          csvReader.Open(memoryStream);

          //Creates a mapping definition based on the Column headers
          var mapping = MemberService.CreateMappingBasedOnColumnHeader(csvReader.Current);


          while (csvReader.MoveNext())
          {
            memberParser.CreateFromCurrentReadLine(csvReader.Current, mapping, clubFk);
          }
          memoryStream.Position = 0;
          memoryStream.Close();
        }
        return RedirectToAction("Uploader");
      }
      catch (Exception e)
      {
        //Write message to the log about failed import
        //

        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.StatusCode = HttpStatusCode.NotAcceptable;
        httpResponseMessage.Content = new StringContent("The message was received successfully but an internal server" +
                                                        "error occured. Exception:" + e.ToString());
        throw new HttpResponseException(httpResponseMessage);
      }
    }
  }
}
