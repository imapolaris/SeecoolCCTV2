/**
 * Created by wyj on 2016/6/8.
 */

function initServerManagerPage() {
    //将表单加载到模态框内
    $(".sc-form-container").load("html/form-server-page.html", function (res, status) {
        if (status == "success") {
            setupServerValidate(serverUpdateSubmit);
            $("#serviceip").on("change",function () {
                var val=$(this).val();
                if(val)
                {
                    $("#streamserverip").val(val);
                    $("#controlserverip").val(val);
                }
            });
        }
    });

    //初始化表格对象。
    var actionList = {
        "editRow": editRow,
        "deleteRow": deleteRow
    };
    var serverDT = new SCDataTable($("#server-manager-table"), {"actionList": actionList});

    //初始化模态框
    var mfOptions = {
        title: {
            "text": "修改节点信息"
        },
        footer: {
            "buttons": [
                {
                    "class": "btn-primary",
                    "text": "保存",
                    "iscancel": false,
                    "action": function (event, modal) {
                        // serverUpdateSubmit();
                        $("#server-info-form").submit();
                    }
                },
                {
                    "text": "取消",
                    "iscancel": true
                }
            ]
        }
    };
    var mform = new SCModalForm($("#server-update-modal-form"), mfOptions);
    getServerInfos();

    $("#btn-add-server").click(function (event) {
        mform.removeData("serverInfo");
        mform.setData("optype", "create").show();
        clearServerInputs();
    });

    function clearServerInputs() {
        // $("#server-info-form")[0].reset();
        $("#server-info-form").validate().resetForm();
    };

    function setupServerValidate(validated) {
        $("#server-info-form").validate({
            rules: {
                "Name": "required",
                "InfoServiceIp": {
                    required:true,
                    ipaddress:true
                },
                "InfoServicePort": {
                    required: true,
                    max: 65535
                },
                "StreamServerIp": {
                    required:true,
                    ipaddress:true
                },
                "StreamServerPort": {
                    required: true,
                    max: 65535
                },
                "ControlServerIp": {
                    required:true,
                    ipaddress:true
                },
                "ControlServerPort": {
                    required: true,
                    max: 65535
                }
            },
            messages: {
                "Name": "必须输入节点名称",
                "InfoServiceIp": {
                    required:"必须输入IP地址",
                },
                "InfoServicePort": {
                    required: "必须输入端口号",
                    max: "端口号不能大于65535"
                },
                "StreamServerIp": {
                    required:"必须输入IP地址",
                },
                "StreamServerPort": {
                    required: "必须输入端口号",
                    max: "端口号不能大于65535"
                },
                "ControlServerIp": {
                    required:"必须输入IP地址",
                },
                "ControlServerPort": {
                    required: "必须输入端口号",
                    max: "端口号不能大于65535"
                }
            },
            submitHandler: function (form, event) {
                validated && validated();
            },
            invalidHandler: function (event, validator) {
                console.log("未通过验证:" + validator.numberOfInvalids());
            },
            errorClass: "text-danger"
        });
    }

    function serverUpdateSubmit() {
        var server = formdataToObject($("#server-info-form").serializeArray());
        var si = mform.getData("serverInfo");
        var isUpdate = mform.getData("optype") == "update";
        var url = "api/server";
        var type = "post";
        if (isUpdate) {
            server.ServerId = si.serverId;
            url += "/" + si.serverId;
            type = "put"
        }
        //发送数据请求。
        $.ajax({
            url: url,
            type: type,
            data: JSON.stringify(server),
            contentType: "application/json",
            success: function (data, status, jqXHR) {
                if (isUpdate)
                    serverDT.update(si.rowIndex, server);
                else
                    getServerInfos();
                mform.hide();
            },
            error: function (jqXHR, status, err) {
                showErrorResponseDialog("保存服务器信息失败", jqXHR, status, err);
            }
        });
    }

    function getServerInfos() {
        $.get("api/server").done(function (data, status, jqXHR) {
            buildServerManagerTable(data);
        }).fail(function (jqXHR, status, errThrow) {
            showErrorResponseDialog("服务器节点列表获取失败", jqXHR, status, errThrow);
        });
    }

    function buildServerManagerTable(data) {
        serverDT.init(data);
    }

    function editRow(index) {
        var cell = serverDT.getData(index);
        //显示模态框
        mform.setData("serverInfo", {
            serverId: cell.ServerId,
            rowIndex: index
        });
        mform.setData("optype", "update").show();
        //初始化表单数据。
        updateFormData($("#server-info-form"), cell);
    }

    function deleteRow(index) {
        var cell = serverDT.getData(index);
        showConfirm("确认删除 " + cell.Name + " ?", "warning", function (result) {
            if (result) {
                $.ajax({
                    url: "api/server/" + cell.ServerId,
                    type: "Delete",
                    success: function () {
                        console.info("删除成功");
                        //从表格数据中删除行，并执行刷新操作。
                        serverDT.remove(index);
                    },
                    error: function (jqXHR, statuc, errThrow) {
                        showErrorResponseDialog("删除 " + cell.Name + " 失败", jqXHR, status, errThrow);
                    }
                });
            }
        });
    }
}
