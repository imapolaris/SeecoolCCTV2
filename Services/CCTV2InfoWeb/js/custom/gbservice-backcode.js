/**
 * Created by wyj on 2016/8/31.
 */

function initPlatSuperManagerPage() {
    var superior = 2; //表示上级域平台的枚举值。
    //将表单加载到模态框内
    $(".sc-form-container").load("html/form-platform-page.html", function (res, status) {
        if (status == "success") {
            setupValidate(updateSubmit);
        }
    });

    //初始化表格对象。
    var actionList = {
        "editRow": editRow,
        "deleteRow": deleteRow,
        "shareDevices": shareDevices
    };
    var processList = {
        "processOnline": processOnline,
        "processTime": processTime
    };
    var platDT = new SCDataTable($("#platsuper-manager-table"),
        {
            "actionList": actionList,
            "processList": processList
        });

    //初始化模态框
    var mfOptions = {
        title: {
            "text": "修改上级域"
        },
        footer: {
            "buttons": [
                {
                    "class": "btn-primary",
                    "text": "保存",
                    "iscancel": false,
                    "action": function (event, modal) {
                        $("#platform-info-form").submit();
                    }
                },
                {
                    "text": "取消",
                    "iscancel": true
                }
            ]
        }
    };
    var mform = new SCModalForm($("#platsuper-update-modal-form"), mfOptions);
    getAllSuperPlatforms();

    $("#btn-add-platsuper").click(function (event) {
        mform.removeData("platformInfo");
        mform.setData("optype", "create").show();
        clearInputs();
    });

    function processOnline(data, propName) {
        if (data[propName] === true)
            return "<span class='text-success'>在线</span>";
        else
            return "<span class='text-danger'>不在线</span>";
    }

    function processTime(data, propName) {
        return moment(data[propName]).format("YYYY-MM-DD HH:mm:ss");
    }

    function clearInputs() {
        $("#platform-info-form").validate().resetForm();
    };

    function setupValidate(validated) {
        $("#platform-info-form").validate({
            rules: {
                "Name": "required",
                "SipNumber": "required",
                "Ip": {
                    required: true,
                    ipaddress: true
                },
                "Port": {
                    required: true,
                    max: 65535
                }
            },
            messages: {
                "Name": "必须输入名称",
                "SipNumber": "必须输入网关编号",
                "Ip": {
                    required: "必须输入IP地址",
                },
                "Port": {
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

    function updateSubmit() {
        var plat = formdataToObject($("#platform-info-form").serializeArray());
        var si = mform.getData("platformInfo");
        var isUpdate = mform.getData("optype") == "update";
        var url = "api/PlatSuperior";
        var type = "post";
        if (isUpdate) {
            plat.Id = si.Id;
            url += "/" + si.Id;
            type = "put"
        }
        plat.Type = superior;
        //发送数据请求。
        $.ajax({
            url: url,
            type: type,
            data: JSON.stringify(plat),
            contentType: "application/json",
            success: function (data, status, jqXHR) {
                if (isUpdate)
                    platDT.update(si.rowIndex, plat);
                else
                    getAllSuperPlatforms();
                mform.hide();
            },
            error: function (jqXHR, status, err) {
                showErrorResponseDialog("保存平台信息失败", jqXHR, status, err);
            }
        });
    }

    function getAllSuperPlatforms() {
        $.get("api/PlatSuperior").done(function (data, status, jqXHR) {
            buildManagerTable(data);
            getAllSuperPlatformStatus();
        }).fail(function (jqXHR, status, errThrow) {
            showErrorResponseDialog("上级域平台列表获取失败", jqXHR, status, errThrow);
        });
    }
    
    function getAllSuperPlatformStatus() {
        $.get("api/PlatSuperior/status").done(function (data, status, jqXHR) {
            buildManagerTable(data);
        }).fail(function (jqXHR, status, errThrow) {
            showErrorResponseDialog("上级域平台列表获取失败", jqXHR, status, errThrow);
        });
    }

    function buildManagerTable(data) {
        platDT.init(data);
    }

    function editRow(index) {
        var cell = platDT.getData(index);
        //显示模态框
        mform.setData("platformInfo", {
            Id: cell.Id,
            rowIndex: index
        });
        mform.setData("optype", "update").show();
        //初始化表单数据。
        updateFormData($("#platform-info-form"), cell);
    }

    function deleteRow(index) {
        var cell = platDT.getData(index);
        showConfirm("确认删除平台 " + cell.Name + " ?", "warning", function (result) {
            if (result) {
                $.ajax({
                    url: "api/PlatSuperior/" + cell.Id,
                    type: "Delete",
                    success: function () {
                        console.info("删除成功");
                        //从表格数据中删除行，并执行刷新操作。
                        platDT.remove(index);
                    },
                    error: function (jqXHR, status, errThrow) {
                        showErrorResponseDialog("删除 " + cell.Name + " 失败", jqXHR, status, errThrow);
                    }
                });
            }
        });
    }

    function shareDevices(index) {
        var cell = platDT.getData(index);
        showConfirm("确认将当用户 [" + cell.UserName + "] 的设备列表共享到平台 [" + cell.Name + "] ?", "info", function (result) {
            if (result) {
                $.ajax({
                    url: "api/PlatSuperior/ShareTo/" + cell.Id,
                    type: "Post",
                    success: function (data, status, jqXHR) {
                        showDialog(data, "info");
                    },
                    error: function (jqXHR, status, errThrow) {
                        showErrorResponseDialog("共享设备列表失败", jqXHR, status, errThrow);
                    }
                });
            }
        });
    }
}

//初始化下级域配置页面。
function initPlatLowerManagerPage() {
    var lower = 1;//下级域枚举值。
    //将表单加载到模态框内
    $("#form-platlower-can").load("html/form-platform-page.html", function (res, status) {
        if (status == "success") {
            setupValidate(updateSubmit);
        }
    });

    $("#form-platlower-devices").load("html/form-devicelist-page.html");

    //初始化表格对象。
    var actionList = {
        "editRow": editRow,
        "deleteRow": deleteRow,
        "queryDevices": queryDevices,
        "listDevice": listDevice
    };
    var processList = {
        "processOnline": processOnline,
        "processTime": processTime
    };
    var platDT = new SCDataTable($("#platlower-manager-table"),
        {
            "actionList": actionList,
            "processList": processList
        });

    //初始化模态框
    var mfOptions = {
        title: {
            "text": "修改下级域"
        },
        footer: {
            "buttons": [
                {
                    "class": "btn-primary",
                    "text": "保存",
                    "iscancel": false,
                    "action": function (event, modal) {
                        $("#platform-info-form").submit();
                    }
                },
                {
                    "text": "取消",
                    "iscancel": true
                }
            ]
        }
    };
    var mform = new SCModalForm($("#platlower-update-modal-form"), mfOptions);
    getAllLowerPlatforms();

    var dfOptions = {
        core:{
            size:"lg"
        },
        title: {
            "text": "设备列表"
        },
        footer: {
            "buttons": [
                {
                    "text": "关闭",
                    "iscancel": true
                }
            ]
        }
    };
    var dform = new SCModalForm($("#platlower-devices-modal-form"), dfOptions);

    $("#btn-add-platlower").click(function (event) {
        mform.removeData("platformInfo");
        mform.setData("optype", "create").show();
        clearInputs();
    });

    function processOnline(data, propName) {
        if (data[propName] === true)
            return "<span class='text-success'>在线</span>";
        else
            return "<span class='text-danger'>不在线</span>";
    }

    function processTime(data, propName) {
        return moment(data[propName]).format("YYYY-MM-DD HH:mm:ss");
    }

    function clearInputs() {
        $("#platform-info-form").validate().resetForm();
    };

    function setupValidate(validated) {
        $("#platform-info-form").validate({
            rules: {
                "Name": "required",
                "SipNumber": "required",
                "Ip": {
                    required: true,
                    ipaddress: true
                },
                "Port": {
                    required: true,
                    max: 65535
                }
            },
            messages: {
                "Name": "必须输入名称",
                "SipNumber": "必须输入网关编号",
                "Ip": {
                    required: "必须输入IP地址",
                },
                "Port": {
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

    function updateSubmit() {
        var plat = formdataToObject($("#platform-info-form").serializeArray());
        var si = mform.getData("platformInfo");
        var isUpdate = mform.getData("optype") == "update";
        var url = "api/PlatLower";
        var type = "post";
        if (isUpdate) {
            plat.Id = si.Id;
            url += "/" + si.Id;
            type = "put"
        }
        plat.Type = lower;
        //发送数据请求。
        $.ajax({
            url: url,
            type: type,
            data: JSON.stringify(plat),
            contentType: "application/json",
            success: function (data, status, jqXHR) {
                if (isUpdate)
                    platDT.update(si.rowIndex, plat);
                else
                    getAllLowerPlatforms();
                mform.hide();
            },
            error: function (jqXHR, status, err) {
                showErrorResponseDialog("保存平台信息失败", jqXHR, status, err);
            }
        });
    }

    function getAllLowerPlatforms() {
        $.get("api/PlatLower").done(function (data, status, jqXHR) {
            buildManagerTable(data);
            getAllLowerPlatformStatus();
        }).fail(function (jqXHR, status, errThrow) {
            showErrorResponseDialog("平台列表获取失败", jqXHR, status, errThrow);
        });
    }

    function getAllLowerPlatformStatus() {
        $.get("api/PlatLower/status").done(function (data, status, jqXHR) {
            buildManagerTable(data);
        }).fail(function (jqXHR, status, errThrow) {
            showErrorResponseDialog("平台列表获取失败", jqXHR, status, errThrow);
        });
    }

    function buildManagerTable(data) {
        platDT.init(data);
    }

    function editRow(index) {
        var cell = platDT.getData(index);
        //显示模态框
        mform.setData("platformInfo", {
            Id: cell.Id,
            rowIndex: index
        });
        mform.setData("optype", "update").show();
        //初始化表单数据。
        updateFormData($("#platform-info-form"), cell);
    }

    function deleteRow(index) {
        var cell = platDT.getData(index);
        showConfirm("确认删除 " + cell.Name + " ?", "warning", function (result) {
            if (result) {
                $.ajax({
                    url: "api/PlatLower/" + cell.Id,
                    type: "Delete",
                    success: function () {
                        console.info("删除成功");
                        //从表格数据中删除行，并执行刷新操作。
                        platDT.remove(index);
                    },
                    error: function (jqXHR, status, errThrow) {
                        showErrorResponseDialog("删除 " + cell.Name + " 失败", jqXHR, status, errThrow);
                    }
                });
            }
        });


    }

    function queryDevices(index) {
        var cell = platDT.getData(index);
        $.ajax({
            url: "api/PlatLower/Query/" + cell.Id,
            type: "Post",
            success: function (data, status, jqXHR) {
                showDialog(data, "info");
            },
            error: function (jqXHR, status, errThrow) {
                showErrorResponseDialog("查询设备列表失败。", jqXHR, status, errThrow);
            }
        });
    }

    function listDevice(index) {
        var cell = platDT.getData(index);
        DeviceForm.loadInfo(cell.Id);
    }

    var DeviceForm = {
        loadInfo: function (platId) {
            //TODO:加载指定平台的设备列表。
            var that = this;
            $.ajax({
                url: "api/PlatLower/Devices/" + platId,
                type: "Get",
                success: function (data, status, jqXHR) {
                    that.initTree(data);
                    dform.show();
                },
                error: function (jqXHR, status, errThrow) {
                    showErrorResponseDialog("查询设备列表失败。", jqXHR, status, errThrow);
                }
            });
        },
        initTree: function (pfDevSet) {
            //TODO:填充树节点，设置节点选中事件
            var zNodes = createZNodes(pfDevSet && pfDevSet.Items) || [];
            var settings = {
                edit: {
                    enable: false,
                    showRemoveBtn: false,
                    showRenameBtn: false
                },
                callback: {
                    onClick: nodeClick
                }
            };
            //初始化树
            var treeObj = $.fn.zTree.init($("#tree-devicelist"), settings, zNodes);

            function createZNodes(nodes) {
                if (nodes && nodes.length > 0) {
                    var treenodes = []
                    for (var i = 0; i < nodes.length; i++) {
                        var node = nodes[i];
                        var tNode = {
                            "id": node.Id,
                            "name": node.Name,
                            "drop": false,
                            realData: node
                        }
                        treenodes.push(tNode);
                    }
                    return treenodes;
                }
            }

            function nodeClick(event, treeId, treeNode) {
                var vm = {
                    devices: []
                };
                if (treeNode.realData) {
                    var index = 0;
                    var item = {};
                    for (var pName in treeNode.realData) {
                        if (index++ % 2 == 0) {
                            item.prop1 = dictPropName[pName];
                            item.value1 = treeNode.realData[pName];
                        }
                        else {
                            item.prop2 = dictPropName[pName];
                            item.value2 = treeNode.realData[pName];
                            vm.devices.push(item);
                            item = {};
                        }
                    }
                }
                $("#table-deviceinfo").scSimpleTable().init(vm.devices);
                // ko.applyBindings(vm, $("#table-deviceinfo")[0]);
                // var vm = {
                //     devices: [{
                //         "aa": 123,
                //         "bb": 456
                //     }]
                // };
                // ko.applyBinding(vm, $(".table.table-bordered.table-striped")[0]);
            }

            var dictPropName = {
                DeviceID: "设备编码",
                Name: "设备名称",
                Event: "状态事件",
                Manufacturer: "设备厂商",
                Model: "设备型号",
                Owner: "设备归属",
                CivilCode: "行政区域",
                Block: "警区",
                Address: "安装地址",
                Parental: "有无子设备",
                ParentID: "父设备编码",
                SafetyWay: "信令安全模式",
                RegisterWay: "注方册式",
                CertNum: "证书序列号",
                Certifiable: "证书有效标识",
                ErrCode: "无效原因码",
                EndTime: "证书有效期",
                Secrecy: "保密属性",
                IPAddress: "IP 地址",
                Port: "端口",
                Password: "设备口令",
                Status: "设备状态",
                Longitude: "经度",
                Latitude: "纬度"
            };
        },
        bindTableData: function (device) {
            //TODO:将节点数据绑定到Table上。
        }
    };
}

function initGatewaySettingPage() {
    setupValidate(updateSubmit);
    getGatewayInfo();
    function setupValidate(validated) {
        $("#gateway-info-form").validate({
            rules: {
                "SipNumber": "required",
                "Port": {
                    required: true,
                    max: 65535
                }
            },
            messages: {
                "SipNumber": "必须输入网关编号",
                "Port": {
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

    function updateSubmit() {
        var plat = formdataToObject($("#gateway-info-form").serializeArray());
        var url = "api/Gateway";
        var type = "post";
        //发送数据请求。
        $.ajax({
            url: url,
            type: type,
            data: JSON.stringify(plat),
            contentType: "application/json",
            error: function (jqXHR, status, err) {
                showErrorResponseDialog("保存平台信息失败", jqXHR, status, err);
            },
            success: function () {
                showDialog("保存平台信息成功", "info");
            }
        });
    }

    function getGatewayInfo() {
        $.get("api/gateway").done(function (data) {
            if (data) {
                updateFormData($("#gateway-info-form"), data);
                if (data.IsStarted === true) {
                    $("#label-status").removeClass("red").addClass("green").text("正在运行");
                }
                else if (data.IsStarted === false)
                    $("#label-status").removeClass("green").addClass("red").text("已停止");
                else
                    $("#label-status").removeClass("green").addClass("red").text("未知");
            }
        }).fail(function () {
            showDialog("获取网关配置信息失败", "error");
        });
    }
}