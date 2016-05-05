TennisChallenge.Utils = (function ($) {
    var states = {
        DEFAULT: 0,
        LOGIN: 1,
        LOGIN_ERROR: 2,
        ASSIGN_RFID: 3,
        ASSIGN_ERROR: 4,
        BOOKING: 5,
        BOOKING_ERROR: 6,
        ALREADY_BOOKED: 7,
        OPEN_BOOKING_ERROR: 8,
        //IPA Relevant Begin added states 11, 12
        CHALLENGE_PLAYER: 9,
        RANKED_VIEW: 10,
        TOURNAMENT_ADMIN: 11,
        ALREADY_TOURNAMENT: 12,
        HIDE_LOGIN: 13
        //IPA relevant End
    };

    var aspNetJsonToDate = function (dateString) {
        var date = new Date(parseInt(dateString.replace(/\/Date\(([-\d]+)\)\//gi, "$1"), 10));
        return date;
    },

    toHhMmString = function (date) {
        var minutes = (date.getMinutes() < 10) ? "0" + date.getMinutes() : "" + date.getMinutes(),
            timeString = date.getHours() + ':' + minutes + " Uhr";
        return timeString;
    };

    return {
        aspNetJsonToDate: aspNetJsonToDate,
        toHhMmString: toHhMmString
    };
}(jQuery));