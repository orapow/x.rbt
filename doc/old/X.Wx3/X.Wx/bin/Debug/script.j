var wp = require('webpage').create(),
    ws = null,
    stop = false,
    uin = "",
    headimg = "",
    nickname = "";

var baseReq = {},
    _syncKey = {
        Count: 0,
        List: [{
            Key: 0,
            Val: 0
        }],
        toString: function () {
            var val = "";
            for (var i in this.List) {
                var k = this.List[i];
                if (val) val += "|"
                val += k.Key + "_" + k.Val;
            }
            return val;
        }
    },
    passticket = "",
    gateway = "";

///websocket
function openws(url) {
    ws = new WebSocket("ws://localhost:10900/");
    ws.onopen = function () {
        console.log("conn:open");
    }
    ws.onclose = function () {
        console.log("conn:close");
        if (!headimg || stop);;//phantom.exit(0);
        setTimeout(openws(), 2 * 1000);
    }
    ws.onerror = function (ex) {
        console.log("err:" + JSON.stringify(ex));
    }
    ws.onmessage = function (m) {
        m = JSON.parse(m);
        switch (m.act) {
            case "quit":
                stop = true;
                wx.quit();
                //phantom.exit(0);
                ws.close();
                break;
        }
        console.log("msg:" + m);
    }
}
function ws_send(act, bd) {
    ws.send(JSON.stringify({
        act: act,
        body: bd
    }));
}
openws();
//

wp.viewportSize = { width: 400, height: 300 };

wp.onResourceRequested = function (req) {
    outlog(JSON.stringify(req));
};

var wx = {
    quit: function () { },
    send: function () { }
}

function outlog(str) {
    consolo.log("log->" + str);
    ws_send("log", str);
}
function loadheadimg() {
    var ua = wp.evaluate(function () {
        return window.userAvatar;
    });
    console.log("headimg->" + ua);
    if (ua) ws_send("headimg", ua);
    else if (!stop) setTimeout(loadheadimg, 2 * 1000);
}

function loadqrcode() {
    var qr = wp.evaluate(function () {
        return window.QRLogin;
    });
    console.log("qrcode->" + qr.uuid);
    if (qr) {
        ws_send("qrcode", "https://login.weixin.qq.com/qrcode/" + qr.uuid);
        setTimeout(loadheadimg, 2 * 1000);
    }
    else if (!stop) setTimeout(loadqrcode, 2 * 1000);
}

//开始加载页面
wp.open("https://wx.qq.com", function (st) {
    if (st != "success") return;
    loadqrcode();
});
