$(function () {

  var type = $('#ad-text-input').attr('data-id');
  var textPart = "";
  var text = "";
  var title = ""
  var price = ""

  //shows all user ads
  function displayAds(ads) {
    var cssLeft = 0;
    var cssTop = 0;
    $.each(ads, function (ind, ad) {

      var adtext = ad.AdText;

      //calculates the vualues for dynamic positioning
      if (ind % 3 != 0) {
        cssLeft += 300;
        cssTop = -263;
      } else {
        cssLeft = 25;
        cssTop = 20;
      }

      adtext = adtext.replace(/°n°/g, "<br>");
      adtext = adtext.replace(/°¦°/g, "<b>");
      adtext = adtext.replace(/°§°/g, "</b>");
      adtext = adtext.replace(/°b0°/g, "<div style='text-align: right; margin-right: 5%;'>");
      adtext = adtext.replace(/°b1°/g, "</div>");


      $('#ads-wrapper').append(
        $('<div class="ad" style="margin-left: ' + cssLeft + 'px; margin-top:' + cssTop + 'px"><div class="spacer"><br></div><br /><div class="ad-text">' + adtext + '</div><br><div class="owner"><a href="mailto:' + ad.UserName + '">' + ad.FullName + '</a></div></div>')
        .data("data-owner", ad.UserName)
        .data("ad-id", ad.AdId)
        );
    });
  }

  //Gives the ad whose owner is loged in a delete class
  function setClose(userName) {
    var isAdmin = $("#create-ad").attr("data-isadmin");
    $('.ad').each(function (ind, ad) {
      var $this = $(this);
      if ($this.data("data-owner").valueOf() == userName || isAdmin == "True") {
        $($this.children('.spacer')).remove();
        $($this).prepend('<div class="delete"></div>')
      }
    });

    $('.ad .delete').click(function () {
      var id = $(this).parent().data("ad-id").valueOf();
      $.post(DeleteAd, { id: id }, function (data) { });
      location.reload();
    });
  }

  //replaces ad add link
  //function updateLinks(text, link) {
  //  var link = $(link)
  //  var oldUrl = link.prop("href")

  //  var indexOfQm = oldUrl.indexOf('&text=');
  //  var indexOfEnd = oldUrl.indexOf('&type=');
  //  if (indexOfQm > 0) {
  //    var oldUrlStart = oldUrl.substr(0, indexOfQm + 6);
  //    var oldUrlEnd = oldUrl.substr(indexOfEnd);
  //    var newUrl = oldUrlStart + text + oldUrlEnd;
  //  }

  //  link.prop("href", newUrl)
  //}

  $.getJSON(GetAds, { type: type }, function () {
  }).success(function (data) {
    displayAds(data);
    setClose($('#create-ad').attr('data-user'))
  });

  $('#create-ad-button').click(function () {
    var user = $('#create-ad').attr('data-user')

    title = $('#ad-title-input').val();
    if (title != "") {
      title = "°¦°" + title + "°§°";
      title += "°n°°n°";
    }


    price = "°n°°n° °b0°" + $('#ad-price-input').val() + "°b1°";

    if (type == 2) {
      price = "";
    }

    text = title + text + price;

    $.post(CreateAd, { userName: user, text: text, type: type });
    location.reload();
  });

  $('#open-add-ad').click(function () {
    if ($('#create-ad').hasClass('hidden')) {
      $('#create-ad').removeClass('hidden');
      $('#ads-wrapper').removeClass('collapsed');
      $('#open-add-ad input').val('-');
    } else {
      $('#create-ad').addClass('hidden');
      $('#ads-wrapper').addClass('collapsed');
      $('#open-add-ad input').val('+');
    }
  });

  $('#ad-text-input').keyup(function (e) {
    text = $('#ad-text-input').val();
    text = text.replace(/\n/g, "°n°");
  });
});