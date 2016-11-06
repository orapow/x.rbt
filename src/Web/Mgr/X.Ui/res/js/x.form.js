/**
 * xform 表单模块
 * 橙子(80xc.com)
 * 1.0.0
 */
(function ($) {

    $.fn.xform = function (op) {
        var el = $(this);
        return new form(el, op);
    }

    var form = function (el, op) {
        var f = this;
        f.el = el;
        f.init(op);
        f.validator = {
            mail: { reg: /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/, msg: "Email格式不正确，zk@80xc.com！" },
            num: { reg: /^\d+$/, msg: "这里只能输入>=0的数字！" },
            ch: { reg: /^[\u0391-\uFFE5]+$/, msg: "这里只能输入汉字！" },
            mp: { reg: /^1\d{10}$/, msg: "这里只能输入手机号！" },
            tel: { reg: /((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)/, msg: "这里格式不正确，应为电话号码（包括手机号）！" },
            nd: { reg: /^\d+([.]\d+)?$/, msg: "这里只能输入小数或整数！" },
            d: { reg: /^\d{4}-\d{2}-\d{2}$/, msg: "不是有效的日期格式（格式：1990-01-01）！" },
            dt: { reg: /^\d{4}-\d{2}-\d{2} \d{2}:\d{2}(:\d{2})?$/, msg: "不是有效的日期时间格式，可以不包含秒（格式：1990-01-01 00:00:00）！" },
            url: { reg: /^([\w\dz\/\-:]+)[.][a-z]{3,4}$/, msg: "不是有效的Url格式，例：http://www.80xc.com！" },
            char: { reg: /^[a-zA-z0-9\u4E00-\u9FA5\s]+$/, msg: "这里只能输入字母、数字、汉字，不能输入标点符号！" },
            c: { reg: /^#[a-fA-F]{6}$/, msg: "不是有效的颜色值格式！" },
            cust: {},
        };
    };

    function callback(d, f) {
        if (f.opts.callback) {
            try {
                if (typeof (f.opts.callback) === "string") eval(f.opts.callback + "(d)");
                else f.opts.callback(d);
            } catch (e) {
                x.alert("callback 出错，提示：" + e);
            }
        }
    }

    form.prototype.init = function (op) {

        var f = this;
        var dom = f.el;

        x.pick.init();
        x.upload.init();

        f.ipts = dom.find("[x-check]");
        f.opts = $.extend(true, {}, {
            api: dom.attr("x-api"),//提交接口
            callback: dom.attr("x-callback"),//回调
            prepost: dom.attr("x-prepost")//提交前回调，返回false 不提交
        }, op);

        dom.submit(function () {
            try {
                f.submit();
            } catch (e) { console.log("dom.submit->" + e); }
            return false;
        });

        dom.on("reset", (function () { }));

        f.ipts.each(function () {
            var i = $(this);
            if (i.data("evt")) { return; }
            i.bind({
                focus: function () { },
                keyup: function (key) { },
                blur: function () { },
                click: function () { return false; }
            });
            i.data("evt", "true");
        });

    }
    form.prototype.submit = function () {

        var f = this;
        var d = {};
        var op = f.opts;
        var ispass = 0;

        if (op.prepost && !op.prepost()) return false;

        f.ipts.blur().keyup();

        if (ispass != 0) {
            x.alert("有必填的项未填写，请填写好必填项（请注意标有红色框的项目）");
            return;
        };

        f.ipts.each(function () {

            var i = $(this);
            var n = i.attr("name");
            var val = "";

            if (i[0].tagName.toLowerCase() === "input") {
                switch (i.attr("type")) {
                    case "text":
                    case "hidden":
                    case "password":
                        val = i.val();
                        break;
                }
                if (i.attr("x-src")) val = i.attr("x-val");
            }
            else {
                val = i.val();
            }
            if (!val && i.attr("x-val")) val = i.attr("x-val") || "";//单选
            if (!d[n]) d[n] = ""; else d[n] += ",";
            d[n] += val;

        });

        if (op.api) {
            var btn = f.el.find(":submit");
            btn.attr("disabled", "disabled");
            x.doapi(op.api, d, function (data) {
                btn.removeAttr("disabled");
                if (data.issucc) callback(data, f);
            }, "json");
        } else {
            callback(d, f);
        }

    }

})(jQuery);
(function ($) {
    var dom = $("#x-pick");
    var pick = {
        src: "",
        ct: null,
        callback: null,
        init: function () {
            if (dom.size() > 0) return;

            dom = $("<div id='x-pick' class='x-pick'><div class='h'></div><div class='c'></div><div class='b'><span class='txt'></span><span class='btn btn-primary ok' onclick='x.pick.ok()'>确定</span> <span class='btn cancel' onclick='x.pick.hide()'>取消</span></div></div>");
            $("body").append(dom);

            $(document).mouseup(function (e) {
                if (dom.find(e.target).size() == 0) x.pick.hide();
            });

            $(".li .btn-group .btn").click(function () {
                var li = $(this);
                var ct = li.parent().parent().attr("x-count");
                if (ct) ct = parseInt(ct);
                else ct = 1;
                if (li.hasClass("btn-primary") && ct != 1) li.removeClass("btn-primary");
                else {
                    var ad = true;
                    if (ct > 1) {
                        var sc = li.parent().find(".btn-primary").size();
                        if (sc >= parseInt(ct)) ad = false;
                    }
                    else if (ct == 1) {
                        li.parent().find(".btn-primary").removeClass("btn-primary");
                    }
                    if (ad) li.addClass("btn-primary");
                }

                var vals = "";
                li.parent().find(".btn-primary").each(function () {
                    if (ct == 1) { vals += $(this).attr("x-val"); }
                    else { vals += "[" + $(this).attr("x-val") + "]"; }
                });

                li.parent().attr("x-val", vals);

            });

            dom.delegate(".pick-item", "click", function () {
                var item = $(this);
                if (pick.count == 1) {
                    dom.find(".pick-item.btn-primary").removeClass("btn-primary");
                    item.addClass("btn-primary");
                    x.pick.ok();
                } else {
                    var sc = 0;
                    if (item.hasClass("btn-primary")) $(this).removeClass("btn-primary");
                    else {
                        sc = dom.find(".pick-item.btn-primary").size();
                        if (sc < pick.count) item.addClass("btn-primary");
                    }
                    sc = dom.find(".pick-item.btn-primary").size();
                    dom.find("div.b .txt").text("最多能选" + pick.count + "个，已选" + sc + "个");
                }
            });

        },
        show: function (ct) {
            var i = pick.ct = $(ct);
            var c = i.parent().attr("x-count");
            if (c) c = parseInt(c);

            this.count = c;

            if (c == 1) { dom.find("div.b").hide(); dom.find("div.h").hide(); }
            else { dom.find("div.b").show(); dom.find("div.h").text(i.attr("title")).show(); }

            loadcontent(i.attr("x-src").split(":"));

            dom.css({
                "top": i.offset().top + i.outerHeight() - 1,
                "left": i.offset().left
            });

            dom.show();
        },
        ok: function () {
            var vals = "";
            var texts = "";
            var i = pick.ct;
            var c = this.count;
            dom.find(".pick-item.btn-primary").each(function () {
                if (vals) { texts += "、"; }
                if (c === 1) {
                    vals += $(this).attr("x-val");
                    texts += $(this).text();
                } else {
                    vals += "[" + $(this).attr("x-val") + "]";
                    texts += $(this).text();
                }
            });
            i.attr("x-val", vals)
            if (i[0].tagName.toLowerCase() !== "input") {
                i.find(".text").text(texts);
            }
            else {
                i.val(texts);
            }
            this.hide();
        },
        hide: function () { dom.hide(); }
    };

    function loadcontent(d) {
        switch (d[0]) {
            case "dict":
                sethtml("<span class='loading'>字典正在加载中...</span>");
                var url = "/com/dict-" + d[1] + (x.pick.ct.attr("x-parms") || "-");
                x.getview(url + ".html", function (data) {
                    sethtml(data);
                    loadback(d);
                });
                break;
            case "url":
                sethtml("<span class='loading'>视图正在加载中...</span>");
                var u = "{key}-" + (pick.ct.attr("x-parms") || "");
                u = d[1].replace("{p}", u);
                x.getview(u, function (data) {
                    sethtml(data);
                    loadback(d);
                });
                break;
        }
    }
    function loadback(d) {
        var i = pick.ct;
        if (!i.attr("x-val")) return;

        var v = i.attr("x-val");

        dom.find(".btn").each(function () {
            var i = $(this);
            if (v.indexOf("[" + i.attr("x-val") + "]") >= 0 || i.attr("x-val") == v) i.addClass("btn-primary");
        })

    }
    function sethtml(html) {
        dom.find("div.c").html(html);
    }

    window.x.pick = pick;    //生成公有静态元素

})(jQuery);
(function ($) {
    var dom = $("#x-upload");
    var file = null;
    var cbt = null;
    var hidetimer = null;

    var upload = {
        init: function () {
            if (dom.size() > 0) return;

            dom = $("<div id='x-upload' style='display:none'><input type='file' multiple='multiple' id='x-upload-file' onchange='x.upload.up(this)' /><i id='x-upload-close' class='icon-remove' onclick='x.upload.del(this)'></i></div>");
            $("body").append(dom);

            file = $("#x-upload-file");
            cbt = $("#x-upload-close");

            $(".upload .xbtn").click(function () {
                var up = $(this).parent();
                upload.doup(up.attr("x-tp"), up.attr("x-name"));
            });

            $(".upload").each(function () {
                var up = $(this);

                var vs = up.find("input").val();
                if (!vs) return;
                vs = vs.split(',');

                var tp = up.attr("x-tp");
                var list = up.find(tp == "img" ? ".imgs" : ".files");

                for (var i in vs) {
                    var li = null;
                    var f = vs[i];
                    if (tp == "img") li = $("<div class='img' x-val='" + f + "'><img src='" + f + "' /></div>");
                    else li = $("<div class='file' x-val='" + f + "'><img src='/img/files/" + getext(f) + ".png' /><p class='attr'><span class='n'>" + getname(f) + "</span><a href='" + f + "' target='_blank'>查看</a></p></div>");
                    list.append(li);
                }

            });

            $(".upload").delegate(".file,.img", "mouseover", function () {
                if (hidetimer) clearTimeout(hidetimer);
                cbt.appendTo($(this));
            }).delegate(".file,.img", "mouseout", function (evt) {
                hidetimer = setTimeout(function () {
                    cbt.appendTo(dom);
                }, 500);
            }).delegate(".file,.img", "click", function (evt) {
                window.open($(this).attr("x-val"));
            });

        },
        del: function (obj) {
            var li = $(obj).parent();
            var url = li.attr("x-val");
            var ipt = $("#id_" + li.parent().parent().attr("x-name"))
            var list = li.parent();

            li.remove();

            setval(list, ipt);

            if (!url) return;

        },
        up: function (file) {

            var list = $("." + upload.ct + (upload.tp == "img" ? " .imgs" : " .files"));
            var ipt = $("#id_" + list.parent().attr("x-name"));

            for (var i = 0; i < file.files.length; i++) {
                var f = file.files[i];
                var li = null;
                if (upload.tp == "img") li = $("<div class='img'><img src='/img/loading.gif' /></div>");
                else li = $("<div class='file' x-val=''><img src='/img/loading.gif' /><p class='attr'><span class='n'>" + f.name + "</span>" + getsize(f.size) + "</p></div>");
                list.append(li);
                send(li, ipt);
            }

            function send(li, name) {
                var form = new FormData();
                form.append("type", upload.tp);
                form.append("file", f);// 文件对象
                var xhr = new XMLHttpRequest();
                xhr.onreadystatechange = function () {
                    if (this.readyState != 4 || this.status != 200) return;
                    var j = jQuery.parseJSON(this.responseText);
                    if (!j.issucc) { li.remove(); return; }
                    li.attr("x-val", j.url);
                    if (upload.tp == "img") li.find("img").attr("src", j.url);
                    else {
                        li.find("img").attr("src", "/img/files/" + j.ext + ".png");
                        li.find("span.n").text(j.name);
                    }
                    setval(li.parent(), ipt);
                }
                xhr.open("post", "/api/com.upload", true);
                xhr.send(form);
            }
        },
        doup: function (tp, ct) {
            upload.ct = ct;
            upload.tp = tp;
            if (ct == 1) file.removeAttr("multiple");
            else file.attr("multiple", "multiple");
            file.click();
        }
    };

    function setval(list, ipt) {
        var vals = "";
        list.find(".img,.file").each(function () {
            if (vals) vals += ",";
            var v = $(this).attr("x-val");
            if (!v) return;
            vals += v;
        });
        ipt.val(vals);
    }

    function getname(n) {
        return n.substr(n.lastIndexOf("/") + 1);
    }

    function getext(n) {
        return n.substr(n.lastIndexOf(".") + 1);
    }

    function getsize(s) {
        if (s < 1000) return s + "byte";
        s = s / 1000;
        if (s < 1000) return s.toFixed(2) + "kb";
        s = s / 1000;
        if (s < 1000) return s.toFixed(2) + "mb";
        s = s / 1000;
        return s.toFixed(2) + "Gb";
    }

    x.upload = upload;    //生成公有静态元素
})(jQuery);