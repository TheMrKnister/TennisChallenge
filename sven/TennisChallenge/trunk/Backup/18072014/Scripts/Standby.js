$(function () {
  "use strict"

  var defaultTimeout = 10000
  var standbyContainerSelector = "#standby-container"
  var standbyUrlDataSelector = "standby-url"

  function failure() {
    setTimeout(updateContent, defaultTimeout)
  }

  function retrieveStandbyData(url) {
    var data = null

    $.ajax({
      url: url,
      async: false,
      cache: false,
      success: function (retrievedData) {
        if (retrievedData && retrievedData.duration && (retrievedData.url || (retrievedData.imageUrl && retrievedData.name))) {
          data = retrievedData
        }
      }
    })

    return data
  }

  function updateContent() {
    var standbyContainer = $(standbyContainerSelector)
    var url = standbyContainer.data(standbyUrlDataSelector)

    var data = retrieveStandbyData(url)

    if (data !== null) {
      var newContent = null
      if (data.imageUrl !== null) {
        newContent = $("<img />", { src: data.imageUrl, alt: data.name })
      }
      else if (data.url !== null) {
        newContent = $("<iframe />", { src: data.url })
      }

      var oldContent = standbyContainer.children()
      standbyContainer.append(newContent)

      oldContent.slideUp("slow", function () {
        newContent.slideDown("slow")
        $(this).remove()
      })

      setTimeout(updateContent, data.duration * 1000)
    }
    else {
      failure()
    }
  }

  updateContent()
})