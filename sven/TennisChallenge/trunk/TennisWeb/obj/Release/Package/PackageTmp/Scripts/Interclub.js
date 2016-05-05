$(function () {
  $('#interclub-booking #booking-series-links').remove();
  $('#interclub-booking #create-booking-series').remove();

  function updateLinks(courtId) {
    var link = $("#selected-court")

    var oldUrl = link.prop("href")
    var newUrl = oldUrl.replace(/[^\/]+$/, courtId)

    link.prop("href", newUrl)
  }

  var courtId = $('#courtSelect').find(":selected").attr('value')
  updateLinks(courtId)

  $('#courtSelect').change(function () {
    courtId = $('#courtSelect').find(":selected").attr('value')
    updateLinks(courtId)
  });
})