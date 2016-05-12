/*==========================================================================;
 *
 *  File:    firsttetris.js
 *  Desc:    Look & feel of the first tetris ever (requires JSTe3s)
 *  Created: Apr-2016
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

// TODO:
// [ ] visual fx + cursor

var loaders = [];
var images = {};
var sounds = {};
var ctx;

var imgInfo = [
    ["BG", "img/first_background.png"],
    ["DOT", "img/first_dot.png"],
    ["BLOCK", "img/first_block.png"],
    ["BLANK", "img/first_blank.png"],
    ["GAMEOVER", "img/first_gameover.png"],
    ["PAUSED", "img/first_paused.png"],
    ["RESUME", "img/first_resume.png"]
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
    ["DROPUP6", "snd/dropup6"],
    ["KEYDOWN1", "snd/keydown1"],
    ["KEYUP1", "snd/keyup1"],
    ["KEYDOWN2", "snd/keydown2"],
    ["KEYUP2", "snd/keyup2"],
    ["KEYDOWN3", "snd/keydown3"],
    ["KEYUP3", "snd/keyup3"],
    ["KEYDOWN4", "snd/keydown4"],
    ["KEYUP4", "snd/keyup4"]
];

var cmdQueue = [];
var keyBuffer = [];

// Renderer

function renderer_Init() {
    drawImage("BG", 0, 0);
}

function renderer_RenderPlayfield() {
    for (var row = 0; row < 20; row++) {
        renderer_RenderRow(row);
    } 
}

function renderer_ClearRow(row) {
    for (var col = 0; col < 10; col++) {
        drawImage("DOT", col * 2 + 27, row + 1);
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
            renderer_ClearRow(row);
        }
    }
    for (var row = JSTe3s.Block.mBlock.mPosY - 1; row < JSTe3s.Block.mBlock.mPosY + 4; row++) {
        if (row >= 0 && row < 20) {
            renderer_RenderRow(row);
        }
    }
}

function renderer_RenderGameOver() {
    drawImage("GAMEOVER", 0, 15);
}

function renderer_RenderPause() {
    drawImage("PAUSED", 0, 15);
}

function renderer_ClearPause() {
    drawImage("RESUME", 0, 15);
}

function renderer_RenderScore() {
    writeNumber(3, 11, JSTe3s.Program.mScore);    
}

function renderer_RenderNextBlock() {
    var shape = JSTe3s.Block.mNextBlock.mShape[0];
    for (var blockY = 0; blockY < 4; blockY++) {
        for (var blockX = 0; blockX < 4; blockX++) {
            if (shape[blockY][blockX] == "1") {
                drawImage("BLOCK", blockX * 2 + 16, blockY + 10);
            } else {
                drawImage("BLANK", blockX * 2 + 16, blockY + 10);
            }
        }
    }
}

function renderer_ClearNextBlock() {
    for (var blockY = 0; blockY < 4; blockY++) {
        for (var blockX = 0; blockX < 4; blockX++) {
            drawImage("BLANK", blockX * 2 + 16, blockY + 10);
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
        case 37:  // left arrow
        case 103: // numpad 7
        case 55:  // 7
            return JSTe3s.Key.left;
        case 39:  // right arrow
        case 105: // numpad 9
        case 57:  // 9
            return JSTe3s.Key.right; 
        case 38:  // up arrow
        case 104: // numpad 8
        case 56:  // 8
            return JSTe3s.Key.rotate;        
        case 32:  // space
        case 101: // numpad 5
        case 53:  // 5
        case 40:  // down arrow
            return JSTe3s.Key.drop;
        case 78:  // n
            return JSTe3s.Key.restart;
        case 80:  // p
        case 19:  // pause/break
            return JSTe3s.Key.pause;
        case 97:  // numpad 1
        case 49:  // 1
            return JSTe3s.Key.showNext;
        case 100: // numpad 4
        case 52:  // 4
            return JSTe3s.Key.speedUp;
    }
    return JSTe3s.Key.other;
}

// Sound

function sound_Play(name) {
}

// Utils

function drawImage(imgName, x, y) {
    var img = images[imgName];
    for (var _Y = 0; _Y < img.height; _Y += 16) {
       for (var _X = 0; _X < img.width; _X += 8) {
            (function(X, Y) {
                cmdQueue.push(function () {
                    ctx.drawImage(img, X, Y, 8, 16, x * 8 + X, y * 16 + Y, 8, 16);
                }); 
            })(_X, _Y);
       }
   }
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
        onload: function () { deferred.resolve(); }
    });
    return deferred.promise();
}

// Main

function gameLoop() {
    JSTe3s.Program.play();
    if (cmdQueue.length == 0) {
        setTimeout(gameLoop, 0);
    } else {
        setTimeout(animLoop, 0);
    }
}

function animLoop() {    
    var i = 50;
    while (cmdQueue.length > 0 && --i >= 0) {
        cmdQueue[0]();
        cmdQueue.shift();
    }
    if (cmdQueue.length > 0) {
        setTimeout(animLoop, 1000/60); 
    } else {
        setTimeout(gameLoop, 0);
    }
}

function other(track) {
    return track == 1 ? 2 : 1;
}

var timeouts = [];

function bgNoiseCrossFade(vol, track, id) {
    delete timeouts[id];
    vol += 0.1;
    var angle = vol * (Math.PI / 2);
    sounds["BGNOISE" + other(track)].volume(Math.sin(angle));
    sounds["BGNOISE" + track].volume(Math.cos(angle));
    if (vol < 1) {
        var id1 = setTimeout(function () { bgNoiseCrossFade(vol, track, id1); }, 100);
        timeouts[id1] = 1;
    } 
}

function playBgNoise(track, vol, id) {
    if (id) { delete timeouts[id]; }
    sounds["BGNOISE" + track].volume(vol).play();
    var id1 = setTimeout(function () { playBgNoise(other(track), 0, id1); }, 5000);
    timeouts[id1] = 1;
    var id2 = setTimeout(function () { bgNoiseCrossFade(0, track, id2); }, 6000);
    timeouts[id2] = 1;
}

var keyStates = [];

$(function () { // wait for document to load
    // keyboard handler
    $(document).on("keydown", function (e) {
        if ($.inArray(e.which, [37, 103, 55, 39, 105, 57, 38, 104, 56, 32, 100, 52, 40, 78, 97, 49, 101, 53, 80, 19]) >= 0) {
            if (!keyStates[e.which]) {
                if ($.inArray(e.which, [32, 101, 53, 40]) >= 0) {
                    sounds["DROPDOWN" + Math.floor((Math.random() * 6) + 1)].play();    
                } else {
                    sounds["KEYDOWN" + Math.floor((Math.random() * 4) + 1)].play();    
                }
                keyStates[e.which] = 1;
            }
            //keyBuffer.push(e.which);
            keyBuffer = [ e.which ];
            e.preventDefault();
        }
    });
     $(document).on("keyup", function (e) {
        if (keyStates[e.which]) {
            if ($.inArray(e.which, [32, 100, 52, 40]) >= 0) {
                sounds["DROPUP" + Math.floor((Math.random() * 6) + 1)].play();
            } else {
                sounds["KEYUP" + Math.floor((Math.random() * 4) + 1)].play();
            }
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
        for (var id in timeouts) {
            clearTimeout(id);
        }
        timeouts = [];
        sounds["BGNOISE1"].stop();
        sounds["BGNOISE2"].stop();
    });
    $(window).focus(function () {
        if (timeouts.length == 0) {
            playBgNoise(2, 1);
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