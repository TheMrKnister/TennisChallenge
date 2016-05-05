$(function () {
  $("#new-newsfeed").click(function () {
    var url = $(this).data("new-newsfeed-url")
    $.ajax({
      url: url,
      cache: false,
      success: function (html) {
        var trimmedHtml = html.trim()
        var parsedHtml = $(trimmedHtml)
        $("#newsfeeds").append(parsedHtml)
      }
    })
  })

  $(document).on("click", ".delete-newsfeed", function () {
    var newsFeedId = $(this).data("newsfeed-id")
    var fieldset = $("fieldset[data-newsfeed-id=" + newsFeedId + "]")

    fieldset.find(".delete").val(true)

    fieldset.hide()
  })
})