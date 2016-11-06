/// <reference path="jquery.min.js" />

/*#region 全局变量*/
var isIE = (document.all) ? true : false;
var isIE6 = false;
var isIE7 = isIE && !isIE6 && !document.documentMode || document.documentMode == 7;
var isIE8 = isIE && !isIE6 && !isIE7 && !document.documentMode || document.documentMode == 8;
/*#endregion*/

/*#region 全局函数*/
var Class = { create: function () { return function () { this.initialize.apply(this, arguments); } } };
var Extend = function (destination, source) { for (var property in source) { destination[property] = source[property]; } };
var Bind = function (object, fun) { return function () { return fun.apply(object, arguments); } };
var Each = function (list, fun) { for (var i = 0, len = list.length; i < len; i++) { fun(list[i], i); } };
var Contains = function (a, b) { return a.contains ? a != b && a.contains(b) : !!(a.compareDocumentPosition(b) & 16); };
var $get = function (id) {
    ///<summary>
    ///代替document.getElementById()方法获取指定ID对象
    ///</summary>
    ///<param name="id" type="String">对象ID</param>
    return ("string" == typeof id ? document.getElementById(id) : id);
}
var $value = function (idName) {
    ///<summary>
    ///代替document.getElementById()或者document.getElementByName()方法获取指定ID或Name对象的value
    ///</summary>
    ///<param name="idName" type="String">对象ID或Name</param>
    ///<returns type="String" />
    var element = ("string" == typeof idName ? document.getElementById(idName) : idName);
    if (element == undefined || element == null) {
        var elements = document.getElementsByName(idName);
        if (elements.length > 0) { element = elements[0]; }
    }
    if (element != undefined && element != null) { return element.value; }
    else { return null; }
}
/*#endregion*/

/*#region 扩展*/

//方法的扩展
Function.prototype.setThis = function () { var curr_function = this; var to_this_object = arguments[0]; return function () { curr_function.apply(to_this_object, []); }; };

//删除字符串俩端的空格
String.prototype.trim = function () { return this.replace(/(^\s*)|(\s*$)/g, ""); };
String.prototype.leftTrim = function () { return this.replace(/(^\s*)/g, ""); };
String.prototype.rightTrim = function () { return this.replace(/(\s*$)/g, ""); };
String.prototype.allTrim = function () { return this.replace(/\s+/g, ""); };

/*#region 字符串表单验证扩展*/
String.prototype.matchEmpty = function () {
    /// <summary>
    /// 字符串是否为空
    /// </summary>
    if (this.trim() == "") { return false; }
    else { return true; }
};
String.prototype.matchRange = function (min, max) {
    /// <summary>
    /// 匹配字符串长度
    /// </summary>
    /// <param name="min" type="Number">字符串最小长度</param>
    /// <param name="max" type="Number">字符串最大长度</param>
    /// <returns type="Boolean" />
    if (!this.match("^[^\S]{" + min + "," + max + "}$")) { return false; }
    else { return true; }
};
String.prototype.matchByteRange = function (min, max) {
    /// <summary>
    /// 匹配字符串字节长度，1个中文2个字节
    /// </summary>
    /// <param name="min" type="Number">字符串最小长度</param>
    /// <param name="max" type="Number">字符串最大长度</param>
    /// <returns type="Boolean" />
    var value = this;
    var length = value.length;
    for (var i = 0; i < value.length; i++) { if (value.charCodeAt(i) > 127) { length++; } }
    if (length >= min && length <= max) { return true; }
    else { return false; }
};
String.prototype.matchInt = function () {
    /// <summary>
    /// 匹配正整数
    /// </summary>
    /// <returns type="Boolean" />
    if (!this.matchIntNumber(false)) { return false; }
    else { return true; }
};
String.prototype.matchIntNumber = function (allowNegative) {
    /// <summary>
    /// 匹配整数
    /// </summary>
    /// <param name="allowNegative" type="Boolean">是否允许负数</param>
    /// <returns type="Boolean" />
    if (!this.match("^" + (allowNegative ? "[\-]?" : "") + "[0-9]+$")) { return false; }
    else { return true; }
};
String.prototype.matchFixedInt = function (size) {
    /// <summary>
    /// 匹配固定位数的正整数
    /// </summary>
    /// <param name="size" type="Number">整数位数</param>
    /// <returns type="Boolean" />
    if (!this.match("^[0-9]{" + size + "}$")) { return false; }
    else { return true; }
};
String.prototype.matchFloat = function () {
    /// <summary>
    /// 匹配正小数
    /// </summary>
    /// <returns type="Boolean" />
    return this.matchFloatNumber(false);
};
String.prototype.matchFloatNumber = function (allowNegative) {
    /// <summary>
    /// 匹配小数
    /// </summary>
    /// <param name="allowNegative" type="Boolean">是否允许负数</param>
    /// <returns type="Boolean" />
    if (!this.match("^" + (allowNegative ? "[\-]?" : "") + "[0-9]+[\.]?[0-9]+$")) { return false; }
    else { return true; }
};
String.prototype.matchTelphone = function () {
    /// <summary>
    /// 匹配固话号码，必须符合87654321|020-87654321(带区号)|87654321-01(带分机号)这样的格式
    /// </summary>
    /// <returns type="Boolean" />
    if (!this.match("(^[0-9]{3,4}\-[0-9]{8}$)|(^[0-9]{8}$)|(^[0-9]{8}\-[0-9]{3,5}$)|(^[0-9]{3,4}\-[0-9]{8}\-[0-9]{3,5}$)")) { return false; }
    else { return true; }
};
String.prototype.matchCellphone = function () {
    /// <summary>
    /// 匹配手机号码，必须符合13660183547共11位数字这样的格式
    /// </summary>
    /// <returns type="Boolean" />
    return this.matchFixedInt(11);
};
String.prototype.matchCLetter = function (min, max) {
    /// <summary>
    /// 匹配中文和字母，全字母或全汉字字符
    /// </summary>
    /// <param name="min" type="Number">字符串最小长度</param>
    /// <param name="max" type="Number">字符串最大长度</param>
    /// <returns type="Boolean" />
    if (!this.match("(^[A-Z a-z]{" + min + "," + max + "}$)|(^[\u4e00-\u9fa5]{" + min + "," + max + "}$)")) { return false; }
    else { return true; }
};
String.prototype.matchNumLetter = function (min, max) {
    /// <summary>
    /// 匹配数字和字母、下划线，只包含数字和字母、下划线
    /// </summary>
    /// <param name="min" type="Number">字符串最小长度</param>
    /// <param name="max" type="Number">字符串最大长度</param>
    /// <returns type="Boolean" />
    if (min == undefined || max == undefined) {
        if (!this.match("^[a-zA-Z0-9_]+$")) { return false; }
        else { return true; }
    }
    if (!this.match("^[a-zA-Z0-9_]{" + min + "," + max + "}$")) { return false; }
    else { return true; }
};
String.prototype.matchEmail = function () {
    /// <summary>
    /// 匹配电子邮箱
    /// </summary>
    /// <returns type="Boolean" />
    //^[a-zA-Z0-9_]+@[a-zA-Z0-9]+[\.][a-zA-Z]{2,4}$
    if (!this.match("^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$")) { return false; }
    else { return true; }
};
String.prototype.matchMail = function () {
    /// <summary>
    /// 匹配邮编，6位长度的纯数字
    /// </summary>
    return this.matchFixedInt(6);
};
String.prototype.matchUserCode = function (min, max) {
    /// <summary>
    /// 匹配用户帐号
    /// </summary>
    /// <param name="min" type="Number">字符串最小长度</param>
    /// <param name="max" type="Number">字符串最大长度</param>
    /// <returns type="Boolean" />

    if (!this.match("^[A-Za-z0-9_]{" + min + "," + max + "}$")) { return false; }
    else { return true; }
};
String.prototype.matchAdminCode = function (min, max) {
    /// <summary>
    /// 匹配管理员帐号
    /// </summary>
    /// <param name="min" type="Number">字符串最小长度</param>
    /// <param name="max" type="Number">字符串最大长度</param>
    /// <returns type="Boolean" />
    if (!this.match("^[A-Za-z]{1}[A-Za-z0-9_]{" + (min - 1) + "," + (max - 1) + "}$")) { return false; }
    else { return true; }
};
String.prototype.matchPassword = function (min, max) {
    /// <summary>
    /// 匹配密码
    /// </summary>
    /// <param name="min" type="Number">字符串最小长度</param>
    /// <param name="max" type="Number">字符串最大长度</param>
    /// <returns type="Boolean" />
    if (!this.match("^[A-Za-z0-9_]{" + min + "," + max + "}$")) { return false; }
    else { return true; }
};
String.prototype.matchUrl = function () {
    /// <summary>
    /// 匹配URL
    /// </summary>
    /// <returns type="Boolean" />
    var str_url = this.toLowerCase();
    var stringBuilder = new StringBuilder();
    stringBuilder.append("^((https|http|ftp|rtsp|mms)?://)");
    stringBuilder.append("?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?"); //ftp的user@ 
    stringBuilder.append("(([0-9]{1,3}\.){3}[0-9]{1,3}"); // IP形式的URL- 199.194.52.184 
    stringBuilder.append("|"); // 允许IP和DOMAIN（域名）
    stringBuilder.append("([0-9a-z_!~*'()-]+\.)*"); // 域名- www. 
    stringBuilder.append("([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\."); // 二级域名 
    stringBuilder.append("[a-z]{2,6})"); // first level domain- .com or .museum 
    stringBuilder.append("(:[0-9]{1,4})?"); // 端口- :80 
    stringBuilder.append("((/?)|"); // a slash isn't required if there is no file name 
    stringBuilder.append("(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$");
    var re = new RegExp(stringBuilder.toString());
    if (!re.test(str_url)) { return false; }
    else { return true; }
};

String.prototype.matchFile = function (suffixs) {
    /// <summary>
    /// 匹配文件地址类型
    /// </summary>
    /// <param name="suffixs" type="String">文件后缀格式集合，多个后缀用英文,隔开</param>
    /// <returns type="Boolean" />
    //if (!this.match("(^\\.|^/|^[a-zA-Z])?:?/.+(/$)?")) { return false; }
    if (!suffixs) { suffixs = "gif,jpg,jpeg,png,bmp,swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb,doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2"; }
    var suffix = this.substring(this.lastIndexOf('.') + 1).toLowerCase();
    var existSuffix = false;
    var suffixStr = "";
    var suffixArray = suffixs.split(",");
    for (var i = 0; i < suffixArray.length; i++) { if (suffixArray[i] == suffix) { existSuffix = true; } }
    return existSuffix;
};
String.prototype.matchImg = function (suffixs) {
    /// <summary>
    /// 匹配图片文件地址类型
    /// </summary>
    /// <param name="suffixs" type="String">文件格式，默认：gif,jpg,jpeg,png,bmp</param>
    /// <returns type="Boolean" />
    if (suffixs == null || suffixs == "") { suffixs = "gif,jpg,jpeg,png,bmp"; }
    return this.matchFile(suffixs);
};
String.prototype.matchFlash = function (alertHead, allowEmpty, showAlert, suffixs) {
    /// <summary>
    /// 匹配flash文件地址类型
    /// </summary>
    /// <param name="suffixs" type="String">文件格式，默认：swf,flv</param>
    /// <returns type="Boolean" />
    if (suffixs == null || suffixs == "") { suffixs = "swf,flv"; }
    return this.matchFile(suffixs);
};
String.prototype.matchMedia = function (suffixs) {
    /// <summary>
    /// 匹配媒体视频文件地址类型
    /// </summary>
    /// <param name="suffixs" type="String">文件格式，默认：swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb</param>
    /// <returns type="Boolean" />
    if (suffixs == null || suffixs == "") { suffixs = "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb"; }
    return this.matchFile(alertHead, allowEmpty, suffixs, showAlert);
};
String.prototype.matchAttach = function (suffixs) {
    /// <summary>
    /// 匹配附件文件地址类型
    /// </summary>
    /// <param name="suffixs" type="String">文件格式，默认：doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2</param>
    /// <returns type="Boolean" />
    if (suffixs == null || suffixs == "") { suffixs = "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2"; }
    return this.matchFile(suffixs);
};

/*#endregion*/

/*Array扩展*/
//Array.prototype.clear = function () { this.length = 0; }; //清空
//Array.prototype.insertAt = function (index, obj) { this.splice(index, 0, obj); }; //插入对象到指定位置
//Array.prototype.removeAt = function (index) { this.splice(index, 1); }; //移除指定位置的对象
//Array.prototype.remove = function (obj) { var index = this.indexOf(obj); if (index >= 0) { this.removeAt(index); } }; //移除对象

//String扩展
String.format = function () {
    if (arguments.length == 0)
        return null;
    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
}
/**
* 时间对象的格式化;
*/
Date.prototype.format = function (format) {
    /*eg:format="yyyy-MM-dd hh:mm:ss"; */
    var o = {
        "M+": this.getMonth() + 1, // month
        "d+": this.getDate(), // day
        "h+": this.getHours(), // hour
        "m+": this.getMinutes(), // minute
        "s+": this.getSeconds(), // second
        "q+": Math.floor((this.getMonth() + 3) / 3), // quarter
        "S": this.getMilliseconds() // millisecond
    }
    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}

/*#endregion*/

/*#region 实用函数类*/
var utils = {
    convertJsonDate: function (jsonDateString) {
        var date = new Date(parseInt(jsonDateString.replace("/Date(", "").replace(")/", ""), 10));
        return date;
    },
    callback: function () {
        if (history.length > 1)
            history.go(-1);
        else
            location.href = "/";
    },
    getYearArray: function (startYear) {
        /// <summary>
        /// 获取以当前年结束的数组集合
        /// </summary>
        /// <param name="startYear" type="Number">开始年</param>
        /// <returns type="Array" />
        var YearArray = new Array(); var NowDate = new Date(); var NowYear = NowDate.getYear();
        for (var i = NowYear; i >= startYear; i--) { YearArray.push(i + "," + i + "年"); }
        return YearArray;
    },
    removeAllChild: function (canObj) {
        /// <summary>
        /// 移除对象所有子节点
        /// </summary>
        if (canObj) { var count = canObj.childNodes.length; for (var i = 0; i < count; i++) { canObj.removeChild(canObj.childNodes[0]); } }
    },
    appendCssFile: function (cssHref) {
        /// <summary>
        /// 附加css文件到文档头部（head标签内）
        /// </summary>
        /// <param name="cssHref" type="String">css文件虚拟路径</param>
        var head = document.getElementsByTagName("HEAD")[0]; var style = document.createElement("link"); style.href = cssHref; style.rel = "stylesheet"; style.type = "text/css"; head.appendChild(style);
    },
    checkAll: function (sender, selector) {
        /// <summary>
        /// 选取所有同css名称的checkbox
        /// </summary>
        /// <param name="sender" type="Object">用于点击全选的checkBox对象</param>
        /// <param name="selector" type="String">统一的css选择器</param>
        var jsender = $(sender);
        var checkItems = $(":checkbox").filter(selector);
        if (jsender.hasClass("selected")) {
            jsender.removeClass("selected");
            checkItems.removeAttr("checked");
        }
        else {
            jsender.addClass("selected");
            checkItems.attr("checked", "checked");
        }
    },
    gethash: function () {
        /// <summary>
        /// 获取当前URL #后面的参数
        /// </summary>
        var hash = window.location.hash;
        if (hash != undefined && hash != "" && hash != "#") {
            return hash.substring(1);
        }
        else {
            return "";
        }
    },
    addFavorite: function (sURL, sTitle) {
        /// <summary>
        /// 添加到收藏夹
        /// </summary>
        /// <param name="sURL" type="String">收藏的页面URL</param>
        /// <param name="sTitle" type="String">收藏的页面标题</param>
        try { window.external.addFavorite(sURL, sTitle); }
        catch (e) { try { window.sidebar.addPanel(sTitle, sURL, ""); } catch (e) { alert("加入收藏失败，请使用Ctrl+D进行添加"); } }
    },
    checkIntFormer: function (obj, formerData) {
        /// <summary>
        /// 检查整形输入，非整形的输入还原到原来字符串
        /// </summary>
        /// <param name="obj" type="Object">检查的对象</param>
        /// <param name="formerData" type="String">验证失败后还原的值，默认为空字符串</param>
        if (!obj.value.match("^[\-]?[0-9]+$")) {
            if (formerData == null) { obj.value = ""; }
            else { obj.value = formerData; }
        }
    },
    checkFloatFormer: function (obj, formerData, p_max) {
        /// <summary>
        /// 检查小数输入，非小数的输入还原到原来字符串
        /// </summary>
        /// <param name="obj" type="Object">检查的对象</param>
        /// <param name="formerData" type="String">验证失败后还原的值，默认为空字符串</param>
        /// <param name="p_max" type="Number">小数点后最大位数，默认为0</param>
        var maxStr = "";
        if (p_max != null) { maxStr = "{0," + p_max + "}"; }
        if (!obj.value.match("^[\-]?[0-9]+[\.]?[0-9]" + maxStr + "$")) {
            if (formerData == null) { obj.value = ""; }
            else {
                if (p_max == null) { obj.value = formerData; }
                else { obj.value = formerData.toFixed(1); }
            }
        }
    },
    stopBubble: function (e) {
        /// <summary>
        /// 阻止冒泡
        /// </summary>
        /// <param name="e" type="Event">出发事件的事件对象</param>
        if (e && e.stopPropagation) { e.stopPropagation(); }
        else { window.event.cancelBubble = true; }
    },
    copyText: function (meintext) {
        /// <summary>
        /// 复制文本
        /// </summary>
        /// <param name="meintext" type="String">文本</param>
        if (window.clipboardData) {
            window.clipboardData.setData("Text", meintext);
        }
        else if (window.netscape) {
            try {
                netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
            } catch (e) {
                alert("你使用的FF浏览器,复制功能被浏览器拒绝！\n请在浏览器地址栏输入'about:config'并回车\n然后将 'signed.applets.codebase_principal_support'设置为'true'");
                return;
            }
            var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
            if (!clip) return;
            var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
            if (!trans) return;
            trans.addDataFlavor('text/unicode');
            var str = new Object();
            var len = new Object();
            var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
            var copytext = meintext;
            str.data = copytext;
            trans.setTransferData("text/unicode", str, copytext.length * 2);
            var clipid = Components.interfaces.nsIClipboard;
            if (!clip) return false;
            clip.setData(trans, null, clipid.kGlobalClipboard);
        }
        alert("内容已成功被复制到剪贴板:\n\n");
        return false;

    },
    submitFormEx: function (onSubmit) {
        /// <summary>
        /// 表单提交并检查异常
        /// </summary>
        /// <param name="onSubmit" type="Function">提交函数</param>
        if (onSubmit != undefined) {
            try {
                return onSubmit();
            }
            catch (ex) {
                alert(ex.Message);
                return true;
            }
            finally {

            }
        }
    },
    redirect: function (url) {
        /// <summary>
        /// 重定向
        /// </summary>
        /// <param name="url" type="String">调转到的地址</param>
        location.href = url;
    },
    action: function (sender, action) {
        /// <summary>
        /// 提交表单
        /// </summary>
        /// <param name="sender" type="Object">操作触发对象</param>
        /// <param name="action" type="String">操作名称</param>
        var form = $(sender).parents("form");
        form.attr("post", function (index, ovalue) {
            var urlop = new objURL(ovalue);
            urlop.set("post", action);
            return urlop.url();
        });
    },
    copyCommand: function (sender) {
        utils.action(sender, "copy");
        $(sender).parents("form").submit();
    },
    scrollAnchor: function (anchorId, speed) {
        /// <summary>
        /// 滚动到锚
        /// </summary>
        /// <param name="anchorId" type="String">锚标签ID</param>
        /// <param name="speed" type="Number">动画速度</param>
        if (speed == undefined)
            speed = 1000;
        $("html,body").animate({ scrollTop: $("#" + anchorId).offset().top }, speed);
    },
    autoSkip: function (count_selector, back_url, time) {
        /// <summary>
        /// 自动跳转
        /// </summary>
        /// <param name="count_selector" type="String">倒计时区域选择器</param>
        /// <param name="back_url" type="String">跳转到的链接</param>
        if (time == undefined)
            time = 5;
        var timer = setInterval(function () {
            if (count_selector != undefined) {
                $(count_selector).html(time);
            }
            time -= 1;
            if (time <= 0) {
                //跳转
                if (back_url != undefined && back_url != "") {
                    location.href = back_url;
                }
                else {
                    if (history.length > 1)
                        history.go(-1);
                }
                clearInterval(timer);
            }
        }, 1000);
    },
    getEmailType: function (email) {
        /// <summary>
        /// 获取Email类型
        /// </summary>
        /// <param name="email" type="String">邮箱地址</param>
        var suffUrl = [
		{ id: "sina.com.cn", url: "http://mail.sina.com.cn/", name: "新浪邮箱" },
		{ id: "sina.com", url: "http://mail.sina.com.cn/", name: "新浪邮箱" },
		{ id: "sina.cn", url: "http://mail.sina.com.cn/", name: "新浪邮箱" },
		{ id: "vip.sina.com", url: "http://vip.sina.com.cn/", name: "VIP新浪邮箱" },
		{ id: "2008.sina.com", url: "http://mail.sina.com.cn/", name: "新浪邮箱" },
		{ id: "163.com", url: "http://mail.163.com/", name: "网易163邮箱" },
		{ id: "126.com", url: "http://mail.126.com/", name: "网易126邮箱" },
		{ id: "popo.163.com", url: "http://popo.163.com/", name: "网易POPO邮箱" },
		{ id: "yeah.net", url: "http://yeah.net/", name: "网易Yeah邮箱" },
		{ id: "vip.163.com", url: "http://vip.163.com/", name: "VIP网易163邮箱" },
		{ id: "vip.126.com", url: "http://vip.126.com/", name: "VIP网易126邮箱" },
		{ id: "188.com", url: "http://188.com/", name: "网易188财富邮" },
		{ id: "vip.188.com", url: "http://vip.188.com/", name: "网易188财富邮" },
		{ id: "tom.com", url: "http://mail.tom.com/", name: "TOM邮箱" },
		{ id: "yahoo.com", url: "http://mail.cn.yahoo.com/", name: "雅虎邮箱" },
		{ id: "yahoo.com.cn", url: "http://mail.cn.yahoo.com/", name: "雅虎邮箱" },
		{ id: "yahoo.cn", url: "http://mail.cn.yahoo.com/", name: "雅虎邮箱" },
		{ id: "sohu.com", url: "http://mail.sohu.com/", name: "搜狐邮箱" },
		{ id: "hotmail.com", url: "https://login.live.com/", name: "Hotmail邮箱" },
		{ id: "139.com", url: "http://mail.10086.cn/", name: "139邮箱" },
		{ id: "gmail.com", url: "https://accounts.google.com", name: "Gmail邮箱" },
		{ id: "msn.com", url: "https://login.live.com", name: "MSN邮箱" },
		{ id: "51.com", url: "http://passport.51.com/", name: "51邮箱" },
		{ id: "yougou.com", url: "http://mail.yougou.com/", name: "QQ企业邮箱" },
		{ id: "qq.com", url: "https://mail.qq.com", name: "QQ邮箱" },
		{ id: "foxmail.com", url: "http://foxmail.com", name: "Foxmail邮箱" },
		{ id: "vip.qq.com", url: "http://mail.qq.com", name: "QQ邮箱" }
	 ];
        var index = email.indexOf("@");
        var subStr = email.substring(index + 1).replace(/\./g, "-");
        var suffIndext = subStr.indexOf(".");
        var exist = false;
        var emailType;
        $.each(suffUrl, function (n, value) {
            var emailId = value.id.replace(/\./g, "-");
            if (subStr == emailId) {
                emailType = value;
                exist = true;
            }
        });
        if (!exist) {
            emailType = { id: email.substring(index + 1), url: "http://www." + email.substring(index + 1), name: "邮箱" };
        }
        return emailType;
    },
    timing: function (seconds) {
        if (seconds > 0) {
            var time_distance = seconds;
            var int_day = Math.floor(time_distance / 86400);
            time_distance -= int_day * 86400;
            var int_hour = Math.floor(time_distance / 3600);
            time_distance -= int_hour * 3600;
            var int_minute = Math.floor(time_distance / 60);
            time_distance -= int_minute * 60;
            var int_second = time_distance;
            var time = new Object();
            time.day = int_day.toFixed("f0");
            time.hour = int_hour.toFixed("f0");
            time.minute = int_minute.toFixed("f0");
            time.second = int_second.toFixed("f0");
            return time;
        }
    },
    getquery: function (param_key, url) {
        if (url == undefined) {
            url = location.href;
        }
        var urlo = new objURL(url);
        return urlo.get(param_key);
    },
    clipboard: function (textId, buttonId, completeFun) {
        /// <summary>
        /// 复制黏贴（ZeroClipboard）插件调用
        /// </summary>
        /// <param name="textId" type="String">文本输入框Id</param>
        /// <param name="buttonId" type="String">复制按钮Id</param>
        /// <param name="completeFun" type="Function">复制完成调用函数</param>
        var clip = new ZeroClipboard.Client();
        if (clip == undefined || clip == null) {
            alert("ZeroClipboard加载失败，请确认引用ZeroClipboard.js");
            return;
        }
        clip.setHandCursor(true);
        clip.addEventListener("mouseover", function (client) {
            clip.setText(document.getElementById(textId).value);
        });
        clip.addEventListener("complete", function (client, text) {
            if (completeFun != undefined) {
                completeFun(text)
            }
            else {
                alert("复制成功");
            }
        });
        clip.glue(buttonId);
        return clip;
    }
};
/*#endregion*/

/*#region 表单验证*/
var FormMatch = function (onWarn) {
    /// <summary>
    /// 表单输入匹配
    /// </summary>
    /// <param name="onWarn" type="Function">触发提示</param>
    this.OnWarn = onWarn ? onWarn : function (meg) {
        alert(meg);
    };
    this.Id = null;
};
FormMatch.prototype = {
    $: function (id) {
        /// <summary>
        /// 设置当前ID
        /// </summary>
        /// <param name="id" type="String">验证对象ID</param>
        /// <returns type="FormMatch">FormMatch对象</returns>
        this.Id = id;
        return this;
    },
    allowEmpty: function (empty) {
        /// <summary>
        /// 是否允许为空
        /// </summary>
        /// <returns type="Boolean"/>
        if (empty && !$value(this.Id).matchEmpty()) { return true; }
        return false;
    },
    matchEmpty: function (meg) {
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns type="Boolean" />
        if ($value(this.Id).matchEmpty()) { return true; }
        else { this.OnWarn(meg); }
        return false;
    },
    matchRange: function (min, max, meg, empty) {
        /// <summary>
        /// 匹配字符串长度
        /// </summary>
        /// <param name="min" type="Number">最小长度</param>
        /// <param name="max" type="Number">最大长度</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchRange(min, max)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchByteRange: function (min, max, meg, empty) {
        /// <summary>
        /// 匹配字符串字节长度，1个中文2个字节
        /// </summary>
        /// <param name="min" type="Number">最小长度</param>
        /// <param name="max" type="Number">最大长度</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchByteRange(min, max)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchIntNumber: function (negative, meg, empty) {
        /// <summary>
        /// 匹配整数
        /// </summary>
        /// <param name="negative" type="Boolean">是否允许负数</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchIntNumber(negative)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchInt: function (meg, empty) {
        /// <summary>
        /// 匹配正整数
        /// </summary>
        /// <returns type="Boolean" />
        return this.matchIntNumber(false, meg, empty);
    },
    matchFixedInt: function (size, meg, empty) {
        /// <summary>
        /// 匹配固定位数的正整数
        /// </summary>
        /// <param name="size" type="Number">固定位数</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchFixedInt(size)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchFloatNumber: function (negative, meg, empty) {
        /// <summary>
        /// 匹配小数
        /// </summary>
        /// <param name="allowNegative" type="Boolean">是否允许负数</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchFloatNumber(negative)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchFloat: function (meg, empty) {
        return this.matchFloatNumber(false, meg, empty);
    },
    matchTelphone: function (meg, empty) {
        /// <summary>
        /// 匹配固话号码
        /// </summary>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchTelphone()) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchCellphone: function (meg, empty) {
        /// <summary>
        /// 匹配手机号码
        /// </summary>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchCellphone()) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchCLetter: function (min, max, meg, empty) {
        /// <summary>
        /// 匹配中文和字母，全字母或全汉字字符
        /// </summary>
        /// <param name="min" type="Number">字符串最小长度</param>
        /// <param name="max" type="Number">字符串最大长度</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchCLetter(min, max)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchNumLetter: function (min, max, meg, empty) {
        /// <summary>
        /// 匹配数字和字母、下划线，只包含数字和字母、下划线
        /// </summary>
        /// <param name="min" type="Number">字符串最小长度</param>
        /// <param name="max" type="Number">字符串最大长度</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchNum_Letter(min, max)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchEmail: function (meg, empty) {
        /// <summary>
        /// 匹配电子邮箱
        /// </summary>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchEmail()) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchMail: function (meg, empty) {
        /// <summary>
        /// 匹配邮编，6位长度的纯数字
        /// </summary>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchMail()) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchUserCode: function (min, max, meg, empty) {
        /// <summary>
        /// 匹配会员帐号
        /// </summary>
        /// <param name="min" type="Number">字符串最小长度</param>
        /// <param name="max" type="Number">字符串最大长度</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchUserCode(min, max)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchAdminCode: function (min, max, meg, empty) {
        /// <summary>
        /// 匹配管理员帐号
        /// </summary>
        /// <param name="min" type="Number">字符串最小长度</param>
        /// <param name="max" type="Number">字符串最大长度</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchAdminCode(min, max)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchPassword: function (min, max, meg, password2, empty) {
        /// <summary>
        /// 匹配密码
        /// </summary>
        /// <param name="min" type="Number">字符串最小长度</param>
        /// <param name="max" type="Number">字符串最大长度</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if (password2 == null) { password2 = $value(this.Id); }
        if (!$value(this.Id).matchPassword(min, max)) { this.OnWarn(meg); return false; }
        if ($value(this.Id) != password2) { this.OnWarn("两次输入的密码不一致"); return false; }
        return true;
    },
    matchUrl: function (meg, empty) {
        /// <summary>
        /// 匹配URL
        /// </summary>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchUrl()) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchImg: function (meg, empty, suffixs) {
        /// <summary>
        /// 匹配图片文件地址类型
        /// </summary>
        /// <param name="suffixs" type="String">文件格式</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchImg(suffixs)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchFlash: function (meg, empty, suffixs) {
        /// <summary>
        /// 匹配flash文件地址类型
        /// </summary>
        /// <param name="suffixs" type="String">文件格式</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchFlash(suffixs)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchMedia: function (meg, empty, suffixs) {
        /// <summary>
        /// 匹配媒体视频文件地址类型
        /// </summary>
        /// <param name="suffixs" type="String">文件格式</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchMedia(suffixs)) { return true; }
        else { this.OnWarn(meg); return false; }
    },
    matchAttach: function (meg, empty, suffixs) {
        /// <summary>
        /// 匹配附件文件地址类型
        /// </summary>
        /// <param name="suffixs" type="String">文件格式</param>
        /// <returns type="Boolean" />
        if (this.allowEmpty(empty)) { return true; }
        if (!this.matchEmpty(meg)) { return false; }
        if ($value(this.Id).matchAttach(suffixs)) { return true; }
        else { this.OnWarn(meg); return false; }
    }
}
/*#endregion*/

/*#region 漂浮层对齐*/
var OverLay = function (options) {
    /// <summary>
    /// 漂浮层对齐 构造函数
    /// </summary>
    /// <param name="options" type="Array">
    /// 1. Lay - 覆盖层对象ID
    /// 2. Color - 背景色，默认#000
    /// 3. Opacity - 透明度(0-100)，默认30
    /// 4. zIndex - 层叠顺序，默认1000
    /// </param>
    this.setOptions(options);
    this.Lay = document.getElementById(this.options.Lay) || document.body.insertBefore(document.createElement("div"), document.body.childNodes[0]);
    this.Color = this.options.Color;
    this.Opacity = parseInt(this.options.Opacity);
    this.zIndex = parseInt(this.options.zIndex);
    with (this.Lay.style) { display = "none"; zIndex = this.zIndex; left = top = 0; position = "fixed"; width = height = "100%"; }
    if (isIE6) {
        this.Lay.style.position = "absolute";
        //ie6设置覆盖层大小程序
        this._resize = Bind(this, function () {
            this.Lay.style.width = Math.max(document.documentElement.scrollWidth, document.documentElement.clientWidth) + "px";
            this.Lay.style.height = Math.max(document.documentElement.scrollHeight, document.documentElement.clientHeight) + "px";
        });
        //遮盖select
        this.Lay.innerHTML = '<iframe style="position:absolute;top:0;left:0;width:100%;height:100%;filter:alpha(opacity=0);"></iframe>'
    }
};
OverLay.prototype = {
    //设置默认属性
    setOptions: function (options) {
        this.options = {//默认值
            Lay: null, //覆盖层对象
            Color: "#000", //背景色
            Opacity: 10, //透明度(0-100)
            zIndex: 102//层叠顺序
        };
        Extend(this.options, options || {});
    },
    show: function () {
        /// <summary>
        /// 显示漂浮层
        /// </summary>
        //兼容ie6
        if (isIE6) { this._resize(); window.attachEvent("onresize", this._resize); }
        //设置样式
        with (this.Lay.style) {
            //设置透明度
            isIE ? filter = "alpha(opacity:" + this.Opacity + ")" : opacity = this.Opacity / 100;
            backgroundColor = this.Color; display = "block";
        }
    },
    close: function () {
        /// <summary>
        /// 关闭漂浮层
        /// </summary>
        this.Lay.style.display = "none";
        if (isIE6) { window.detachEvent("onresize", this._resize); }
    }
};
/*#endregion*/

/*#region 弹出关闭漂浮层*/
var LightBox = function (boxId, options) {
    /// <summary>
    /// 弹出关闭漂浮层 构造函数
    /// </summary>
    /// <param name="box" type="String">显示层ID</param>
    /// <param name="options" type="Array">
    /// 1. Lay - 覆盖层对象ID
    /// 2. Color - 背景色，默认#000
    /// 3. Opacity - 透明度(0-100)，默认30
    /// 4. zIndex - 层叠顺序，默认100
    /// 5. Fixed - 是否固定定位，默认true
    /// 6. Center - 是否居中，默认true
    /// 7. onShow - 显示时执行的方法
    /// </param>
    this.Box = document.getElementById(boxId); //显示层
    this.OverLay = new OverLay(options); //覆盖层
    this.setOptions(options);
    this.Fixed = !!this.options.Fixed;
    this.Over = !!this.options.Over;
    this.Center = !!this.options.Center;
    this.onShow = this.options.onShow;
    this.Box.style.zIndex = this.OverLay.zIndex + 1;
    this.Box.style.display = "none";
    //兼容ie6用的属性
    if (isIE6) {
        this._top = this._left = 0; this._select = [];
        this._fixed = Bind(this, function () { this.Center ? this.setCenter() : this.setFixed(); });
    }
};
LightBox.prototype = {
    //设置默认属性
    setOptions: function (options) {
        this.options = {//默认值
            Over: true, //是否显示覆盖层
            Fixed: true, //是否固定定位
            Center: true, //是否居中
            onShow: function () { } //显示时执行
        };
        Extend(this.options, options || {});
    },
    setFixed: function () {
        /// <summary>
        /// 兼容ie6的固定定位程序
        /// </summary>
        this.Box.style.top = document.documentElement.scrollTop - this._top + this.Box.offsetTop + "px";
        this.Box.style.left = document.documentElement.scrollLeft - this._left + this.Box.offsetLeft + "px";

        this._top = document.documentElement.scrollTop; this._left = document.documentElement.scrollLeft;
    },
    setCenter: function () {
        /// <summary>
        /// 兼容ie6的居中定位程序
        /// </summary>
        this.Box.style.marginTop = document.documentElement.scrollTop - this.Box.offsetHeight / 2 + "px";
        this.Box.style.marginLeft = document.documentElement.scrollLeft - this.Box.offsetWidth / 2 + "px";
    },
    show: function (options) {
        /// <summary>
        /// 显示
        /// </summary>
        //固定定位
        this.Box.style.position = this.Fixed && !isIE6 ? "fixed" : "absolute";
        //覆盖层
        this.Over && this.OverLay.show();
        this.Box.style.display = "block";

        //居中
        if (this.Center) {
            //设置margin
            if (this.Fixed) {
                this.Box.style.marginTop = -(this.Box.offsetHeight / 2) + "px";
                this.Box.style.marginLeft = -(this.Box.offsetWidth / 2) + "px";
            } else {
                this.setCenter();
            }
            this.Box.style.top = this.Box.style.left = "50%";
        }

        //兼容ie6
        if (isIE6) {
            if (!this.Over) {
                //没有覆盖层ie6需要把不在Box上的select隐藏
                this._select.length = 0;
                Each(document.getElementsByTagName("select"), Bind(this, function (o) {
                    if (!Contains(this.Box, o)) { o.style.visibility = "hidden"; this._select.push(o); }
                }))
            }
            //设置显示位置
            this.Center ? this.setCenter() : this.Fixed && this.setFixed();
            //设置定位
            this.Fixed && window.attachEvent("onscroll", this._fixed);
        }

        this.onShow();
    },
    //关闭
    close: function () {
        this.Box.style.display = "none";
        this.OverLay.close();
        if (isIE6) {
            window.detachEvent("onscroll", this._fixed);
            Each(this._select, function (o) { o.style.visibility = "visible"; });
        }
    }
};
/*#endregion*/

/*#region StringBuilder*/
function StringBuilder() {
    this._strings = new Array();
}
//append方法定义
StringBuilder.prototype.append = function (str) {
    this._strings.push(str);
}
//toString方法定义
StringBuilder.prototype.toString = function () {
    return this._strings.join('');
}
/*#endregion*/

/*#region select控件帮助类*/
var select = {
    getedOption: function (obj) {
        /// <summary>
        /// 获取已选的option
        /// </summary>
        /// <param name="obj" type="Object">select对象</param>
        /// <returns type="Object" />
        return obj.options[selectObj.selectedIndex];
    },
    getedValue: function (obj) {
        /// <summary>
        /// 获取已选的option的值
        /// </summary>
        /// <param name="obj" type="Object">select对象</param>
        /// <returns type="String" />
        return this.getSelectedOption(obj).value;
    },
    setedValue: function (obj, value) {
        /// <summary>
        /// 设置已选的option的值
        /// </summary>
        /// <param name="obj" type="Object">select对象</param>
        /// <param name="value" type="String">已选值</param>
        for (var i = 0; i < obj.options.length; i++) { if (obj.options[i].value == value.toString()) { obj.selectedIndex = i; break; } }
    },
    clear: function (obj) {
        /// <summary>
        /// 清除所有选项
        /// </summary>
        /// <param name="obj" type="Object">select对象</param>
        var count = obj.options.length;
        for (var i = 0; i < count; i++) { obj.removeChild(obj.options[0]); }
    },
    change: function (parentor, url, selector) {
        /// <summary>
        /// 改变select的数据项
        /// </summary>
        /// <param name="parentor" type="Object">事件驱动对象</param>
        /// <param name="url" type="String">获取数据的URL</param>
        /// <param name="selector" type="String">select的选择器名称</param>
        var Id = $(parentor).val();
        if (Id != null && Id != "") {
            $.getJSON(url, { "Id": Id }, function (data) {
                var select = $(selector);
                if (select.find("option").length == 0) {
                    return;
                }
                var option0 = select.find("option:eq(0)");
                select.find("option").remove();
                if (option0.val() == "0" || option0.val() == "") {
                    select.append(option0);
                }
                if (data != null) {
                    $.each(data, function (index, element) {
                        var option = $('<option value="' + element.ID + '">' + element.Name + '</option>');
                        select.append(option);
                    });
                }
            });
        }
    }
};
/*#endregion*/

/*#region 验证码帮助类*/
var Authcode = function (code_selector, img_selector) {
    /// <summary>
    /// 验证码 构造函数
    /// </summary>
    /// <param name="code_selector" type="String">验证码输入框</param>
    /// <param name="img_selector" type="String">验证码显示图</param>
    /// <param name="url" type="String">验证码地址</param>
    this.AuthName = "";
    this.AuthInput = $(code_selector);
    this.AuthImg = $(img_selector);
    this.CodeIndex = 0;
    this.Url = "";
    this.ini();
};
Authcode.prototype = {
    ini: function () {
        if (this.AuthImg.attr("src") != "") {
            this.Url = this.AuthImg.attr("src");
            this.AuthName = utils.getquery("auth_code", this.Url);
            if (this.Url.indexOf('?') > 0) {
                this.Url = this.Url.substring(0, this.Url.indexOf('?'));
            }
        }
        this.AuthImg.click(this.setCode.setThis(this));
    },
    setCode: function () {
        /// <summary>
        /// 设置验证码
        /// </summary>
        var src = this.Url + "?auth_code=" + this.AuthName + "&" + this.CodeIndex.toString();
        this.AuthImg.attr("src", src);
        this.AuthInput.val("");
        this.CodeIndex++;
    },
    checkCode: function () {
        /// <summary>
        /// 检查验证码
        /// </summary>
        var code = $.cookie(this.AuthName).toLowerCase();
        if (this.AuthInput.val().toLowerCase() != code) { return false; }
        return true;
    }
};
/*#endregion*/

/*#region jquery扩展*/
jQuery.extend({
    /** * 调整到指定位置 */
    fixedPostion: function (postion) {
        /// <summary>
        /// 定位到页面指定位置，地址符号#后面
        /// </summary>
        /// <param name="postion" type="String">位置名称，地址符号#后面的字符</param>
        var url = location.href;
        url = url.substr(0, url.lastIndexOf("#")) + "#" + postionName;
        location.href = url;
    },
    equalPostion: function (postion) {
        /// <summary>
        /// 是否与页面指定位置相等，地址符号#后面
        /// </summary>
        /// <param name="postion" type="String">位置名称，地址符号#后面的字符</param>
        var url = location.href;
        postion = postion.toLowerCase();

        if (url.indexOf("#" + postion) > 1) { return true; }
        else { return false; }
    }
});
jQuery.extend({
    evalJSON: function (strJson) {
        /// <summary>
        /// @see 将json字符串转换为对象 * @param json字符串 * @return 返回object,array,string等对象
        /// </summary>
        /// <param name="strJson" type="String">Json字符串</param>
        return eval("(" + strJson + ")");
    },
    cloneJSON: function (jsonObj) {
        /// <summary>
        /// 克隆json对象
        /// </summary>
        /// <param name="jsonObj" type="Object">Json对象</param>
        var rePara = null;
        var type = Object.prototype.toString.call(jsonObj);
        if (type.indexOf("Object") > -1) {
            rePara = jQuery.extend(true, {}, jsonObj);
        } else if (type.indexOf("Array") > 0) {
            rePara = [];
            jQuery.each(jsonObj, function (index, obj) {
                rePara.push(jQuery.cloneJSON(obj));
            });
        }
        else { rePara = jsonObj; }
        return rePara;
    },
    toJSON: function (object) {
        /// <summary>
        /// @see 将javascript数据类型转换为json字符串 * @param 待转换对象,支持object,array,string,function,number,boolean,regexp * @return 返回json字符串
        /// </summary>
        /// <param name="object" type="Object">Json对象</param>
        var type = typeof object;
        if ('object' == type) {
            if (Array == object.constructor) type = 'array';
            else if (RegExp == object.constructor) type = 'regexp';
            else type = 'object';
        }
        switch (type) {
            case 'undefined':
            case 'unknown':
                return;
                break;
            case 'function':
            case 'boolean':
            case 'regexp':
                return object.toString();
                break;
            case 'number':
                return isFinite(object) ? object.toString() : 'null';
                break;
            case 'string':
                return '"' + object.replace(/(\\|\")/g, "\\$1").replace(/\n|\r|\t/g, function () {
                    var a = arguments[0];
                    return (a == '\n') ? '\\n' : (a == '\r') ? '\\r' : (a == '\t') ? '\\t' : ""
                }) + '"';
                break;
            case 'object':
                if (object === null) return 'null';
                var results = [];
                for (var property in object) {
                    var value = jQuery.toJSON(object[property]);
                    if (value !== undefined) results.push(jQuery.toJSON(property) + ':' + value);
                }
                return '{' + results.join(',') + '}';
                break;
            case 'array':
                var results = [];
                for (var i = 0; i < object.length; i++) {
                    var value = jQuery.toJSON(object[i]);
                    if (value !== undefined) results.push(value);
                }
                return '[' + results.join(',') + ']';
                break;
        }
    }
});
jQuery.extend({
    limit: function (objString, limitLength) {
        /// <summary>
        /// 用jquery 实现 超出字符 截断并加上省略号（自定义jquery函数）
        /// </summary>
        /// <param name="objString" type="String">原字符串</param>
        /// <param name="limitLength" type="Number">限制长度</param>
        var objLength = objString.length;
        if (objLength > limitLength) {
            objString = objString.substring(0, num);
        }
        return objString;
    },
    autoframe: function (frameId, addheight) {
        /// <summary>
        /// 框架自适应
        /// </summary>
        /// <param name="frameId" type="String">框架ID</param>
        /// <param name="addheight" type="Number">在原基础上加高</param>
        $(window.parent.document).find("#" + frameId).ready(function () {
            var main = $(window.parent.document).find("#" + frameId);
            if (!addheight) { addheight = 0; }
            var thisheight = $(document).height() + addheight;
            main.height(thisheight);
        });
    },
    search: function (sender, inputName, defaultUrl, allowNull) {
        /// <summary>
        /// 关键字搜索函数，提供输入框离开焦点，获取焦点操作
        /// </summary>
        /// <param name="sender" type="Object">事件发送者</param>
        /// <param name="inputName" type="String">输入框的名称name</param>
        /// <param name="defaultUrl" type="String">如果输入内容为空，默认跳转的地址</param>
        /// <param name="allowNull" type="Boolean">是否允许输入为空</param>
        var jsender = $(sender);
        var form = jsender;
        if (form.length > 0) {
            var input = form.find("input[name=" + inputName + "]");
            input.focus();
            var keyword = input.val();
            if (keyword != null && $.trim(keyword) != "") {
                return true;
            }
            else {
                if (allowNull) {
                    return true;
                }
                if (defaultUrl != null && defaultUrl != "") {
                    form.attr("action", defaultUrl);
                    return true;
                }
            }
        }
        return false;
    },
    cookie: function (name, value, options) {
        /// <summary>
        /// Cookie操件 jQuery.Cookie，包括获取和设置cookies
        /// </summary>
        /// <param name="name" type="String">cookies名称</param>
        /// <param name="value" type="String">cookies值</param>
        /// <param name="options" type="Array">
        /// 1. expires:Number - cookies过期天数
        /// 2. path:String - cookies有效目录地址
        /// 3. domain:String - cookies有效域名
        /// 4. secure:Boolean - 是否使用安全保存
        /// </param>
        if (typeof value != 'undefined') { // name and value given, set cookie
            options = options || {};
            if (value === null) {
                value = '';
                options.expires = -1;
            }
            var expires = '';
            if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
                var date;
                if (typeof options.expires == 'number') {
                    date = new Date();
                    date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
                } else {
                    date = options.expires;
                }
                expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
            }
            // CAUTION: Needed to parenthesize options.path and options.domain
            // in the following expressions, otherwise they evaluate to undefined
            // in the packed version for some reason...
            var path = options.path ? '; path=' + (options.path) : '';
            var domain = options.domain ? '; domain=' + (options.domain) : '';
            var secure = options.secure ? '; secure' : '';
            document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
        } else { // only name given, get cookie
            var cookieValue = null;
            if (document.cookie && document.cookie != '') {
                var cookies = document.cookie.split(';');
                for (var i = 0; i < cookies.length; i++) {
                    var cookie = jQuery.trim(cookies[i]);
                    // Does this cookie string begin with the name we want?
                    if (cookie.substring(0, name.length + 1) == (name + '=')) {
                        cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                        break;
                    }
                }
            }
            return cookieValue;
        }
    },
    asynDelete: function (url, map_data, fun, sender) {
        /// <summary>
        /// AJAX删除
        /// </summary>
        /// <param name="url" type="String">请求地址</param>
        /// <param name="map_data" type="Map">回发数据</param>
        /// <param name="sender" type="Object">触发事物</param>
        var jsender = $(sender);
        $.get(url, map_data, function (data) {
            if (data == 1) {
                jsender.animate({ opacity: 0 }, function () {
                    $(this).hide(300, function () {
                        $(this).remove();
                        if (fun != undefined) {
                            fun();
                        }
                    });
                });
                
            }
        });

    },
    focusPrompt: function (selector, promptMeg) {
        var inputNode = $(selector);
        var titles = promptMeg;
        inputNode.addClass("L_input_focus");
        if (inputNode.val() === '') {
            inputNode.val(titles);
        }
        inputNode.focus(function () {
            if (inputNode.val() == titles) {
                inputNode.val('').removeClass("L_input_focus");
            }
        });
        inputNode.blur(function () {
            if (inputNode.val() === '') {
                inputNode.val(titles).addClass("L_input_focus");
            }
        });
    },
    getQuery: function (name) {
        var urlo = new objURL(location.href);
        return urlo.get(name);
    }
});
jQuery.fn.extend({
    loadModule: function (module_code) {
        /// <summary>
        /// 异步加载html模块
        /// </summary>
        /// <param name="module_code" type="String">模块编码</param>
        this.load("/handlers/getmodulehtml.ashx?code=" + module_code);
    },
    toInt: function () {
        var val = parseInt(this.val());
        if (isNaN(val)) {
            return 0;
        }
        else {
            return val;
        }
    },
    toFloat: function () {
        var val = parseFloat(this.val());
        if (isNaN(val)) {
            return 0;
        }
        else {
            return val;
        }
    }
});
/*#endregion*/

/*#region Url操作*/

var objURL = function (url) {
    this.ourl = url || window.location.href;
    this.href = ""; //?前面部分
    this.params = {}; //url参数对象
    this.jing = ""; //#及后面部分
    this.init();
}
//分析url,得到?前面存入this.href,参数解析为this.params对象，#号及后面存入this.jing
objURL.prototype.init = function () {
    var str = this.ourl;
    var index = str.indexOf("#");
    if (index > 0) {
        this.jing = str.substr(index);
        str = str.substring(0, index);
    }
    index = str.indexOf("?");
    if (index > 0) {
        this.href = str.substring(0, index);
        str = str.substr(index + 1);
        var parts = str.split("&");
        for (var i = 0; i < parts.length; i++) {
            var kv = parts[i].split("=");
            this.params[kv[0]] = kv[1];
        }
    }
    else {
        this.href = this.ourl;
        this.params = {};
    }
}
//只是修改this.params
objURL.prototype.set = function (key, val) {
    this.params[key] = val;
}
//只是设置this.params
objURL.prototype.remove = function (key) {
    this.params[key] = undefined;
}
//根据三部分组成操作后的url
objURL.prototype.url = function () {
    var strurl = this.href;
    var objps = []; //这里用数组组织,再做join操作
    for (var k in this.params) {
        if (this.params[k]) {
            objps.push(k + "=" + this.params[k]);
        }
    }
    if (objps.length > 0) {
        strurl += "?" + objps.join("&");
    }
    if (this.jing.length > 0) {
        strurl += this.jing;
    }
    return strurl;
}
//得到参数值
objURL.prototype.get = function (key) {
    var param_value = this.params[key];
    
    if (param_value == null || param_value == undefined) {
        param_value = "";
    }
    return param_value;
}
/*#endregion*/

/*#region 加载中*/
var Loading = function (title) {
    this.OverLay = $("<div class='loading'><em>" + title + "...</em></div>");
};
Loading.prototype = {
    show: function () {
        this.OverLay.appendTo('body').show().css("top", "50%");
    },
    close: function () {
        this.OverLay.hide().remove();
    }
};
/*#endregion*/

/*#region 短信验证码*/
var AuthSms = function (sms_name, code_selector, btn_selector) {
    /// <summary>
    /// 验证码 构造函数
    /// </summary>
    /// <param name="sms_name" type="String">验证名称</param>
    /// <param name="code_selector" type="String">验证码输入框</param>
    /// <param name="btn_selector" type="String">发送按钮</param>
    this.SmsName = sms_name;
    this.SmsInput = $(code_selector);
    this.SmsButton = $(btn_selector);
    this.ini();
};
AuthSms.prototype = {
    ini: function () {

    },
    sendCode: function (number, fun, sign) {
        /// <summary>
        /// 发送验证码
        /// </summary>
        this.SmsInput.val("");
        this.SmsButton.hide();
        $.getJSON("/handlers/sendAuthSms.ashx?sign=" + sign, { "number": number, "sms_name": this.SmsName }, function (data) {
            if (fun != undefined) {
                fun(data);
            }
        });
    },
    checkCode: function (number, fun) {
        /// <summary>
        /// 检查验证码
        /// </summary>
        var code = this.SmsInput.val().trim();
        if (code == "") {
            return;
        }
        $.getJSON("/handlers/checkAuthSms.ashx", { "number": number, "sms_name": this.SmsName, "code": code }, function (data) {
            if (fun != undefined) {
                fun(data);
            }
        });
    }
};
/*#endregion*/





