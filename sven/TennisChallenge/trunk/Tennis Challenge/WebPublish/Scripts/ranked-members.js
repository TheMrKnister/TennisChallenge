$(function () {  
  function updateList(members) {
    $.each(members, function (_, member) {
      $('#rankning-list').append(
        $("<li></li>")
        .text(member.Rank + ". " + member.FullName)
        //.data(member.Id)
        );
    });
  }  
  
  var gender = $('#ranking-list-wrap').attr("data-gender")
  var user = $('#ranking-list-wrap').attr("data-user")

  $.ajaxSetup({
    cache: false
  })

  $.getJSON(GetMembers, { gender: gender, userName: user }, function () {    
  }).success(function (data) {
    var members = data;
    updateList(members);
  });

  $('#back-ranked').click(function () {
    parent.history.back();
    return false;
  });
});