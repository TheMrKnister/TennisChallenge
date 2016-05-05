using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TennisChallenge.Dal;

namespace TennisWeb.Models
{
  public class CourtVm
  {
    public string Name { get; set; }
    public int Id { get; set; }
    public Guid ClubFk { get; set; }
    public double PosY { get; set; }
    public double PosX { get; set; }
    public string Allignment { get; set; }
    public Guid CourtKey { get; set; }

    public CourtVm From(Court entity)
    {
      var vm = new CourtVm();
      vm.Allignment = entity.Allignment;
      vm.ClubFk = entity.ClubFk;
      vm.Id = entity.Id;
      vm.Name = entity.Name;
      vm.PosX = entity.PosX ?? 10;
      vm.PosY = entity.PosY ?? 10;
      vm.CourtKey = entity.CourtKey;

      return vm;
    }
  }
}