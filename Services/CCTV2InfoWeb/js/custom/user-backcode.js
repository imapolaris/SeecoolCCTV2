/**
 * Created by wyj on 2016/7/13.
 */
function initUserManagerPage() {
    //将表单加载到模态框内
    $(".sc-form-container").load("html/form-user-page.html", function (res, status) {
        if (status == "success") {
            UserForm.initForm();
            UserForm.setupUserValidate(userUpdateSubmit);
        }
    });

    //初始化表格对象。
    var actionList = {
        "editRow": editRow,
        "deleteRow": deleteRow
    };
    var processList = {
        "listPrivilege": listPrivilege
    }
    var userDT = new SCDataTable($("#user-manager-table"), {"actionList": actionList, "processList": processList});

    //初始化模态框
    var mfOptions = {
        title: {
            "text": "修改用户信息"
        },
        footer: {
            "buttons": [
                {
                    "class": "btn-primary",
                    "text": "保存",
                    "iscancel": false,
                    "action": function (event, modal) {
                        $("#user-info-form").submit();
                    }
                },
                {
                    "text": "取消",
                    "iscancel": true
                }
            ]
        }
    };
    var mform = new SCModalForm($("#user-update-modal-form"), mfOptions);
    loadAllUsers();
    loadDefaultTree();
    loadPrivileges();


    $("#btn-add-user").click(function (event) {
        mform.removeData("userinfo");
        mform.setData("optype", "create").show();
        UserForm.clearUserInputs();
    });

    function userUpdateSubmit() {
        var uvm = UserForm.getUserData();
        var ui = mform.getData("userinfo");
        var isUpdate = mform.getData("optype") == "update";
        var url = "api/user";
        var type = "post";
        if (isUpdate) {
            url += "/" + ui.userName;
            type = "put"
        }
        //发送数据请求。
        $.ajax({
            url: url,
            type: type,
            data: JSON.stringify(uvm),
            contentType: "application/json",
            success: function (data, status, jqXHR) {
                if (isUpdate)
                    userDT.update(ui.rowIndex, mergeUserData(uvm));
                else
                    loadAllUsers();
                mform.hide();
            },
            error: function (jqXHR, status, err) {
                showErrorResponseDialog("保存用户信息失败", jqXHR, status, err);
            }
        });
    }
    
    function loadDefaultTree() {
        $.get("api/Hierarchy/LogicalTree/Default").done(function (data, status, jqXHR) {
            UserForm.initNodeTree(data);
        }).fail(function (jqXHR, status, err) {
            console.log("获取默认节点树数据失败!", jqXHR, status, err);
        });
    }

    function loadPrivileges() {
        $.get("api/Privilege").done(function (data, status, jqXHR) {
            UserForm.initPrivileges(data);
        }).fail(function (jqXHR, status, err) {
            console.log("获取权限列表失败!", jqXHR, status, err);
        });
    }

    function loadAllUsers() {
        $.get("api/user").done(function (data, status, jqXHR) {
            buildUserManagerTable(data);
        }).fail(function (jqXHR, status, errThrow) {
            showErrorResponseDialog("用户列表获取失败", jqXHR, status, errThrow);
        });
    }

    function buildUserManagerTable(data) {
        var disData = [];
        for (var i = 0; i < data.length; i++) {
            disData.push(mergeUserData(data[i]));
        }
        userDT.init(disData);
    }

    function mergeUserData(userVM) {
        var cData = {};
        copyObject(userVM.User, cData);
        cData.Privileges = userVM.Privilege && userVM.Privilege.Privileges;
        cData.AccessibleNodes = userVM.Privilege && userVM.Privilege.AccessibleNodes;
        return cData;
    }

    function listPrivilege(data, propName) {
        var privNames = [];
        if(data[propName]){
            $(data[propName]).each(function (index,ele) {
                privNames.push(ele.DisplayName);
            })
        }
        return privNames.join(",");
    }

    function editRow(index) {
        var cell = userDT.getData(index);
        //显示模态框
        mform.setData("userinfo", {
            userName: cell.UserName,
            rowIndex: index
        });
        mform.setData("optype", "update").show();
        //初始化表单数据。
        UserForm.clearUserInputs();
        UserForm.updateUserInfo(cell);
        UserForm.updateUserPrivileges(cell.Privileges);
        UserForm.updateUserAccessableNodes(cell.AccessibleNodes);
    }

    function deleteRow(index) {
        var cell = userDT.getData(index);
        showConfirm("确认删除用户 " + cell.UserName + " ?", "warning", function (result) {
            if (result) {
                $.ajax({
                    url: "api/user/" + cell.UserName,
                    type: "Delete",
                    success: function () {
                        console.info("删除成功");
                        //从表格数据中删除行，并执行刷新操作。
                        userDT.remove(index);
                    },
                    error: function (jqXHR, statuc, errThrow) {
                        showErrorResponseDialog("删除用户 " + cell.Name + " 失败", jqXHR, status, errThrow);
                    }
                });
            }
        });
    }
}