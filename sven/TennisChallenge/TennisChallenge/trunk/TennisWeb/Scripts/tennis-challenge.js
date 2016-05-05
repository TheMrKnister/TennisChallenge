"use strict";

if (window.JSON && !window.JSON.dateParser) {
    var reISO = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*))(?:Z|(\+|-)([\d|:]*))?$/;
    var reMsAjax = /^\/Date\((d|-|.*)\)[\/|\\]$/;

    JSON.dateParser = function (key, value) {
        if (typeof value === 'string') {
            var a = reISO.exec(value);
            if (a)
                return new Date(value);
            a = reMsAjax.exec(value);
            if (a) {
                var b = a[1].split(/[-+,.]/);
                return new Date(b[0] ? +b[0] : 0 - +b[1]);
            }
        }
        return value;
    };

}

var TennisChallenge = TennisChallenge || {};

TennisChallenge.Constants = TennisChallenge.Constants || {};

TennisChallenge.Utils = TennisChallenge.Utils || {};

$.ajaxSetup({ cache: false });

TennisChallenge.InfoBoard = TennisChallenge.InfoBoard || {};

TennisChallenge.Clock = TennisChallenge.InfoBoard.Clock || {};

TennisChallenge.Error = TennisChallenge.InfoBoard.Error || {};

TennisChallenge.Model = TennisChallenge.InfoBoard.Model || {};

TennisChallenge.UI = TennisChallenge.InfoBoard.UI || {};