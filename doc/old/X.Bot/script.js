var wp = require('webpage').create(),
    sys = require("system"),
    ws = null,

    stop = false,
    conn = false,

    baseReq = null,
    syncKey = null,
    pass_ticket = "",
    host = "",
    uin = "",
    nickname = "",
    headimg = "",
    username = "",
    contacts = [];

if (sys.args.length === 1) phantom.exit(0);
uin = sys.args[1];

phantom.onError = function (msg, trace) {
    console.log("err->" + msg + "\r\n" + JSON.stringify(trace));
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
                if (nickname) { loaduser(); ws_send("contact", JSON.stringify(contacts)); }
                else loadwx();
                break;
            case "quit":
                stop = true;
                nickname = "";
                wx.quit();
                break;
            case "send":
                var wm = JSON.parse(m.body);
                wx.send(wm.type, wm.to, wm.content);
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
            contacts = JSON.parse(msg.data);
            ws_send("contact", JSON.stringify(contacts));
            break;
        case "sync":
            wp.checking = 0;
            break;
        case "newmsg":
            ws_send("newmsg", JSON.stringify(msg.data))
            break;
    }
};
wp.viewportSize = { width: 640, height: 480 };
wp.onResourceRequested = function (req) {
    var ns = req.url.split('.');
    var n = ns[ns.length - 1];
    var flag = "";
    if (n == "css" || n == "gif" || n == "png" || n == "jpg" || req.url.indexOf("/webwxgeticon?") > 0 || req.url.indexOf("/webwxgetheadimg?") > 0 || req.url.indexOf("/webwxbatchgetcontact?") > 0) flag = "->abort";

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
    send: function (tp, to, txt) {
        var url = "";

        var o = {
            BaseRequest: baseReq,
            Msg: {
                FromUserName: username,
                ToUserName: to,
                LocalID: new Date().getTime(),
                ClientMsgId: new Date().getTime()
            }
        }
        if (tp == 1) {
            url = "https://" + host + "/cgi-bin/mmwebwx-bin/webwxsendmsg?pass_ticket=" + pass_ticket;
            o.Msg.Type = 1;
            o.Msg.Content = txt;
        } else if (tp == 2) {
            url = "https://" + host + "/cgi-bin/mmwebwx-bin/webwxsendmsgimg?fun=async&f=json&pass_ticket=" + pass_ticket;
            o.Msg.Type = 3;
            o.Msg.MediaId = mmid;
        }

        wp.evaluate(function (u, bd) {
            $.post(u, JSON.stringify(bd), function (d) {
                window.callPhantom({ act: 'sendback', data: d });
            })
        }, url, o);

    },
    sendImg: function (to, url) {

    }
}

function quit(res) {
    stop = true;
    ws_send("quit", res);
    setTimeOut(function () { phantom.exit(0); }, 2 * 1000);
}
function senduser() {
    ws_send("setuser", JSON.stringify({ uin: uin, username: username, nickname: nickname, headimg: headimg }));
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
    //wp.render("d:\\wx.jpg");
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

        try {
            list.each(function () {
                var it = $(this);

                var id = JSON.parse(it.find(".js_message_bubble").attr("data-cm")).msgId;

                if (ids[id] != undefined) return;

                var fr = it.find(".avatar");
                var from = JSON.parse(fr.attr("data-cm"));

                var msg = {
                    fr: {
                        name: from.username,
                        text: fr.attr("title")
                    },
                    rm: room,
                    text: it.find(".js_message_plain").html()
                };

                ids[id] = "";
                window.callPhantom({ act: 'newmsg', data: msg });

            });
        } catch (e) {
            window.callPhantom({ act: 'msgerr', data: JSON.stringify(e) });
        }
    });
}
function loadcontact(url) {
    outlog("loadcontact->" + url);
    wp.evaluate(function (u, h, req) {
        $.get(u, function (d) {
            var cts = d.MemberList;
            if (!cts || cts.length == 0) return;
            var list = [];
            for (var i in cts) {
                var c = cts[i];
                if (c.KeyWord === 'gh_') continue;

                if (c.UserName[1] == '@') {
                    var o = {
                        BaseRequest: req,
                        Count: 1,
                        List: [{
                            EncryChatRoomId: c.EncryChatRoomId,
                            UserName: c.UserName
                        }]
                    };
                    $.ajax("https://" + h + "/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=" + new Date().getTime(), {
                        async: true,
                        data: JSON.stringify(o),
                        dataType: "json",
                        type: "POST",
                        success: function (data) {
                            var gp = data.ContactList[0];
                            c = gp;
                        },
                        complete: function (xhr, st) {
                            window.callPhantom({ act: 'log', data: st });
                        }
                    });
                }
                list.push(c);
            }

            window.callPhantom({ act: 'contact', data: list });
        });
    }, url, host, baseReq);
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
    else {
        host = wp.evaluate(function () { return document.location.host; });
        username = wp.evaluate(function () { return /@[^&]+/.exec($(".header .avatar .img").attr("src"))[0]; })
        senduser();
    }
}


