using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Omu.ValueInjecter;
using TennisChallenge.Dal;
using TennisWeb.Models;
using TennisWeb.Utils;

namespace TennisWeb.Controllers
{
  public class TestApiController : Controller
  {
    //
    // GET: /Test/

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult GetCurrentPlayers(int id)
    {
      var bookingModel = new BookingInfoModel();

      var booking = new BookingAccessor().GetAllWhere(b => b.Court.Id == id && b.StartTime < DateTime.Now && b.EndTime > DateTime.Now);

      foreach (var bookingN in booking)
      {
        bookingModel.StartTime = bookingN.StartTime;
        bookingModel.EndTime = bookingN.EndTime;
        bookingModel.Player0Name = bookingN.Member0.FullName;
        if (bookingN.Member2Fk != null)
        {
          bookingModel.Player1Name = bookingN.Member1.FullName;
          if (bookingN.Member2Fk != null || bookingN.Member3Fk != null)
          {
            bookingModel.Player2Name = bookingN.Member2.FullName;
            bookingModel.Player3Name = bookingN.Member3.FullName;
          }
        }
      }

      return Json(bookingModel, JsonRequestBehavior.AllowGet);
    }

  }
}
