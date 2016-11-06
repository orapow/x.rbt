/// <reference path="jquery.js" />

/*#region 实用函数类*/
var cutils = {
    redirectBackurl: function (selector) {
        var back_url = $.cookie("login_backurl");
        if (back_url != null && back_url != "") {
            location.href = back_url;
        }
        else {
            location.href = "/member/index.aspx";
        }
    },
    saveBackurl: function () {
        var back_url = decodeURIComponent(utils.getquery("backurl"));
        if (back_url != null && back_url != "") {
            $.cookie("login_backurl", back_url, { domain: ".camel.com.cn" });
        }
    },
    redirectWap: function (pageUrl) {
        /// <summary>
        /// 判断来自网页端浏览器，并跳转到手机WAP网页端
        /// </summary>
        /// <param name="pageUrl" type="String">绝对页面链接</param>
        var userAgentInfo = navigator.userAgent;
        var Agents = ["Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod"];
        var flag = true;
        for (var v = 0; v < Agents.length; v++) {
            if (userAgentInfo.indexOf(Agents[v]) > 0) {
                flag = false;
                break;
            }
        }
        if (flag == false) {
            if (pageUrl == undefined) {
                pageUrl = "";
            }
            location.href = "http://m.camel.com.cn" + pageUrl;
        }
    }
}
/*#endregion*/

/*#region 弹窗*/

//弹出信息提示框
var alertBox = {
    error: function (message) {
        var alertwin = $("#alertwin_error");
        if (alertwin.length == 0) {
            var html = '<div id="alertwin_error" class="alertBoxGlobel" >' +
	                    '<div class="pr">' +
		                    '<a href="javascript:;" class="close">x</a>' +
		                    '<div class="hb">提示</div>' +
		                    '<div class="msg-content clearfix">' +
			                    '<div class="ar1 clearfix">' +
				                    '<span class="icon1 icon2 fl"></span>' +
				                    '<p class="msg"></p>' +
			                    '</div>' +
			                    '<div class="ar2">' +
				                    '<a href="javascript:;" class="btn1">确 定</a>' +
			                    '</div>' +
		                    '</div>' +
	                    '</div>' +
                    '</div>';
            alertwin = $(html);
            $('body').append(alertwin);
        }
        alertwin.find(".msg").text(message);
        var box = new LightBox("alertwin_error");
        box.show();
        alertwin.find(".btn1,.close").click(function () { box.close(); });
    },
    right: function (message) {
        var alertwin = $("#alertwin_right");
        if (alertwin.length < 1) {
            var html = '<div id="alertwin_right" class="alertBoxGlobel" >' +
	                '<div class="pr">' +
		                '<a href="javascript:;" class="close">x</a>' +
		                '<div class="hb">提示</div>' +
		                '<div class="msg-content clearfix">' +
			                '<div class="ar1 clearfix">' +
				                '<span class="icon1 fl"></span>' +
				                '<p class="msg"></p>' +
			                '</div>' +
			                '<div class="ar2">' +
				                '<a href="javascript:;" class="btn1">确 定</a>' +
			                '</div>' +
		                '</div>' +
	                '</div>' +
                '</div>';
            alertwin = $(html);
            $('body').append(alertwin);
        }
        alertwin.find(".msg").text(message);
        var box = new LightBox("alertwin_right");
        box.show();
        alertwin.find(".btn1,.close").click(function () { box.close(); });
    }
};
//弹出登录窗
var login_box = null;
var loginBox = {
    show: function () {
        var backurl = encodeURIComponent(location.href);
        if (login_box == null) {
            var html = '<div id="loginBar" class="alert-login">' +
	                '<div class="Shelter"></div>' +
	                '<div class="alogin-content">' +
		                '<a href="javascript:;" class="close es_close_btn">x</a>' +
		                '<div class="hb">用户登录</div>' +
                        '<div class="pr inputWrap">' +
		                '<div class="login-wrap clearfix">' +
                        '<form id="loginForm" action="" method="post">' +
			                '<div class="colm">' +
				                '<label for="reg-email" class="g-label" style="width:80px;">用户名/邮箱</label><input type="text" name="login_name" class="g-txt" class="reg-email" />' +
			                '</div>' +
			                '<div class="colm">' +
				                '<label for="reg-psd" class="g-label" style="width:80px;">密 码</label><input type="password" name="password" class="g-txt" class="reg-email" />' +
			                '</div>' +
			                '<p class="Protection" style="margin-left:85px;">' +
				                '<input id="checkbox" type="checkbox" name="save" value="1">\r\n' +
				                '<label for="checkbox">保存登录</label><a href="/findpassword.html?v=1" class="fogetPwd">忘记密码？</a>' +
			                '</p>' +
			                '<div class="colm">' +
				                '<input type="submit" value="立即登录" class="submitBtn" /><a href="/regist.html?backurl=' + backurl + '&v=1" class="register-link">注册</a>' +
			                '</div>	' +
                           '</form>' +
		                '</div>' +
		                '<div class="loopLogin">' +
			                '<a href="/handlers/qqlogin.ashx?backurl=' + backurl + '" class="coopinco"><img src="/images/coop_qq_03.gif?v=1" alt=""></a>' +
			                '<a href="/handlers/sinalogin.ashx?backurl=' + backurl + '" class="coopinco"><img src="/images/coop_sina_06.gif?v=1" alt=""></a>' +
			                '<a href="/handlers/alipaylogin.ashx?backurl=' + backurl + '" class="coopinco"><img src="/images/coop_ali_09.gif?v=1" alt=""></a>' +
                            '<a href="https://account.hnagroup.com/sso-login.do?appId=2492C04C4C7A4249BE1CFADF07AF6F96" class="coopinco"><img src="/images/coop_yzt_09.gif" alt=""></a>' +
		                '</div>' +
                        '<div class="scen-wx" id="scenWx">' +
				            '<a href="javascript:;" class="abtn" ></a>' +
				            '<div class="scen-wx-wrap" id="scenWrap">' +
					            '<p class="p1 tc">把“<span>骆驼</span>”装进手机，最新优惠活动抢先体验</p>' +
					            '<p class="p2 tc"><a href="javascript:;" class="back-login fr">返回登录</a><img src="/images/scen-wx.gif" alt="" /></p>' +
				            '</div>' +
			           '</div>' +
                    '</div>' +
	                '</div>' +
                '</div>';
            login_box = $(html);
            $('body').append(login_box);
        }
        var box = new LightBox("loginBar");
        box.show();
        var form = login_box.find("#loginForm");
        form.submit(function () {
            var loginName_input = login_box.find("input[name='login_name']");
            var password_input = login_box.find("input[name='password']");
            var user_name = $.trim(loginName_input.val());
            var password = $.trim(password_input.val());
            if (user_name == "") {
                alertBox.error("请输入账号");
                return false;
            }
            if (password == "") {
                alertBox.error("请输入密码");
                return false;
            }
            $(this).ajaxSubmit({ url: "/login.aspx?post=asynlogin", type: 'POST', success: function (data) {
                if (data == "1") {
                    location.href = location.href;
                }
                else {
                    loginName_input.val(""); password_input.val("");
                    alertBox.error("账号或密码有误");
                }
            }
            });
            box.close();
            return false;
        });
        login_box.find(".es_close_btn").click(function () {
            box.close();
        });
        //app二维码展示
        var wxBtn = login_box.find('#scenWx .abtn');
        var scenWrap = login_box.find('#scenWrap');
        var backLogin = login_box.find('#scenWrap .back-login');
        wxBtn.bind('click', function () {
            scenWrap.show();
            scenWrap.animate({ top: 0 + 'px' }, 300, 'swing');
        });
        backLogin.bind('click', function () {
            scenWrap.animate({ top: -270 + 'px' }, 300, 'swing', function () { scenWrap.hide(); });
        });
    }
}
//正在加载提示框
var loading_box = {
    show: function () {
        $("<div class='loading'><em>正在加载...</em></div>").ajaxStart(function () {
            $(this).appendTo('body').show().css("top", "50%");
        }).ajaxStop(function () {
            $(this).hide().remove();
        });
    },
    hide: function () {
        $(".loading").hide();
    }
};
/*#endregion*/





