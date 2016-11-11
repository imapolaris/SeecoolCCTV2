/**
 * Created by wyj on 2016/7/22.
 */
var UserForm = {
    initForm: function () {
        $("#privilege-table").scSimpleTable();
        $("#privilege-table").find("thead input:checkbox").change(function (param) {
            var ckChecked = $(this).prop("checked");
            $("#privilege-table").scSimpleTable().eachRow(function (index, row) {
                row.find("input:checkbox").prop("checked", ckChecked);
            });
        });
    },
    clearUserInputs: function () {
        $("#user-info-form")[0].reset();
        UserForm.updateUserPrivileges(null);
        UserForm.updateUserAccessableNodes(null);
    },
    setupUserValidate: function (validated) {
        $("#user-info-form").validate({
            rules: {
                "UserName": "required",
            },
            messages: {
                "UserName": "必须输入用户名",
            },
            submitHandler: function (form, event) {
                validated && validated();
            },
            invalidHandler: function (event, validator) {
                console.log("未通过验证:" + validator.numberOfInvalids());
            },
            errorClass: "text-danger"
        });
    },
    getUserData: function () {
        var ui = formdataToObject($("#user-info-form").serializeArray());
        var priv = [];
        var dt = $("#privilege-table").scSimpleTable();
        dt.eachRow(function (index, row) {
            if (row.find("input:checkbox").prop("checked")) {
                priv.push(dt.getData(index));
            }
        });
        var aNodeId = [];
        var tree = $.fn.zTree.getZTreeObj("logical-node-hierarchy");
        if (tree) {
            var cNodes = tree.getCheckedNodes(true);
            if (cNodes) {
                $(cNodes).each(function (index, ele) {
                    if (!ele.getCheckStatus().half)
                        aNodeId.push(ele.id);
                });
            }
        }
        return {
            User: ui,
            Privilege: {
                UserName: ui.UserName,
                Privileges: priv,
                AccessibleNodes: aNodeId
            }
        };
    },
    updateUserInfo: function (user) {
        updateFormData($("#user-info-form"), user);
    },
    updateUserPrivileges: function (privs) {
        var privNames = {};
        if (privs) {
            for (var i = 0; i < privs.length; i++) {
                privNames[privs[i].Name] = true;
            }
        }
        var dt = $("#privilege-table").scSimpleTable();
        dt.getFilteredIndices(function (row, data) {
            row.find("input:checkbox").prop("checked", privNames[data.Name] != undefined);
        });
    },
    updateUserAccessableNodes: function (nodes) {
        var tree = $.fn.zTree.getZTreeObj("logical-node-hierarchy");
        tree.checkAllNodes(false);
        if (nodes) {
            for (var i = 0; i < nodes.length; i++) {
                var nd = tree.getNodeByParam("id", nodes[i]);
                if (nd) {
                    tree.checkNode(nd, true, true, false);
                }
            }
        }
    },
    initPrivileges: function (privileges) {
        $("#privilege-table").scSimpleTable().init(privileges);
    },
    initNodeTree: function (rootNodes) {
        //从dom中移除旧节点并重新添加。
        var zNodes = createZNodes(rootNodes) || [];
        var settings = {
            check: {
                enable: true,
                chkboxType: {"Y": "ps", "N": "ps"}
            }
        }
        //初始化树
        var treeObj = $.fn.zTree.init($("#logical-node-hierarchy"), settings, zNodes);
        //如果根节点的个数为一个，则展开他。
        if (rootNodes.length == 1) {
            var selNode = treeObj.getNodeByParam("id", rootNodes[0].Id);
            if (selNode) {
                treeObj.expandNode(selNode, true, false, true, true);
            }
        }

        function createZNodes(nodes, parentId, parentName) {
            if (nodes && nodes.length > 0) {
                var treenodes = []
                for (var i = 0; i < nodes.length; i++) {
                    var node = nodes[i];
                    if (parentId) {
                        node.ParentId = parentId;
                        node.ParentName = parentName;
                    }
                    var tNode = {
                        "id": node.Id,
                        "name": node.Name,
                        "drop": false,
                        realData: node
                    }
                    if (node.Type && node.Type != 2) {
                        tNode.isParent = true; //表示是枝节点。
                    }
                    else {
                        tNode.drop = false; //叶子节点不支持拖放操作。
                    }

                    if (node.Children && node.Children.length > 0) {
                        tNode.children = createZNodes(node.Children, node.Id, node.Name);
                    }
                    treenodes.push(tNode);
                }
                return treenodes;
            }
        }
    }
};