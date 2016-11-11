/**
 * Created by wyj on 2016/6/8.
 */
var globalDialog;

//添加Ip地址的验证方法。
$.validator.addMethod("ipaddress", function (value, ele, param) {
    var exp = /^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$/;
    return exp.test(value);
},"Ip地址格式不正确。");

$(function () {
    //扩展
    globalDialog = new SCSimpleDialog($("#mycustom-dialog"), {
        size: "sm",
        animation: false
    });
});

function showDialog(message, type) {
    globalDialog.showDialog(message, "消息提示", type);
}

function showErrorResponseDialog(message, jqXHR, status, err) {
    globalDialog.showAjaxErrorResponse(message, jqXHR, status, err);
}

function showConfirm(message, type, resultCallback) {
    globalDialog.showConfirm(resultCallback, message, "消息确认", type);
}

/**
 * 主要用于重新初始化模态框中的选择框。
 * @param mForm
 */
function initModalForm(mForm) {
    // mForm.on('shown.bs.modal', function () {
    //     loadChoosenSelect();
    //     $(this).find('.chosen-container').each(function () {
    //         $(this).find('a:first-child').css('width', '100%');
    //         $(this).find('.chosen-drop').css('width', '100%');
    //         $(this).find('.chosen-search input').css('width', '100%');
    //     });
    // })
}

function loadChoosenSelectInModalForm($modal) {
    loadChoosenSelect();
    // $modal.find('.chosen-container').each(function () {
    //     $modal.find('a:first-child').css('width', '100%');
    //     $modal.find('.chosen-drop').css('width', '100%');
    //     $modal.find('.chosen-search input').css('width', '100%');
    // });
}

function loadChoosenSelect(select) {
    if (!select)
        select = $(".chosen-select");
    select.chosen({
        allow_single_deselect: true,
        width: "100%"
    }).trigger("chosen:updated");
    //resize the chosen on window resize
    // $(window)
    //     .off('resize.chosen')
    //     .on('resize.chosen', function () {
    //         $('.chosen-select').each(function () {
    //             var $this = $(this);
    //             $this.next().css({'width': $this.parent().width()});
    //         })
    //     }).trigger('resize.chosen');
}

function formdataToObject(data, desObj, preprocess) {
    $(data).each(function (index, ele) {
        //针对每个元素进行预处理。
        if (preprocess) {
            ele = preprocess(ele) || ele;
        }
        //如果指定了目标对象，则使用目标对象，否则创建一个新对象。
        if (!desObj)
            desObj = {};
        //遍历表单数据数组，为对象赋值。
        if (desObj[ele.name]) {
            if (!desObj[ele.name].push) {
                desObj[ele.name] = [desObj[ele.name]];
            }
            desObj[ele.name].push(ele.value);
        }
        else {
            desObj[ele.name] = ele.value;
        }
    });
    return desObj;
}

function updateFormData(form, data) {
    if (form && data) {
        clearFormInputData(form);
        var inputs = form.find(".sc-form-input");
        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i].name && data[inputs[i].name] != undefined) {
                if (inputs[i].type == "checkbox") {
                    $(inputs[i]).prop("checked", data[inputs[i].name]).trigger("change");
                }
                else if (inputs[i].type == "radio") {
                    if (inputs[i].value == data[inputs[i].name]) {
                        $(inputs[i]).prop("checked", data[inputs[i].name]).trigger("change");
                    }
                }
                else if ($(inputs[i]).hasClass("chosen-select")) {
                    $(inputs[i]).val(data[inputs[i].name]).trigger("chosen:updated");
                }
                else {
                    $(inputs[i]).val(data[inputs[i].name]);
                }
            }
        }
    }
}

function clearFormInputData(form) {
    form.find("input[type='text']").val(null);
    form.find("select").val(null).trigger("chosen:updated");
    form.find("input[type='radio']").prop("checked", false);
    form.find("input[type='checkbox']").prop("checked", false);
}

function updateSelecterOptions(selecter, data, valueName, dispName, changeCallback) {
    if (selecter) {
        var value = selecter.val();
        selecter.empty();
        selecter.append($("<option>")); //添加空白项以显示删除选中的叉号。
        if (data && ("push" in data)) {
            for (var i = 0; i < data.length; i++) {
                selecter.append(
                    $("<option>").text(data[i][dispName]).val(data[i][valueName])
                );
            }
        }
        selecter.val(value);
        loadChoosenSelect(selecter);
        //监听逻辑节点树选择改变事件。
        if (changeCallback) {
            selecter.chosen().change(function (event, value) {
                changeCallback(event, value);
            });
        }
    }
}

function updateSelecterValue(selecter, value) {
    selecter.val(value).trigger("chosen:updated");
}

function createGuid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }

    return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
}

function copyObject(src, des) {
    var pNames = Object.getOwnPropertyNames(src);
    for (var i = 0; i < pNames.length; i++) {
        des[pNames[i]] = src[pNames[i]];
    }
}