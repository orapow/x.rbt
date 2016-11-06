//$(document).ready(function () {
// 点击查看购物车
$(".header_cart, .header_cart_easy").click(function () {
	if ((get_logon_type() != null) && (get_logon_type() == "admin")) {
		alert("管理员登录模式无法使用购物车");
	}
	else {
		goto_link(getActionPath("Cart", "Home"));
	}
});

// 鼠标滑过,显示小购物车
$(".header_cart_box").hover(function () {
	if ((get_logon_type() != null) && (get_logon_type() == "admin")) {
		return;
	}
	else {
		update_small_cart(false);
	}
},
function () {
	if ((get_logon_type() != null) && (get_logon_type() == "admin")) {
		return;
	}
	else {
		$(".header_cart_box").find(".small_cart").hide();
	}
});

// logon处理
function logon_process() {
	// 显示登录信息
	if ((get_logon_type() != null) && (get_logon_type() == "buyer")) {
		$("#store_header").find(".buyer_show").show();
		$("#store_header").find(".admin_show").hide();
		$("#store_header").find(".not_buyer_show").hide();
		$("#store_header").find("#buyer_name").text(get_logon_name());
		$("#store_header").find(".store_message").show();
		$("#store_header").find(".store_message a").text('站内信(' + get_logon_message_count() + ')');
	}
	else if ((get_logon_type() != null) && (get_logon_type() == "admin")) {
		$("#store_header").find(".buyer_show").hide();
		$("#store_header").find(".admin_show").show();
		$("#store_header").find(".not_buyer_show").hide();
		$("#store_header").find("#admin_name").text(get_logon_name());
	}
	else {
		$("#store_header").find(".buyer_show").show();
		$("#store_header").find(".admin_show").hide();
		$("#store_header").find(".not_buyer_show").show();
		$("#store_header").find(".un_login").hide();

		// 第三方登录点击处理
		var callback = encodeURIComponent(getFullActionPath("Logon", "Account") + "?url=" + encodeURIComponent(get_callback_url()));
		// QQ第三方登录
		$("#store_header").find("#third_qq").click(function () {
			var url = Configure.Third_Jump + "/QQLogon" + "?cb=" + callback + "&device=" + Constant.STORE_DEVICE_PC;
			goto_link(url);
		});
		// Sina微博第三方登录
		$("#store_header").find("#third_sina").click(function () {
			var url = Configure.Third_Jump + "/SinaLogon" + "?cb=" + callback + "&device=" + Constant.STORE_DEVICE_PC;
			goto_link(url);
		});
		// 人人网第三方登录
		$("#store_header").find("#third_renren").click(function () {
			var url = Configure.Third_Jump + "/RenRenLogon" + "?cb=" + callback + "&device=" + Constant.STORE_DEVICE_PC;
			goto_link(url);
		});
		// 支付宝第三方登录
		$("#store_header").find("#third_alipay").click(function () {
			var url = Configure.Third_Jump + "/AlipayLogon" + "?cb=" + callback + "&device=" + Constant.STORE_DEVICE_PC;
			goto_link(url);
		});
	}
}

// 更新小购物车内容
var block_get_cart = false;
function update_small_cart(async) {
	if (!block_get_cart) {
		$.ajax({
			url: getActionPath("GetCartAmount", "Cart"),
			dataType: "json",
			async: async,
			contentType: 'application/json',
			type: "POST",
			data: stringify({ get_type: Constant.GET_ALL, calc_freight: false, sellersub_calculate: null, calc_invoice: Constant.INVOICETYPE_NONE, use_points: 0, discount_card_password: null, coupon_card_password: null, prepaid_card_password: null, prov_id: -1 }),
			success: function (data) {
				if ((data.success == true) || (data.error == ErrorCode.ORDER_OUT_OF_STOCK)) {
					// 刷新页面
					data.GoodsTotalCount = 0;
					data.Subtotal = 0;

					if (data.amount != null) {
						for (var k = 0; k < data.amount.length; k++) {
							for (var i = 0; i < data.amount[k].SalesRuleInfo.length; i++) {
								for (var j = 0; j < data.amount[k].SalesRuleInfo[i].GoodsList.length; j++) {
									data.GoodsTotalCount += data.amount[k].SalesRuleInfo[i].GoodsList[j].Count;
								}
							}
							data.Subtotal += data.amount[k].Subtotal;
						}
						data.SubtotalDisplay = parseFloat(data.Subtotal / 100).toFixed(2);
					}

					var cart_div = $(".header_cart_box").find(".small_cart");
					cart_div.empty();
					$("#small_cart_tmpl").tmpl(data).appendTo(cart_div);
					$(".header_cart_box").find(".small_cart").show();

					var height = cart_div.find(".cart_content").height();
					if (height > 330) {
						cart_div.find(".cart_content_scroll").height(330);
					}
					else {
						cart_div.find(".cart_content_scroll").height(height);
					}

					cart_div.find(".cart_content_scroll").css("overflow", "auto");

					// 点击去购物车结算
					cart_div.find(".cart_goods_submit").click(function () {
						goto_cart();
					});

					// 点击商品链接
					cart_div.find(".goods_a_img, .goods_a_text p").click(function () {
						var goods_id = $(this).attr("goods_id");
						goto_goods_link(goods_id, true);
					});

					// 点击商品删除
					cart_div.find(".small_cart_goods_delete").click(function () {
						var goods_id = $(this).attr("goods_id");
						var seller_subid = $(this).attr("seller_subid");
						var prop1_id = $(this).attr("prop1_id");
						var prop2_id = $(this).attr("prop2_id");
						var prop3_id = $(this).attr("prop3_id");

						var goods_list = [];
						goods_list.push({ GoodsId: goods_id, SellerSubId: seller_subid, Prop1ItemId: prop1_id, Prop2ItemId: prop2_id, Prop3ItemId: prop3_id });

						del_cart_goods(goods_list, $(this).parents(".cart_content_goods"));
					});
				}
			},
			error: function (request, status, errorThrown) {
			}
		});
	}
}

function del_cart_goods(goods_list, goods_div) {
	block_get_cart = true;

	$.ajax({
		url: getActionPath("DeleteCartGoods", "Cart"),
		dataType: "json",
		contentType: 'application/json',
		type: "POST",
		//		async: false,
		data: stringify({ cart_goods: goods_list }),
		success: function (data) {
			block_get_cart = false;
			$(".header_cart_box").find(".small_cart").hide();
		},
		error: function (request, status, errorThrown) {
			ShowAjaxError(request, status, errorThrown);
		}
	});
}

// ******** 模板必须实现该函数 ******** //
// 显示商店信息,包括logo等
function show_store_info() {
	// 搜索框 商品,店铺 显示切换
	function load_search_info(val) {
		if (val == Constant.STORE_HEADSEARCH_TYPE_GOODS) {
			var header_style_type = $("#hid_header_style").val();
			$("#header_search_type_tab_good").addClass("patt_bg_header_checked");
			$("#header_search_type_tab_good").css("color", "#fff");
			$("#header_search_type_tab_shop").css("color", "#000");
			$("#header_search_type_tab_shop").removeClass("patt_bg_header_checked");
			$("#hid_type_value").val($("#header_search_type_tab_good").attr("search_type"));
		} else {
			var search_type = $("#hid_header_style").val();
			$("#header_search_type_tab_shop").addClass("patt_bg_header_checked");
			$("#header_search_type_tab_shop").css("color", "#fff");
			$("#header_search_type_tab_good").removeClass("patt_bg_header_checked");
			$("#header_search_type_tab_good").css("color", "#000");
			$("#hid_type_value").val($("#header_search_type_tab_shop").attr("search_type"));
		}
	}

	if (Configure.ShowHeader) {
		// 搜索框 商品,店铺 初始化显示
		load_search_info($("#hid_type_value").val());

		// 选择搜索商品还是商店(下拉)
		$("#header_search_type_select").click(function () {
			var search_type = $(this).attr('search_type');
			$("#hid_type_value").val(search_type);
			$("#show_header_search_type_select").focus();
			$(".header_search_type").toggle();
			$(".header_search_type").show();
			$("#header_search_type_" + search_type).hide();
		});

		// 选择搜索商品还是商店(Tab)
		$("#header_search_type_tab_good").click(function () {
			load_search_info(Constant.STORE_HEADSEARCH_TYPE_GOODS);
		});
		$("#header_search_type_tab_shop").click(function () {
			load_search_info(Constant.STORE_HEADSEARCH_TYPE_SELLERSUB);
		});

		$("#show_header_search_type_select").blur(function () {
			$(".header_search_type").hide();
		});

		$("#header_search_type_" + Constant.STORE_HEADSEARCH_TYPE_GOODS).click(function () {
			$(".header_search_type").hide();
			$("#header_search_type_select").text("商品");
			$("#header_search_type_select").attr("search_type", Constant.STORE_HEADSEARCH_TYPE_GOODS);
			$("#hid_type_value").val(Constant.STORE_HEADSEARCH_TYPE_GOODS);
		});

		$("#header_search_type_" + Constant.STORE_HEADSEARCH_TYPE_SELLERSUB).click(function () {
			$(".header_search_type").hide();
			$("#header_search_type_select").text("店铺");
			$("#header_search_type_select").attr("search_type", Constant.STORE_HEADSEARCH_TYPE_SELLERSUB);
			$("#hid_type_value").val(Constant.STORE_HEADSEARCH_TYPE_SELLERSUB);
		});

		$("#search_button").click(function () {
			var key = $("input[name=search_text]").val();
			var search_type = $("#hid_type_value").val();
			if ((key != null) && (key != "")) {
				goto_search_link(search_type, key);
			}
		});

		// 在搜索栏输入回车
		$("input[name=search_text]").keyup(function (e) {
			if (e.keyCode == 13) {
				var key = $("input[name=search_text]").val();
				var search_type = $("#hid_type_value").val();
				if ((key != null) && (key != "")) {
					goto_search_link(search_type, key);
				}
			}
		});

		//搜索框获取焦点
		$('input[name=search_text]').focus(function () {
			if ($.trim($(this).val()) == $.trim($(this).attr('remind'))) {
				$(this).val('');
				$('[name=search_text]').css("color", "#000");
			}
		});

		$('input[name=search_text]').blur(function () {
			if ($.trim($(this).val()) == '') {
				$(this).val($.trim($(this).attr('remind')));
				$('[name=search_text]').css("color", "#999");
			}
		});
	}

	// 筑云支持显示
	if (Configure.ShowZhuyun) {
		$(".footer-box").find(".footer").removeClass("footer_no_zhuyun");
	}
	else {
		$(".footer-box").find(".footer").addClass("footer_no_zhuyun");
	}
}

// ******** 模板必须实现该函数 ******** //
// 显示商店分类导航
function show_store_nav() {
	if (Configure.ShowNav) {
		// 导航栏宽度计算
		var nav_width = 50;
		$("#show_store_nav").find(".subnav_li").each(function () {
			nav_width += $(this).outerWidth();
		});
		nav_width = nav_width > 1200 ? 1200 : nav_width;
		$("#show_store_nav").find(".header_subnav").width(nav_width);

		$("#show_store_nav .header_subnav .subnav_li").hover(function () {
			$(this).siblings().removeClass("subnav_header");
			$(this).addClass("subnav_header");

			var index = $("#show_store_nav .header_subnav .subnav_li").index($(this));
			$("#show_store_nav .header_subclass .header_subclass_nav").hide();

			var width = $("#show_store_nav .header_subnav_header").width();
			var x = $(this).position().left;
			var sub_class = $("#show_store_nav .header_subclass .header_subclass_nav").eq(index);
			if (x <= (width / 2)) {
				sub_class.css("left", 0);
			}
			else {
				sub_class.css("left", Math.floor(width - sub_class.width()));
			}
			sub_class.show();
		});

		$("#show_store_nav .header_subclass .header_subclass_nav").hover(function () {
		},
		function () {
			$(this).hide();
			$("#show_store_nav .header_subnav .subnav_li").removeClass("subnav_header");
		});

		$("#show_store_nav .header_subnav_header").hover(function () {
		},
		function () {
			$("#show_store_nav .header_subclass .header_subclass_nav").hide();
			$("#show_store_nav .header_subnav .subnav_li").removeClass("subnav_header");
		});
	}

	// 全部商品分类
	// 光标设置
	var min_height = 60;
	var header_short_nav = $("#store_header").find(".header_sort");
	header_short_nav.find(".nav_style1_content").css("cursor", "default");


	// 鼠标滑过展开下级处理
	header_short_nav.find(".nav_style1_content_li").hover(function () {
		header_short_nav.find(".nav_style1_content_li").removeClass("nav_style1_content_hover");
		$(this).addClass("nav_style1_content_hover");

		var index = header_short_nav.find(".nav_style1_content_li").index(this);

		header_short_nav.find(".nav_style1_sub").hide();
		header_short_nav.find(".nav_style1_sub").eq(index).show();
		if (header_short_nav.find(".nav_style1_sub").eq(index).height() < min_height) {
			header_short_nav.find(".nav_style1_sub").eq(index).height(min_height);
		}

		var h_pos = $(this).offset().top - header_short_nav.find(".nav_style1_main_content").offset().top;
		h_pos += $(this).height() + 10;
		// 位置偏移处理
		if (header_short_nav.find(".nav_style1_sub").eq(index).height() < h_pos) {
			header_short_nav.find(".nav_style1_sub").eq(index).css("top", Math.round(h_pos - header_short_nav.find(".nav_style1_sub").eq(index).height()) + "px");
		}
	},
	function () {
	});

	header_short_nav.find(".nav_style1_main_content").hover(function () {
	},
	function () {
		header_short_nav.find(".nav_style1_content_li").removeClass("nav_style1_content_hover");
		header_short_nav.find(".nav_style1_sub").hide();
	});

	// 显示隐藏全体商品分类
	header_short_nav.hover(function () {
		$(this).find(".Nav_TitleStyleBox").show();
	},
	function () {
		$(this).find(".Nav_TitleStyleBox").hide();
	});
}

//设置头部广告
function show_store_header_av() {
	if (StoreTemplate == null) {
		return;
	}
	var width;
	if (StoreTemplate.HeaderBackGroundMap.FullScreenOption) {
		width = GetWinWidth();
	} else {
		width = 1190;
	}

	switch (StoreTemplate.HeaderAdType) {
		case Common.Constant.HEADERAD_TYPE_BACKPIC:
			switch (StoreTemplate.HeaderBackGroundMap.Type) {
				case Common.Constant.BACKPIC_TYPE_ORDINARY:
					$("#header_content_box").find("img").width(width);
					if (StoreTemplate.HeaderBackGroundMap.AutomaticClose) {
						$("#header_content_box").show().delay(StoreTemplate.HeaderBackGroundMap.AutomaticCloseTime).animate({ height: "0" }, 1000);
					}
					break;
				case Common.Constant.BACKPIC_TYPE_FOLD:
					$("#header_content_box").find("img").width(width);
					$("#fold_open").delay(1000).animate({ height: "300" }, 300, function () {
						$("#fold_open").delay(3000).animate({ height: "60" }, 500, function () {
							$("#fold_stop").animate({ height: "60" });
							$("#fold_open").animate({ height: "0" })
						});
					});
					break;
				case Common.Constant.BACKPIC_TYPE_ROLL:
					$("#header_content_box").find(".banner_img").find("img").width(width);
					var autobool = false;
					if (StoreTemplate.HeaderBackGroundMap.Roll) {
						autobool = Number(StoreTemplate.HeaderBackGroundMap.Interval);
						// 图片滚动处理
						$("#header_content_box").find(".banner_img").carouFredSel({
							items: 1,
							auto: autobool,
							scroll: {
								items: 1,
								pauseOnHover: true,
								onBefore: function () {
									$("#header_content_box").find("img.lazy[src='/blank.png']").lazyload({ threshold: 20 });
								}
							},
							direction: (StoreTemplate.HeaderBackGroundMap.RollDirection == Constant.PLUGIN_CAROUSEL_DIR_LEFT) ? "left" : "right",
							pagination:
							{
								container: $("#header_content_box").find(".Topbanner_number"),
								anchorBuilder: function (nr) {
									return '<a class="bg_j" href="javascript:void(0)"></a>';
								}
							}
						});
					}
					$("#header_content_box").find("img.lazy").lazyload({ threshold: 20 });
					break;
			}
			break;
		case Common.Constant.HEADERAD_TYPE_CODE:

			break;
	}
	if (StoreTemplate.HeaderBackGroundMap.CloseOption) {
		$("#header_content_box").find(".icon_header_ad_close_drag").click(function () {
			$("#header_content_box").animate({ height: "0" });
		});
		$("#header_content_box").hover(function () {
			$(this).find(".icon_header_ad_close_drag").show();
		}, function () {
			$(this).find(".icon_header_ad_close_drag").hide();
		});
	}
}

var rongcloud_app_key = null;
var rongcloud_token = null;
var rongcloud_target_id = null;
function show_store_custom_service() {
	var service = {
		service_qq: new Array(),
		service_company_qq: new Array(),
		service_marketing_qq: new Array(),
		service_wang: new Array()
	}
	if (store_set_service.ServiceAccount != null) {
		service.service_qq = new Array();
		service.service_company_qq = new Array();
		service.service_wang = new Array();

		for (var i = 0; i < store_set_service.ServiceAccount.length; i++) {
			switch (store_set_service.ServiceAccount[i].Type) {
				case Constant.STORE_SERVICE_QQ:
					service.service_qq.push(store_set_service.ServiceAccount[i]);
					break;
				case Constant.STORE_SERVICE_COMPANY_QQ:
					service.service_company_qq.push(store_set_service.ServiceAccount[i]);
					break;
				case Constant.STORE_SERVICE_MARKETING_QQ:
					service.service_marketing_qq.push(store_set_service.ServiceAccount[i]);
					break;
				case Constant.STORE_SERVICE_WANGWANG:
					service.service_wang.push(store_set_service.ServiceAccount[i]);
					break;
				default:
					break;
			}
		}
		$("#store_custom_service").addClass("store_service_position" + store_set_service.ServicePos);
		$("#custom_service_tmpl").tmpl(service).appendTo($("#store_custom_service"));
		if (store_set_service.ServiceSet == Constant.OPEN) {
			switch (parseInt(store_set_service.ServiceUse)) {
				case Constant.OPEN_ACROSS_DISPLAY:         //支持(划过显示)
					$("#custom_service_1").hover(function () {
						$("#custom_service_1").hide();
						$("#custom_service_2").show();
					}, function () {
						$("#custom_service_1").hide();
						$("#custom_service_2").show();
					});

					$("#custom_service_2").hover(function () {
					}, function () {
						$("#custom_service_1").show();
						$("#custom_service_2").hide();
					});
					break;
				case Constant.OPEN_ALWAYS_DISPLAY:         //支持(一直显示)
					$("#custom_service_1").hide();
					$("#custom_service_2").show();
					break;
				default:
					$("#store_custom_service").hide();
					break;
			}
		}
		else {
			$("#store_custom_service").hide();
		}

	}
	else {
		$("#store_custom_service").hide();
	}
	// 加载营销QQ
	if ((service.service_marketing_qq != null) && (service.service_marketing_qq.length > 0)) {
		load_marketing_qq(function () {
			for (var i = 0; i < service.service_marketing_qq.length; i++) {
				BizQQWPA.addCustom([
				{ aty: '0', a: '0', nameAccount: service.service_marketing_qq[i].Account, selector: "yx_" + service.service_marketing_qq[i].Account }
				]);
			}
		});
	}
}

// 显示商店信息,包括logo等
function show_sub_store_info() {
	//搜本站
	$("#sub_header_search_type_" + Constant.SELLERSUB_HEADSEARCH_TYPE_SELLERSUB).click(function () {
		$("#sub_header_search_type_select").attr("sub_search_type", Constant.SELLERSUB_HEADSEARCH_TYPE_SELLERSUB);
		sub_search(Constant.SELLERSUB_HEADSEARCH_TYPE_SELLERSUB);
	});
	//搜全站
	$("#sub_header_search_type_" + Constant.SELLERSUB_HEADSEARCH_TYPE_STORE).click(function () {
		$("#sub_header_search_type_select").attr("sub_search_type", Constant.SELLERSUB_HEADSEARCH_TYPE_STORE);
		sub_search(Constant.SELLERSUB_HEADSEARCH_TYPE_STORE);
	});
	//简约纯朴搜索框
	$("#sub_show_header_search_type_select").click(function () {
		var $this = $(this);
		$("#sub_show_header_search_type_select").find(".sub_header_search_type").hide();
		var search_type = $("#sub_header_search_type_select").attr("search_type");
		if (search_type == Constant.SELLERSUB_HEADSEARCH_TYPE_SELLERSUB) {
			$(".sub_header_search_type" + Constant.SELLERSUB_HEADSEARCH_TYPE_STORE + "").show();
		} else {
			$(".sub_header_search_type" + Constant.SELLERSUB_HEADSEARCH_TYPE_SELLERSUB + "").show();
		}
		$this.find(".sub_header_search_type").click(function (e) {
			var sub_header_search_type = $(this).attr("sub_header_search_type");
			var sub_header_search_name = $(this).html();
			$this.find("#sub_header_search_type_select").attr("search_type", sub_header_search_type).html(sub_header_search_name);
			$this.find(".sub_header_search_type").hide();
			e.stopPropagation();
		});
	});
	$("#sub_search_button").click(function () {
		var search_type = $("#sub_header_search_type_select").attr("search_type");
		if (search_type == Constant.SELLERSUB_HEADSEARCH_TYPE_SELLERSUB) {
			sub_search(Constant.SELLERSUB_HEADSEARCH_TYPE_SELLERSUB);
		} else {
			sub_search(Constant.SELLERSUB_HEADSEARCH_TYPE_STORE);
		}
	});
	// 在搜索栏输入回车
	$("input[name=sub_search_text]").keyup(function (e) {
		if (e.keyCode == 13) {
			var sub_search_type = $("#sub_header_search_type_select").attr('sub_search_type');
			sub_search(sub_search_type);
		}
	});
	function sub_search(sub_search_type) {
		var key = $("input[name=sub_search_text]").val();
		if (!CheckInput.IsEmpty(key)) {
			if (parseInt(sub_search_type) == Constant.SELLERSUB_HEADSEARCH_TYPE_SELLERSUB) {
				goto_search_goods_link(key, null, null, null, null, null, sub_store_data.SubId);
			} else {
				goto_search_goods_link(key, null, null, null, null, null, null);
			}
		}
	}
}

function show_sub_store_custom_service() {
	var service = {
		service_qq: new Array(),
		service_company_qq: new Array(),
		service_marketing_qq: new Array(),
		service_wang: new Array()
	}
	if ((store_set_service.ServiceAccount != null) && (store_set_service.ServiceAccount.length > 0)) {
		$("#sub_store_grade").find("#seller_sub_service_ctrl").removeClass("sub_header_grade_Offline");
		$("#sub_store_grade").find("#seller_sub_service_ctrl").addClass("sub_header_grade_Online");
		$("#sub_store_grade").find("#seller_sub_service_ctrl_text").text("店铺咨询");

		service.service_qq = new Array();
		service.service_company_qq = new Array();

		service.service_wang = new Array();

		for (var i = 0; i < store_set_service.ServiceAccount.length; i++) {
			switch (store_set_service.ServiceAccount[i].Type) {
				case Constant.STORE_SERVICE_QQ:
					service.service_qq.push(store_set_service.ServiceAccount[i]);
					break;
				case Constant.STORE_SERVICE_COMPANY_QQ:
					service.service_company_qq.push(store_set_service.ServiceAccount[i]);
					break;
				case Constant.STORE_SERVICE_MARKETING_QQ:
					service.service_marketing_qq.push(store_set_service.ServiceAccount[i]);
					break;
				case Constant.STORE_SERVICE_WANGWANG:
					service.service_wang.push(store_set_service.ServiceAccount[i]);
					break;
				default:
					break;
			}
		}

		if ($("#seller_sub_service_tmpl").html() != undefined) {
			$("#seller_sub_service_tmpl").tmpl(service).appendTo($("#sub_store_grade").find("#seller_sub_service"));
		}

		$("#sub_store_grade").find("#seller_sub_service_ctrl").hover(function () {
			$("#sub_store_grade").find("#seller_sub_service").show();
		}, function () {
			$("#sub_store_grade").find("#seller_sub_service").hide();
		});
		// 加载营销QQ
		var marketing_qq = [];
		if ((service.service_marketing_qq != null) && (service.service_marketing_qq.length > 0)) {
			for (var i = 0; i < service.service_marketing_qq.length; i++) {
				marketing_qq.push({ aty: '0', a: '0', nameAccount: service.service_marketing_qq[i].Account, selector: "yx_" + service.service_marketing_qq[i].Account });
			}
		}
		//插件中营销QQ绑定
		$('.SubStore_Control_Id').each(function () {
			var control_id = $(this).attr('control_id');
			if (control_id != undefined && control_id != '') {
				for (var i = 0; i < service.service_marketing_qq.length; i++) {
					marketing_qq.push({ aty: '0', a: '0', nameAccount: service.service_marketing_qq[i].Account, selector: "yx_control" + control_id + "_" + service.service_marketing_qq[i].Account });
				}
			}
		});
		if (marketing_qq.length > 0) {
			load_marketing_qq(function () {
				BizQQWPA.addCustom(marketing_qq);
			});	
		}
	} else {
		$("#sub_store_grade").find("#seller_sub_service_ctrl").removeClass("sub_header_grade_Online");
		$("#sub_store_grade").find("#seller_sub_service_ctrl").addClass("sub_header_grade_Offline");
		$("#sub_store_grade").find("#seller_sub_service_ctrl_text").text("暂无客服").css('color', '#bbb');
		$("#sub_store_grade").find("#seller_sub_service").hide();
		// 隐藏客服
		$("#sub_store_grade").find("#seller_sub_service_ctrl").hide();
	}
}

//仿淘宝banner图渐进隐藏切换效果
function taobao_carouFredSel(data) {
	var ali = $(data.a1 + ' li');
	var aPage = $(data.a2 + ' div');
	var iNow = 0;

	ali.each(function (index) {
		$(this).mouseover(function () {
			slide(index);
		})
	});
	aPage.not(":first").hide();
	function slide(index) {
		iNow = index;
		ali.eq(index).addClass('selected').siblings().removeClass("selected");
		aPage.eq(index).siblings().stop().removeClass("current").fadeOut(1250);
		aPage.eq(index).stop().addClass('current').fadeIn(1250);
	}

	function autoRun() {
		iNow++;
		if (iNow == ali.length) {
			iNow = 0;
		}
		slide(iNow);
	}

	var timer = setInterval(autoRun, 4000);

	ali.hover(function () {
		clearInterval(timer);
	}, function () {
		timer = setInterval(autoRun, 4000);
	});
}

function banner_ad_location(show, StoreTemplate) {
	if (banner_is_home_page()) {
		if (StoreTemplate.StoreTemplateBanner.BannerPhotoStyle == Constant.MAIN_NAVIGATION_ALL) {
			var banner_sort = $(show).find(".j_store_info_logo").offset();//根据头部logo定位
			$(show).find(".j_shop_adv").css("right", banner_sort.left + "px").show();//广告图位置
		} else {
			$(show).find(".j_shop_adv").show();
		}
		if (StoreTemplate.StoreTemplateBanner.ClassifyInfo == Constant.MAIN_CART_STYLE_BANNER_ClASS_INFO) {
			$(show).find(".j_navsidebar").show();
		}
	}
}
//Baner图片滚动以及高度处理
function banner_carouFredSel(show, StoreTemplate) {
	if (StoreTemplate.HeaderStyle != 3) { return; }

	var height = StoreTemplate.StoreTemplateBanner.BannerHeight;
	if (StoreTemplate.StoreTemplateBanner.BannerType <= Constant.MAIN_CART_STYLE_BANNER_TYPE4) {		
		//高度处理
		$(show).find(".bannerbg").css("height", height + "px");
		var banner_img_width = 1920;
		if (StoreTemplate.StoreTemplateBanner.BannerPhotoStyle == Constant.MAIN_NAVIGATION_STANDARD) {
			banner_img_width = 1200
		} 
		$(show).find(".bannerbg").find(".banner_img").css("background-size", "" + banner_img_width + "px " + height + "px");
		$(show).find(".bannerbg").find(".banner_img a").css("height", height + "px").css("width", banner_img_width);
		//右侧一张图
		$(show).find(".bannerbg").find(".j_rightadv_bg1").css("height", (height - 10) + "px");
		$(show).find(".bannerbg").find(".j_rightadv_bg1 li").css("height", "50%");
		$(show).find(".bannerbg").find(".j_rightadv_bg1 .rightadv_img").css("height", (height - 20) + "px");
		//右侧二张图
		$(show).find(".bannerbg").find(".j_rightadv_bg2").css("height", (height - 10) + "px");
		$(show).find(".bannerbg").find(".j_rightadv_bg2 li").css("height", "50%");
		$(show).find(".bannerbg").find(".j_rightadv_bg2 .rightadv_img").css("height", ((height - 10) / 2) - 10 + "px");
		$(show).find(".bannerbg").find(".j_rightadv_bg2 .rightadv_img img").css("height", ((height - 10) / 2) - 10 + "px");
		//右侧三张图
		$(show).find(".bannerbg").find(".j_rightadv_bg3").css("height", (height - 10) + "px");
		$(show).find(".bannerbg").find(".j_rightadv_bg3 li").css("height", "33.33%");
		$(show).find(".bannerbg").find(".j_rightadv_bg3 .rightadv_img").css("height", ((height - 10) / 3) - 10 + "px");
		$(show).find(".bannerbg").find(".j_rightadv_bg3 .rightadv_img img").css("height", ((height - 10) / 3) - 10 + "px");

		//底部一张图
		$(show).find(".bannerbg").find(".j_btmadv_list1 li").css("width", "100%");
		//底部二张图
		$(show).find(".bannerbg").find(".j_btmadv_list2 li").css("width", "50%");
		$(show).find(".bannerbg").find(".list_box img").css("height", ((height / 2) * 0.947) + "px");
		$(show).find(".j_navsidebar").css("height", height + "px");

		//底部广告图高度
		var ad_min_height = 200;
		var ad_height = height * 0.2;
		if (ad_height > ad_min_height) {
			$(show).find(".j_bannerbg_ad").css("height", ad_height + "px");
			$(show).find(".j_bannerbg_ad").find(".btmadv_list li").css("height", ad_height + "px");
			$(show).find(".j_bannerbg_ad").find(".btmadv_img ").css("height", (ad_height - 20) + "px");
			$(show).find(".j_bannerbg_ad").find(".btmadv_img img").css("height", (ad_height - 20) + "px");
			//设置底部焦点图位置
			if (StoreTemplate.StoreTemplateBanner.BannerAdType == Constant.MAIN_CART_STYLE_BANNER_AD_TYPE1) {
				$(show).find(".j_shop_focus").css("bottom", (ad_height + 2) + "px");
			}
		}
		
		//注册浏览器宽度变化分类菜单与广告位置
		banner_ad_location(show, StoreTemplate);
		$(window).resize(function () {
			banner_ad_location(show, StoreTemplate);
		});
		if (banner_is_home_page()) {
			//分类详情==停靠
			if (StoreTemplate.StoreTemplateBanner.ClassifyInfo == Constant.MAIN_CART_STYLE_BANNER_ClASS_INFO) {
				//注册分类样式悬停事件
				$(show).find(".j_navsidebar li").hover(function () {
					var $this = $(this);
					var dialog = $(show).find(".j_extendedmenus");
					$this.parent().find("li").removeClass("checked");
					$this.addClass("checked");
					var id = $(this).attr("class1_id");
					dialog.each(function (i, v) {
						var j_class1_id = $(this).attr("j_class1_id")
						if (j_class1_id == id) {
							$(this).show();
							$(this).hover(function () {
								$(this).show();
								$this.parent().find("#class_" + id + "").addClass("checked");
							}, function () {
								$(this).hide();
								$this.parent().find("#class_" + id + "").removeClass("checked");
							});
						} else { $(this).hide(); }
					});
				}, function () {
					$(show).find(".j_extendedmenus").hide();
					$(this).parent().find("li").removeClass("checked");
				});
			} else {
				preview_class_style(show, StoreTemplate);
			}
			$(show).find("#bannerbg").show();
		} else {
			$(show).find("#bannerbg").remove();
			preview_class_style(show, StoreTemplate);
		}
	}
	//仿淘宝banner部分图片渐进循环显示效果
	taobao_carouFredSel({ a1: "#lunbonum", a2: ".j_banner_img" })
}



//判断当前页面是否是首页
function banner_is_home_page() {
	if ((controller.toLocaleLowerCase() == "home") && (action.toLocaleLowerCase() == "index") && (index_id == null || index_id == 0)) {
		return true;
	} else {
		return false;
	}
}


function preview_class_style(show, StoreTemplate) {
	//注册全部商品分类悬停事件
	$(show).find(".j_AllGoodsClassStyle").hover(function () {
		if (!banner_is_home_page()) {
			if (StoreTemplate.StoreTemplateBanner.AllGoodsClassStyleIsShow) {
				$(this).find(".j_navsidebar").show();
			}
		} else {
			$(this).find(".j_navsidebar").show();
		}		
		var dialog = $(show).find(".j_extendedmenus");
		//注册分类样式悬停事件
		$(show).find(".j_navsidebar li").hover(function () {
			var id = $(this).attr("class1_id");
			$(this).parent().find("li").removeClass("checked");
			$(this).addClass("checked");
			dialog.each(function (i, v) {
				var j_class1_id = $(this).attr("j_class1_id")
				if (j_class1_id == id) {
					$(this).show();
				} else { $(this).hide(); }
			});
		});
	}, function () {
		$(this).find(".j_navsidebar").hide();
		$(show).find(".j_extendedmenus").hide();
		$(this).parent().find("li").removeClass("checked");
	});
}
