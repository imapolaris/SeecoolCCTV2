/**
 * Created by wyj on 2016/7/5.
 */
if (!$.fn.scChosen) {
    $.fn.scChosen = function (options) {
        var chosen = $(this).data("scChosen");
        if (chosen) {
            if (options) {
                chosen.destroy();
                chosen = new SCChosen(this, options);
            }
        }
        else {
            chosen = new SCChosen(this, options);
        }
        return chosen;
    }
}
/**
 * 单选/多选下拉选项。
 * @param select 下拉菜单容器，是一个select元素
 * @param options 参数项。格式如下：
 * {
        allowDeselect: true, //可选项。//在单选状态下，是否允许取消当前项选中，默认为false。
        multiple: false, //可选项。//是否允许多选，默认为false。
        searchBoxDispLimit: 3, //可选项。显示搜索框所需的最小项数，默认值为10。
        changeHandler: function (selectedValue) {   //可选项。//当选中项改变后的回掉方法。
            console.log(selectedValue);
        }
    }
 * @constructor
 */
function SCChosen(select, options) {
    select = $(select);
    this._select = select;
    this._options = options;
    this._destroyed = false;
    if (!select.hasClass("chosen-select"))
        select.addClass("chosen-select");
    if (!select.attr("data-placeholder"))
        select.attr("data-placeholder", "请选择...");
    //封装组件并刷新。
    this._init();
    select.data("scChosen", this);
}

SCChosen.prototype._readyToDeselect = function () {
    //是否允许取消选中
    var deselect = this._options && this._options.allowDeselect === true;
    if (deselect) {
        var op = this._select.find("option:first-child");
        if (op && op.text()) {
            this._select.prepend($("<option>"));
        }
    }
}

SCChosen.prototype._init = function () {
    this._readyToDeselect();

    var deselect = false;
    var multiple = false;
    var sLimit = 10;
    var changeHandler = undefined;
    if (this._options) {
        deselect = this._options.allowDeselect === true;
        multiple = this._options.multiple === true;
        sLimit = this._options.searchBoxDispLimit >= 0 ? this._options.searchBoxDispLimit : 10;
        changeHandler = this._options.changeHandler;
    }

    if (multiple)
        this._select.attr("multiple", true);
    //选择内容改变后的回掉事件。
    if (changeHandler)
        this._select.on("change", function (event, value) {
            changeHandler(value);
        });
    this._select.chosen({
        allow_single_deselect: deselect,
        width: "100%",
        disable_search_threshold: sLimit //小于n个选项时，不显示搜索框。
    }).trigger("chosen:updated");
}

/**
 * 更新选择项数据内容
 * @param data 数据数组。
 * @param valueName 用于赋值的字段名称。
 * @param displayName 用于显示的字段名称。
 */
SCChosen.prototype.update = function (data, valueName, displayName) {
    if (this._destroyed)
        throw "对象已被销毁";
    if (!valueName)
        valueName = "value";
    if (!displayName)
        displayName = "display";

    var value = this._select.val();
    this._select.empty();
    if (data && ("push" in data)) {
        for (var i = 0; i < data.length; i++) {
            this._select.append(
                $("<option>").text(data[i][displayName]).val(data[i][valueName]).data("chosenItemData", data[i])
            );
        }
    }
    this._readyToDeselect();
    this._select.val(value);
    return this.refresh();
}

/**
 * 获取当前选中项的值。
 * @returns {boolean|Array|undefined}
 */
SCChosen.prototype.getSelectedValues = function () {
    if (this._destroyed)
        throw "对象已被销毁";
    var vals = [];
    this._select.find("option").each(function (index, ele) {
        ele = $(ele);
        if (ele.prop("selected") && ele.val())
            vals.push(ele.val());
    });
    return (vals.length > 0 && vals) || undefined;
}

/**
 * 获取当前选中项的数据。
 * @returns {boolean|Array|undefined}
 */
SCChosen.prototype.getSelectedDatas = function () {
    if (this._destroyed)
        throw "对象已被销毁";
    var datas = [];
    this._select.find("option").each(function (index, ele) {
        ele = $(ele);
        if (ele.prop("selected") && ele.data("chosenItemData"))
            datas.push(ele.data("chosenItemData"));
    });
    return (datas.length > 0 && datas) || undefined;
}

/**
 * 手动选中某一项。
 * @param value
 */
SCChosen.prototype.select = function (value) {
    if (this._destroyed)
        throw "对象已被销毁";
    this._select.val(value);
    this._select.trigger("change", {selected: value});
    return this.refresh();
}

SCChosen.prototype.selectMulti = function (values) {
    if (this._destroyed)
        throw "对象已被销毁";
    if (values && values.length > 0) {
        this._select.find("option").each(function (index, ele) {
            ele = $(ele);
            var eleVal = ele.val();
            var isEqual = false;
            for (var i = 0; i < values.length; i++) {
                if (values[i] == eleVal) {
                    isEqual = true;
                    break;
                }
            }
            if (isEqual)
                ele.prop("selected", true);
        });
    }
    this._select.change();
    return this.refresh();
}

SCChosen.prototype.refresh = function () {
    if (this._destroyed)
        throw "对象已被销毁";
    this._select.trigger("chosen:updated");
    return this;
}

/**
 * 销毁对象，销毁后对象不可重用。
 */
SCChosen.prototype.destroy = function () {
    this._select.chosen("destroy");
    this._select.removeData("scChosen");
    this._destroyed = true;
}