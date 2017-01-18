var wp = require('webpage').create(),
    ws = null,

    stop = false,
    conn = false,

    baseReq = null,
    pass_ticket = "",
    uin = "",
    nickname = ""

///websocket
function openws(url) {
    console.log("开始链接...");
    ws = new WebSocket("ws://127.0.0.1:10500");
    ws.onopen = function () {
        console.log("conn:open");
        conn = true;
    }
    ws.onclose = function () {
        console.log("conn:close");
        if (!uin || stop) phantom.exit(0);
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
                phantom.exit(0);
                ws.close();
                break;
        }
        console.log("msg:" + m);
    }
}
function ws_send(act, bd) {
    if (ws) ws.send(JSON.stringify({
        from: uin,
        act: act,
        body: bd
    }));
}
openws();
//

wp.viewportSize = { width: 400, height: 300 };

wp.onResourceRequested = function (req) {
    outlog("req->" + req.url);
    if (!pass_ticket) { var pt = /pass_ticket=([\w\d%]+)/.exec(req.url); if (pt.length == 2) pass_ticket = pt[1]; }//获取pass_ticket
    if (req.url.indexOf("/webwxinit?") > 0) baseReq = JSON.parse(req.postData).BaseRequest; //更新basereq
    if (!uin && baseReq) { uin = baseReq.Uin; }//获取uin

}

wp.onResourceReceived = function (rsp) {
    if (rsp.url.indexOf("/webwxstatusnotify?") > 0 && rsp.stage == "end") {
        nickname = wp.evaluate(function () {
            return $(".display_name").text();
        });
    }
    //if (rsp.stage != "start") outlog("rsp.start->" + rsp.url)
    //if (rsp.stage != "end") outlog("rsp.end->" + JSON.stringify(rsp.body));
};
wp.onResourceError = function (err) {
    outlog("err->" + JSON.stringify(err));
};
var wx = {
    quit: function () { },
    send: function () { }
}

function outlog(str) {
    console.log("log->" + str);
    ws_send("log", str);
}
function loadheadimg() {
    var ua = wp.evaluate(function () {
        return window.userAvatar;
    });
    console.log("headimg->" + ua);
    if (ua) { ws_send("headimg", ua); loaduser(); }
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
        console.log("open wx.qq.com status:" + st);
        if (st != "success") return;
        loadqrcode();
    });
}
function loaduser() {
    if (!uin || !nickname) setTimeout(loaduser, 2 * 1000);
    else ws_send("setuser", JSON.stringify({ uin: uin, nickname: nickname }));
}

loadwx();


