/**
 * Created by wyj on 2016/6/29.
 */
if (!$.fn.scModalForm) {
    $.fn.scModalForm = function (options) {
        var form = $(this).data("scModalForm");
        if (form) {
            if (options) {
                form.updateOptions(options);
            }
        }
        else {
            form = new SCModalForm(this, options);
        }
        return form;
    }
}
if (!$.fn.scSimpleDialog) {
    $.fn.scSimpleDialog = function (options) {
        var dia = $(this).data("scSimpleDialog");
        if (dia) {
            if (options) {
                dia.updateOptions(options);
            }
        }
        else {
            dia = new SCSimpleDialog(this, options);
        }
        return dia;
    }
}
/**
 * 生成一个bootstrap风格的模态框对象。
 * @param modal 窗口容器，推荐使用一个<div>元素。
 * @param options 窗口配置选项。使用如下的格式和参数。
 *  {
 *          //核心选项。
            core: {
                size: , //可选项，表示窗口大小的枚举，sm--小窗口,md--普通窗口,lg--大窗口。默认为md
                animation: true //可选项，是否以动画方式弹出窗口。默认为false。
            },
            //标题选项
            title: {
                "class": , //可选项。为标题栏添加类定义，
                "text": , //可选项，建议设置。标题栏文本,
                "istexthtml": //可选项, 标题文本是否以html格式显示，false--以纯文本方式显示。
                "iconclass": //可选项。 使用[awesome风格]或[jquery-ui风格]或[bootstrap风格]的图标。显示在标题文本前面，
            },
            footer: {
                "class": //可选项。为模态窗口页脚添加类定义。
                //窗口按钮组，//必选项。
                "buttons": [
                    {
                        "id": //可选项。为按钮定义个id标识，
                        "class": //可选项。,为按钮添加类定义。
                        "iconclass": //可选的,显示在按钮文本前面，使用[awesome风格]或[jquery-ui风格]或[bootstrap风格]的图标。
                        "text": //必选项,按钮文本。
                        "iscancel": //可选项。标记按钮是否是默认的取消按钮，true--点击按钮时，模态框将关闭。
                        "action": //必选项，按钮点击事件的回掉方法，包含两个参数，event：标准点击事件参数，modal:当前模态窗口对象。
                    }
                ]
            }
            events:{
                "showing":模态框show()被调用且窗口尚未显示之前激发此事件。//可选项。
                "shown":模态框show()方法被调用且窗口完全显示之后激发此事件。//可选项。
                "hiding"：模态框hide()方法被调用但窗口未隐藏之前激发此事件。//可选项。
                "hidden":模态框hide()方法被调用且窗口完全隐藏之后激发此事件。//可选项。
            }
        }
 * @constructor
 */
function SCModalForm(modal, options) {
    if (!modal)
        throw "无效的模态框对象。";
    modal = $(modal) //jquery对象确认。
    this._modal = modal;
    this._hasParent = modal.parent().length > 0;
    this._events;
    this._data = {};
    this._dialogId = undefined;
    this._bodyId = undefined;
    this._titleId = undefined;
    this._footerId = undefined;
    var inner = modal.children().detach();
    if (inner) {
        if (inner.hasClass("modal-dialog")) {
            if (inner.find(".modal-content .modal-body").length > 0) {
                inner = inner.find(".modal-content .modal-body").children().detach();
            }
        }
    }
    this._buildModal(modal, options);
    this._modal.find("#" + this._bodyId).append(inner); //附加原始对象。
    modal.data("scModalForm", this);
}

SCModalForm.prototype._buildModal = function (modalDiv, options) {
    var modalId = modalDiv.attr("id") ? modalDiv.attr("id") : "custom";
    // modalDiv.empty(); //清空。
    //基本设置
    modalDiv.removeClass("hidden").removeClass("hide").addClass("modal");
    modalDiv.attr("role", "dialog");
    modalDiv.attr("tabindex", -1);
    if (!(options.core && options.core.animation === false)) {
        modalDiv.addClass("fade");
    }

    if (options.events) {
        this._updateEvents(options.events);
    }

    //窗口。
    this._dialogId = modalId + "-m-d-id";
    var dialog = $("<div class='modal-dialog'>").attr("id", this._dialogId).appendTo(modalDiv);
    if (options.core) {
        this._updateCore(dialog, options.core);
    }

    //内容
    var content = $("<div class='modal-content'>").appendTo(dialog);
    //标题
    var header = $("<div class='modal-header'>").appendTo(content);
    header.append(
        $("<button class='close' type='button' data-dismiss='modal'>").html("&times;")
    );

    this._titleId = modalId + "-m-t-id";
    var title = $("<h4 class='modal-title'>").attr("id", this._titleId).appendTo(header);
    if (options.title) {
        this._updateTitle(title, options.title);
    }

    //正文。
    var mbody = $("<div class='modal-body'>").appendTo(content);
    this._bodyId = modalId + "-m-b-id";
    mbody.attr("id", this._bodyId);

    //页脚。
    this._footerId = modalId + "-m-f-id";
    var mfooter = $("<div class='modal-footer'>").attr("id", this._footerId).appendTo(content);
    if (options.footer) {
        this._updateFooter(mfooter, options.footer);
    }
}

SCModalForm.prototype._updateEvents = function (events) {
    if (events) {
        var that = this;
        this._events = events;
        if ("showing" in events) {
            this._modal.unbind("show.bs.modal");
            if (events["showing"])
                this._modal.on("show.bs.modal", function (e) {
                    that.events["showing"](e, that);
                })
        }
        if ("shown" in events) {
            this._modal.unbind("shown.bs.modal");
            if (events["shown"]) {
                this._modal.on("shown.bs.modal", function (e) {
                    that._events["shown"](e, that);
                })
            }
        }
        if ("hiding" in events) {
            this._modal.unbind("hide.bs.modal");
            if (events["hiding"]) {
                this._modal.on("hide.bs.modal", function (e) {
                    that._events["hiding"](e, that);
                })
            }
        }
        if ("hidden" in events) {
            this._modal.unbind("hidden.bs.modal");
            if (events["hidden"]) {
                this._modal.on("hidden.bs.modal", function (e) {
                    that._events["hidden"](e, that);
                })
            }
        }
    }
}

SCModalForm.prototype._updateCore = function (dialog, cData) {
    if (dialog && cData) {
        if (cData && cData.animation === true) {
            this._modal.addClass("fade");
        } else {
            this._modal.removeClass("fade");
        }
        if (cData.size) {
            dialog.removeClass("modal-sm modal-lg");
            if (cData.size == 'sm')
                dialog.addClass("modal-sm");
            else if (cData.size == "lg")
                dialog.addClass("modal-lg");
        }
    }
}

SCModalForm.prototype._updateTitle = function (title, tData) {
    if (title && tData) {
        if (tData["class"]) {
            title.removeClass().addClass("modal-title");
            title.addClass(tData["class"]);
        }
        if (tData["iconclass"]!== undefined && tData["iconclass"] !== null) {
            title.find("label").first().remove();
            if(tData["iconclass"]) {
                title.prepend(
                    $("<label style='min-width: 1.5em;'>").addClass(tData["iconclass"])
                );
            }
        }
        if (tData["text"] !== undefined && tData["text"] !== null) {
            title.find("span").first().remove();
            if (tData["istexthtml"] === true)
                title.append($("<span>").html(tData["text"]));
            else
                title.append($("<span>").text(tData["text"]));
        }
    }
}

SCModalForm.prototype._updateFooter = function (footer, fData) {
    var that = this;
    if (footer && fData) {
        if (fData["class"]) {
            footer.removeClass().addClass("modal-footer");
            footer.addClass(fData["class"]);
        }
        if (fData.buttons) {
            footer.children().remove();
            for (var i = 0; i < fData.buttons.length; i++) {
                var obtn = fData.buttons[i];
                var btn = $("<div class='btn btn-default'>").appendTo(footer);
                if (obtn["id"])
                    btn.attr("id", obtn["id"]);
                if (obtn["class"])
                    btn.addClass(obtn["class"]);
                if (obtn["iconclass"]) {
                    btn.append($("<label style='min-width: 2em;'>").addClass(obtn["iconclass"]));
                }
                if (obtn["text"])
                    btn.append($("<span>").text(obtn["text"]));
                if (obtn["action"]) {
                    btn.data("btnaction", obtn["action"]); //记录action
                    btn.click(function (event) {
                        $(this).data("btnaction")(event, that); //激发事件回掉。
                    })
                }
                if (obtn["iscancel"])
                    btn.attr("data-dismiss", "modal");
                else
                    btn.removeAttr("data-dismiss");
            }
        }
    }
}

SCModalForm.prototype.updateOptions = function (options) {
    if (options) {
        this._updateCore(this._modal.find("#" + this._dialogId), options.core);
        this._updateTitle(this._modal.find("#" + this._titleId), options.title);
        this._updateFooter(this._modal.find("#" + this._footerId), options.footer);
    }
}

SCModalForm.prototype.getModal = function () {
    return this._modal;
}

SCModalForm.prototype.getData = function (name) {
    return this._data[name];
}

SCModalForm.prototype.setData = function (name, data) {
    this._data[name] = data;
    return this;
}

SCModalForm.prototype.removeData = function (name) {
    if (name === undefined)
        this._data = {};
    else
        delete this._data[name];
    return this;
}

SCModalForm.prototype.setContent = function (html) {
    this._modal.find("#" + this._bodyId).html(html);
    return this;
}

SCModalForm.prototype.show = function () {
    this._modal.modal("show");
    return this;
}

SCModalForm.prototype.hide = function () {
    this._modal.modal("hide");
    return this;
}

SCModalForm.prototype.toggle = function () {
    this._modal.modal("toggle");
    return this;
}

SCModalForm.prototype.destroy = function () {
    if (this._hasParent) {
        var inner = this._modal.find("#" + this._bodyId).children().detach();
        this._modal.children().detach().append(inner).removeClass("hidden");
        this._modal.removeData("scModalForm");
    }
    else {
        this._modal.remove();
    }
}
//==========================//
//弹出窗口。
//==========================//
/**
 * 生成一个对话框弹窗对象。
 * @param dialog 对话框容器，推荐使用一个<div>元素。
 * @param options 对话框设置选项。格式如下。
 *      {
            size: "sm", //可选项，表示对话框的大小。"sm"--小对话框,"md"--中等大小对话框,"lg"--大对话框。默认为"sm"
            animation: false //可选项。是否以动画方式弹出对话框，true--动画方式弹出，false--不使用动画。默认为 false
        }
 * @constructor
 */
function SCSimpleDialog(dialog, options) {
    this._core = {};
    this.updateOptions(options);
    var mfOptiosn = {
        core: this._core,
        title: {
            "text": "提示窗口", //标题栏文本, //可选项，建议设置。
            "istexthtml": false, //标题文本是否以html格式显示，false--以纯文本方式显示。//可选项。
            "iconclass": "glyphicon glyphicon-info-sign text-primary", //显示在标题文本前面，使用[awesome风格]或[jquery-ui风格]或[bootstrap风格]的图标。//可选的。
        },
        footer: {
            //窗口按钮组，//必选项。
            "buttons": [
                {
                    "class": "btn-sm btn-primary",//为按钮添加类定义。//可选项。
                    "text": "确定",//按钮文本。//可选项，建议设置。
                    "iscancel": true//标记按钮是否是默认的取消按钮，true--点击按钮时，模态框将关闭。//可选项。
                }
            ]
        }
    };
    this._dialog = new SCModalForm(dialog, mfOptiosn);
    $(dialog).data("scSimpleDialog", this);
}

SCSimpleDialog.prototype._showInnerForm = function (message, title, type, options, isconfirm, callback) {
    var titleIcon = "glyphicon-info-sign text-primary";
    var btnClass = "btn-primary";
    if (type == "warning") {
        titleIcon = "glyphicon-exclamation-sign text-warning";
        btnClass = "btn-warning"
    }
    else if (type == "error") {
        titleIcon = "glyphicon-remove-sign text-danger";
        btnClass = "btn-danger";
    }
    var mfoptions = {
        core: {
            size: (options && options.size) ? options.size : this._core.size,
            animation: (options && options.animation) ? options.animation : this._core.animation
        },
        title: {
            "text": title,
            "iconclass": "glyphicon " + titleIcon
        },
        footer: {
            "buttons": [
                {
                    "class": "btn-sm " + btnClass,
                    "text": "确定",
                    "iscancel": true
                }
            ]
        }
    };
    //判断是否是确认窗口，已确定弹出框的按钮样式。
    if (isconfirm) {
        mfoptions.footer = {
            "buttons": [
                {
                    "class": "btn-sm " + btnClass,
                    "text": "确定",
                    "iscancel": true,
                    "action": function () {
                        if (callback)
                            callback(true);
                    }
                },
                {
                    "class": "btn-sm ",
                    "text": "取消",
                    "iscancel": true,
                    "action": function () {
                        if (callback)
                            callback(false);
                    }
                }
            ]
        }
    }
    else {
        mfoptions.footer = {
            "buttons": [
                {
                    "class": "btn-sm " + btnClass,
                    "text": "确定",
                    "iscancel": true
                }
            ]
        }
    }
    this._dialog.updateOptions(mfoptions);
    this._dialog.setContent(message);
    this._dialog.show();
}

SCSimpleDialog.prototype.updateOptions = function (options) {
    this._core = {
        size: options ? options.size : "sm", //可选的sm--小窗口,md--普通窗口,lg--大窗口。
        animation: options && options.animation
    };
}

SCSimpleDialog.prototype.getData = function (name) {
    return this._dialog.getData(name);
}

SCSimpleDialog.prototype.setData = function (name, data) {
    this._dialog.setData(name, data);
    return this;
}

SCSimpleDialog.prototype.removeData = function (name) {
    this._dialog.removeData(name);
    return this;
}

SCSimpleDialog.prototype.showDialog = function (message, title, type, options) {
    this._showInnerForm(message, title, type, options, false);
    return this;
}

SCSimpleDialog.prototype.showConfirm = function (resultCallback, message, title, type, options) {
    this._showInnerForm(message, title, type, options, true, resultCallback);
    return this;
}

SCSimpleDialog.prototype.showAjaxErrorResponse = function (message, jqXHR, status, err, options) {
    var content = message + "<p>错误码:" + jqXHR.status + " " + err + "</p>";
    if (jqXHR && jqXHR.responseJSON && jqXHR.responseJSON.Message)
        content += "<p>" + jqXHR.responseJSON.Message + "</p>";
    this.showDialog(content, "Ajax错误", "error", options);
    return this;
}

SCSimpleDialog.prototype.destroy = function () {
    this._dialog.destroy();
    this._dialog.removeData("scSimpleDialog");
}