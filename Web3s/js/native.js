﻿// Renderer

function renderer_Init() {
    drawImage("BG", 0, 0);
}

function renderer_RenderPlayfield() {
    for (var row = 0; row < 20; row++) {
        renderer_RenderRow(row);
    } 
}

function renderer_RenderRow(row) {
    for (var col = 0; col < 10; col++) {
        var type = JSTe3s.Playfield.mGrid[row][col];
        var img = type != 0 ? ("B" + type) : (col % 2 != 0 ? "DOT" : "B8");
        drawImage(img, col + 15, row + 1);
    }
}

function renderer_RenderBlock() {
    for (var row = JSTe3s.Block.mBlock.mPosY - 1; row < JSTe3s.Block.mBlock.mPosY + 4; row++) {
        if (row >= 0 && row < 20) {
            renderer_RenderRow(row);
        }
    }
}

function renderer_RenderGameOver() {
    drawImage("GAMEOVER", 14, 10);
}

function renderer_RenderPause() {
    drawImage("PAUSED", 14, 10);
}

function renderer_ClearPause() {
    for (var i = 9; i < 12; i++) {
        renderer_RenderRow(i);
        drawImage("STAR", 14, i + 1);
        drawImage("STAR", 25, i + 1);
    }
}

function renderer_RenderScore() {
}

function renderer_RenderNextBlock() {
}

function renderer_ClearNextBlock() {
}

function renderer_RenderFullLines() {
}

function renderer_RenderLevel() {
}

function renderer_RenderStats() {
}

// Keyboard

var keyBuffer = [];

$(document).bind("keydown", function (e) {
    keyBuffer.push(e.which);
    //console.log(e.which);
});

function keyboard_GetKey() {
    if (keyBuffer.length == 0) { return JSTe3s.Key.none; }
    var keyCode = keyBuffer.shift();
    switch (keyCode) {
        case 37:
        case 103:
        case 55:
            return JSTe3s.Key.left;
        case 39:
        case 105:
        case 57:
            return JSTe3s.Key.right; 
        case 38:
        case 104:
        case 56:
            return JSTe3s.Key.rotate;        
        case 32:
        case 100:
        case 52:
            return JSTe3s.Key.drop;
        case 40:
            return JSTe3s.Key.down; // *** not working (?)
        case 82:
            return JSTe3s.Key.restart;
        case 112:
            return JSTe3s.Key.pause;
        case 97:
        case 49:
            return JSTe3s.Key.showNext;
        case 102:
        case 54:
            return JSTe3s.Key.speedUp;
    }
    return JSTe3s.Key.none;
}

// Utils

function drawImage(imgName, x, y) {
    ctx.drawImage(getImage(imgName), x * 16, y * 16);
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

function getImage(name) {
    if (images[name]) { return images[name]; }
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
var colors = ["#ffff55", "#aa00aa", "#aa0000", "#aa5500", "#00aa00", "#00aaaa", "#0000aa", "#aaaaaa", "#000000"];

var imgInfo = [
    ["BG", "img/background.png"],
    ["DOT", "img/dot.png"],
    ["GAMEOVER", "img/gameover.png"],
    ["PAUSED", "img/paused.png"],
    ["STAR", "img/star.png"]
];
for (var i = 0; i < imgInfo.length; i++) {
    loaders.push(loadImage(imgInfo[i][0], imgInfo[i][1]));
}
for (var i = 0; i <= 9; i++) {
    loaders.push(loadImage(i, "img/" + i + ".png"));
}

$(function () { // wait for document to load
    $.when.apply(null, loaders).done(function () { // wait for all images to load
        ctx = $("#screen")[0].getContext("2d");
        JSTe3s.Program.init();
        var animFrame = window.requestAnimationFrame ||
            window.webkitRequestAnimationFrame ||
            window.mozRequestAnimationFrame ||
            window.oRequestAnimationFrame ||
            window.msRequestAnimationFrame ||
            null;
        if (animFrame) {
            var recursivePlay = function () {
                JSTe3s.Program.play();
                animFrame(recursivePlay);
            };
            animFrame(recursivePlay);
        } else {
            setInterval(JSTe3s.Program.play, 0);
        }
    });
});