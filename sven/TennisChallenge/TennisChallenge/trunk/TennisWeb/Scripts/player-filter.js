$(function () {

  function reDisplayAds(ads) {
    var cssLeft = 0;
    var cssTop = 0;

    $.each(ads, function (ind, ad) {
      if (ind % 3 != 0) {
        cssLeft += 300;
        cssTop = -263;
      } else {
        cssLeft = 25;
        cssTop = 20;
      }
      $(this).css('margin-left', cssLeft);
      $(this).css('margin-top', cssTop);
    });

  }

  $('#ad-filters').change(filterAds);
  $('#age-input-min').keyup(filterAds);
  $('#age-input-max').keyup(filterAds);

  function filterAds() {
    var ranked = $('#ranking-input').prop('checked')
    var ageMin = parseInt($('#age-input-min').val());
    var ageMax = parseInt($('#age-input-max').val());
    var male = $('#male-input').prop('checked')
    var female = $('#female-input').prop('checked')
    var rating = parseInt($('#rating-input').val());

    if (isNaN(ageMin)) {
      ageMin = 1;
    }

    if (isNaN(ageMax)) {
      ageMax = 150;
    }

    $.get(FilterAds, { ranked: ranked, ageMin: ageMin, ageMax: ageMax, male: male, female: female, rating: rating }, function () { }).success(function (data) {;
      var ads = data;
      if (ads.length != 0) {
        $('.ad').each(function (ind, val) {
          var adId = $(this).data('ad-id');
          var test = $.inArray(adId, ads);
          if (($.inArray(adId, ads)) != -1) {
            $(this).show();
          } else {
            $($(this)).hide();
          }
        });
      } else {
        $('.ad').hide();
      }
      reDisplayAds($('.ad:visible'));
    });
  }
});