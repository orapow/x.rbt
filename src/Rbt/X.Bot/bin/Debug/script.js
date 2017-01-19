var wp = require('webpage').create(),
    sys = require("system"),
    ws = null,

    stop = false,
    conn = false,

    baseReq = null,
    pass_ticket = "",
    uin = "",
    nickname = "",
    headimg = ""

if (sys.args.length === 1) phantom.exit(0);
uin = sys.args[1];

phantom.onError = function (msg, trace) {
    console.log("err->" + msg + "\r\n" + trace);
}
phantom.outputEncoding = "gb2312";

///websocket
function openws(url) {
    console.log("ws:开始连接...");
    ws = new WebSocket("ws://127.0.0.1:10500");
    ws.onopen = function () {
        console.log("ws:已连接");
        ws_send("setcode", uin);
        conn = true;
    }
    ws.onclose = function () {
        console.log("ws:连接断开");
        if (!nickname || stop) phantom.exit(0);
        console.log("ws:等待重连")
        setTimeout(openws(), 2 * 1000);
    }
    ws.onerror = function (ex) {
        console.log("ws:err->" + JSON.stringify(ex));
    }
    ws.onmessage = function (msg) {
        console.log("ws:msg->" + msg.data);
        var m = JSON.parse(msg.data);
        switch (m.act) {
            case "ok":
                if (nickname) loaduser();
                else loadwx();
                break;
            case "quit":
                stop = true;
                nickname = "";
                wx.quit();
                break;
        }
    }
}
function ws_send(act, bd) {
    if (!ws) return;
    console.log("ws:send->" + act + "@" + bd);
    ws.send(JSON.stringify({
        from: uin,
        act: act,
        body: bd
    }));
}
openws();
//

wp.viewportSize = { width: 400, height: 300 };
wp.onResourceRequested = function (req) {
    outlog("res.req->" + req.url);
    if (req.url.indexOf("/webwxinit?")) { baseReq = JSON.parse(req.postData).BaseRequest; } //更新basereq
    if (!pass_ticket) { var pt = /pass_ticket=([\w\d%]+)/.exec(req.url); if (pt.length == 2) pass_ticket = pt[1]; }//获取pass_ticket
}
wp.onResourceReceived = function (rsp) {
    //if (rsp.stage != "start") outlog("rsp.start->" + rsp.url)
    //if (rsp.stage != "end") outlog("rsp.end->" + JSON.stringify(rsp.body));
};
wp.onResourceError = function (err) {
    if (err.url.indexOf("/webwxnewloginpage?") > 0) loadwx();
    outlog("res.err->" + JSON.stringify(err));
};

var wx = {
    quit: function () { },
    send: function () { }
}

function senduser() {
    ws_send("setuser", JSON.stringify({ uin: uin, nickname: nickname, headimg: headimg }));
}
function outlog(str) {
    //console.log("log->" + str);
    ws_send("log", str);
}
function loadheadimg() {
    var ua = wp.evaluate(function () {
        return window.userAvatar;
    });
    console.log("headimg->" + ua);
    if (ua) { ws_send("headimg", ua); headimg = ua; loaduser(); }
    else if (!stop) setTimeout(loadheadimg, 2 * 1000);
}
function loadqrcode() {
    var qr = wp.evaluate(function () {
        return window.QRLogin;
    });
    console.log("qrcode->" + qr.uuid);
    if (qr && qr.uuid) {
        ws_send("qrcode", "https://login.weixin.qq.com/qrcode/" + qr.uuid);
        setTimeout(loadheadimg, 2 * 1000);
    }
    else if (!stop) setTimeout(loadqrcode, 2 * 1000);
}
function loadwx() {
    if (!conn) { setTimeout(loadwx, 1 * 1000); return; }
    //开始加载页面
    wp.open("https://wx.qq.com/", function (st) {
        if (st != "success") return;
        loadqrcode();
    });
}
function loaduser() {
    console.log("loaduer->");
    nickname = wp.evaluate(function () { return $(".display_name").text(); });
    if (baseReq) uin = baseReq.Uin; //获取uin
    if (uin.length == 36 || !nickname) setTimeout(loaduser, 2 * 1000);
    else senduser();
}


