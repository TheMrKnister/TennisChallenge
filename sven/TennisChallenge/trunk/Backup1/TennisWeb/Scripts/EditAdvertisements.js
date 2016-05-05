"use strict"
$(function () {
  //  function generateList() {
  //  }
  //  function registerEvents(object) {
  //    $(object)
  //    .click(function (event) {
  //      var t = $(this)
  //      var advertisementId = t.data("advertisement-id")
  //      selectAdvertisement(advertisementId)
  //    })
  //    .each(function (index, element) {
  //      var e = $(element)
  //      var advertisementId = e.data("advertisement-id")
  //      e.find(".delete-entry").click(function (event) {
  //        deleteAdvertisement(advertisementId)
  //      })
  //    })
  //  }

  //  function selectAdvertisement(advertisementId) {
  //    $(".advertisement").hide()
  //      .filter("[data-advertisement-id=" + advertisementId + "]").show()
  //    $(".advertisement-entry").removeClass("selected")
  //      .filter("[data-advertisement-id=" + advertisementId + "]").addClass("selected")
  //  }

  //  function deleteAdvertisement(advertisementId) {
  //    $("[data-advertisement-id=" + advertisementId + "]").remove()
  //  }

  //  registerEvents(".advertisement-entry")

  $("#new-advertisement").click(function (event) {
    $.ajax({
      url: $(this).data("new-advertisement-url"),
      cache: false,
      success: function (html) {
        var trimmedHtml = html.trim()
        var parsedHtml = $(trimmedHtml)
        $("#advertisements").append(parsedHtml)
        //registerEvents(parsedHtml)
      }
    })
  })

  $(document).on("click", ".delete-advertisment", function () {
    var advertisementId = $(this).data("advertisement-id")

    var advertisementDiv = $("div.advertisement[data-advertisement-id=" + advertisementId + "]")

    advertisementDiv.find(".delete").val(true)

    advertisementDiv.hide()
  })

  //  var advertisements = $(".advertisement")
  //  var first = advertisements.first()
  //  advertisements.hide()
  //  first.show()
  //  var advertisementId = first.data("advertisement-id")
  //  $(".advertisement-entry[data-advertisement-id=" + advertisementId + "]").addClass("selected")
})