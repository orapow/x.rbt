var SearchStore = function (show) {
	this.Show = $(show);
	search_option["Page"] = page == null ? 0 : page;
	this.search_option = search_option;
	this.goods_class = null;
}
SearchStore.prototype = {
	// 显示商品
	show_search_seller_sub: function () {
		var This = this;
		//加载商品信息
		This.SearchList(function (data) {
			//当商品信息返回结果后查询店铺信息
			if (data.success) {
				This.goods_class = data.infos_list.GoodsClass;//获取商品分类信息
			}
			//加载店铺信息
			This.get_search_store();
		});

	},
	ruleGoods: function (data, sub_id) {
		$(".j_goodsbox_list[myid=" + sub_id + "]").each(function (index) {
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
	//获取商品信息
	SearchList: function (hd, sub_id) {
		var sub = "";
		if ((sub_id != "") && (sub_id != null)) {
			sub = sub_id;
		}
		var This = this;
		$.ajax({
			url: getActionPath("GetGoodsSearch", "Home"),
			dataType: "json",
			type: "POST",
			contentType: 'application/json',
			data: stringify({
				key: This.search_option.Key, c1: This.search_option.Class1Id, c2: This.search_option.Class2Id, c3: This.search_option.Class3Id, sa: This.search_option.RuleId, ti: This.search_option.F_TagItemId, o: This.search_option.o, p_f: This.search_option.p_f, p_t: This.search_option.PriceTo, cp_f: This.search_option.ClickPriceFrom, cp_t: This.search_option.ClickPriceTo,
				f_c1: This.search_option.f_c1, f_c2: This.search_option.F_Class2Id, f_c3: This.search_option.f_c3, f_tc: "", f_ti: This.search_option.f_ti, sub: sub, page: This.search_option.Page,
			}),
			success: function (data) {
				CloseLoadingDialog();
				if (typeof hd) {
					hd(data);
				}
				//延时加载图片
				This.lazyload();
			},
			error: function (request, status, errorThrown) {
				ShowAjaxError(request, status, errorThrown);
			}
		});
	},
	//获取商家信息
	get_search_store: function () {
		var This = this;
		OpenLoadingDialog();
		$.ajax({
			url: getActionPath("GetStoreSearch", "Home"),
			dataType: "json",
			contentType: 'application/json',
			type: "POST",
			data: stringify({ key: This.search_option.Key, c1: This.search_option.Class1Id, page: This.search_option.Page }),
			success: function (data) {
				CloseLoadingDialog();
				if (data.success == true) {
					This.Show.empty();
					This.collection_store_list(data);
					var json = This.dispose_data(data);
					json.data["GoodsClass"] = This.goods_class
					//填充模版
					$("#search_store_tmpl").tmpl(json.data, {
						substring: function (str) {
							if (str.length > 45) {
								str = str.substring(0, 45) + '...';
							}
							return str;
						}, get_search_list: function (sub_id, sub_name) {
							This.SearchList(function (search_data) {
								if (search_data.success) {
									search_data["sub_name"] = sub_name;
									This.collection_goods_list(search_data);

									$(".j_searchstore_goodsbox").empty();
									$("#search_tmpl").tmpl(search_data).appendTo($(".j_searchstore_goodsbox"));
									//商品是否促销
									This.ruleGoods(search_data, sub_id);
									
									//收藏商品与取消收藏事件
									This.collection_goods();
								} else {
									$(".j_searchstore_goodsbox").hide();
								}
							}, sub_id)
							return "";
						}
					}).appendTo(This.Show);
					This.show_sub_store_custom_service(json);
					//店铺小搜索框处理
					This.min_seach_input();
					//延时加载图片
					This.lazyload();
					//店铺分页处理
					This.paging(data);
					//店铺收藏与取消收藏事件
					This.collection_store();
					//商品展示
					This.show_goods();
					//店铺与商品切换
					This.goodsselect();
					//左侧菜单点击事件
					This.firstMenu();

				}
				else {
					ShowActionError(data.error, "");
				}
			},
			error: function (request, status, errorThrown) {
				ShowAjaxError(request, status, errorThrown);
			}
		});
	},
	//获取店铺商品信息

	//延时加载图片
	lazyload: function () {
		var This = this;
		$("img.lazy").lazyload({
			effect: "fadeIn",
			threshold: 20
		});
	},
	//店铺小搜索框处理
	min_seach_input: function () {
		var This = this;
		//头部大搜索框初始化值
		$("input[name=search_text]").val(This.search_option.Key);
		//店铺小搜索框初始化值
		This.Show.find(".j_main_pathbox_searchbox input").val(This.search_option.Key);
		//店铺头部小搜索框
		This.Show.find(".j_main_pathbox_searchbox_iconbox").click(function () {
			This.search_option.Key = This.Show.find(".j_main_pathbox_searchbox input").val();
			if (!CheckInput.IsEmpty(This.search_option.Key)) {
				This.search_option.Page = 0;
				This.get_search_store();
			} else {
				alert("请输入需要搜索的店铺关键字");
			}
		});
		This.Show.find(".j_main_pathbox_searchbox input").keyup(function (e) {
			if (e.keyCode == 13) {
				This.search_option.Key = $(this).val();
				if (!CheckInput.IsEmpty(This.search_option.Key)) {
					This.search_option.Page = 0;
					This.get_search_store();
				} else {
					alert("请输入需要搜索的店铺关键字")
				}
			}
		});
	},
	//店铺分页处理
	paging: function (data) {
		var This = this;
		//店铺头部 上一页 下一页 点击事件
		This.Show.find(".j_searchgoods_content_pageturn li").click(function () {
			var page = parseInt($(this).attr("page"));
			if (page >= 0) {
				This.search_option.Page = page;
				if (!CheckInput.IsEmpty(This.search_option.Page)) {
					This.get_search_store();
				}
			}
		});
		// 页码直接跳转
		This.Show.find("#page_submit").click(function () {
			var page = This.Show.find("#page_input").val();
			var pagetotal = parseInt($(this).attr("pagetotal"));
			This.goto_page(page, pagetotal);
		});
		//键盘enter事件
		This.Show.find("#page_input").keyup(function (e) {
			if (e.keyCode == 13) {
				var page = This.Show.find("#page_input").val();
				var pagetotal = parseInt($(this).attr("pagetotal"));
				This.goto_page(page, pagetotal);
			}
		});
		//店铺底部 分页事件
		This.Show.find(".j_main_page li").click(function () {
			if (!$(this).hasClass("omit")) {
				This.search_option.Page = parseInt($(this).find('a').attr("page"));
				if (!CheckInput.IsEmpty(This.search_option.Page)) {
					This.get_search_store();
				}
			}
		});
	},
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
				This.search_option.Page = (page - 1);
				This.get_search_store();
			} else {
				alert("请输入数字");
			}
		}
	},
	//处理入驻商家客服数据与分页数据处理
	dispose_data: function (data) {
		var This = this;
		//入驻商家客服数据
		var marketing_qq = [];
		for (var i = 0; i < data.search_store_model.SellerSub.length; i++) {
			//获取第一个店铺商品信息
			data.search_store_model.SellerSub[i]["service"] = {
				service_qq: new Array(),
				service_company_qq: new Array(),
				service_marketing_qq: new Array(),
				service_wang: new Array()
			};
			if (!CheckInput.IsEmpty(data.search_store_model.SellerSub[i].StoreSetService.ServiceAccount)) {
					for (var j = 0; j < data.search_store_model.SellerSub[i].StoreSetService.ServiceAccount.length; j++) {
						switch (data.search_store_model.SellerSub[i].StoreSetService.ServiceAccount[j].Type) {
							case Constant.STORE_SERVICE_QQ:
								data.search_store_model.SellerSub[i].service.service_qq.push(data.search_store_model.SellerSub[i].StoreSetService.ServiceAccount[j]);
								break;
							case Constant.STORE_SERVICE_COMPANY_QQ:
								data.search_store_model.SellerSub[i].service.service_company_qq.push(data.search_store_model.SellerSub[i].StoreSetService.ServiceAccount[j]);
								break;
							case Constant.STORE_SERVICE_MARKETING_QQ:
								data.search_store_model.SellerSub[i].service.service_marketing_qq.push(data.search_store_model.SellerSub[i].StoreSetService.ServiceAccount[j]);
								marketing_qq.push(data.search_store_model.SellerSub[i].StoreSetService.ServiceAccount[j]);//获取所有营销QQ数据
								break;
							case Constant.STORE_SERVICE_WANGWANG:
								data.search_store_model.SellerSub[i].service.service_wang.push(data.search_store_model.SellerSub[i].StoreSetService.ServiceAccount[j]);
								break;
							default:
								break;
						}
					}
			}
		}
		//分页数据
		var page_list_tpml = new Array();
		var page_tpml = 0;
		for (var i = data.search_store_model.PageFrom; i <= data.search_store_model.PageTo; i++) {
			if (i == data.search_store_model.Page) {
				page_tpml = i + 1;
				page_list_tpml.push(i + 1);
			} else {
				page_list_tpml.push(i + 1);
			}
		}
		data.search_store_model["PageTpml"] = page_tpml;
		data.search_store_model["PageListTpml"] = page_list_tpml;
		return { data: data, marketing_qq: marketing_qq };
	},
	//商家客服信息营销QQ特殊处理
	show_sub_store_custom_service: function (json) {
		var This = this;
		This.Show.find(".j_storeinfo_nameicon").hover(function () {
			$(this).find('.j_StoreService_Popup').show();
		}, function () {
			$(this).find('.j_StoreService_Popup').hide();
		})
		// 加载营销QQ
		var marketing_qq = [];
		if ((json.marketing_qq != null) && (json.marketing_qq.length > 0)) {
			for (var i = 0; i < json.marketing_qq.length; i++) {
				marketing_qq.push({ aty: '0', a: '0', nameAccount: json.marketing_qq[i].Account, selector: "yx_" + json.marketing_qq[i].Account });
			}
		}
		if (marketing_qq.length > 0) {
			load_marketing_qq(function () {
				BizQQWPA.addCustom(marketing_qq);
			});
		}
	},
	//收藏店铺 && 取消收藏店铺
	collection_store: function () {
		var This = this;
		This.Show.find(".j_collection").click(function () {
			var $parent_div = $(this).parent();
			var sub_id = parseInt($(this).attr("sub_id"));
			if (CheckInput.IsEmpty(sub_id)) {
				return;
			}
			if (collection_store != undefined) {
				if (collection_store(sub_id)) {
					$(this).hide();
					$parent_div.find(".j_collection_collect").show();
				};
			}
		})
		This.Show.find(".j_collection_collect").click(function () {
			var $parent_div = $(this).parent();
			var sub_id = parseInt($(this).attr("sub_id"));
			if (CheckInput.IsEmpty(sub_id)) {
				return;
			}
			if (cancel_collection_store != undefined) {
				if (cancel_collection_store(sub_id)) {
					$(this).hide();
					$parent_div.find(".j_collection").show();
				};
			}
		})
	},
	//收藏商品 && 取消商品
	collection_goods: function () {
		var This = this;
		//收藏
		This.Show.find(".j_readycollection").click(function () {
			var $parent_div = $(this).parent();
			var goods_id = parseInt($(this).attr("goods_id"));
			if (CheckInput.IsEmpty(goods_id)) {
				return;
			}
			if (collection_goods != undefined) {
				var goodsid = new Array();
				goodsid[0] = goods_id;
				if (collection_goods(goodsid)) {
					$(this).hide();
					$(this).parent().find('.j_goodsbox_collectyes').show();

					$(this).parent().find('.j_goodsbox_collectno').attr("isqx", "qx");
				};
			}
		});
		//取消收藏
		This.Show.find(".j_goodsbox_collectno").click(function () {
			var $parent_div = $(this).parent();
			var goods_id = parseInt($(this).attr("goods_id"));
			if (CheckInput.IsEmpty(goods_id)) {
				return;
			}
			if (cancel_collection_goods != undefined) {
				var goodsid = new Array();
				goodsid[0] = goods_id;
				if (cancel_collection_goods(goodsid)) {
					$(this).hide();
					$(this).parent().find('.j_goodsbox_collectyes').hide();
					$(this).parent().find('.j_readycollection').show();
					$(this).attr("isqx", "yqx");
				};
			}
		});
		//鼠标移动到已收藏时显示取消收藏按钮
		This.Show.find("div[name=imgs]").hover(function () {
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
	},
	//商品展示
	show_goods: function () {
		var This = this;
		This.Show.find(".j_button_goods").click(function () {
			$(this).find(".j_arrow_store_goods").toggleClass("arrow_icon06 arrow_icon06_up");

			var $parents_div = $(this).parents(".j_searchstore_bg").find(".j_searchstore_goodsbox");
			var $parents_div2 = $(this).parents(".j_searchstore_bg").find(".j_searchstore_goodsbox_" + $(this).attr("sub_id"));

			if ($parents_div.length > 0) {
				if ($parents_div.is(":hidden")) {
					var $parents_div_child = $parents_div.find(".j_goodsbox_list");
					if ($parents_div_child.length > 0) {
						$parents_div.show();
						$parents_div.find(".j_searchstore_goodsbox_none_"+$(this).attr("sub_id")).hide();
					} else {
						//alert("暂无商品");
						$parents_div.show();
						$parents_div.find("#j_searchstore_goodsbox_none_" + $(this).attr("sub_id")).show();
					}
				} else {
					$parents_div.hide();
				}
			}

			if ($parents_div2.length > 0) {
				var sub_id = $(this).attr("sub_id");
				var sub_name = $(this).attr("sub_name");
				var $this_parents = $(this).parents(".j_searchstore_bg");
				if ($parents_div2.is(":hidden")) {
					var $parents_div2_child = $parents_div2.find(".j_goodsbox_list");
					if ($parents_div2_child.length <= 0) {
						//填充元素并显示
						This.search_option.Page = 0;
						This.SearchList(function (search_data) {
							if (search_data.success) {
								if ((search_data.infos_list.Goods != null) && (search_data.infos_list.Goods.length > 0)) {
									This.collection_goods_list(search_data);

									search_data["sub_name"] = sub_name;
									$this_parents.find(".j_searchstore_goodsbox_" + sub_id).empty();
									$("#search_tmpl").tmpl(search_data).appendTo($this_parents.find(".j_searchstore_goodsbox_" + sub_id));

									$parents_div2.show();
									
									//商品是否促销
									This.ruleGoods(search_data, sub_id);
									//收藏商品与取消收藏事件
									This.collection_goods();
								} else {
									//alert("暂无商品");
									$parents_div2.show();
									$parents_div2.find("#j_searchstore_goodsbox_none_" + sub_id).show();
								}

							} else {
								//alert("暂无商品");
								$parents_div2.show();
								$parents_div2.find("#j_searchstore_goodsbox_none_" + sub_id).show();
							}
						}, sub_id)
					} else {
						$parents_div2.show();
					}
				} else {
					$parents_div2.hide();
				}
			}
		});
	},
	goodsselect: function () {
		var This = this;
		//点击店铺
		This.Show.find(".j_goodsselect_store").click(function () {
			This.options.Page = 0;
			search_store.show_search_seller_sub();
		});
		This.Show.find(".j_goodsselect_goods").click(function () {
			reset.show_comment_reset();
		});
		$("#header_search_type_tab_shop").addClass("patt_bg_header_checked");
		$("#header_search_type_tab_shop").css("color", "#fff");
		$("#header_search_type_tab_good").removeClass("patt_bg_header_checked");
		$("#header_search_type_tab_good").css("color", "#000");
		$("#hid_type_value").val($("#header_search_type_tab_shop").attr("search_type"));
	},
	//左侧菜单
	firstMenu: function () {
		var This = this;
		//跟菜单
		This.Show.find(".j_searchgoods_nav_title").click(function () {
			search_option = get_search_option();
			search_option.First = 0;
			search_option.F_Class1Id = null;
			search_option.F_Class2Id = null;
			search_option.F_Class3Id = null;
			search_option.F_TagClassId = null;
			search_option.F_TagItemId = null;
			search_goods(search_option);
		});
		//二级菜单
		This.Show.find(".j_searchgoods_nav_first").click(function () {
			This.search_option.f_c1 = $(this).attr("class1_id");
			search_option = get_search_option();
			search_option.First = 0;
			search_option.F_Class1Id = This.search_option.f_c1;
			search_option.F_Class2Id = null;
			search_option.F_Class3Id = null;
			search_option.F_TagClassId = null;
			search_option.F_TagItemId = null;
			search_goods(search_option);
		});
		//三级菜单
		This.Show.find(".j_searchgoods_nav_second li").click(function () {
			This.search_option.f_c3 = $(this).attr("class3_id");
			search_option = get_search_option();
			search_option.First = 0;
			search_option.F_Class1Id = null;
			search_option.F_Class2Id = null;
			search_option.F_Class3Id = This.search_option.f_c3;
			search_option.F_TagClassId = null;
			search_option.F_TagItemId = null;
			search_goods(search_option);
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
	},
	//批量判断当前商品是否已收藏返回收藏商品ID
	collection_store_list: function (store_data) {
		var This = this;
		var storeid = new Array();
		for (var i = 0; i < store_data.search_store_model.SellerSub.length; i++) {
			store_data.search_store_model.SellerSub[i]["StoreIsExist"] = false;
			storeid.push(store_data.search_store_model.SellerSub[i].SubId);
		}
		if ((storeid != null) && (storeid.length > 0)) {
			$.ajax({
				url: getActionPath("CollecttionExistStore", "Cart"),
				dataType: "json",
				async: false,
				contentType: 'application/json',
				type: "POST",
				data: JSON.stringify({ seller_sub_id: JSON.stringify(storeid) }),
				success: function (data) {
					if (data.success == true) {
						for (var i = 0; i < store_data.search_store_model.SellerSub.length; i++) {
							for (var j = 0; j <= data.store_list.length; j++) {
								if (store_data.search_store_model.SellerSub[i].SubId == data.store_list[j]) {
									store_data.search_store_model.SellerSub[i]["StoreIsExist"] = true;
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
		return store_data;
	}

}
