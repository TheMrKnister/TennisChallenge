"use strict"
$(function () {
  function UpdateList(data, textStatus, jqXHR) {
    userList.children("li").remove()
    data.sort(function (a, b) {
      var nameA = a.fullName.toUpperCase();
      var nameB = b.fullName.toUpperCase();
      return (nameA < nameB) ? -1 : (nameA > nameB) ? 1 : 0;
    });
    $.each(data, function (_, member) {
      userList.append(
        $("<li></li>")
        .text(member.fullName)
        .addClass("user-list-entry has-card-" + member.rfid)
        .data("user-id", member.id)
        .attr("data-gender", member.gender)
        .click(SelectUser)
      )
    })
    //Entfernt die Member die nicht im spiel sind wieder aus der Liste
    $('#ranked-members > li').each(function (ind, val) {
      var $this = $(this);
      var name = $this.text();
      if ($.inArray(name, rankedMember) == -1) {
        $this.remove();
      }
    });

    $('#ranked-members > li').each(function (ind, val) {
      var name = $(this).text();
      var inArraPos = $.inArray(name, rankedMember);
      $(this).attr("data-player-rank", rankes[inArraPos]);
      $(this).text(rankes[inArraPos] + ". " + name);
    });

    var elems = $('#ranked-members').children('li');
    elems.sort(function (a, b) {
      var rankA = parseInt($(a).attr("data-player-rank"));
      var rankB = parseInt($(b).attr("data-player-rank"));
      return rankA > rankB;
    });
    $('#ranked-members').append(elems);
  }

  //ändert den clubrank auf das eingegebene im rank-value 
  $('#club-rank').keyup(function () {
    $('#add-player').show();
    var link = $("#add-player");
    var oldUrl = link.prop("href")
    var clubrank = parseInt($('#rank-value').val());
    var ind = oldUrl.indexOf("=");
    if (oldUrl.length >= ind) {
      var i = oldUrl.indexOf("=");
      var slice = (oldUrl.length - i) - 1;
      var newUrl = oldUrl.slice(0, -slice);
    } else {
      newUrl = oldUrl;
    }
    var oldUrl = link.prop("href", "")
    newUrl += clubrank;
    link.prop("href", newUrl)
    if (link.prop("href").length < ind || clubrank < 1 || isNaN(clubrank)) {
      $('#add-player').hide();
    }
  });


  $('#delete-user').click(function () {
    return confirm('Wollen Sie diesen Benutzer wirklich löschen?');
  });

  $('#delete-cards').click(function () {
    return confirm('Wollen sie wirklich alle verbundene Karten wieder freigeben?')
  });

  $("#filter-women").click(function () {
    filterGender(2);
  });

  $("#filter-men").click(function () {
    filterGender(1);
  });

  $("#filter-all").click(function () {
    filterGender(0);
  });

  //gender = 1: women only, gender = 2: men only, gender = 0: all
  function filterGender(gender) {
    $('#ranked-members li[data-gender]').show();
    if (gender != 0) {
      $('#ranked-members li[data-gender=' + gender + ']').hide();
    }
  }

  function updateLinks(userId, link) {
    var link = $(link)
    link.show()
    var oldUrl = link.prop("href")
    var newUrl = oldUrl.replace(/[^\/]+$/, userId)

    if (link.selector == "#add-player") {
      var indexOfQm = oldUrl.indexOf('?');
      if (indexOfQm > 0) {
        var oldUrlPart = oldUrl.substr(indexOfQm);
        newUrl = newUrl + oldUrlPart;
      }
    }
    link.prop("href", newUrl)
  }

  function UpdateForm(data, textStatus, jqXHR) {
    $("input[name='UsersInClubsKey']").val(data.usersInClubsKey)
    var checkboxes = $("input[name='Roles']")
    checkboxes.prop("checked", false)
    checkboxes.each(function (_, elem) {
      var val = $(elem).val()
      for (var i in data.roles) {
        if (val == data.roles[i]) {
          $(elem).prop("checked", true)
          return
        }
      }
    })
  }

  function SelectUser(event) {
    var userName = $(this).text();
    $('.selected-user').text(userName);
    var userId = $(this).data("user-id")
    if ($('#greyback').length != 0) {
      updateLinks(userId, "#reset-password")
      updateLinks(userId, "#delete-user")
      updateLinks(userId, "#delete-cards")
    } else {
      var parentId = $(this).parent().attr("id");
      updateLinks(userId, "#add-player")
      updateLinks(userId, "#delete-ranked-player")

      if (parentId == "ranked-members") {
        $('#edit-ranked-user').hide();
      }
      else {
        $('#edit-ranked-user').show()
        $('#add-player').hide();
        $('#delete-ranked-player').hide();
      }
    }

    $.ajax({ url: tc.UserAPI + "/" + userId, success: UpdateForm })
  }

  if (tc == null || tc.UserAPI == null) {
    return
  }

  $.ajaxSetup({ url: tc.UserAPI, type: 'GET', cache: false})

  var userList = $("#user-list > ul")
  var userInput = $("input[name='user-name-input']")
  var rankedMember = new Array();
  var rankes = new Array();

  $.getJSON(tc.GetRankedPlayers, function () {    
  }).success(function (data) {
    $.each(data, function (i, val) {
      rankedMember.push(val.FullName);
      rankes.push(val.Rank)
    });
  });

  userInput.keyup(function () {
    var val = $(this).val()
    $.ajax({
      data: val != "" ? { filter: val } : {},
      success: UpdateList
    })
  })
  userInput.keyup()
})