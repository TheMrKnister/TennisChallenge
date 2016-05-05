"use strict"
$(document).ready(function () {
  var xInput = $("input[name='crop_x']")
  var yInput = $("input[name='crop_y']")
  var wInput = $("input[name='crop_w']")
  var hInput = $("input[name='crop_h']")
  var pictureSpan = $("#pictureLink")
  var pictureFileInput = $("#pictureInput")
  var pictureUrlHidden = $("#PictureUrl")
  var profilePictureImg = $("#profilePicture")
  var jcropApi = null

  pictureFileInput.hide()
  $("#newPicture").click(function () {
    $("#pictureInput").show()
    if (jcropApi != null) {
      jcropApi.destroy()
      jcropApi = null
    }
    pictureUrlHidden.val("")
    pictureSpan.hide()
  })

  pictureFileInput.fileupload({
    dataType: 'json',
    url: "../Member/EditPicture",
    done: function (_, data) {
      pictureSpan.show()
      pictureUrlHidden.val(data.result.fileName)
      // reset the element styles that jcrop gives to the img tag
      profilePictureImg
      .css("display", "")
      .css("visibility", "")
      .css("width", "")
      .css("height", "")
      .attr("src", data.result.pictureUrl)
      .attr("alt", data.result.pictureUrl)
      // temporary img tag to calculate true size of the image
      // not the cleanest way to do things
      $("<img />")
      .attr("src", profilePictureImg.attr("src"))
      // get the true image size and activate jcrop as soon as
      // the temporary img tag loads
      .load(function () {
        var trueWidth = this.width
        var trueHeight = this.height
        profilePictureImg.Jcrop({
          aspectRatio: 1,
          boxWidth: 160,
          boxHeight: 160,
          trueSize: [trueWidth, trueHeight],
          onChange: function (c) {
            xInput.val(c.x)
            yInput.val(c.y)
            wInput.val(c.w)
            hInput.val(c.h)
          }
        },
        function () {
          jcropApi = this
          jcropApi.setSelect([0, 0, trueWidth, trueHeight])
        })
      })

      $("#pictureInput").hide()
    }
  })
});