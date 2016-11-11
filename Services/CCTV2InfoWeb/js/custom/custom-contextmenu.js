/**
 * Created by wyj on 2016/6/28.
 */
if (!$.fn.scContextMenu) {
    $.fn.scContextMenu = function (options) {
        var menu = $(this).data("scContextMenu");
        if (menu) {
            if (options) {
                menu.destroy();
                menu = new SCContextMenu(this, options);
            }
        }
        else {
            menu = new SCContextMenu(this, options);
        }
        return menu;
    }
}
/**
 * 右键菜单类型定义。
 * @param menu 菜单容器，推荐使用一个<div>元素。
 * @param options 菜单设置项，items属性用于标记菜单项(menuitem)数组.每个item元素可识别5中属性，分别是
 * {
 *      "text": //必选项。//菜单项的显示文本,
 *      "action"://必选项。//当点击菜单项时的回掉方法,
 *      "class"://可选项。//菜单项的元素类,可用于自定义css样式,
 *      "iconclass"://可选项。//指定菜单项的[awesome风格]或[jquery-ui风格]或[bootstrap风格]的图标,
 *      "id"://可选项。//用于标记菜单项Id，在弹出菜单时可以根据此Id标记是否需要禁用菜单项。
 *      "children"：//可选项。子菜单项，定义一个item数组，格式与option的items属性相同。
 * }
 * @constructor
 */
function SCContextMenu(menu, options) {
    if (!menu)
        throw "无效的menu对象。";
    if ((!options) || (!options.items) || (!options.items.push))
        throw "必须包含options参数，切该参数中拥有名为items的数组属性。"
    menu = $(menu);
    this._data = null;
    this._menu = menu.empty();
    this._initMenu(this._menu, options.items)
    menu.data("scContextMenu", this);
}

/**
 * 私有的初始化方法。
 */
SCContextMenu.prototype._initMenu = function (menucan, itemarr) {
    var menuul = $("<ul style='min-width: 150px;'>").appendTo(menucan);
    var that = this;
    fillMenu(menuul, itemarr);
    menuul.menu();

    function fillMenu(parentul, items) {
        for (var i = 0; i < items.length; i++) {
            if (items[i]) {
                var item = items[i];
                if (item === "separator") {
                    parentul.append($("<div class='hr hr-4'>"));
                }
                else {
                    var li = $("<li>");
                    //添加类定义
                    if (item["class"])
                        li.addClass(item["class"]);
                    //指定菜单项id。
                    if (item["id"])
                        li.attr("id", item["id"]);
                    //设置回掉方法。
                    if (item["action"]) {
                        li.data("itemaction", item["action"]);
                        li.click(function (event) {
                            //只有在非禁用状态下响应点击事件。
                            if (!$(this).hasClass("ui-state-disabled")) {
                                var callback = $(this).data("itemaction");
                                var data = that._data;
                                if (callback)
                                    callback(event, data);
                                that.hide();
                            }
                        });
                    }
                    //为图标项添加图标类
                    var iconSpan = $("<label style='min-width: 1.3em;'>").appendTo(li);
                    if (item["iconclass"]) {
                        iconSpan.addClass(item["iconclass"]);
                    }

                    //设置菜单项文本。
                    if (item["text"])
                        li.append($("<span >").html(item["text"]));
                    else
                        li.append($("<span>").html("菜单项" + i));

                    //添加子菜单项。
                    if (item.children && item.children.push && item.children.length > 0) {
                        var subul = $("<ul style='min-width: 150px;'>").appendTo(li);
                        fillMenu(subul, item.children);
                    }
                    parentul.append(li);
                }
            }
        }
    }

}

/**
 * 将菜单与指定的DOM元素绑定，当右键点击此元素时，自动弹出菜单。
 * @param ele 文档元素。
 * @param data 关联数据对象，该数据将在点击菜单项后被传递至回掉方法。
 * @param disableIds 禁用的菜单项Id列表(数组[])。
 */
SCContextMenu.prototype.bind = function (ele, data, disableIds) {
    if (ele) {
        ele = $(ele); //jquery对象确认。
        if (data || disableIds) {
            ele.data("contextmenu-data", {
                "refData": data,
                "disableIds": disableIds
            });
        }
        ele.on("contextmenu", this._oncontextmenu.bind(this));
    }
}

/**
 * 将菜单与指定的DOM元素解绑定。
 * @param ele
 */
SCContextMenu.prototype.unbind = function (ele) {
    if (ele) {
        ele = $(ele);//jquery对象确认。
        ele.removeData("contextmenu-data");
        ele.unbind("contextmenu", this._oncontextmenu);
    }
}

/**
 * 弹出右键。
 * @param clickevent 激发弹出菜单的dom事件参数。一般为点击事件的参数。改参数中应包含鼠标点击点位置，用于定位菜单的弹出位置。
 * @param data 关联数据对象，该数据将在点击菜单项后被传递至回掉方法。
 * @param disableIds 禁用的菜单项Id列表(数组[])。
 */
SCContextMenu.prototype.show = function (clickevent, data, disableIds) {
    clickevent.preventDefault(); //不知道此句管不管用。
    if (disableIds && disableIds.pop) {
        for (var i = 0; i < disableIds.length; i++) {
            var id = disableIds[i];
            this._menu.find("li#" + id).addClass("ui-state-disabled");
        }
    }
    this._menu.css({"top": clickevent.clientY + "px", "left": clickevent.clientX + "px", "position": "absolute"});
    this._menu.removeClass("hidden");

    this._data = data;
    $("html").bind("mousedown", this._onbodymousedown.bind(this));
}

/**
 * 隐藏弹出菜单。该操作一般由菜单对象在满足一定条件后，自动执行。
 */
SCContextMenu.prototype.hide = function () {
    this._menu.css({"top": "-100px", "left": "-100px"});
    this._menu.addClass("hidden");
    this._menu.find("li").removeClass("ui-state-disabled");
    this._data = null;
    $("html").unbind("mousedown", this._onbodymousedown.bind(this));
}

SCContextMenu.prototype.destroy = function () {
    this.hide();
    this._menu.empty();
    this._menu.removeData("scContextMenu");
}

//在全局点击时，隐藏菜单项
SCContextMenu.prototype._onbodymousedown = function (event) {
    if (this._menu.find(event.target).length == 0)
        this.hide();
}

//监听的dom元素的右键点击事件。
SCContextMenu.prototype._oncontextmenu = function (event) {
    var cmdata = $(event.target).data("contextmenu-data") || {};
    this.show(event, cmdata.refData, cmdata.disableIds);
}