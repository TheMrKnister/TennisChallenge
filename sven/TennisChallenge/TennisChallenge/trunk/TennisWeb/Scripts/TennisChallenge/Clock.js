
TennisChallenge.InfoBoard.Clock = (function ($) {
    var hourHand, minuteHand, secondHand, canvas, i,

        updateClock = function () {
            var now = new Date();
            var hours = now.getHours();
            var minutes = now.getMinutes();
            var seconds = now.getSeconds();
            hourHand.rotate(30 * hours + (minutes / 2.5), 200, 200);
            minuteHand.rotate(6 * minutes, 200, 200);
            secondHand.rotate(6 * seconds, 200, 200);
        },
        initClock = function () {
            canvas = Raphael("clock_id", 400, 400);
            var clock = canvas.circle(200, 200, 190);
            clock.attr({ "fill": "#f5f5f5", "stroke": "#444444", "stroke-width": "10" });
            var hourSign;
            for (i = 0; i < 12; i++) {
                var startX = 200 + Math.round(160 * Math.cos(30 * i * Math.PI / 180));
                var startY = 200 + Math.round(160 * Math.sin(30 * i * Math.PI / 180));
                var endX = 200 + Math.round(180 * Math.cos(30 * i * Math.PI / 180));
                var endY = 200 + Math.round(180 * Math.sin(30 * i * Math.PI / 180));
                hourSign = canvas.path("M" + startX + " " + startY + "L" + endX + " " + endY);
                hourSign.attr({ "stroke-width": 2 });
            }
            hourHand = canvas.path("M200 200L200 100");
            hourHand.attr({ stroke: "#444444", "stroke-width": 12 });
            minuteHand = canvas.path("M200 200L200 80");
            minuteHand.attr({ stroke: "#444444", "stroke-width": 8 });
            secondHand = canvas.path("M200 220L200 50");
            secondHand.attr({ stroke: "#444444", "stroke-width": 4 });
            var pin = canvas.circle(200, 200, 10);
            pin.attr("fill", "#000000");
            updateClock();
            setInterval(updateClock, 1000);
        };

    $(function () {
        initClock();
    });

}(jQuery));