var wp = require('webpage').create(),
    sys = require("system"),
    ws = null,

    stop = false,
    conn = false,

    baseReq = null,
    syncKey = null,
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
        if (!nickname || stop) { phantom.exit(0); }
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
    console.log("ws:send->" + act + "@" + bd.substring(0, 400));
    ws.send(JSON.stringify({
        from: uin,
        act: act,
        body: bd
    }));
}
openws();
//

wp.settings.userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36";
wp.onCallback = function (msg) {
    console.log("callback->" + JSON.stringify(msg).substring(0, 400));
    switch (msg.act) {
        case "contact":
            ws_send("contact", msg.data.replace(/[\s]/g, ""))
            break;
        case "sync":
            wp.checking = 0;
            break;
        case "newmsg":
            ws_send("newmsg", JSON.stringify(msg.data))
            break;
    }
};
wp.viewportSize = { width: 400, height: 300 };
wp.onResourceRequested = function (req) {
    var ns = req.url.split('.');
    var n = ns[ns.length - 1];
    var flag = "";
    if (n == "css" || n == "gif" || n == "png" || n == "jpg" || req.url.indexOf("/webwxgeticon?") > 0 || req.url.indexOf("/webwxgetheadimg?") > 0) flag = "->abort";

    if (req.url.indexOf("/webwxlogout?") > 0) {
        quit("微信退出"); //退出
    }
    if (req.url.indexOf("/webwxinit?") > 0) {
        var breq = JSON.parse(req.postData).BaseRequest;//更新basereq
        if (!baseReq || !baseReq.Uin) console.log("init->" + req.postData);
        baseReq = breq;
    }
    if (req.url.indexOf("/webwxgetcontact?") > 0) {//通讯录
        flag = "->abort";
        wp.iscontact = 1;
        loadcontact(req.url);
    }
    if (flag) req.abort();
    outlog("res.req->" + req.url + flag);

    if (!pass_ticket) { var pt = /pass_ticket=([\w\d%]+)/.exec(req.url); if (pt.length == 2) pass_ticket = pt[1]; }//获取pass_ticket

}
wp.onResourceReceived = function (rsp) {
    if (rsp.url.indexOf("/webwxsync?") > 0 && rsp.stage == "end") setTimeout(loadmsg, 200);
};
wp.onResourceError = function (err) {
    if (err.url.indexOf("/webwxnewloginpage?") > 0) {
        outlog("res.err->" + JSON.stringify(err));
        quit("登陆失败");
    }
};

var wx = {
    quit: function () { },
    send: function () { }
}

function quit(res) {
    stop = true;
    ws_send("quit", res);
    setTimeOut(function () { phantom.exit(0); }, 2 * 1000);
}
function senduser() {
    ws_send("setuser", JSON.stringify({ uin: uin, nickname: nickname, headimg: headimg }));
}
function outlog(str) {
    ws_send("log", str);
}
function loadheadimg() {
    var ua = wp.evaluate(function () { return window.userAvatar; });
    console.log("headimg->" + ua);
    if (ua) { ws_send("headimg", ua); headimg = ua; loaduser(); }
    else if (!stop) setTimeout(loadheadimg, 2 * 1000);
}
function loadmsg() {
    outlog("loadmsg");
    wp.render("d:\\wx.jpg");
    wp.evaluate(function () {
        if (!window.ids) window.ids = {};
        var item = $(".icon.web_wechat_reddot_middle,.icon.web_wechat_reddot").first();
        if (item.size() > 0) item.parents(".chat_item").click();

        var ti = $(".title_name");
        if (ti.size() == 0) return;
        var list = $("[ng-switch] .you"); // 未回复的列表
        if (list.size() == 0) return;

        var room = {
            name: ti.attr("data-username"),
            text: ti.text()
        }

        list.each(function () {
            var it = $(this);

            var id = JSON.parse(it.find(".js_message_bubble").attr("data-cm")).msgId;

            if (ids[id] != undefined) return;

            var fr = it.find(".avatar");
            var from = JSON.parse(fr.attr("data-cm"));
            from.nk = fr.attr("title");

            var msg = {
                from: from,
                room: room,
                text: it.find(".js_message_plain").text()
            };

            ids[id] = "";
            window.callPhantom({ act: 'newmsg', data: msg });

        });

    });
}
function loadcontact(url) {
    outlog("loadcontact->" + url);
    wp.evaluate(function (u) {
        $.get(u, function (d) {
            if (typeof window.callPhantom === 'function') window.callPhantom({ act: 'contact', data: d });
        });
    }, url);
    loadmsg();
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


