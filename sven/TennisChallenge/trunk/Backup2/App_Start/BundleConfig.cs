using System.Web;
using System.Web.Optimization;

namespace TennisWeb
{
  public class BundleConfig
  {
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
        "~/Scripts/modernizr-{version}.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
        "~/Scripts/jquery-{version}.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
        "~/Scripts/jquery-ui-{version}.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
        "~/Scripts/jquery.unobtrusive*",
        "~/Scripts/jquery.validate*",
        "~/Scripts/localization/methods_de.js",
        "~/Scripts/localization/messages_de.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/pie").Include(
        "~/Scripts/PIE.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/underscore").Include(
        "~/Scripts/underscore.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/useredit").Include(
        "~/Scripts/jquery.ui.datepicker-de-CH.js",
        "~/Scripts/jquery.fileupload.js",
        "~/Scripts/jquery.iframe-transport.js",
        "~/Scripts/jquery.Jcrop.js",
        "~/Scripts/RangeDateValidator.js",
        "~/Scripts/PictureEdit.js",
        "~/Scripts/EditorHookup.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/captcha").Include(
        "~/Scripts/captcha.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/infotable").Include(
        "~/Scripts/fullcalendar.js",
        "~/Scripts/FullcalendarHookup.js",
        "~/Scripts/TennisChallengeUtils.js",
        "~/Scripts/jquery.ui.datepicker-de-CH.js",
        "~/Scripts/jquery-ui-timepicker-addon.js",
        "~/Scripts/EditorHookup.js",
        "~/Scripts/TennisChallengeUtils.js",
        "~/Scripts/InfoBoard.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/infoboard").Include(
        "~/Scripts/jquery.keyboard.js",
        "~/Scripts/keyboard-custom-layouts.js",
        "~/Scripts/raphael-min.js",
        //IPA Relevant Beginn
        "~/Scripts/tennis-challenge.js",
        "~/Scripts/infoboard-tournament.js",
        "~/Scripts/infoboard-gameplan.js",
        //IPA Relevant End
        "~/Scripts/assign-player.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/fullcalendar").Include(
        "~/Scripts/fullcalendar.js",
        "~/Scripts/FullcalendarHookup.js",
        "~/Scripts/TennisChallengeUtils.js",
        "~/Scripts/jquery.ui.datepicker-de-CH.js",
        "~/Scripts/jquery-ui-timepicker-addon.js",
        "~/Scripts/EditorHookup.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/edituser").Include(
        "~/Scripts/UserList.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/interclub").Include(
        "~/Scripts/Interclub.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/courtlocking").Include(
        "~/Scripts/CourtLocking.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/editadvertisements").Include(
        "~/Scripts/EditAdvertisements.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/editnewsfeeds").Include(
        "~/Scripts/EditNewsFeeds.js"
      ));

      bundles.Add(new ScriptBundle("~/bundle/standby").Include(
        "~/Scripts/jquery-{version}.js",
        "~/Scripts/Standby.js"
      ));

      bundles.Add(new ScriptBundle("~/bundle/rankedmembers").Include(
        "~/Scripts/ranked-members.js"
        ));

      bundles.Add(new ScriptBundle("~/bundle/challengeplayer").Include(
        "~/Scripts/challenge-player.js"
        ));

      bundles.Add(new ScriptBundle("~/bundle/enterresult").Include(
        "~/Scripts/enter-result.js"
        ));

      bundles.Add(new ScriptBundle("~/bundle/rules").Include(
        "~/Scripts/ranked-rules.js"
        ));

      bundles.Add(new ScriptBundle("~/bundle/userads").Include(
        "~/Scripts/user-ads.js",
        "~/Scripts/player-filter.js"
        ));

      bundles.Add(new ScriptBundle("~/bundle/rankedgameslist").Include(
        "~/Scripts/ranked-games-list.js"
        ));

      bundles.Add(new ScriptBundle("~/bundle/rankedgamesadmin").Include(
        "~/Scripts/ranked-game-admin.js"
        ));

      bundles.Add(new ScriptBundle("~/bundle/viewrankedgames").Include(
        "~/Scripts/view-ranked-games.js"
        ));

      //IPA relevant beginn
      bundles.Add(new ScriptBundle("~/bundle/tournamentstart").Include(
        "~/Scripts/tournament-begin.js"        
        ));
      //IPA relevant end

      bundles.Add(new ScriptBundle("~/bundle/clubtournwebsite").Include(
        "~/Scripts/clubtourn-website.js"
        ));

      bundles.Add(new ScriptBundle("~/bundle/challadmin").Include(
        "~/Scripts/challenge-admin.js",
        "~/Scripts/jquery.ui.datepicker-de-CH.js",
        "~/Scripts/jquery-ui-timepicker-addon.js"
        ));

      bundles.Add(new StyleBundle("~/Content/css").Include(
        "~/Content/Site.css"
      ));

      bundles.Add(new StyleBundle("~/Content/themes/base/jqueryui").Include(
        "~/Content/themes/base/jquery.ui.core.css",
        "~/Content/themes/base/jquery.ui.accordion.css",
        "~/Content/themes/base/jquery.ui.autocomplete.css",
        "~/Content/themes/base/jquery.ui.button.css",
        "~/Content/themes/base/jquery.ui.datepicker.css",
        "~/Content/themes/base/jquery.ui.dialog.css",
        "~/Content/themes/base/jquery.ui.menu.css",
        "~/Content/themes/base/jquery.ui.progressbar.css",
        "~/Content/themes/base/jquery.ui.resizable.css",
        "~/Content/themes/base/jquery.ui.selectable.css",
        "~/Content/themes/base/jquery.ui.slider.css",
        "~/Content/themes/base/jquery.ui.spinner.css",
        "~/Content/themes/base/jquery.ui.tabs.css",
        "~/Content/themes/base/jquery.ui.tooltip.css",
        "~/Content/themes/base/jquery.ui.theme.css"
      ));

      bundles.Add(new StyleBundle("~/Content/useredit").Include(
        "~/Content/jquery.Jcrop.css"
      ));

      bundles.Add(new StyleBundle("~/Content/infoboard").Include(        
        "~/Content/InfoBoard.css",
        "~/Content/keyboard.css"
      ));

      bundles.Add(new StyleBundle("~/Content/fullcalendar").Include(
        "~/Content/fullcalendar.css"
      ));
    }
  }
}