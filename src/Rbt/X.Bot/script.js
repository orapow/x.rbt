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
                //case "loadcontact":
                //    loadcontact();
                //    break;
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

wp.settings.userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36";
wp.onCallback = function (msg) {
    console.log("callback->" + JSON.stringify(msg));
    switch (msg.act) {
        case "contact":
            ws_send("loadcontact", msg.data.replace(/[\s]/g, ""))
            break;
        case "sync":
            wp.checking = 0;
            break;
        case "newmsg":
            //console.log(msg.data);
            //syncKey = msg.sk;
            //ws_send("newmsg", JSON.stringify(msg.list))
            break;
    }
};
wp.viewportSize = { width: 400, height: 300 };
//wp.onConsoleMessage = function (msg, lineNum, sourceId) {
//    console.log('CONSOLE: ' + msg + ' (from line #' + lineNum + ' in "' + sourceId + '")');
//};
wp.onResourceRequested = function (req) {
    var ns = req.url.split('.');
    var n = ns[ns.length - 1];
    var flag = "";
    if (n == "css" || n == "gif" || n == "png" || n == "jpg") flag = "->abort";

    if (req.url.indexOf("/webwxlogout?") > 0) {
        quit("微信退出"); //退出
    }
    if (req.url.indexOf("/webwxinit?") && baseReq == null) {
        baseReq = JSON.parse(req.postData).BaseRequest;//更新basereq
        console.log("init->" + req.postData);
    }
    if (req.url.indexOf("/synccheck?") > 0) {
        //flag = "->abort";
        //if (syncKey == null) {
        //    var sk = /synckey=([^&|"]+)/.exec(req.url)[1];
        //    var keys = sk.split("%7C");
        //    syncKey = [];
        //    for (var i in keys) {
        //        var k = keys[i].split("_");
        //        syncKey.push({ Key: k[0], Val: k[1] });
        //    }
        //}
        if (!wp.checking) synccheck(req.url);
    }
    if (req.url.indexOf("/webwxgetcontact?") > 0 && !wp.iscontact) {//通讯录
        flag = "->abort";
        wp.iscontact = 1;
        loadcontact();
    }
    //if (req.url.indexOf("/webwxsync?") > 0) {
    //    //flag = "->abort";
    //    loadmsg();
    //}

    outlog("res.req->" + req.url + flag);
    if (flag) req.abort();

    if (!pass_ticket) { var pt = /pass_ticket=([\w\d%]+)/.exec(req.url); if (pt.length == 2) pass_ticket = pt[1]; }//获取pass_ticket

}
wp.onResourceReceived = function (rsp) {
    //if (rsp.stage == "start") return;
    //if (rsp.url.indexOf("/synccheck?") > 0) {
    //    var exit = wp.evaluate(function () { return (window.synccheck && window.synccheck.retcode) == "1102" });
    //    if (exit) quit("微信退出");
    //}
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
    var ua = wp.evaluate(function () {
        return window.userAvatar;
    });
    console.log("headimg->" + ua);
    if (ua) { ws_send("headimg", ua); headimg = ua; loaduser(); }
    else if (!stop) setTimeout(loadheadimg, 2 * 1000);
}
function synccheck(url) {
    wp.checking = 1;
    wp.evaluate(function (u) {
        $.get(u, function (d) {
            window.callPhantom({ act: 'sync', data: d });
        });
    }, url);
}
function loadmsg() {
    var msg = wp.evaluate(function () {
        var reply = {};
        var $lst = $("[ng-switch] .you"); // 未回复的列表
        var $info = $lst.not('[reply]').first(); // 未回复的记录
        if ($info != null && $info.length > 0 && !$info.attr("data-isreplay")) {
            reply["text"] = $info.find(".js_message_plain").text(); // 未回复的内容
            reply["nick"] = $info.find(".avatar").attr("title"); // 未回复的备注名称
            reply["title"] = $info.parents('.chat_bd.scroll-wrapper:first').prev().find('.title_name').text(); // 聊天框title
            reply["touser"] = $info.parents('[jquery-scrollbar]').attr('data-cm');
            reply["touser"] = JSON.parse(reply["touser"]).username;
            reply["fromuser"] = $info.find(".avatar").attr("data-cm");
            reply["fromuser"] = JSON.parse(reply["fromuser"]).username;
            reply["index"] = $lst.index($info);
            reply["item"] = $info;
            reply["msgid"] = JSON.parse($info.find(".js_message_bubble").attr("data-cm")).msgId;
            reply["chat"] = reply["touser"].indexOf("@@") == 0 ? 1 : 0;
            $info.attr("reply", 1);
        }
        if (typeof window.callPhantom === 'function') window.callPhantom({ act: 'newmsg', data: reply });
        //$.post("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid=" + brq.Sid + "&skey=" + brq.Skey + "&lang=zh_CN&pass_ticket=" + pk, JSON.stringify({
        //    BaseRequest: brq,
        //    SyncKey: {
        //        Count: sk.length,
        //        List: sk
        //    },
        //    rr: ~new Date().getTime()
        //}), function (d) {
        //    //var msg = JSON.parse(d);
        //    if (typeof window.callPhantom === 'function') window.callPhantom({ act: 'newmsg', data: d });
        //});
    });
    setTimeout(loadmsg, 500);
    //console.log("msg->" + JSON.stringify(msg));
}
function loadcontact() {
    wp.evaluate(function (pk, sk) {
        $.get("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?lang=zh_CN&pass_ticket=" + pk + "&r=" + new Date().getTime() + "&seq=0&skey=" + sk, function (d) {
            if (typeof window.callPhantom === 'function') window.callPhantom({ act: 'contact', data: d });
        });
    }, pass_ticket, baseReq.Skey);
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


