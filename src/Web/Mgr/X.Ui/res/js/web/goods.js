var current_photo = 0;
var store_collection_flag = false;
// 显示团购函数
function countdown(goods) {
	var div = $("#show_goods").find(".countdown_product_time");
	setInterval(function () {
		var now = get_real_now();
		var endDate = jsonTime(goods.RuleEndTime);
		var leftTime = endDate.getTime() - now.getTime();
		var leftsecond = parseInt(leftTime / 1000);

		var data = {};
		data.h = Math.floor(leftsecond / 3600);
		data.m = Math.floor((leftsecond - data.h * 3600) / 60);
		data.s = Math.floor(leftsecond - data.h * 3600 - data.m * 60);

		if ((data.h <= 0) && (data.m <= 0) && (data.s <= 0)) {
			// 倒计时满,重新加载页面
			goto_reload();
		}
		else {
			div.empty();
			$("#goods_countdown_tmpl").tmpl(data).appendTo(div);
			div.show();
		}
	}, 1000);
}
// 显示秒杀函数
function countdown_seckilling(goods) {
	var div = $("#show_goods").find(".countdown_product_time");
	setInterval(function () {
		var now = get_real_now();
		var endDate = jsonTime(goods.RuleStartTime);
		var leftTime = endDate.getTime() - now.getTime();
		var leftsecond = parseInt(leftTime / 1000);

		var data = {};
		data.h = Math.floor(leftsecond / 3600);
		data.m = Math.floor((leftsecond - data.h * 3600) / 60);
		data.s = Math.floor(leftsecond - data.h * 3600 - data.m * 60);

		if ((data.h <= 0) && (data.m <= 0) && (data.s <= 0)) {
			// 倒计时满,重新加载页面
			goto_reload();
		}
		else {
			div.empty();
			$("#goods_seckilling_tmpl").tmpl(data).appendTo(div);
			div.show();
		}
	}, 1000);
}
// 显示显示优惠函数
function countdown_timetoreduce(goods) {
	var div = $("#show_goods").find(".countdown_product_time");
	setInterval(function () {
		var now = get_real_now();
		var endDate = jsonTime(goods.RuleEndTime);
		var leftTime = endDate.getTime() - now.getTime();
		var leftsecond = parseInt(leftTime / 1000);

		var data = {};
		data.h = Math.floor(leftsecond / 3600);
		data.m = Math.floor((leftsecond - data.h * 3600) / 60);
		data.s = Math.floor(leftsecond - data.h * 3600 - data.m * 60);

		if ((data.h <= 0) && (data.m <= 0) && (data.s <= 0)) {
			// 倒计时满,重新加载页面
			goto_reload();
		}
		else {
			div.empty();
			$("#goods_countdown_tmpl").tmpl(data).appendTo(div);
			div.show();
		}
	}, 1000);
}

function show_goods_photo(index) {
	$(".productbigimg img").eq(index).addClass("blockimg");
	$(".productbigimg img").eq(index).siblings().removeClass("blockimg");

	var photo_url = goods.data["Photo" + (index + 1)];
	// 动态加载图片
	if ($(".productbigimg img").eq(index).attr("src") != photo_url) {
		$(".productbigimg img").eq(index).attr("src", photo_url);
	}

	$(".productsmallimg li").removeClass("libg");
	$(".productsmallimg img[photo_index=" + index + "]").parents("li").addClass("libg");
	if (index != 0) {
		$("#goodssold").hide();
	}
}

function show_goods(goods) {
	function show_goods_property() {
		// 选择第一个库存不为0的属性
		var selected_prop = [0, 0, 0];
		for (var i = 0; i < goods.data.Property.length; i++) {
			if (goods.data.Property[i].Number > 0) {
				selected_prop[0] = goods.data.Property[i].Prop1ItemId;
				selected_prop[1] = goods.data.Property[i].Prop2ItemId;
				selected_prop[2] = goods.data.Property[i].Prop3ItemId;
				break;
			}
		}
		// 显示商品属性
		$("#goods_property_content").find(".goods_prop").remove();
		$("#goods_property_tmpl").tmpl({ data: goods.data, selected_prop: selected_prop }).prependTo("#goods_property_content");

		// 设置被选择属性名
		if (goods.data.PropertyClass != null) {
			for (var i = 0; i < goods.data.PropertyClass.length; i++) {
				var prop_class_content = $("#PropClassId_" + goods.data.PropertyClass[i].PropClassId);
				prop_class_content.attr("sel_item_name", prop_class_content.find(".prop_item[item_id=" + selected_prop[i] + "]").attr("item_name"));
			}
		}

		// 初始化属性选择
		change_property();
		// 用户点击改变属性
		$(".prop_item").click(function () {
			// 点击取消处理
			if ($(this).attr("item_id") == $(this).parents(".goods_prop").attr("sel_item")) {
				$(this).parents(".goods_prop").attr("sel_item", "");
				$(this).parents(".goods_prop").attr("sel_item_name", "");
				$(this).find(".prop_select").removeClass("selected_icon");
				$(this).removeClass("checked");
				if ($(this).attr("is_photo")) {
					$(this).parent().find("li").removeClass("no_stock_border");
					$(this).parent().find("li").find(".no_stock").hide();
				} else {
					$(this).parent().find("li").removeClass("no_stock");
				}
				$("#goodsname").text("");
				change_property();
				return;
			}
			// 无货属性不可点击选择
			if ($(this).attr("is_photo")) {
				if ($(this).hasClass("no_stock_border")) {
					return false;
				}
			} else {
				if ($(this).hasClass("no_stock")) {
					return false;
				}
			}
			$(this).parents(".goods_prop").find(".prop_select").removeClass("selected_icon");
			$(this).find(".prop_select").addClass("selected_icon");

			$(this).parents(".goods_prop").find(".prop_item").removeClass("checked");
			$(this).addClass("checked");

			// 设置被选择的属性
			var is_photo = $(this).attr("is_photo");
			var item_id = $(this).attr("item_id");
			var item_name = $(this).attr("item_name");
			$(this).parents(".goods_prop").attr("sel_item", item_id);
			$(this).parents(".goods_prop").attr("sel_item_name", item_name);
			change_property();

			// 点击图片属性处理
			if ((is_photo != null) && (is_photo == "true")) {
				current_photo = Number($(this).attr("item_index"));
				show_goods_photo(current_photo);
			}
		});
		//商品每个下拉属性选择改变
		$(".buy_prop_dropdown").change(function () {
			var item_id = $(this).val();
			var item_name = $(this).find("option:selected").text();
			$(this).parents(".goods_prop").attr("sel_item", item_id);
			$(this).parents(".goods_prop").attr("sel_item_name", item_name);
			change_property();
		});
	}

	if (goods.data.IsTempGoods != null) {
		// 临时商品
		$("#show_goods").empty();
		goods.data.IsTempGoods = true;
		goods.data.GoodsRecommend = null;
		goods.data.TopGoodsId = goods.data.GoodsId;
		$("#show_temp_goods_tmpl").tmpl(goods.data).appendTo("#show_goods");

		// 主页面跳转
		$("#show_goods").find("#goto_home").click(function () {
			goto_home();
		});

		// 一级类点击
		$("#show_goods").find("#class1").click(function () {
			class1_id = $(this).attr("class1_id");
			goto_search_goods_link(null, class1_id, null, null, null, null, null);
		});

		// 二级类点击
		$("#show_goods").find("#class2").click(function () {
			class2_id = $(this).attr("class2_id");
			goto_search_goods_link(null, null, class2_id, null, null, null, null);
		});

		// 三级类点击
		$("#show_goods").find("#class3").click(function () {
			class3_id = $(this).attr("class3_id");
			goto_search_goods_link(null, null, null, class3_id, null, null, null);
		});

		// 促销类型点击
		$("#show_goods").find("#sales").click(function () {
			rule_id = $(this).attr("rule_id");
			goto_search_goods_link(null, null, null, null, rule_id, null, null);
		});

		// 大图片未设置对策
		if (CheckInput.IsEmpty(goods.data.Detail.Photo1L)) {
			goods.data.Detail.Photo1L = goods.data.Detail.Photo1;
		}
		if (CheckInput.IsEmpty(goods.data.Detail.Photo2L)) {
			goods.data.Detail.Photo2L = goods.data.Detail.Photo2;
		}
		if (CheckInput.IsEmpty(goods.data.Detail.Photo3L)) {
			goods.data.Detail.Photo3L = goods.data.Detail.Photo3;
		}
		if (CheckInput.IsEmpty(goods.data.Detail.Photo4L)) {
			goods.data.Detail.Photo4L = goods.data.Detail.Photo4;
		}
		if (CheckInput.IsEmpty(goods.data.Detail.Photo5L)) {
			goods.data.Detail.Photo5L = goods.data.Detail.Photo5;
		}
		if (CheckInput.IsEmpty(goods.data.Detail.Photo6L)) {
			goods.data.Detail.Photo6L = goods.data.Detail.Photo6;
		}
		if (CheckInput.IsEmpty(goods.data.Detail.Photo7L)) {
			goods.data.Detail.Photo7L = goods.data.Detail.Photo7;
		}
		if (CheckInput.IsEmpty(goods.data.Detail.Photo8L)) {
			goods.data.Detail.Photo8L = goods.data.Detail.Photo8;
		}
		if (CheckInput.IsEmpty(goods.data.Detail.Photo9L)) {
			goods.data.Detail.Photo9L = goods.data.Detail.Photo9;
		}
		if (CheckInput.IsEmpty(goods.data.Detail.Photo10L)) {
			goods.data.Detail.Photo10L = goods.data.Detail.Photo10;
		}
	}

	//图片移动
	$('#user_message_box').carouFredSel({
		auto: false,
		prev: ".Arrow_right",
		next: ".Arrow_left",
		width: 318,
		circular: false, //true为无线循环，false为单轮循环
		infinite: false,	//是否启用循环
		scroll: 1		//默认等于显示的个数，一次滚动个数
	});

	goods.data.PropType = new Array({ IsPhoto: false, Style: goods.data.Prop1ClassStyle }, { IsPhoto: false, Style: goods.data.Prop2ClassStyle }, { IsPhoto: false, Style: goods.data.Prop3ClassStyle });
	if ((goods.data.PropertyClass != null) && (goods.data.PropertyClass.length > 0)) {
		for (var i = 0; i < goods.data.PropertyClass.length; i++) {
			goods.data.PropType[i].IsPhoto = goods.data.PropertyClass[i].IsPhoto;
		}
	}

	show_goods_property();

	if (goods.data.IsTempGoods == null) {
		// 非临时商品,取得商品信息(动态)
		goods.get_goods_dynamic_data(function (goods_dynamic) {
			// 促销销量
			$("#promo_number").text(goods_dynamic[0].PromoNumber);
			// 评论总数
			$("#show_evaluate_total").text(goods_dynamic[0].EvaluateTotal);

			$("#goods_evaluate_total_tmpl").tmpl(goods_dynamic[0]).appendTo("#goods_evaluate_total_show");
			$("#show_evaluate_up").click(function () {
				$("#show_evaluate_li").addClass("checked");
				$("#show_evaluate_li").siblings().removeClass("checked");
				var n = $(".goodsdetails_content_tabbox li").index($("#show_evaluate_li"))
				$(".goods_info").hide();

				$(".goods_info").eq(n).show();

				// 显示商品评论
				goods.GetEvaluate(0, Evaluate_PageSize, 0);
				// 滚动页面到指定位置
				$("html,body").stop(true);
				$("html,body").animate({ scrollTop: $("#show_evaluate_li").offset().top - 30 }, 300);
			});

			goods.data.Property = goods_dynamic[0].Property;
			show_goods_property();
		});
	}
	// 设定二维码URL
	$("#goods_url").qrcode({
		render: $.browser.msie ? 'div' : 'canvas',
		width: 130,
		height: 130,
		correctLevel: QRErrorCorrectLevel.M,
		text: window.location.href
	});

	$("#buy_content_text_b_Open").parent().addClass("buy_content_text_b_Open");

	// 显示商品详细,评论
	$(".goodsdetails_content_tabbox li").click(function () {
		$(this).addClass("checked");
		$(this).siblings().removeClass("checked");
		var n = $(".goodsdetails_content_tabbox li").index($(this))
		$(".goods_info").hide();

		$(".goods_info").eq(n).show();
	});

	$("#show_evaluate").click(function () {
		// 显示商品评论
		goods.GetEvaluate(0, Evaluate_PageSize, 0);
	});

	$("#show_advisory").click(function () {
		// 显示商品咨询
		goods.SearchAdvisory(0, Advisory_PageSize);
	});

	$(".goods_info").hide();
	$(".goods_info").eq(0).show();

	var now = get_real_now();

	if ((goods.data != null) &&
		(goods.data.RuleId != null)) {
		if (goods.data.RuleType == Constant.SALESRULE_GROUP_BUY) {
			if ((now >= jsonTime(goods.data.RuleStartTime)) &&
				(now < jsonTime(goods.data.RuleEndTime))) {
				countdown(goods.data);
			}
		}
		else if (goods.data.RuleType == Constant.SALESRULE_SECKILLING) {
			if (now < jsonTime(goods.data.RuleStartTime)) {
				countdown_seckilling(goods.data);
			}
			else if ((now < jsonTime(goods.data.RuleEndTime)) && (now > jsonTime(goods.data.RuleStartTime))) {
				countdown(goods.data);
			}
		}
		else if (goods.data.RuleType == Constant.SALESRULE_TIME_TO_REDUCE) {
			if (now < jsonTime(goods.data.RuleEndTime)) {
				countdown_timetoreduce(goods.data);
			}
		}
	}

	// 鼠标滑过小图显示大图
	$(".productsmallimg li").mouseover(function () {								//添加类
		current_photo = Number($(this).find("img").attr("photo_index"));
		show_goods_photo(current_photo);
	});

	// 鼠标滑过大图显示放大图
	$(".productbigimg_box").mouseover(function () {								//添加类
		var photo_url = goods.data["Photo" + (current_photo + 1)];
		var photo_l_url = goods.data["Photo" + (current_photo + 1) + "L"];

		if (goods.data.IsTempGoods) {
			if ($(this).find(".product_zoom img").attr("src") != photo_url) {
				$(this).find(".product_zoom img").attr("src", photo_url);
			}
		}
		else {
			if ($(this).find(".product_zoom img").attr("src") != photo_l_url) {
				$(this).find(".product_zoom img").attr("src", photo_l_url);
			}
		}

		$(this).find(".product_zoom").show();
		$(this).find(".product_lens").show();
	});

	$(".productbigimg_box").mouseout(function () {								//添加类
		$(this).find(".product_zoom").hide();
		$(this).find(".product_lens").hide();
	});

	$(".productbigimg_box").mousemove(function (e) {								//添加类
		// 商品显示图片宽高
		var l_width = $(this).find(".productbigimg").width();
		var l_height = $(this).find(".productbigimg").height();
		// 放大指示器宽高
		var s_width = $(this).find(".product_lens").width();
		var s_height = $(this).find(".product_lens").height();
		// 实际预览图片宽高
		var p_width = $(this).find(".product_zoom img").width();
		var p_height = $(this).find(".product_zoom img").height();

		var left = e.pageX - $(this).find(".productbigimg").offset().left - s_width / 2;
		var top = e.pageY - $(this).find(".productbigimg").offset().top - s_height / 2;

		if (left < 0) {
			left = 0;
		}
		else if (left > (l_width - s_width)) {
			left = l_width - s_width;
		}

		if (top < 0) {
			top = 0;
		}
		else if (top > (l_height - s_height)) {
			top = l_height - s_height;
		}

		$(this).find(".product_lens").css("top", top + "px");
		$(this).find(".product_lens").css("left", left + "px");
		$(this).find(".product_zoom img").css("top", "-" + top * (p_height / l_height) + "px");
		$(this).find(".product_zoom img").css("left", "-" + left * (p_width / l_width) + "px");
	});

	if (goods.data.IsTempGoods == null) {
		// 显示组合购买
		goods.get_pack_buy(function (data) {
			show_pack_buy_goods(data);
		});
		//显示入驻商户客服
		if (goods.data.SellerStoreInfo.SellerSubId != Constant.STORE_SELLER_SUB_ID) {
			show_seller_sub_service(goods.data.SellerStoreInfo.StoreSetService);
		}
	}
	// 显示商品浏览历史
	show_goods_history(goods.get_goods_history());

	//获取商品关联推荐商品
	show_goods_recommend();

	if (!goods.data.IsTempGoods && (goods.data.Status == Constant.OPEN)) {
		// 设置商品浏览历史
		goods.add_goods_history();
	}

	// 关闭购物车提示框
	$(document).click(function (event) {
		if (!$(event.target).parents().add(event.target).hasClass('mod_carttip')) {
			if ($('#add_goods_to_cart_box').hasClass("cardHide")) {
				$('#add_goods_to_cart_box').fadeOut(600).removeClass('cardHide');
			}
		}

		if (!$(event.target).parents().add(event.target).hasClass('addcart')) {
			if (!$('#add_goods_to_cart_dialog').is(":hidden")) {
				$('#add_goods_to_cart_dialog').dialog("close");
			}
		}
	});

	$(window).resize(function () {
		if ($('#add_goods_to_cart_box').hasClass("cardHide")) {
			$('#add_goods_to_cart_box').hide().removeClass('cardHide');
		}
	});

	if (goods.data.RuleType != Constant.SALESRULE_PACK_BUY) {
		var elem = $("#top_menu");
		var sub_lock = $("#sub_lock_tab")
		var startPos = elem.offset().top;
		var test_height = elem.offset().top;
		var header_height = $("#header_content_box").height();
		var hidden_sub_height = $("#sub_lock_tab").height();
		$("#hidden_sub_lock").css({ "height": hidden_sub_height });
		$(window).scroll(function () {
			startPos = test_height;
			startPos -= (header_height - $("#header_content_box").height());
			var scroll = $(this).scrollTop();
			var elem_array = elem.find("a");
			if (scroll > startPos) {
				$("#hidden_tabbox").show();
				$("#hidden_sub_lock").show();
				$(elem).css({ "position": "fixed", "top": 0, "width": 950, "z-index": 2 });
				$(sub_lock).css({ "position": "fixed", "top": 0, "width": 226, "z-index": 9, background: "white" });
				for (var i = 0; i < elem_array.length; i++) {
					elem_array.eq(i).attr("href", "#tabbox");
				}
			} else if (scroll < startPos) {
				$("#hidden_tabbox").hide();
				$("#hidden_sub_lock").hide();
				$(elem).css({ "position": "static" });
				$(sub_lock).css({ "position": "static" });
				for (var i = 0; i < elem_array.length; i++) {
					elem_array.eq(i).removeAttr("href");
				}
			}
		});
	}

	$.ajax({
		url: getActionPath("CollectionGoodsIsExist", "Cart"),
		dataType: "json",
		async: false,
		contentType: 'application/json',
		type: "POST",
		data: JSON.stringify({ goods_id: JSON.stringify(goods.goods_id) }),
		success: function (data) {
			if (data.success) {
				if (data.is_exist) {
					$("#btn_ycollection").show();
				} else {
					$("#btn_collection").show();
				}
				show_btn_collection();
			}
		},
		error: function (request, status, errorThrown) {
			ShowAjaxError(request, status, errorThrown);
		}
	});

	if (goods.data.SellerSubId > 0) {
		$.ajax({
			url: getActionPath("CollectionStoreIsExist", "Cart"),
			dataType: "json",
			async: false,
			contentType: 'application/json',
			type: "POST",
			data: JSON.stringify({ seller_sub_id: JSON.stringify(goods.data.SellerSubId) }),
			success: function (data) {
				if (data.success) {
					if (data.is_exist) {
						store_collection_flag = true;
					} else {
						store_collection_flag = false;
					}
				} else {
					store_collection_flag = false;
				}
			},
			error: function (request, status, errorThrown) {
				ShowAjaxError(request, status, errorThrown);
				store_collection_flag = false;
			}
		});
		if (store_collection_flag) {
			$(".j_store_collection").find("div").addClass("collect_icon_red").removeClass("collect_icon_gray");
			$("#store_collection_text").text("已收藏");
		} else {
			$(".j_store_collection").find("div").addClass("collect_icon_gray").removeClass("collect_icon_red");
			$("#store_collection_text").text("收藏店铺");
		}
	}
	// 收藏商品
	$(".j_goods_collection").click(function () {
		var goodsid = new Array();
		goodsid[0] = $(this).attr("goods_id");

		collection_goods(goodsid);
		$("#btn_ycollection").show();
		$("#btn_collection").hide();
		show_btn_collection();
	});
	// 取消收藏商品
	$(".j_goods_cancel_collection").click(function () {
		var goodsid = new Array();
		goodsid[0] = $(this).attr("goods_id");

		cancel_collection_goods(goodsid);

		$("#btn_ycollection").hide();
		$("#btn_collection").show();
		$("#btn_cancel_collection").find("a").hover(function () {
		}, function () {
			$("#btn_cancel_collection").hide();
			$("#btn_ycollection").hide();
		});
	});
	// 收藏店铺
	$(".j_store_collection").click(function () {
		var seller_sub_id = $(this).attr("seller_sub_id");
		if (!store_collection_flag) {
			collection_store(seller_sub_id);
			store_collection_flag = true;
			$(".j_store_collection").find("div").addClass("collect_icon_red").removeClass("collect_icon_gray");
			$("#store_collection_text").text("已收藏");
		} else {
			cancel_collection_store(seller_sub_id);
			store_collection_flag = false;
			$(".j_store_collection").find("div").addClass("collect_icon_gray").removeClass("collect_icon_red");
			$("#store_collection_text").text("收藏店铺");
		}
	});

	if (goods.data.SellerSubId > 0) {
		$("#sellersub_select").click(function () {
			var key = $("#keyword").val();
			var price_from = $("#price_from").val();
			var price_to = $("#price_to").val();
			var select_url = getActionPath("Search", "Home") + "?sub=" + goods.data.SellerSubId + "&search_type=0";
			if (key != "") {
				select_url += "&key=" + key;
			}
			if (price_from != "") {
				select_url += "&p_f=" + parseInt(price_from) * 100;
			}
			if (price_to != "") {
				select_url += "&p_t=" + parseInt(price_to) * 100;
			}
			window.open(select_url);
		});
	}
}

function show_btn_collection(flag) {
	$("#btn_collection").find("a").hover(function () {
		$("#btn_collection").find("a").css({ "background": "#df3032", "color": "#fff" }).find("div").addClass("collect_icon_white");
	}, function () {
		$("#btn_collection").find("a").css({ "background": "#f0f0f0", "color": "#666" }).find("div").removeClass("collect_icon_white");
	});
	$("#btn_ycollection").find("a").hover(function () {
		$("#btn_cancel_collection").show();
		$("#btn_ycollection").hide();
	}, function () {
	});
	$("#btn_cancel_collection").find("a").hover(function () {
	}, function () {
		$("#btn_cancel_collection").hide();
		$("#btn_ycollection").show();
	});
}

// 显示弹出的购物车对话框
function show_popup_cart(data) {
	var cart_data = {};
	cart_data.GoodsTotalCount = 0;
	cart_data.Subtotal = 0;
	if (data.amount != null) {
		for (var k = 0; k < data.amount.length; k++) {
			for (var i = 0; i < data.amount[k].SalesRuleInfo.length; i++) {
				for (var j = 0; j < data.amount[k].SalesRuleInfo[i].GoodsList.length; j++) {
					cart_data.GoodsTotalCount += data.amount[k].SalesRuleInfo[i].GoodsList[j].Count;
				}
			}
			cart_data.Subtotal += data.amount[k].Subtotal;
		}
		cart_data.SubtotalDisplay = parseFloat(cart_data.Subtotal / 100).toFixed(2);
	}

	$('#add_goods_to_cart_dialog').empty();
	$("#add_goods_to_cart_dialog_tmpl").tmpl(cart_data).appendTo('#add_goods_to_cart_dialog');
	$('#add_goods_to_cart_dialog').dialog({
		dialogClass: "no_title_dialog",
		minHeight: 0,
		width: 306,
		title: "添加商品成功",
		autoOpen: false,
		resizable: false
	});
	$('#add_goods_to_cart_dialog').dialog("open");

	$('#add_goods_to_cart_dialog').find("#continue").click(function () {
		$('#add_goods_to_cart_dialog').dialog("close");
	});

	$('#add_goods_to_cart_dialog').find("#close").click(function () {
		$('#add_goods_to_cart_dialog').dialog("close");
	});

	$('#add_goods_to_cart_dialog').find("#goto_cart").click(function () {
		goto_cart();
	});

	//2秒后自动隐藏
	if (tempInter != null) {
		clearTimeout(tempInter);
	}
	tempInter = setTimeout(function () {
		if (!$('#add_goods_to_cart_dialog').is(":hidden")) {
			$('#add_goods_to_cart_dialog').dialog("close");
		}
	}, 2000);
}

// 显示购物车对话框
var tempInter = null;
function show_new_popup_cart(obj) {
	$('#add_goods_to_cart_box').empty();
	$('#add_goods_to_cart_box').show().addClass('cardHide');
	$("#add_goods_to_cart_box_tmpl").tmpl(null).appendTo('#add_goods_to_cart_box');

	var win_height = $(window).height();
	var popup_height = $('#add_goods_to_cart_box').height();
	var ele = $(obj);
	var ele_data = {
		top: ele.offset().top,
		left: ele.offset().left
	};

	//默认显示在下面 弹出框是否在可见范围内
	if (win_height - (ele_data.top - $(window).scrollTop()) > (popup_height + 180)) {
		ele_data.top = ele_data.top + 47 + "px";
		$('#add_goods_to_cart_box').find('.mod_carttip_arr').addClass('mod_carttip_arr1');
	}
	else {
		ele_data.top = (ele_data.top - popup_height) - 12 + "px";
		$('#add_goods_to_cart_box').find('.mod_carttip_arr').addClass('mod_carttip_arr3');
	}

	$('#add_goods_to_cart_box')[0].style.top = ele_data.top;
	$('#add_goods_to_cart_box')[0].style.left = ele_data.left + "px";

	$('#add_goods_to_cart_box').find(".mod_carttip_close").click(function () {
		$('#add_goods_to_cart_box').fadeOut(600);
	});

	$('#add_goods_to_cart_box').find("#continue").click(function () {
		$('#add_goods_to_cart_box').fadeOut(600);
	});

	$('#add_goods_to_cart_box').find("#goto_cart").click(function () {
		goto_cart();
	});

	//2秒后自动隐藏
	if (tempInter != null) {
		clearTimeout(tempInter);
	}
	tempInter = setTimeout(function () {
		if (!$('#add_goods_to_cart_box').is(":hidden")) {
			$('#add_goods_to_cart_box').fadeOut(800);
		}
	}, 2000);
}

// 取得当前设置的属性值
function get_prop_value() {
	var property_class = goods.data.PropertyClass;
	if ((property_class != null) && (property_class.length > 0)) {
		var prop_value = [];
		for (var i = 0; i < property_class.length; i++) {
			var prop = $("#PropClassId_" + property_class[i].PropClassId);
			if (prop.length > 0) {
				var item_id = prop.attr("sel_item");
				if ((item_id == null) || (item_id == "") || (item_id == 0)) {
					// 无效的item id
					return null;
				}
				else {
					prop_value[i] = item_id;
				}
			}
			else {
				return null;
			}
		}
		return prop_value;
	}
	else if ((goods.data.Property != null) && (goods.data.Property.length > 0)) {
		return goods.data.Property[0];
	}
	else {
		return null;
	}
}

// 取得当前设置的属性值名称
function get_prop_name() {
	var property_class = goods.data.PropertyClass;
	if ((property_class != null) && (property_class.length > 0)) {
		var prop_value = [];
		for (var i = 0; i < property_class.length; i++) {
			var prop = $("#PropClassId_" + property_class[i].PropClassId);
			if (prop.length > 0) {
				var item_name = prop.attr("sel_item_name");
				if ((item_name == null) || (item_name == "") || (item_name == 0)) {
					// 无效的item id
					return null;
				}
				else {
					prop_value[i] = item_name;
				}
			}
			else {
				return null;
			}
		}
		return prop_value;
	}
	else {
		return null;
	}
}

// 商品属性选择变化处理
function change_property() {
	// 取得买家选择的属性
	var prop_value = get_prop_value();
	if (prop_value != null) {
		// 显示当前属性的数量和价格
		var curr_prop = goods.get_sel_property_data(prop_value);
		if (curr_prop != null) {
			//设定库存
			//$("#goods_number").text(curr_prop.Number);
			curr_prop.selItemName = get_prop_name();
			curr_prop.Unit = goods.data.Unit;
			$("#buy_goods_num_box").empty();
			$(".buy_content_button").empty();
			$("#goods_buy_num_tmpl").tmpl(curr_prop).appendTo($("#buy_goods_num_box"));
			$("#goods_buy_button_tmpl").tmpl(curr_prop).appendTo($(".buy_content_button"));
			if (curr_prop.Number <= 0) {
				$("#goodssold").show();
			} else {
				$("#goodssold").hide();
			}

			//设定价格
			goods.data.Price = curr_prop.Price;
			//显示促销价格信息
			show_sales_price();

			if (!CheckInput.IsEmpty(curr_prop.Code)) {
				$("#goods_property_code").show();
				$("#goods_code").text(curr_prop.Code);
			} else {
				$("#goods_property_code").hide();
				$("#goods_code").text("");
			}

			if (curr_prop.selItemName != null) {
				var prop_length = curr_prop.selItemName.length;
				if (prop_length > 0) {
					var select_prop = "(";
					for (var i = 0; i < prop_length; i++) {
						select_prop += curr_prop.selItemName[i] + "  ";
					}
					select_prop += ")";
					$("#goodsname").text(select_prop);
				}
			}

			// 数量加点击事件
			$("#goods_number_plus").click(function () {
				var num = goods.set_number($("input[name=goods_number_input]").val(), 1, prop_value);
				$("input[name=goods_number_input]").val(num);
			});
			// 数量减点击事件
			$("#goods_number_minus").click(function () {
				var num = goods.set_number($("input[name=goods_number_input]").val(), -1, prop_value);
				$("input[name=goods_number_input]").val(num);
			});
			// 数量输入失去焦点
			$("input[name=goods_number_input]").blur(function () {
				var num = goods.set_number($("input[name=goods_number_input]").val(), 0, prop_value);
				$("input[name=goods_number_input]").val(num);
			});

			// 调整商品数量
			var num = goods.set_number($("input[name=goods_number_input]").val(), 0, prop_value);
			$("input[name=goods_number_input]").val(num);

			// 点击添加商品到购物车
			$("#add_to_cart").click(function (e) {
				if ((goods.data.IsTempGoods == undefined) && (curr_prop != undefined) && (curr_prop.Number > 0)) {
					if (goods.add_goods_to_cart(false)) {
						//添加成功弹出层
						show_new_popup_cart(this);
						e.stopPropagation();
					}
				}
			});

			// 点击立即购买
			$("#buy_now").click(function () {
				if ((goods.data.IsTempGoods == undefined) && (curr_prop != undefined) && (curr_prop.Number > 0)) {
					//goods.add_goods_to_cart(true);
					var prop_value = get_prop_value();
					if (prop_value == null) {
						alert("请选择商品属性");
						return;
					}
					//构建单个商品的购物车信息（不存放在购物车里，只是将该商品的信息打包），生成buy_cart_id,页面跳转到订单确认页面
					var cart_data = [], goods_data = [], fromsellersubid;
					var count = $("#goods_number_input").val();
					if (parseInt(get_url_param("From")) >= 0) {
						fromsellersubid = get_url_param("From");
					} else {
						fromsellersubid = Constant.FROM_SELLER_SUB_ID_LOCALSTORE;
					}
					if (count > 0) {
						goods_data[0] = {
							"CheckStatus": Constant.GOODS_CHECK_STATUS_VERIFIED_SUCCESS,
							"Count": count,
							"FromSellerSubId": fromsellersubid,
							"GoodsId": goods.data.GoodsId,
							"Prop1ItemId": (prop_value.length >= 1) ? prop_value[0] : 0,
							"Prop2ItemId": (prop_value.length >= 2) ? prop_value[1] : 0,
							"Prop3ItemId": (prop_value.length >= 3) ? prop_value[2] : 0,
							"SellerSubId": goods.data.SellerSubId,
							"Status": Constant.OPEN,
							"StoreId": undefined
						};
						cart_data.push(goods_data);
						create_buy_cart_id(cart_data, function (buy_cart_id) {
							goto_link(getActionPath("OrderConfirm", "Home") + "?buy_cart=" + buy_cart_id);
						});
					} else {
						alert("商品数量必须大于0");
					}
				}
			});
		}
	} else {
		if (goods.data.Property.length > 0) {
			var no_number_prop_value = [];
			no_number_prop_value.push(goods.data.Property[0].Prop1ItemId);
			no_number_prop_value.push(goods.data.Property[0].Prop2ItemId);
			no_number_prop_value.push(goods.data.Property[0].Prop3ItemId);
			var no_number_curr_prop = goods.get_sel_property_data(no_number_prop_value);

			no_number_curr_prop.selItemName = null;
			no_number_curr_prop.Unit = goods.data.Unit;
			$("#buy_goods_num_box").empty();
			//$("#goods_buy_num_tmpl").tmpl(no_number_curr_prop).appendTo($("#buy_goods_num_box"));
			$(".buy_content_button").empty();
			$("#goods_buy_button_disabled_tmpl").tmpl(curr_prop).appendTo($(".buy_content_button"));
			$("#goodssold").hide();

			//设定价格
			goods.data.Price = no_number_curr_prop.Price;
			//显示促销价格信息
			show_sales_price();
		}
	}


	// 遍历所有属性,设置完全无货的属性为不可用
	if ((goods.data.PropertyClass != null) && (goods.data.PropertyClass.length > 0)) {
		// 仅有属性商品做此处理
		var prop_class1_ele = goods.data.PropertyClass.length >= 1 ? $("#PropClassId_" + goods.data.PropertyClass[0].PropClassId) : null;
		var prop_class2_ele = goods.data.PropertyClass.length >= 2 ? $("#PropClassId_" + goods.data.PropertyClass[1].PropClassId) : null;
		var prop_class3_ele = goods.data.PropertyClass.length >= 3 ? $("#PropClassId_" + goods.data.PropertyClass[2].PropClassId) : null;
		// 清除历史状态
		if (prop_class1_ele != null) {
			prop_class1_ele.find(".prop_item").attr("instock1", null).attr("instock2", null).attr("instock3", null);
		}
		if (prop_class2_ele != null) {
			prop_class2_ele.find(".prop_item").attr("instock1", null).attr("instock2", null).attr("instock3", null);
		}
		if (prop_class3_ele != null) {
			prop_class3_ele.find(".prop_item").attr("instock1", null).attr("instock2", null).attr("instock3", null);
		}

		for (var i = 0; i < goods.data.Property.length; i++) {
			if (goods.data.Property[i].Number > 0) {
				if ((prop_class1_ele != null) && (goods.data.Property[i].Prop1ItemId > 0)) {
					prop_class1_ele.find(".prop_item[item_id=" + goods.data.Property[i].Prop1ItemId + "]").attr("instock1", "true");
				}
				if ((prop_class2_ele != null) && (goods.data.Property[i].Prop2ItemId > 0)) {
					prop_class2_ele.find(".prop_item[item_id=" + goods.data.Property[i].Prop2ItemId + "]").attr("instock1", "true");
				}
				if ((prop_class3_ele != null) && (goods.data.Property[i].Prop3ItemId > 0)) {
					prop_class3_ele.find(".prop_item[item_id=" + goods.data.Property[i].Prop3ItemId + "]").attr("instock1", "true");
				}
			}
		}

		function stock_process(ele, diag_attr) {
			var instock = $(ele).attr(diag_attr);
			if ((instock != null) && (instock == "true")) {
				if ($(ele).attr("is_photo")) {
					$(ele).removeClass("no_stock_border");
					$(ele).children().find("div").hide();
				}
				else {
					$(ele).removeClass("no_stock");
				}
			}
			else {
				if ($(ele).attr("is_photo")) {
					$(ele).addClass("no_stock_border");
					$(ele).children().find("div").show();
				}
				else {
					$(ele).addClass("no_stock");
				}
			}
		}
		// 设置所有属性的显示状态
		if (prop_class1_ele != null) {
			prop_class1_ele.find(".prop_item").each(function (i, ele) {
				stock_process(ele, "instock1");
			});
		}
		if (prop_class2_ele != null) {
			prop_class2_ele.find(".prop_item").each(function (i, ele) {
				stock_process(ele, "instock1");
			});
		}
		if (prop_class3_ele != null) {
			prop_class3_ele.find(".prop_item").each(function (i, ele) {
				stock_process(ele, "instock1");
			});
		}

		// 第一属性选择判断
		var prop1_sel = prop_class1_ele != null ? prop_class1_ele.attr("sel_item") : null;
		var prop2_sel = prop_class2_ele != null ? prop_class2_ele.attr("sel_item") : null;

		if ((prop_class1_ele != null) && (prop_class2_ele != null) && (prop1_sel != null) && (prop1_sel > 0)) {
			for (var i = 0; i < goods.data.Property.length; i++) {
				if ((goods.data.Property[i].Prop1ItemId == prop1_sel) && (goods.data.Property[i].Number > 0)) {
					if (goods.data.Property[i].Prop2ItemId > 0) {
						prop_class2_ele.find(".prop_item[item_id=" + goods.data.Property[i].Prop2ItemId + "]").attr("instock2", "true");
					}
					if (prop_class3_ele != null) {
						if (goods.data.Property[i].Prop3ItemId > 0) {
							prop_class3_ele.find(".prop_item[item_id=" + goods.data.Property[i].Prop3ItemId + "]").attr("instock2", "true");
						}
					}
				}
			}
			prop_class2_ele.find(".prop_item").each(function (i, ele) {
				stock_process(ele, "instock2");
			});
			if (prop_class3_ele != null) {
				prop_class3_ele.find(".prop_item").each(function (i, ele) {
					stock_process(ele, "instock2");
				});
			}
		}
		// 第二属性选择判断
		if ((prop_class2_ele != null) && (prop_class3_ele != null) && (prop2_sel != null) && (prop2_sel > 0)) {
			if ((prop1_sel != null) && (prop1_sel > 0)) {
				for (var i = 0; i < goods.data.Property.length; i++) {
					if ((goods.data.Property[i].Prop1ItemId == prop1_sel) && (goods.data.Property[i].Prop2ItemId == prop2_sel) && (goods.data.Property[i].Number > 0)) {
						if (goods.data.Property[i].Prop3ItemId > 0) {
							prop_class3_ele.find(".prop_item[item_id=" + goods.data.Property[i].Prop3ItemId + "]").attr("instock3", "true");
						}
					}
				}
			}
			else {
				for (var i = 0; i < goods.data.Property.length; i++) {
					if ((goods.data.Property[i].Prop2ItemId == prop2_sel) && (goods.data.Property[i].Number > 0)) {
						if (goods.data.Property[i].Prop3ItemId > 0) {
							prop_class3_ele.find(".prop_item[item_id=" + goods.data.Property[i].Prop3ItemId + "]").attr("instock3", "true");
						}
					}
				}
			}
			prop_class3_ele.find(".prop_item").each(function (i, ele) {
				stock_process(ele, "instock3");
			});
		}
	}
}

function create_buy_cart_id(data, on_ok) {
	OpenLoadingDialog();
	$.ajax({
		url: getActionPath("CreateBuyCarId", "Cart"),
		dataType: "json",
		contentType: 'application/json',
		type: "POST",
		data: stringify({ cart: data }),
		success: function (data) {
			CloseLoadingDialog();
			if (data.success == true) {
				if (on_ok != null) {
					on_ok(data.buy_cart_id);
				}
			}
			else {
				ShowActionError(data.error, "buyer");
			}
		},
		error: function (request, status, errorThrown) {
			ShowAjaxError(request, status, errorThrown);
		}
	});
}

// 显示促销 + 属性价格
function show_sales_price() {

	// 显示促销信息
	var sales = get_goods_sales(goods.data);
	if (sales.rule_display) {
		// 商品有促销,显示促销价格
		if (sales.rule_icon != null) {
			sales.rule_icon.appendTo($("#sales_icon"));
		}
		//取促销价
		if (sales.rule_price2 != null) {
			$("#sales_price").text((sales.rule_price2 / 100).toFixed(2));
		}
		else {
			$("#sales_price").text((goods.data.Price / 100).toFixed(2));
		}

		if (sales.rule_discount_txt != null) {
			$("#rule_discount_txt").text(sales.rule_discount_txt);
		} else {
			$("#rule_discount_txt").hide();
			$("#rule_discount_txt").text("");
		}
		$(".j_sales_display").show();
		$("#sale_rule_div").click(function () {
			window.location = "/Home/Search?sa=" + goods.data.RuleId;
		})

		// 显示价格标题
		$(".j_sales_title_price").hide();
		$(".j_sales_title_level_price").hide();
		$(".j_sales_title_sales_price").show();
	}
	else {
		var level = get_logon_level_index();
		if ((level != null) && (level > 0)) {
			var price = goods.data.Price - parseInt(goods.data.Price * goods.data.LevelDiscount[level - 1] / 10000);
			$("#sales_price").text((price / 100).toFixed(2));

			// 显示已优惠金额
			var org_price = goods.data.Price;
			if ((goods.data.MarketPrice != null) && (goods.data.MarketPrice > 0)) {
				org_price = goods.data.MarketPrice;
			}
			$("#rule_discount_txt").text(" 已优惠 " + ((org_price - price) / 100).toFixed(2) + "元");

			// 显示价格标题
			$(".j_sales_title_price").hide();
			$(".j_sales_title_sales_price").hide();
			$(".j_sales_title_level_price").show();
		}
		else {
			// 普通价格
			$(".j_sales_title_sales_price").hide();
			$(".j_sales_title_level_price").hide();
			$(".j_sales_title_price").show();
			$("#rule_discount_txt").text("");
			$("#rule_discount_txt").hide();
			$("#sales_price").text((goods.data.Price / 100).toFixed(2));
		}
	}

	$("#sales_price").show();
}

// 显示商品浏览历史
function show_goods_history(goods_history) {
	$("#goods_history_display").hide();
	$("#goods_history_display").empty();
	if (goods_history != null) {
		var id_array = [];
		for (var i = 0; i < goods_history.length; i++) {
			id_array.push(goods_history[i].id);
		}

		if ((id_array != null) && (id_array.length > 0)) {
			$.ajax({
				url: getActionPath("GetSimpleGoods", "Goods"),
				dataType: "json",
				contentType: 'application/json',
				type: "POST",
				data: stringify(id_array),
				success: function (data) {
					if (data.success == true) {

						var tmp_goods = [];
						// 去除掉不是有效状态的商品
						for (var i = 0; i < data.goods.length; i++) {
							if (data.goods[i].Status == Constant.OPEN) {
								tmp_goods.push(data.goods[i]);
							}
							else {
								// 把无效商品从浏览记录中删除
								goods.remove_goods_history(data.goods[i].GoodsId);
							}
						}
						data.goods = tmp_goods;

						$("#goods_history_display_tmpl").tmpl(data).appendTo("#goods_history_display");
						$("#goods_history_display").show();

						// 设置促销信息
						$("#goods_history_display").find(".goodsbox_list").each(function (index) {
							var the_goods = data.goods[index];
							// 显示促销信息
							var sales = get_goods_sales(the_goods);
							if (sales.rule_display) {
								// 商品有促销,显示促销价格
								if (sales.rule_icon != null) {
									sales.rule_icon.appendTo($(this).find("#sales_icon"));
								}
								//促销价
								if (sales.rule_price != null) {
									$(this).find("#sales_price").text((sales.rule_price / 100).toFixed(2));
								}
								$(this).find("#sales_text").text("促销价");
							}
							else {
								var level = get_logon_level_index();
								if ((level != null) && (level > 0)) {
									// 显示会员价
									var price = the_goods.Price - parseInt(the_goods.Price * the_goods.LevelDiscount[level - 1] / 10000);
									$(this).find("#sales_price").text((price / 100).toFixed(2));
									$(this).find("#sales_text").text("会员价");
								}
							}
						});
					}
					else {
						// 清除商品浏览历史
						goods.clear_goods_history();
					}
				},
				error: function (request, status, errorThrown) {
					ShowAjaxError(request, status, errorThrown);
				}
			});
		}
	}
}

//商品关联推荐商品
function show_goods_recommend() {
	if ((goods.data.RecommendGoods != null) && (goods.data.RecommendGoods.length > 0)) {
		var data = {
			goods: []
		};
		//判断推荐组开启、关闭状态
		if (goods.data.GoodsRecommendGroupStatus != null) {
			if (goods.data.GoodsRecommendGroupStatus == Constant.OPEN) {
				for (var i = 0; i < goods.data.RecommendGoods.length; i++) {
					data.goods.push(goods.data.RecommendGoods[i]);
				}
			}
		}
		$("#goods_recommend_display_tmpl").tmpl(data).appendTo("#goods_recommend_display");

		// 设置促销信息
		$("#goods_recommend_display").find(".goodsbox_list").each(function (index) {
			var the_goods = goods.data.RecommendGoods[index];
			// 显示促销信息
			var sales = get_goods_sales(the_goods);
			if (sales.rule_display) {
				// 商品有促销,显示促销价格
				if (sales.rule_icon != null) {
					sales.rule_icon.appendTo($(this).find("#sales_icon"));
				}
				//促销价
				if (sales.rule_price != null) {
					$(this).find("#sales_price").text((sales.rule_price / 100).toFixed(2));
				}
				$(this).find("#sales_text").text("促销价");
			}
			else {
				var level = get_logon_level_index();
				if ((level != null) && (level > 0)) {
					// 显示会员价
					var price = the_goods.Price - parseInt(the_goods.Price * the_goods.LevelDiscount[level - 1] / 10000);
					$(this).find("#sales_price").text((price / 100).toFixed(2));
					$(this).find("#sales_text").text("会员价");
				}
			}
		});
	} else {
		$("#goods_recommend_display").hide();
	}
}

// 显示组合购买商品
function show_pack_buy_goods(data) {
	$("#pack_buy_goods_display").empty();
	$("#pack_buy_goods_display_tmpl").tmpl(data).appendTo("#pack_buy_goods_display");
	// 商品点击事件
	$("#pack_buy_goods_display").find(".goodsbox_img, .goodsbox_name").click(function () {
		var goods_id = $(this).parents(".much-tag_goods_a").attr("goods_id");
		goto_goods_link(goods_id, true);
	});

	// 点击组合购买
	$("#pack_buy_goods_display").find("#add_pack_buy").click(function (e) {
		goods.add_pack_buy_to_cart(function (data) {
			if (data.success) {
				show_popup_cart(data);
				e.stopPropagation();
			}
			else {
				ShowActionError(data.error, "buyer");
			}
		});
	});
}

//显示入驻商户客服
function show_seller_sub_service(store_set_service) {
	var service = {
		service_qq: new Array(),
		service_company_qq: new Array(),
		service_marketing_qq: new Array(),
		service_wang: new Array()
	}
	if ((store_set_service.ServiceAccount != null) && (store_set_service.ServiceAccount.length > 0)) {
		$("#seller_sub_service_ctrl").addClass("StoreInfo_Service");
		$("#seller_sub_service_ctrl_text").text("店铺咨询");

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
		$("#seller_sub_goods_service_tmpl").tmpl(service).appendTo($("#seller_sub_service"));
		$("#seller_sub_service_ctrl").hover(function () {
			$("#seller_sub_service").show();
		}, function () {
			$("#seller_sub_service").hide();
		});
		// 加载营销QQ
		if ((service.service_marketing_qq != null) && (service.service_marketing_qq.length > 0)) {
			load_marketing_qq(function () {
				for (var i = 0; i < service.service_marketing_qq.length; i++) {
					BizQQWPA.addCustom([
					{ aty: '0', a: '0', nameAccount: service.service_marketing_qq[i].Account, selector: "yx_goods_" + service.service_marketing_qq[i].Account }
					]);
				}
			});
		}
	} else {
		$("#seller_sub_service_ctrl").addClass("StoreInfo_NoService");
		$("#seller_sub_service_ctrl_text").text("暂无客服").css('color', '#bbb');
		$("#seller_sub_service").hide();
		// 隐藏客服
		$("#seller_sub_online").hide();
	}
}
