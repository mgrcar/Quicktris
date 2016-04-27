﻿/*==========================================================================;
 *
 *  File:    firsttetris.js
 *  Desc:    Look & feel of the first tetris ever (requires JSTe3s)
 *  Created: Apr-2016
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

var loaders = [];
var images = {};
var sounds = {};
var ctx;

var imgInfo = [
    ["BG", "img/first_background.png"],
    ["DOT", "img/first_dot.png"],
    ["BLOCK", "img/first_block.png"]
];

var sndInfo = [
    ["BGNOISE1", "snd/bgnoise"],
    ["BGNOISE2", "snd/bgnoise"],
    ["DROPDOWN1", "snd/dropdown1"],
    ["DROPUP1", "snd/dropup1"],
    ["DROPDOWN2", "snd/dropdown2"],
    ["DROPUP2", "snd/dropup2"],
    ["DROPDOWN3", "snd/dropdown3"],
    ["DROPUP3", "snd/dropup3"],
    ["DROPDOWN4", "snd/dropdown4"],
    ["DROPUP4", "snd/dropup4"],
    ["DROPDOWN5", "snd/dropdown5"],
    ["DROPUP5", "snd/dropup5"],
    ["DROPDOWN6", "snd/dropdown6"],
    ["DROPUP6", "snd/dropup6"]
];

var cmdQueue = [];
var keyBuffer = [];

var sndFx = false;

// Renderer

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
        var img = type != 0 ? "BLOCK" : "DOT";
        drawImage(img, col * 2 + 27, row + 1);
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
}

function renderer_RenderPause() {
}

function renderer_ClearPause() {
}

function renderer_RenderScore() {
    writeNumber(3, 11, JSTe3s.Program.mScore);    
}

function renderer_RenderNextBlock() {
    var shape = JSTe3s.Block.mNextBlock.mShape[0];
    var color = JSTe3s.Block.mNextBlock.mType;
    for (var blockY = 0; blockY < 4; blockY++) {
        for (var blockX = 0; blockX < 4; blockX++) {
            if (shape[blockY][blockX] == "1") {
                drawImage("B" + color, blockX + 3, blockY + 20);
            } else {
                drawImage("B8", blockX + 3, blockY + 20);
            }
        }
    }
}

function renderer_ClearNextBlock() {
    for (var blockY = 0; blockY < 4; blockY++) {
        for (var blockX = 0; blockX < 4; blockX++) {
            drawImage("B8", blockX + 3, blockY + 20);
        }
    }
}

function renderer_RenderFullLines() {
    writeNumber(1, 15, JSTe3s.Program.mFullLines);
}

function renderer_RenderLevel() {
    writeNumber(2, 15, JSTe3s.Program.mLevel);
}

function renderer_RenderStats() {
}

// Keyboard

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
        case 40:
            return JSTe3s.Key.drop;
        case 82:
            return JSTe3s.Key.restart;
        case 80:
        case 19:
            return JSTe3s.Key.pause;
        case 97:
        case 49:
            return JSTe3s.Key.showNext;
        case 102:
        case 54:
            return JSTe3s.Key.speedUp;
    }
    return JSTe3s.Key.other;
}

// Sound

function sound_Play(name) {
}

// Utils

function drawImage(imgName, x, y) {
    cmdQueue.push(function () {
        ctx.drawImage(getImage(imgName), x * 8, y * 16);
    });
}

function writeNumber(row, col, num) {
    if (num == 0) {
        drawImage("0", col, row);
    } else {
        while (num > 0) {
            var digit = num % 10;
            num = Math.floor(num / 10);
            drawImage(digit, col, row);
            col--;
        }
    }
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

function loadSound(name, file) {
    var deferred = $.Deferred();
    sounds[name] = new Howl({
        src: [file + ".ogg", file + ".mp3", file + ".wav"],
        onload: function () { deferred.resolve(); },
        onend: function() { sndFx = false; } 
    });
    return deferred.promise();
}

function getImage(name) {
    if (images[name]) { return images[name]; }
    return null;
}

// Main

function gameLoop() {
    JSTe3s.Program.play();
    if (cmdQueue.length == 0 && !sndFx) {
        setTimeout(gameLoop, 0);
    } else {
        setTimeout(animLoop, 0);
    }
}

function animLoop() {
    while (cmdQueue.length > 0 && !sndFx) {
        cmdQueue[0]();
        cmdQueue.shift();
    }
    if (cmdQueue.length > 0 || sndFx) {
        setTimeout(animLoop, 0);
    } else {
        setTimeout(gameLoop, 0);
    }
}

function other(track) {
    return track == 1 ? 2 : 1;
}

function bgNoiseCrossFade(vol, track) {
    vol += 0.1;
    var angle = vol * (Math.PI / 2);
    sounds["BGNOISE" + other(track)].volume(Math.sin(angle));
    sounds["BGNOISE" + track].volume(Math.cos(angle));
    if (vol < 1) {
        setTimeout("bgNoiseCrossFade(" + vol + ", " + track + ")", 100);
    } 
}

function playBgNoise(track, vol) {
    sounds["BGNOISE" + track].volume(vol).play();
    setTimeout("playBgNoise(" + other(track) + ", 0)", 5000);
    setTimeout("bgNoiseCrossFade(0, " + track + ")", 6000);   
}

var keyStates = [];

$(function () { // wait for document to load
    // keyboard handler
    $(document).on("keydown", function (e) {
        if ($.inArray(e.which, [37, 103, 55, 39, 105, 57, 38, 104, 56, 32, 100, 52, 40, 82, 97, 49, 102, 54, 80, 19]) >= 0) {
            if (!keyStates[e.which]) {
                sounds["DROPDOWN" + Math.floor((Math.random() * 6) + 1)].play();
                keyStates[e.which] = 1;
            }
            keyBuffer.push(e.which);
            e.preventDefault();
        }
    });
     $(document).on("keyup", function (e) {
        if (keyStates[e.which]) {
            sounds["DROPUP" + Math.floor((Math.random() * 6) + 1)].play();
            delete keyStates[e.which];
        }
     });
    // initialize loaders
    // images
    for (var i = 0; i < imgInfo.length; i++) {
        loaders.push(loadImage(imgInfo[i][0], imgInfo[i][1]));
    }
    for (var i = 0; i <= 9; i++) {
        loaders.push(loadImage(i, "img/first_" + i + ".png"));
    }
    // sounds
    for (var i = 0; i < sndInfo.length; i++) {
        loaders.push(loadSound(sndInfo[i][0], sndInfo[i][1]));
    }
    // warn on unload
    $(window).on("beforeunload", function () {
        return "If you navigate away, your current game progress will be lost.";
    });
    // pause on blur
    $(window).blur(function () {
        if (JSTe3s.Program.mState != JSTe3s.State.pause) {
            keyBuffer.push(80); // push pause key
        }
    });
    // run main loop
    $.when.apply(null, loaders).done(function () { // wait for images and sounds to load
        // background sound loop
        playBgNoise(2, 1);
        ctx = $("#screen")[0].getContext("2d");
        JSTe3s.Program.init();
        setTimeout(animLoop, 0);
    });
});