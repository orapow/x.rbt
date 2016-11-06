var current_page = 0;

var click_price_from = null;
if (get_url_param("cp_f") != null) {
	click_price_from = get_url_param("cp_f") / 100;
}

var click_price_to = null;
if (get_url_param("cp_t") != null) {
	click_price_to = get_url_param("cp_t") / 100;
}
var page = get_url_param("page");
var show_tagclass = false;
var show_tagitem = false;

var search_more = [];//多个查询条件
var sel = null;
// 取得价格区间
function get_search_option() {
	var ret = $.extend({}, search_option);
	ret.PriceFrom = null;
	ret.PriceTo = null;
	ret.ClickPriceFrom = null;
	ret.ClickPriceTo = null;

	var from = $.trim($("input[name=price_from]").val());
	var to = $.trim($("input[name=price_to]").val());
	if (CheckInput.IsMoney(from)) {
		ret.PriceFrom = from * 100;
	}
	if (CheckInput.IsMoney(to)) {
		ret.PriceTo = to * 100;
	}
	else if (to == "") {
	}

	if ((ret.PriceFrom != null) && (ret.PriceTo != null)) {
		ret.ClickPriceFrom = null;
		ret.ClickPriceTo = null;
	} else {
		from = click_price_from;
		to = click_price_to;
		if (CheckInput.IsMoney(from)) {
			ret.ClickPriceFrom = from * 100;
		}
		if (CheckInput.IsMoney(to)) {
			ret.ClickPriceTo = to * 100;
		}
	}
	return ret;
}


var TemplateList = function (show) {
	this.show = show;
	search_option["page"] = page == null ? 0 : page;
	this.goods_type = [];
	this.options = search_option;
}

TemplateList.prototype = {
	show_comment_reset: function () {
		var This = this;
		This.options.Page = 0;
		// 设置搜索框的关键字
		$("input[name=search_text]").val(This.options.Key);
		This.SearchList();
	},
	ruleGoods: function (data) {
		$(".j_goodsbox_list").each(function (index) {
			var sales = get_goods_sales(data.infos_list.Goods[index]);
			if (sales.rule_display) {
				// 商品有促销,显示促销价格
				if (sales.rule_icon != null) {
					sales.rule_icon.appendTo($(this).find("#sales_icon"));
				}
				if (sales.rule_price != null) {
					$(this).find("#sales_price").text((sales.rule_price / 100).toFixed(2));
				}
			}
			else {

				var level = get_logon_level_index();
				if ((level != null) && (level > 0)) {
					// 显示会员价
					var price = data.infos_list.Goods[index].Price - parseInt(data.infos_list.Goods[index].Price * data.infos_list.Goods[index].LevelDiscount[level - 1] / 10000);
					$(this).find("#sales_price").text((price / 100).toFixed(2));
				}
			}
		});

	},

	infoSerachList: function () {
		for (var i = 0; i <= Constant.GOODS_SEARCH_SHOW_TAG_CLASS_MAX; i++) {
			for (var j = 0; j <= Constant.GOODS_SEARCH_SHOW_TAG_ITEM_DEFAULT; j++) {
				$("#search_item_attr_" + i.toString() + "_" + j.toString()).show();
			}
			if ($("#search_item_attr_" + i.toString() + "_" + j.toString()).length > 0) {
				$("#show_more_tagitem_" + i).show();
			}
			if (j > 11) {
				$("#show_comment_reset").find(".selectbox_more").show();
			}
		}
	},

	lazyload: function () {
		var This = this;
		$(".j_searchgoods_content_goodsbox img.lazy").lazyload({
			effect: "fadeIn",
			threshold: 20
		});
	},
	paging: function (data) {
		var This = this;
		This.show = $(This.show);
		//店铺头部 上一页 下一页 点击事件
		This.show.find(".j_searchgoods_content_pageturn li").click(function () {
			var page = parseInt($(this).attr("page"));
			if (page >= 0) {
				This.options.Page = page;
				This.SearchList();
			}
		});
		// 页码直接跳转
		This.show.find("#page_submit").click(function () {
			var page = This.show.find("#page_input").val();
			var pagetotal = parseInt($(this).attr("pagetotal"));
			This.goto_page(page, pagetotal);
		});
		//键盘enter事件
		This.show.find("#page_input").keyup(function (e) {
			if (e.keyCode == 13) {
				var page = This.show.find("#page_input").val();
				var pagetotal = parseInt($(this).attr("pagetotal"));
				This.goto_page(page, pagetotal);
			}
		});
		//店铺底部 分页事件
		This.show.find(".j_main_page li").click(function () {
			if (!$(this).hasClass("omit")) {
				This.options.Page = parseInt($(this).find('a').attr("page")) - 1;
				This.SearchList();
			}
		});
	},
	dispose_data: function (data) {
		//入驻商家客服数据
		var This = this;
		//分页数据
		var page_list_tpml = new Array();
		var page_tpml = 0;
		for (var i = data.infos_list.PageFrom; i <= data.infos_list.PageTo; i++) {
			if (i == data.infos_list.Page) {
				page_tpml = i + 1;
				page_list_tpml.push(i + 1);
			} else {
				page_list_tpml.push(i + 1);
			}
		}
		data.infos_list["PageTpml"] = page_tpml;
		data.infos_list["PageListTpml"] = page_list_tpml;
		return data;
	},

	SearchList: function (hd, hn, topmenu, t1) {

		var This = this;
		var F_TagItemId_List = "";
		if (This.options.F_TagItemId != null) {
			for (var i = 0; i < This.options.F_TagItemId.length; i++) {
				F_TagItemId_List += This.options.F_TagItemId[i] + ","
			}
		}
		var dailog = $("#show_navigation_search_tmpl");
		$.ajax({
			url: getActionPath("GetGoodsSearch", "Home"),
			dataType: "json",
			type: "POST",
			contentType: 'application/json',
			data: stringify({
				key: This.options.Key, c1: This.options.Class1Id, c2: This.options.Class2Id, c3: This.options.Class3Id, sa: This.options.RuleId, ti: This.options.TagItemId, o: This.options.Order, p_f: This.options.PriceFrom, p_t: This.options.PriceTo, cp_f: This.options.ClickPriceFrom, cp_t: This.options.ClickPriceTo,
				f_c1: This.options.F_Class1Id, f_c2: This.options.F_Class2Id, f_c3: This.options.F_Class3Id, f_tc: "", f_ti: F_TagItemId_List, sub: This.options.SellerSubId, page: This.options.Page,
			}),
			success: function (data) {
				CloseLoadingDialog();
				//清空模板
				$(This.show).empty();
				//如果返回正确的结果才会进行分页
				if (data.success) {
					This.collection_goods_list(data);
					data = This.dispose_data(data);
				}
				//else {
				//	data.infos_list = null;
				//}
				if (typeof This.options.F_TagItemId != "undefined") {
					$.extend(data, { "f_ti": This.options.F_TagItemId });
				} else {
					$.extend(data, { "f_ti": new Array() });
				}
				data["key"] = This.options.Key;
				data["f_c1"] = parseInt(This.options.F_Class1Id);
				data["f_c3"] = parseInt(This.options.F_Class3Id);
				search_more = [];
				if (data.f_ti.length > 0) {
					for (var i = 0; i < data.infos_list.GoodsTag.length; i++) {
						for (var j = 0; j < data.infos_list.GoodsTag[i].TagItem.length; j++) {
							for (var k = 0; k < data.f_ti.length; k++) {
								if (data.f_ti[k] == data.infos_list.GoodsTag[i].TagItem[j].TagItemId) {
									var search_tag = {
										tag_class_name: data.infos_list.GoodsTag[i].TagItem[j].TagItemName,
										tag_item_id: data.f_ti[k],
										tag_item_name: data.infos_list.GoodsTag[i].TagClassName
									};
									search_more.push(search_tag);
								}
							}
						}
					}
					data["f_more"] = search_more;
				} else {
					data["f_more"] = [];
				}
				//分页后滚动条保持在头部
				$(This.show).scrollTop(0);

				//追加数据至模板处
				dailog.tmpl(data).appendTo(This.show);

			
				if (data.success) {

					//回调函数（用点击排序时加样式）
					if ((typeof hd != "undefined") && (hd != null)) {
						hd();
					}
					//回调函数（用点击导航下的搜索）
					if ((typeof hn != "undefined") && (hn != null)) {
						hn();
					}
					if ((typeof topmenu != "undefined") && (topmenu != null)) {
						topmenu();
					}

					//初始化加载搜索条件
					This.infoSerachList();

					for (var i = 0; i < data.f_ti.length; i++) {
						$('.j_selectbox_listbg').find("#selectbox_list2").each(function () {
							$(this).children("li").each(function () {
								if ($(this).attr("tag_item_id") == data.f_ti[i]) {
									$(this).parent().parent().parent().hide();
								}
							});
						});
					}

					if ($('.j_selectbox_listbg').length > 5) {
						$(".j_show_more_tagclass").show();
					} else {
						$(".j_show_more_tagclass").hide();
					}
					$(".j_search_tag_class").click(function () {

						var tag_item_id = $(this).attr("tag_item_id");
						var is_exist = false;
						for (var i = 0; i < search_option.F_TagItemId.length; i++) {
							if (search_option.F_TagItemId[i] == tag_item_id) {
								is_exist = true;
								search_option.F_TagItemId.splice(i, 1);
							}
						}
						if (!is_exist) {
							search_option.F_TagItemId.push(f_ti);
							This.options.F_TagItemId = search_option.F_TagItemId;
						}

						search_goods(search_option);
					});

					$(".j_goodsbox_list").each(function () {
						var goods_id = $(this).attr("goods_id");
						var length = data.infos_list.GoodsDynamic.length;
						for (var i = 0; i < length; i++) {
							if (goods_id == data.infos_list.GoodsDynamic[i].GoodsId) {
								var prop_length = data.infos_list.GoodsDynamic[i].Property.length;
								var flag = 0;
								for (var k = 0; k < prop_length; k++) {
									if (data.infos_list.GoodsDynamic[i].Property[k].Number > 0) {
										flag++;
									}
								}
								if (flag == 0) {
									$(this).find(".j_no_number").show();
								}
							}
						}

					});

					This.paging(data);
					//商品是否促销
					This.ruleGoods(data);
					//2级菜单
					This.secend();
					//三级菜单
					This.threeMenu();
					//跟菜单
					This.firstMenu();
					//右侧搜索
					This.RightSerach();
					//展开更多
					This.moreList();
					//默认排序
					This.defaultOrder();
					//销量排序
					This.salesOrder();
					//价格排序
					This.priceOrder();
					//新品
					This.timeOrder();
					//评论数
					This.commentOrder();
					//清空最低最高价格
					This.clearPrice();
					//按价格查询
					This.priceok();
					This.priceList1();
					This.priceList2();
					This.priceList3();
					This.priceList4();
					This.priceList5();
					//ABCD后的展开按钮事件
					This.moresLists();
					//点击更多按钮
					This.ClickMore();
					//按搜索条件中的价格区间搜索
					//This.priceSection();
					//收藏
					This.collectionGoods();
					//取消收藏
					This.notcollectionGoods();
					//点击店铺
					This.ClickStore();
					//点击商品
					This.ClickGoods();
					This.styles();
					//最上方导航sousuo
					This.menuSerch();
					//品牌筛选
					This.tag_item_list();
					This.testlist();
					//放大镜回车事件
					This.EnterClick();
					// 延迟加载图片
					This.lazyload();
					if (This.options.PriceFrom != "" && This.options.PriceFrom != undefined) {
						$("#min_price").val(parseInt(This.options.PriceFrom) / 100);
					}
					if (This.options.PriceTo != "" && This.options.PriceTo != undefined) {
						$("#maxs_pirce").val(parseInt(This.options.PriceTo) / 100);
					}
					if (parseInt(get_url_param("sub")) >= 0) {
						$(".j_searchgoods_content_selectbg").hide();
					}
					$("#show_comment_reset").find("div[name=imgs]").hover(function () {


						if ($(this).attr("status") == "sc") {

						}
						else if ($(this).attr("status") == "ysc") {
							$(this).hide();
							$(this).parent().find(".j_goodsbox_collectno").show();
						}
					}, function () {
						if ($(this).attr("status") == "sc") {

						}
						else if ($(this).attr("status") == "qxsc") {
							if ($(this).attr("isqx") == "qx") {
								$(this).hide();
								$(this).parent().find(".j_goodsbox_collectyes").show();
							}
						}
					});


				} else {
					$(".j_goodsselect_store").click(function () {
						This.options.Page = 0;
						search_store.show_search_seller_sub();
					});
					$("#keyName").click(function () {
						This.options.Key = $("#keyNames").val();
						This.SearchList(function () {
							$("#keyNames").val(This.options.Key)
						});

					});
				}
			},
			error: function (request, status, errorThrown) {
				ShowAjaxError(request, status, errorThrown);
			}

		});
	},
	//点击商店或者商品，头部的搜索框改变样式
	styles: function () {
		$("#header_search_type_tab_good").addClass("patt_bg_header_checked");
		$("#header_search_type_tab_good").css("color", "#fff");
		$("#header_search_type_tab_shop").css("color", "#000");
		$("#header_search_type_tab_shop").removeClass("patt_bg_header_checked");
		$("#hid_type_value").val($("#header_search_type_tab_good").attr("search_type"));
	},
	//二级菜单
	secend: function () {
		var This = this;
		This.show.find(".searchgoods_nav_first").click(function () {
			This.options.F_Class2Id = $(this).attr("class1_id");
			//var mval = $(this).find('a').html();
			search_option = get_search_option();
			search_option.First = 0;
			search_option.F_Class1Id = This.options.F_Class2Id;
			search_option.F_Class2Id = null;
			search_option.F_Class3Id = null;
			search_option.F_TagClassId = null;
			//search_option.F_TagItemId = null;

			//This.SearchList(function () {
			//	$('.j_main_pathbox_arrow').show();
			//	$(".j_main_pathbox_itembox1").show();
			//	$(".j_main_pathbox_itembox1").find("a").html(mval);;
			//});
			This.options = search_option;
			This.options.o = search_option.Order;

			search_goods(search_option);
		});
	},
	//三级菜单
	threeMenu: function () {
		var This = this;
		This.show.find(".searchgoods_nav_second li").click(function () {
			This.options.F_Class3Id = $(this).attr("class3_id");
			//var aval = $(this).find('a').html();
			//var fatherid = $(this).attr("father_id");
			//var fid = $(this).parent().parent().find('.searchgoods_nav_first').attr('class1_id');
			//var mval;
			//if (fatherid == fid) {
			//	mval = $(this).parent().parent().find('.searchgoods_nav_first').find('a').html();
			//}
			search_option = get_search_option();
			search_option.First = 0;
			search_option.F_Class1Id = null;
			search_option.F_Class2Id = null;
			search_option.F_Class3Id = This.options.F_Class3Id;
			search_option.F_TagClassId = null;
			//search_option.F_TagItemId = null;
			//This.SearchList(function () {
			//	$('.main_pathbox_arrow').show();
			//	$('.m_main_pathbox_itembox').show();
			//	$('.m_main_pathbox_itembox').find('a').html(aval);
			//	$(".j_main_pathbox_itembox1").show();
			//	$(".j_main_pathbox_itembox1").find("a").html(mval);;
			//});
			//$('.m_main_pathbox_itembox').find('a').html(aval);
			//$(".j_main_pathbox_itembox1").find("a").html(mval);

			This.options = search_option;
			This.options.o = search_option.Order;

			search_goods(search_option);
		});
	},

	//跟菜单
	firstMenu: function () {

		var This = this;
		This.show.find(".searchgoods_nav_title").click(function () {
			search_option = get_search_option();
			search_option.First = 0;
			search_option.F_Class1Id = null;
			search_option.F_Class2Id = null;
			search_option.F_Class3Id = null;
			search_option.F_TagClassId = null;
			//search_option.F_TagItemId = null;

			This.options = search_option;
			This.options.o = search_option.Order;
			search_goods(search_option);
		});
	},

	//右边点击搜索
	RightSerach: function () {
		var This = this;
		This.show.find(".tag_item_search").click(function () {
			var f_ti = $(this).attr("tag_item_id");
			var dd = $(this).attr("dd");
			sel = $(this).attr("index");
			var tag_item_name = $(this).text();
			var tag_class_name = $(this).attr("tag_class_name");
			search_option = get_search_option();
			//search_option.First = 0;
			//search_option.F_Class1Id = null;
			//search_option.F_Class2Id = null;
			//search_option.F_Class3Id = null;
			//search_option.F_TagClassId = null;

			This.options = search_option;
			This.options.o = search_option.Order;

			if (search_option.F_TagItemId == null) {
				search_option.F_TagItemId = [];
			}
			if (This.options.F_TagItemId == null) {
				This.options.F_TagItemId = [];
			}

			var is_exist = false;
			for (var i = 0; i < search_option.F_TagItemId.length; i++) {
				if (search_option.F_TagItemId[i] == f_ti) {
					is_exist = true;
					search_option.F_TagItemId.splice(i, 1);
				}
			}
			if (!is_exist) {
				search_option.F_TagItemId.push(f_ti);
				This.options.F_TagItemId = search_option.F_TagItemId;
			}

			//var search_tag = {
			//	tag_class_name: tag_class_name,
			//	tag_item_id: f_ti,
			//	tag_item_name: tag_item_name
			//};
			//search_more.push(search_tag);
			//This.options.search_more = search_more;
			//This.SearchList(null, null, null, sel);
			search_goods(search_option);
		});
	},

	//展开更多
	moreList: function () {
		var This = this;
		This.show.find("#show_more_tagclass").click(function () {

			var mainserach = $("#show_comment_reset").find(".searchgoods_content_selectbox_listbg")
			if (mainserach.is(":hidden")) {
				mainserach.show();
				$(this).find("span").html("收起标签");
				$(this).find("div").removeClass();
				$(this).find("div").addClass("selectbox_labelicon arrow_icon02");;
			} else {
				mainserach.hide();
				$(this).find("span").html("展开标签");
				$(this).find("div").removeClass();
				$(this).find("div").addClass("selectbox_labelicon arrow_icon01");;
				$("#show_comment_reset").find(".searchgoods_content").find("*[types='yes']").show();

			}
		});
	},

	//默认排序
	defaultOrder: function () {
		var This = this;
		This.show.find("#order_default").click(function () {
			current_page = 0;
			search_option.Order = Constant.GOODS_SEARCH_ORDER_DEFAULT;
			search_option = get_search_option();
			This.options = search_option;
			This.options.o = search_option.Order;
			//This.SearchList(function () {
			//	$("#show_comment_reset").find(".searchgoods_content_sortbox li").removeClass("checked");  //第一步 先移除li所有的class=“checked”属性
			//	$("#order_default").addClass("checked");
			//});
			search_goods(search_option);
		});
	},
	//销量
	salesOrder: function () {
		var This = this;
		This.show.find("#order_sales").click(function () {
			current_page = 0;
			search_option.Order = Constant.GOODS_SEARCH_ORDER_SALES_HIGH_LOW;
			search_option = get_search_option();
			This.options = search_option;
			This.options.o = search_option.Order;
			//This.SearchList(function () {
			//	$("#show_comment_reset").find(".searchgoods_content_sortbox li").removeClass("checked");  //第一步 先移除li所有的class=“checked”属性
			//	$("#order_sales").addClass("checked");
			//});
			//This.SearchList();
			search_goods(search_option);
		});
	},
	//价格
	priceOrder: function () {
		var This = this;
		//价格
		This.show.find("#order_price_up").click(function () {

			var high = $(this).attr("types");
			if (high == 'high') {
				current_page = 0;
				search_option.Order = Constant.GOODS_SEARCH_ORDER_PRICE_LOW_HIGH;
				search_option = get_search_option();
				This.options.o = search_option.Order;
				//This.SearchList(function () {
				//	$("#show_comment_reset").find(".searchgoods_content_sortbox li").removeClass("checked");  //第一步 先移除li所有的class=“checked”属性
				//	$("#order_price_up").addClass("checked");
				//	$("#order_price_arrow").removeClass("arrow_icon04");
				//	$("#order_price_arrow").removeClass("arrow_icon04_down");
				//	$("#order_price_arrow").addClass("arrow_icon04_up");
				//	$("#order_price_up").removeAttr('types');
				//})
			} else {
				current_page = 0;
				search_option.Order = Constant.GOODS_SEARCH_ORDER_PRICE_HIGH_LOW;
				search_option = get_search_option();
				This.options.o = search_option.Order;
				//This.SearchList(function () {
				//	$("#show_comment_reset").find(".searchgoods_content_sortbox li").removeClass("checked");  //第一步 先移除li所有的class=“checked”属性
				//	$("#order_price_up").addClass("checked");
				//	$("#order_price_arrow").removeClass("arrow_icon04");
				//	$("#order_price_arrow").removeClass("arrow_icon04_up");
				//	$("#order_price_arrow").addClass("arrow_icon04_down");
				//});
			}
			search_goods(search_option);
		});
	},
	//新品排序
	timeOrder: function () {
		var This = this;
		This.show.find("#order_time").click(function () {
			current_page = 0;
			search_option.Order = Constant.GOODS_SEARCH_ORDER_TIME_NEW_OLD;
			search_option = get_search_option();
			This.options = search_option;
			This.options.o = search_option.Order;
			//This.SearchList(function () {
			//	$("#show_comment_reset").find(".searchgoods_content_sortbox li").removeClass("checked");  //第一步 先移除li所有的class=“checked”属性
			//	$("#order_time").addClass("checked");
			//});
			search_goods(search_option);
		});
	},
	//评论数
	commentOrder: function () {
		var This = this;
		This.show.find("#order_commment_up").click(function () {
			current_page = 0;
			search_option.Order = Constant.GOODS_SEARCH_ORDER_COMMENT_LOW;
			search_option = get_search_option();
			This.options = search_option;
			This.options.o = search_option.Order;
			//search_goods(search_option);
			//This.SearchList(function () {
			//	$("#show_comment_reset").find(".searchgoods_content_sortbox li").removeClass("checked");  //第一步 先移除li所有的class=“checked”属性
			//	$("#order_commment_up").addClass("checked");
			//});
			search_goods(search_option);
		});
	},
	//清空价格
	clearPrice: function () {
		$("#price_no").click(function () {
			$("#min_price").val("");
			$("#maxs_pirce").val("");
		});
	},
	//更多
	moresLists: function () {
		var This = this;
		This.show.find("#opens").click(function () {
			var open = $("#show_comment_reset").find("#selectbox_list1");
			if (open.is(":hidden")) {
				open.show();
				$("#opemImg").removeClass().addClass("selectbox_moreicon arrow_icon02");
				$("#opens").html("收起");
			}
			else {
				open.hide();
				$("#opemImg").removeClass().addClass("selectbox_moreicon arrow_icon01");
				$("#opens").html("更多");
			}
		});
	},
	//点击更多按钮
	ClickMore: function () {
		var This = this;
		This.show.find(".selectbox_more").click(function () {
			var tag_class_index = $(this).attr("tag_class_index");
			if (show_tagitem) {
				show_tagitem = false;

				$(this).find('span').text("更多");
				$(this).find('div').removeClass("arrow_icon02");
				$(this).find('div').addClass("arrow_icon01");
				$(".j_tag_class_" + tag_class_index).hide();
				$(".j_selectbox_brandbox_" + tag_class_index).attr("style", "padding:0px 0px;")
				for (var j = Constant.GOODS_SEARCH_SHOW_TAG_ITEM_DEFAULT + 1 ; j < Constant.GOODS_SEARCH_SHOW_TAG_ITEM_MAX; j++) {
					$("#search_item_attr_" + tag_class_index + "_" + j.toString()).hide();
					$("#search_item_attr_" + tag_class_index + "_" + j.toString()).parent().hide();
				}
				for (var j = 0; j < Constant.GOODS_SEARCH_SHOW_TAG_ITEM_DEFAULT + 1; j++) {
					$("#search_item_attr_" + tag_class_index + "_" + j.toString()).show();
					$("#search_item_attr_" + tag_class_index + "_" + j.toString()).parent().show();
				}
			} else {
				show_tagitem = true;
				$(this).find('span').text("收起");
				$(this).find('div').removeClass("arrow_icon01");
				$(this).find('div').addClass("arrow_icon02");
				for (var j = Constant.GOODS_SEARCH_SHOW_TAG_ITEM_DEFAULT + 1 ; j < Constant.GOODS_SEARCH_SHOW_TAG_ITEM_MAX; j++) {
					$("#search_item_attr_" + tag_class_index + "_" + j.toString()).show();
					$("#search_item_attr_" + tag_class_index + "_" + j.toString()).parent().show();
				}
				if ($("#selectbox_list2 li").length > 20) {
					$(".j_tag_class_" + tag_class_index).show();
					$(".j_selectbox_brandbox_" + tag_class_index).attr("style", "border-bottom:1px dotted #bebebe;padding:5 0;")
				}
			}
		});
	},
	//收藏
	collectionGoods: function () {
		var This = this;
		This.show.find(".j_readycollection").click(function () {
			var goodsid = new Array();
			goodsid[0] = $(this).attr("goods_id");
			if (goodsid.length > 0) {
				if (collection_goods(goodsid)) {
					$(this).hide();
					$(this).parent().find('.j_goodsbox_collectyes').show();

					$(this).parent().find('.j_goodsbox_collectno').attr("isqx", "qx");
				};
			}

		});
	},
	//取消shoucang
	notcollectionGoods: function () {

		var This = this;
		This.show.find(".j_goodsbox_collectno").click(function () {
			var goodsid = new Array();
			goodsid[0] = $(this).attr("goods_id");
			if (goodsid.length > 0) {
				if (cancel_collection_goods(goodsid)) {
					$(this).hide();
					$(this).parent().find('.j_goodsbox_collectyes').hide();
					$(this).parent().find('.j_readycollection').show();
					$(this).attr("isqx", "yqx");

				}
			}
		});
	},
	//点击店铺
	ClickStore: function () {
		var This = this;
		This.show.find(".j_goodsselect_store").click(function () {
			This.options.Page = 0;
			search_store.show_search_seller_sub();
		});
	},
	//点击商品
	ClickGoods: function () {
		var This = this;
		This.show.find(".j_goodsselect_goods").click(function () {
			reset.show_comment_reset();
		});
	},
	//最上方导航里的搜索
	menuSerch: function () {
		var This = this;
		This.show.find("#keyName").click(function () {
			This.options.Key = $("#keyNames").val();
			This.SearchList(function () {
				$("#keyNames").val(This.options.Key)
			});

		});
	},
	//搜索条件中的区间价格查询
	//priceSection: function () {
	//	var This = this;
	//	This.show.find(".j_search_price_range_click").click(function () {

	//		var priceA = parseInt($(this).attr('price_f')) * 100;
	//		var priceB = parseInt($(this).attr('price_t')) * 100;
	//		This.options.ClickPriceFrom = priceA;
	//		This.options.ClickPriceTo = priceB;
	//		This.SearchList();

	//	});
	//	This.show.find(".j_search_border").click(function () {
	//		This.options.ClickPriceFrom = null;
	//		This.options.ClickPriceTo = null;
	//		This.SearchList();
	//	});
	//},
	//小价格查询例如(1--100)
	priceok: function () {
		var This = this;
		This.show.find("#price_ok").click(function () {
			var priceA, priceB;
			if (($("#min_price").val() == "") || ($("#min_price") == undefined)) {
				priceA = "";
			} else {
				priceA = parseInt($("#min_price").val()) * 100;
			}

			if (($("#maxs_pirce").val() == "") || ($("#maxs_pirce") == undefined)) {
				priceB = "";
			} else {
				priceB = parseInt($("#maxs_pirce").val()) * 100;
			}
			//var priceB = parseInt($("#maxs_pirce").val()) * 100;
			This.options.PriceFrom = priceA;
			This.options.PriceTo = priceB;
			This.SearchList();
		})
	},
	priceList1: function () {
		var This = this;
		This.show.find("#price1").click(function () {
			This.options.PriceFrom = 0;
			This.options.PriceTo = 99 * 100;
			This.SearchList();
		});
	},
	priceList2: function () {
		var This = this;
		This.show.find("#price2").click(function () {
			This.options.PriceFrom = 100 * 100;
			This.options.PriceTo = 199 * 100;
			This.SearchList();
		});
	},
	priceList3: function () {
		var This = this;
		This.show.find("#price3").click(function () {
			//$("#min_price").val("200");
			//$("#maxs_pirce").val("399");
			This.options.PriceFrom = 200 * 100;
			This.options.PriceTo = 399 * 100;
			This.SearchList();
		});
	},
	priceList4: function () {
		var This = this;
		This.show.find("#price4").click(function () {
			//$("#min_price").val("400");
			//$("#maxs_pirce").val("599");
			This.options.PriceFrom = 400 * 100;
			This.options.PriceTo = 599 * 100;
			This.SearchList();
		});
	},
	priceList5: function () {
		var This = this;
		This.show.find("#price5").click(function () {
			//$("#min_price").val("1000");
			//$("#maxs_pirce").val("");
			This.options.PriceFrom = 1000 * 100;
			This.options.PriceTo = "";
			This.SearchList();
		});
	},
	//跳转
	goto_page: function (page, pagetotal) {
		var This = this;
		if (!CheckInput.IsEmpty(page)) {
			if (CheckInput.IsNumber(page)) {
				page = parseInt(page);
				if (page > pagetotal) {
					page = pagetotal;
				}
				if (page < 1) {
					page = 1;
				}
				This.options.Page = (page - 1);
				This.SearchList();
			} else {
				alert("请输入数字");
			}
		}
	},
	//品牌筛选
	tag_item_list: function () {
		var This = this;
		//根据关键字筛选
		This.show.find('.screen').bind("input propertychange", function (event) {
			var content = $.trim(event.target.value);
			This.screen($(this), content);
		});
		//根据字母筛选品牌
		This.show.find('.j_selectbox_brand').find('a').bind("click", function (event) {
			$(this).parent().find('.checked').removeClass("checked");
			$(this).addClass("checked");
			var content = $.trim($(this).attr("tagitemname"));
			This.screen($(this), content);
		})
		//点击筛选按钮根据关键字筛选
		This.show.find(".j_search_button").bind("click", function () {
			var content = $.trim($(this).parent().find('.screen').val());
			var $this = $(this).parent();
			This.screen($($this), content);
		});
	},
	//筛选
	screen: function ($this, content) {
		var This = this;
		content = $.fn.toPinyin(content).toLowerCase();

		var content_arr = new Array();
		if (content == "other") {
			content_arr = null;
		} else {
			for (var i = 0; i < content.length; i++) {
				var ch = content.charAt(i);
				content_arr.push(ch);
			}
		}

		$this.parents('.j_selectbox_listbox').find('#selectbox_list2').children("li").each(function () {
			var indexofVluer = ($.fn.toPinyin($.trim($(this).attr("TagItemName"))).toLowerCase()).replace(/\s+/g, "");
			if (content == "other") {
				if (indexofVluer.match(/\d+/g)) {
					$(this).show();
				} else {
					$(this).hide();
				}
			} else {
				if ((indexofVluer.indexOf(content) * 1) >= 0) {
					$(this).show();
				} else {
					var str_exist = new Array();
					for (var i = 0; i < content_arr.length; i++) {
						if ((indexofVluer.indexOf(content_arr[i]) * 1) >= 0) {
							str_exist.push("true")
						}
					}
					if (str_exist.length == content_arr.length) {
						$(this).show();
					} else {
						$(this).hide();
					}
				}
			}
		});
	},
	// 自营/会员等条件筛选
	testlist: function () {
		var This = this;
		$(".j_searchgoods_content_selectlist li").click(function () {
			var type = $(this).attr("types");
			var s = $(this).find('i').attr('checked');
			if (s == 'checked') {
				$(this).find('i').removeAttr('checked');
			} else {
				$(this).find('i').attr("checked", true);
			}

			This.goods_type = [];
			for (var i = 0; i < $(".j_searchgoods_content_selectlist li").length; i++) {
				if ($(".j_searchgoods_content_selectlist li:eq(" + i + ") a i").attr("checked") == "checked") {
					This.goods_type.push($(".j_searchgoods_content_selectlist li:eq(" + i + ")").attr("types"));
				}
			}
			This.showGoodsList();
		});

	},
	showGoodsList: function () {
		var This = this;
		$(".j_searchgoods_content_goodsbox").find(".j_goodsbox_list").hide();

		$(".j_searchgoods_content_goodsbox").find(".j_goodsbox_list").each(function () {
			var myid = $(this).attr('myid');
			var ruleid = $(this).attr('ruleid');
			var discountprice = $(this).attr('discountprice');
			if (This.goods_type.length > 0) {
				if (This.goods_type.indexOf('0') > -1 && myid == 0) {
					$(this).show();

				}
				if (This.goods_type.indexOf('1') > -1 && ruleid > 0) {
					$(this).show();

				}
				if (This.goods_type.indexOf('2') > -1 && discountprice > 0) {
					$(this).show();

				}

			}
			else {
				$(this).show();

			}
		});
	},
	//放大镜回车事件
	EnterClick: function () {
		var This = this;
		This.show.find("#keyNames").keydown(function (e) {
			if (e.keyCode == 13) {
				This.options.Key = $("#keyNames").val();
				This.SearchList(function () {
					$("#keyNames").val(This.options.Key)
				});
			}
		});
	},
	//批量判断当前商品是否已收藏返回收藏商品ID
	collection_goods_list: function (goods_data) {
		var This = this;
		var goodsid = new Array();
		for (var i = 0; i < goods_data.infos_list.Goods.length; i++) {
			goods_data.infos_list.Goods[i]["GoodsIsExist"] = false;
			goodsid.push(goods_data.infos_list.Goods[i].GoodsId);
		}
		if ((goodsid != null) && (goodsid.length > 0)) {
			$.ajax({
				url: getActionPath("CollecttionExistGoodsId", "Cart"),
				dataType: "json",
				async: false,
				contentType: 'application/json',
				type: "POST",
				data: JSON.stringify({ goods_id: JSON.stringify(goodsid) }),
				success: function (data) {
					if (data.success == true) {
						for (var i = 0; i < goods_data.infos_list.Goods.length; i++) {
							for (var j = 0; j <= data.goods_list.length; j++) {
								if (goods_data.infos_list.Goods[i].GoodsId == data.goods_list[j]) {
									goods_data.infos_list.Goods[i]["GoodsIsExist"] = true;
								}
							}
						}
					}
				},
				error: function (request, status, errorThrown) {
					ShowAjaxError(request, status, errorThrown);
				}
			});
		}
		return goods_data;
	}
}
//生成规则信息
function get_seach_sales_rule_displayinfo(rule) {
	var ret = "";
	switch (rule.RuleType) {
		case Constant.SALESRULE_REDUCE:				// 直降
			ret = "直降" + (rule.DiscountPrice / 100).toString() + "元";
			break;
		case Constant.SALESRULE_TIME_TO_REDUCE:				// 限时优惠
			ret = "限时优惠" + (rule.DiscountPrice / 100).toString() + "元";
			break;
		case Constant.SALESRULE_FULL_REDUCE:		// 满减
			ret = "满" + (rule.TargetPrice / 100).toString() + "元，减" + (rule.DiscountPrice / 100).toString() + "元";
			break;
		case Constant.SALESRULE_GROUP_BUY:			// 团购
			ret = "团减" + (rule.DiscountPrice / 100).toString() + "元";
			break;
		case Constant.SALESRULE_PACK_BUY:			// 组合购买
			ret = "共减" + (rule.DiscountPrice / 100).toString() + "元";
			break;
		case Constant.SALESRULE_SECKILLING:			// 秒杀
			ret = "秒杀直减" + (rule.DiscountPrice / 100).toString() + "元";
			break;
		case Constant.SALESRULE_DEFINE:				// 自定义
			ret = "自定义";
			break;
		case Constant.SALESRULE_DISCOUNT:			// 折扣
			ret = (rule.DiscountPrice / 10).toString() + "折";
			break;
		case Constant.SALESRULE_FULL_CUT:			// 数量满减（批发）
			switch (rule.ChangeType) {
				case Constant.FULL_CUT_TYPE_GOODS_CUT:
					ret = "购买数量到达 " + rule.ChangeNumber + "件时，每件商品减" + (rule.DiscountPrice / 100).toString() + "元";;
					break;

				case Constant.FULL_CUT_TYPE_GOODS_DISCOUNT:
					ret = "购买数量到达 " + rule.ChangeNumber + "件时，每件商品打" + (rule.DiscountPrice / 10).toString() + "折";;
					break;
			}
			break;
		default:
			break;
	}
	return ret;
}
