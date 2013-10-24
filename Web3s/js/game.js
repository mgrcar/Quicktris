// Rendering

function x(x) {
    return x * 16;
}

function loadImage(name, src) {
    var deferred = $.Deferred();
    var img = new Image();
    images[name] = img;
    img.onload = function () {
        deferred.resolve();
    };
    img.src = src;
    return deferred.promise();
}

function getTile(name) {
    if (name[0] == "B") {
        var color = parseInt(name[1]);
        tmpCtx.fillStyle = colors[color];
        tmpCtx.fillRect(0, 0, 16, 16);
        return tmpCanvas;
    } else if (name[0] >= "0" && name[0] <= "9") {
        var color = parseInt(name[1]);
        tmpCtx.fillStyle = colors[color];
        tmpCtx.fillRect(0, 0, 16, 16);
        tmpCtx.drawImage(images[name[0]], 0, 0);
        return tmpCanvas;
    }
    return null;
}

// Main

var loaders = [];
var images = {};
var ctx;
var tmpCanvas = $("<canvas width=\"16\" height=\"16\"></canvas>")[0];
var tmpCtx = tmpCanvas.getContext("2d");
var colors = ["#ffff55", "#aa00aa", "#aa0000", "#aa5500", "#00aa00", "#00aaaa", "#0000aa", "#aaaaaa"];

loaders.push(loadImage("BG", "img/background.png"));
for (var i = 0; i <= 9; i++) {
    loaders.push(loadImage(i, "img/" + i + ".png"));
}

$(function () { // wait for document to load
    $.when.apply(null, loaders).done(function () { // wait for all images to load
        ctx = $("#screen")[0].getContext("2d");
        ctx.drawImage(images["BG"], 0, 0);
        ctx.drawImage(getTile("B1"), x(5), x(5));
    });
});

