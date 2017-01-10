x.grid = function (cfg) {
    this.config = cfg;
    var gDom = $(cfg.el);
    this.getlist = function (d, callback) {
        var g = this;
        //$(g.config.el + " .btn.btn-primary.pi").html("<span class='loading'>数据加载中...</span>");
        g.config.scroll = $(g.config.el + " .grid.body").scrollTop();
        var pd = $.extend(true, g.config.searchcon, d);
        x.doapi(g.config.api, pd, function (data) {
            g.config.dat = data;
            render(data);
            $(g.config.el + " .grid.body").scrollTop(g.config.scroll);
            if (callback) callback();
        }, false);
    }
    this.settpl = function (tp) {
        var g = this;
        g.config.tpl = tp;
        render(g.config.dat);
    }
    var g = this;
    bindevt();
    this.getlist({ page: 1 });
    //$(window).resize(function () {
    //    $(cfg.el + " .grid.body").css("max-height", $(window).height() - $(".title").outerHeight() - $(".search").outerHeight() - $(cfg.el + " .grid.head").outerHeight() - $(cfg.el + " .grid.pager").outerHeight() - 50 - cfg.exheight)
    //});
    function bindevt() {
        gDom.delegate(".grid-item", "dblclick", function (evt) {
            clearTimeout(cfg.clicktimer);
            var tr = $(this);
            if (cfg.rowdblclick) cfg.rowdblclick(tr.data("data"));
            if (evt.stopPropagation) evt.stopPropagation();
            else evt.cancelBubble = true;
        }).delegate(".grid-item", "click", function (evt) {
            if (evt.target.tagName === "INPUT") return;
            var tr = $(this);
            cfg.clicktimer = setTimeout(function () {
                if (cfg.rowclick) cfg.rowclick(tr.data("data"));
            }, 300);
        }).delegate(":checkbox[d]", "click", function (e) {
            if ($(this).attr("checked")) gDom.find(".grid-item[d='" + $(this).attr("d") + "']").attr("sel", "true");
            else gDom.find(".grid-item[d='" + $(this).attr("d") + "']").removeAttr("sel");
        }).delegate("#checkall", "click", function () {
            if ($(this).attr("checked")) {
                gDom.find("input[type='checkbox'][data-id]").attr("checked", "checked")
                gDom.find(".grid-item[d]").attr("sel", "true");
                if (cfg.allselected) cfg.allselected();
            }
            else {
                gDom.find("input[type='checkbox'][data-id]").removeAttr("checked")
                gDom.find(".grid-item[d]").removeAttr("sel");
            }
        }).delegate("th.sortable", "click", function () {
            dosort($(this).attr("f"), this);
        }).delegate("[data-act]", "click", function () {
            if (cfg.cellclick == undefined) return;
            var btn = $(this);
            var confirm = btn.data("confirm");
            if (confirm) {
                var tip = (confirm == true ? "是否执行 " + btn.text() + " 操作？" : confirm);
                x.confirm(tip, function () {
                    cfg.cellclick(btn.attr("data-act"), cfg.data[btn.attr("data-id")], btn);
                });
            } else {
                cfg.cellclick(btn.attr("data-act"), cfg.data[btn.attr("data-id")], btn);
            }
        });
    }
    function dosort(f, th) {
        var h = $(th);
        var t = 1 - parseInt(h.attr("order") || 1);
        h.parent().find("i").remove();
        h.append(" <i class='icon-caret-" + (t == 0 ? "down" : "up") + "'></i>");
        h.attr("order", t);
        g.getlist({ sort: (f.toLowerCase().indexOf("name") > 0 ? f.toLowerCase().replace("name", "") : f) + "," + t, page: 1 });
    }
    function gettablehead() {
        var fs = "<table class='table' grid='head'><thead><tr>";
        if (cfg.showcheckbox) {
            fs += '<th width="30" align="center"><input type="checkbox" title="全选" id="checkall"/></th>';
        }
        if (cfg.shownumber) {
            fs += "<th width='20' align='" + (fs.align || "left'") + ">No</th>";
        }
        for (var f in cfg.fs) {
            var w = cfg.fs[f].w > 0 ? cfg.fs[f].w : 120;
            var sort = cfg.fs[f].sort;
            fs += '<th ' + (sort ? " class='sortable' title='点击可排序' " : "") + ' width="' + w + '" style=\"text-align:' + (cfg.fs[f].align || "left") + '\">' + cfg.fs[f].t + '</th>';
        }
        if (cfg.btns.length > 0) {
            fs += '<th>操作</th>';
        } else {
            fs += "<th></th>";
        }
        fs += "</tr></thead></table>";
        return fs;
    }
    function renderpager(g, d) {
        if (!d.items) d.items = [];
        d.limit = d.limit || d.items.length;
        d.count = d.count || d.items.length;
        d.page = d.page || 1;
        g.html("");
        var h = gDom.find("th[f]");
        var p = gDom.find(".pager");

        if (!d[cfg.listvar] || d[cfg.listvar].length === 0) {
            var t = $("<tr><td cols=" + h.length + ">没有找到相关数据</td></tr>");
            p.hide();
            g.append(t);
            return;
        }

        var pz = cfg.searchcon.limit;
        var pc = Math.ceil(d.count / pz);
        //p.find(".pi").html(pz + "条/页，" + "共" + d.count + "条");//，当前" + d.page + "页/共" + pc + "页"

        //if (d.count <= pz) gDom.find(".pull-right.pagination").hide();
        //else { gDom.find(".pull-right.pagination").show(); }

        var pb = p.find(".pages");
        if (pc <= 1) p.hide();

        pb.html("");
        p.show();

        if (d.page > 1) pb.append("<span class='btn' onclick='g.getlist({page:" + (d.page - 1) + "})'><i class='icon-caret-left'></i></span>");
        pb.append("<span class='page_num'> " + d.page + " / " + pc + " </span>");
        if (d.page < pc) pb.append("<span class='btn' onclick='g.getlist({page:" + (d.page + 1) + "})'><i class='icon-caret-right'></i></span>");
        pb.append("<input type='text' id='page_goto'/><span class='btn' onclick='g.getlist({page:$(this).prev().val()})'>跳转</span>");

        //if (d.page > 1) pb.append('<li><a title="上一页" class="btn" i="' + (d.page - 1) + '"><i class="icon-angle-left"></i>上一页</a></li>');
        //var cp = cfg.cpage;
        //var s = d.page - Math.floor(cp / 2);
        //if (s <= 1) s = 1;
        //if (s > pc - cp) s = pc - cp + 1;
        //if (pc < cp) { s = 1; cp = pc; }
        //for (var i = s; i < s + cp; i++) {
        //    if (d.page == i) pb.append('<li><a title="第' + i + '页" class="btn btn-primary"  i="' + i + '">' + i + '</a></li>');
        //    else pb.append('<li><a title="第' + i + '页" class="btn"  i="' + i + '">' + i + '</a></li>');
        //}

        //if (d.page < pc) pb.append('<li><a title="下一页" class="btn"  i="' + (d.page + 1) + '">下一页<i class="icon-angle-right"></i></a></li>');
    }
    function rendertable(body, d) {
        body.append("<table class='table' grid='body'></table>");
        var table = body.find("table");
        for (var i = 0; i < d[cfg.listvar].length; i++) {
            var row = d.items[i];
            var tr = $("<tr class='grid-item'></tr>");
            tr.attr("data-id", i).attr("id", i);
            if (cfg.showcheckbox) tr.append("<td width='30' valign='center'><input type='checkbox' data-id='" + row["id"] + "' /></td>");
            if (cfg.shownumber) tr.append("<td width='20' valign='center' align='center' style='text-indent:0'>" + (i < 9 ? "0" : "") + (i + 1) + "</td>");

            for (var x = 0; x < cfg.fs.length; x++) {
                var fe = cfg.fs[x];

                var val = fe.f || "";

                val = val.replace(/{([\w_-\d]+)}/g, function (all, w) { return row[w] || ""; });

                var td = $("<td></td>");
                td.attr("title", val || "").attr("width", fe.w || "100");

                td.css({ "text-align": fe.align || "left" });

                if (fe.substr) {
                    var substr = fe.substr.split(',');
                    if (substr.length == 2) {
                        if (substr[0] && substr[1]) {
                            val = val.substr(substr[0], substr[1]);
                        } else if (substr[0]) {
                            val = val.substr(substr[0]);
                        } else {
                            val = val.substr(0, substr[1]);
                        }
                    }
                }

                if (fe.tpl) {
                    var t = fe.tpl;
                    t = t.replace(/{([\w_-\d]+)}/g, function (all, w) { return row[w] || ""; });
                    td.append(t);
                } else if (fe.link) {
                    var link = fe.link;
                    var text = fe.
                    link = link.replace(/{([\w_-\d]+)}/g, function (all, w) { return escape(escape(row[w])) || ""; });
                    td.append("<a href='" + link + "' target='" + fe.target + "'>" + (val || "点击打开") + "</a>");
                } else if (fe.act) {
                    td.attr("data-id", i).attr("data-act", fe.act).css("cursor", "pointer");
                    td.append(val);
                } else if (fe.chks) {
                    td.addClass("chks")
                    for (var c in fe.chks) {
                        var ck = fe.chks[c];
                        td.append("<span class='btn " + (row[ck.f] && "btn-primary") + "' data-id='" + i + "' data-act='chk_" + ck.f + "' data-val='" + row[ck.f] + "' title='" + ck.title + "'>" + (ck.icon ? "<i class='" + ck.icon + "'></i>" : ck.title) + "</span>")
                    }
                }
                else {
                    td.append(val);
                }
                tr.append(td);
            }

            var ops = $("<td class='ops'></td>");
            var btns = cfg.btns.slice(0);
            if (cfg.getbtns) btns = btns.concat(cfg.getbtns(row))
            for (var n in btns) {
                var b = btns[n];
                ops.append(getbtn(b, row, i));
            }
            tr.append(ops);
            table.append(tr);
        }
    }
    function getbtn(b, row, i) {
        var btn = $("<a class='btn'></a>").attr("title", b.txt);
        if (b.target) btn.attr("target", b.target);
        if (b.ico) btn.append("<i class='icon-" + b.ico + "'/> ");
        btn.append(b.txt);
        if (b.act) btn.data("confirm", b.confirm == undefined ? true : b.confirm).attr("data-id", i).attr("data-act", b.act).css("cursor", "pointer");
        else if (b.link) {
            var link = b.link;
            link = link.replace(/{([\w_-\d]+)}/g, function (all, w) { return escape(escape(row[w])) || ""; });
            btn.attr("href", link);
        }
        return btn;
    }
    function rendertpl(g, d) {
        for (var i = 0; i < d[cfg.listvar].length; i++) {
            var tpl = "<div class='grid-item'>" + $("<div></div>").append($(cfg.tpl).clone()).html() + "</div>";
            var r = d[cfg.listvar][i];
            tpl = tpl.replace(/{{(([\S ](?!{{))+)}}/ig, function (all, tp) {
                var tps = tp.split("--|-|--");
                var _items = r;
                var ps = tps[0].split(".");
                for (var p in ps) { _items = _items[ps[p]]; }
                var sbtp = "";
                for (var i in _items) {
                    var _da = _items[i];
                    sbtp += tps[1].replace("{_count}", _items.length).replace("{_index}", i).replace(/{([\w\d]+)}/ig, function (all, s1) { console.log(s1); return _da[s1]; });
                }
                return sbtp;
            });

            tpl = tpl.replace(/{([\w\d]+)}/g, function (all, w) { return r[w]; });

            var t = $(tpl);
            t.data("data", r);
            g.append(t);
        }
    }
    function render(d) {
        $(cfg.el).html(cfg.html.replace("{head}", cfg.tpl ? "" : gettablehead()));
        var g = gDom.find(".grid.body");
        if (cfg.showpager) {
            renderpager(g, d);
        }
        if (cfg.tpl) {
            rendertpl(g, d);
        } else {
            rendertable(g, d);
        }
        cfg.data = d[cfg.listvar];
        $(window).resize();
        if (cfg.rended) cfg.rended();
    }
}
x.grid.init = function (cfg) {
    var option = {
        el: null,
        limit: 10,
        api: "",
        searchcon: null,
        showcheckbox: false,
        showpager: true,
        rended: null,
        cpage: 5,//显示页码数
        fs: [],
        rowclick: null,
        listvar: "items",
        allselected: null,
        exheight: 0,
        btns: [],
        shownumber: true,
        html: '<div class="grid head">{head}</div>' +
              '<div class="grid body" exh="180"></div>' +
              '<div class="grid pager"><div class="pages"></div></div>'
    };
    cfg = $.extend(true, option, cfg);
    return new x.grid(cfg);
}