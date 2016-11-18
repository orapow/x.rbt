var page = require('webpage').create();
var uuid = "";
page.viewportSize = {
    width: 150,
    height: 150
};
page.settings.userAgent = 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.120 Safari/537.36';
function loaduuid() {
    page.open("https://login.wx.qq.com/jslogin?appid=wx782c26e4c19acffb&fun=new&lang=zh_CN&_=1479346913589", function () { loadqrcode(); });
}
function loadqrcode() {
    var reg = /QRLogin.uuid = "([^"]+)"/;
    uuid = reg.exec(page.content)[1];
    page.open("https://login.weixin.qq.com/qrcode/" + uuid, function () { loadback(); });
}
function loadback() {
    var qr = page.renderBase64('PNG');
    console.log("qrcode|data:image/png;base64," + qr);
    waitfor(1, 0);
}
function waitfor(t, c) {
    console.log("wait->" + t + "->" + c);
    var dt = new Date().getTime();
    page.open("https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=" + uuid + "&tip=0&r=" + dt + "&_=" + ~dt, function () {
        var cot = page.content;
        if (cot.indexOf("code=201") > 0) {
            var reg = /window.userAvatar = '([^']+)'/.exec(cot);
            console.log("hdimg|" + reg[1]);
            waitfor(2, 0);
        } else if (cot.indexOf("code=200") > 0) {
            var reg = /window.redirect_uri="([^"]+)"/.exec(cot);
            console.log("login|" + reg[1]);
            phantom.exit();
        } else if (c == 3) phantom.exit();
        else waitfor(t, c + 1);
    });
}
loaduuid();