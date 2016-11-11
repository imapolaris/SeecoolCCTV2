/**
 * Created by wyj on 2016/6/15.
 */
function initLogicalTreePage() {
    var selectedTree;
    var videoMapper;

    var treeSelecter = new SCChosen($("#logicaltree-selection"), {
        "allowDeselect": true,
        "changeHandler": function (value) {
            var datas = treeSelecter.getSelectedDatas()
            selectedTree = datas && datas[0];
            if (selectedTree) {
                reloadLogicalTree(selectedTree.LogicalName);
            }
            else {
                $("#modify-logicaltree-btn").addClass("disabled");
                $.fn.zTree.destroy("logicaltree-node-tree");
            }
            showOrHideDefaultTree(!isDefaultTree());
        }
    });

    //获取逻辑树节点。
    loadLogicalTrees();

    prepareTreeSelection();
    prepareTree();

    function isDefaultTree() {
        return selectedTree && selectedTree.LogicalName == "Default";
    }

    function loadAllVideos() {
        videoMapper == null;
        $.get("api/VideoInfo").done(function (data, status, jqXHR) {
            if (data) {
                videoMapper = {};
                for (var i = 0; i < data.length; i++) {
                    if (data[i].StaticInfo && data[i].StaticInfo.VideoId)
                        videoMapper[data[i].StaticInfo.VideoId] = data[i];
                }
            }
        }).fail(function (jqXHR, status, err) {
            console.error("获取所有视频信息失败。");
        });
    }

    /**
     * 获取节点树列表。
     */
    function loadLogicalTrees() {
        $.get("api/logicaltree").done(function (data, status, jqXHR) {
            treeSelecter.update(data, "LogicalName", "DisplayName");
            treeSelecter.select("Default");
        }).fail(function (jqXHR, status, err) {
            showErrorResponseDialog("获取节点树列表失败!", jqXHR, status, err);
        });
    }

    /**
     * 刷新逻辑节点树(节点分级结构)
     * @param logicalName
     * @param selNodeId 加载完成后，选中的节点。
     */
    function reloadLogicalTree(logicalName, selNodeId) {
        $.get("api/Hierarchy/LogicalTree/" + logicalName).done(function (data, status, jqXHR) {
            updateTreeComponent(data, selNodeId);
            if (logicalName == "Default") {
                updateDefaultTreeCom(data, selNodeId);
            }
        }).fail(function (jqXHR, status, err) {
            showErrorResponseDialog("获取节点树数据失败!", jqXHR, status, err);
        });
        if (logicalName == "Default") {
            loadAllVideos();
        }
    }

    function showOrHideDefaultTree(isShow) {
        if (isShow) {
            $("#default-node-tree").closest(".widget-box").removeClass("hidden");
            $("#add-video-btn").addClass("hidden");
        }
        else {
            $("#default-node-tree").closest(".widget-box").addClass("hidden");
            $("#add-video-btn").removeClass("hidden");
        }
    }

    function prepareTreeSelection() {
        $("#modify-logicaltree-btn").click(modifyLogicalTreeBtnClick);
        $("#add-logicaltree-btn").click(addLogicalTreeBtnClick);
        $("#delete-logicaltree-btn").click(deleteLogicalTreeBtnClick);
        setupLogicalTreeValidate(logicalTreeInputEnsureClick);
        //生成节点树模态框对象。
        var treeModal = new SCModalForm($("#logicaltree-input-modal-form"), {
            title: {
                "text": "添加节点树"
            },
            footer: {
                "buttons": [
                    {
                        "class": "btn-primary",
                        "text": "保存",
                        "iscancel": false,
                        "action": function (event, modal) {
                            $("#logicaltree-input-form").submit();
                        }
                    },
                    {
                        "text": "取消",
                        "iscancel": true
                    }
                ]
            }
        });

        function setupLogicalTreeValidate(validated) {
            $("#logicaltree-input-form").validate({
                rules: {
                    "LogicalName": "required",
                },
                messages: {
                    "LogicalName": "必须输入逻辑名称",
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

        function clearLogicalTreeInputForm() {
            clearFormInputData($("#logicaltree-input-form"));
        }

        function modifyLogicalTreeBtnClick() {
            var selTree = treeSelecter.getSelectedDatas();
            if (selTree) {
                clearLogicalTreeInputForm();
                showLigicalTreeInputModalForm("modify");
                updateFormData($("#logicaltree-input-form"), selTree[0]);
            }
        }

        function addLogicalTreeBtnClick() {
            clearLogicalTreeInputForm();
            showLigicalTreeInputModalForm("add");
        }

        function deleteLogicalTreeBtnClick() {
            var selTree = treeSelecter.getSelectedDatas();
            if (selTree) {
                showConfirm("是否确定删除节点树:" + selTree[0].DisplayName, "warning", function (rst) {
                    if (rst) {
                        $.ajax({
                            url: "api/logicalTree/" + selTree[0].LogicalName,
                            type: "delete",
                            success: function () {
                                loadLogicalTrees();
                            },
                            error: function (jqXHR, status, err) {
                                showErrorResponseDialog("删除失败", jqXHR, status, err);
                            }
                        });
                    }
                });
            }
        }

        function showLigicalTreeInputModalForm(type) {
            treeModal.setData("optype", type);
            treeModal.updateOptions({
                title: {
                    "text": type == "add" ? "添加节点树" : "删除节点树"
                }
            });
            var ln = treeModal.getModal().find("input[name='LogicalName']");
            ln.prop("readonly", false);
            if (type == 'modify')
                ln.prop("readonly", true);
            treeModal.show();
        }

        function logicalTreeInputEnsureClick() {
            var tree = formdataToObject($("#logicaltree-input-form").serializeArray());
            if (tree) {
                var isAdd = treeModal.getData("optype") == "add";
                var url = "api/LogicalTree";
                var method = "Post";
                if (!isAdd) {
                    url += "/" + tree.LogicalName;
                    method = "Put";
                }
                $.ajax({
                    url: url,
                    type: method,
                    data: JSON.stringify(tree),
                    contentType: "application/json",
                    success: function () {
                        treeModal.hide();
                        loadLogicalTrees();
                    },
                    error: function (jqXHR, status, err) {
                        showErrorResponseDialog("修改失败", jqXHR, status, err);
                    }
                });
            }
        }
    }

    function prepareTree() {
        setupServerNodeValidate(doAddOrUpdateServerNode);
        $(".sc-video-form-container").load("html/form-video-page.html", function (data) {
            initFormVideoPageEvent();
        });
        //点击添加管理单位按钮。
        $("#add-server-btn").click(function (event) {
            if (selectedTree == null) {
                showDialog("没有选择节点树。", "warning");
                return;
            }
            //获取选中目标节点。
            var treeObj = $.fn.zTree.getZTreeObj("logicaltree-node-tree");
            if (treeObj) {
                var nodes = treeObj.getSelectedNodes();
                toAddServerNode(nodes && nodes.length > 0 && nodes[0].realData);
            }
        });

        //点击添加视频按钮
        $("#add-video-btn").click(function (evet) {
            if (selectedTree == null) {
                showDialog("没有选择节点树。", "warning");
                return;
            }
            if (!isDefaultTree())
                return;
            //获取选中目标节点。
            var treeObj = $.fn.zTree.getZTreeObj("logicaltree-node-tree");
            if (treeObj) {
                var nodes = treeObj.getSelectedNodes();
                toAddVideoNode(nodes && nodes.length > 0 && nodes[0].realData);
            }
        });
    }

    // /**
    //  * 将服务器类型的树节点封装成Map
    //  * @param data
    //  * @param untilId 截止Id，不记录此Id及其后续子节点Id。
    //  */
    // function updateParentIdSelection(selecter, data, untilId) {
    //     var mapper = [];
    //     if (data && data.length > 0) {
    //         for (var i = 0; i < data.length; i++) {
    //             getLogicalNodeIdNames(mapper, data[i])
    //         }
    //     }
    //     updateSelecterOptions(selecter, mapper, "Id", "Name");
    //
    //     function getLogicalNodeIdNames(mapper, node) {
    //         if (node && node.Type == 1 && node.Id != untilId) {
    //             mapper.push({
    //                 Id: node.Id,
    //                 Name: node.Name
    //             });
    //             if (node.Children && node.Children.length > 0) {
    //                 for (var i = 0; i < node.Children.length; i++) {
    //                     getLogicalNodeIdNames(mapper, node.Children[i])
    //                 }
    //             }
    //         }
    //     }
    // }

    function setupServerNodeValidate(validated) {
        $("#servernode-input-form").validate({
            rules: {
                "Name": "required"
            },
            messages: {
                "Name": "必须输入管理单位名称"
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

    // function setupVideoNodeValidate(validated) {
    //     $("#videonode-input-form").validate({
    //         rules: {
    //             "Name": "required",
    //             "ElementId": "required"
    //         },
    //         messages: {
    //             "Name": "必须输入管理单位名称",
    //             "ElementId": "必须选择关联视频"
    //         },
    //         submitHandler: function (form, event) {
    //             validated && validated();
    //         },
    //         invalidHandler: function (event, validator) {
    //             console.log("未通过验证:" + validator.numberOfInvalids());
    //         },
    //         errorClass: "text-danger"
    //     });
    // }

    var serverModal = new SCModalForm($("#servernode-input-modal-form"), {
        title: {
            "text": "添加管理单位"
        },
        footer: {
            "buttons": [
                {
                    "class": "btn-primary",
                    "text": "保存",
                    "iscancel": false,
                    "action": function (event, modal) {
                        $("#servernode-input-form").submit();
                    }
                },
                {
                    "text": "取消",
                    "iscancel": true
                }
            ]
        }
    });

    var videoModal = new SCModalForm($("#videonode-input-modal-form"), {
        core: {
            size: 'lg',
            animation: true
        },
        title: {
            "text": "添加视频"
        },
        footer: {
            "buttons": [
                {
                    "class": "btn-primary",
                    "text": "保存",
                    "iscancel": false,
                    "action": function (event, modal) {
                        doAddOrUpdateVideoNode();
                    }
                },
                {
                    "text": "取消",
                    "iscancel": true
                }
            ]
        }
    });


    var menu = new SCContextMenu($("#contextmenu"), {
        "items": [
            {
                "text": "添加管理单位",
                "id": "addserver",
                "iconclass": "fa fa-plus blue",
                "action": function (event, data) {
                    toAddServerNode(data.realData);
                }
            },
            {
                "text": "添加视频",
                "id": "addvideo",
                "iconclass": "fa fa-plus blue",
                "action": function (event, data) {
                    toAddVideoNode(data.realData);
                }
            },
            {
                "text": "修改",
                "id": "modifyitem",
                "iconclass": "fa fa-pencil green",
                "action": function (event, data) {
                    if (data && data.realData) {
                        if (data.realData.Type == 2) {
                            toModifyVideoNode(data.realData);
                        }
                        else {
                            toModifyServerNode(data.realData);
                        }
                    }
                }
            },
            {
                "text": "删除",
                "id": "deleteitem",
                "iconclass": "fa fa-trash-o red",
                "action": function (event, data) {
                    if (data && data.realData) {
                        if (data.realData.Type == 2) {
                            toDeleteVideoNode(data.realData);
                        }
                        else {
                            toDeleteServerNode(data.realData);
                        }
                    }
                }
            }
        ]
    });

    function toAddServerNode(selNode) {
        //更新可选父节点列表，并选中当前节点(当前是server时)，或当前节点的父节点。
        // updateParentIdSelection($("#serverparentId-selection"), selTreeNodes, undefined);
        var fData = {"Type": 1};
        if (selNode) {
            fData.ParentId = selNode.Type == 1 ? selNode.Id : selNode.ParentId;
            fData.ParentName = selNode.Type == 1 ? selNode.Name : selNode.ParentName;
        }
        $("#servernode-input-form").validate().resetForm();
        updateFormData($("#servernode-input-form"), fData);
        serverModal.setData("formtype", "add");
        serverModal.show();
    }

    function toModifyServerNode(selNode) {
        $("#servernode-input-form").validate().resetForm();
        updateFormData($("#servernode-input-form"), selNode);
        serverModal.setData("formtype", "update");
        serverModal.show();
    }

    function toDeleteServerNode(selNode) {
        var tip = "确定删除管理单位: " + selNode.Name + " 及其子节点?";
        showConfirm(tip, "warning", function (rst) {
            if (rst) {
                var url = "api/Hierarchy/" + selNode.Id + "/" + selectedTree.LogicalName;
                $.ajax({
                    url: url,
                    type: "Delete",
                    contentType: "application/json",
                    success: function () {
                        reloadLogicalTree(selectedTree.LogicalName, selNode.ParentId);
                    },
                    error: function (jqXHR, status, err) {
                        showErrorResponseDialog("删除管理单位失败!", jqXHR, status, err);
                    }
                });
            }
        });
    }

    function doAddOrUpdateServerNode() {
        var node = formdataToObject($("#servernode-input-form").serializeArray());
        node.Type = 1;
        delete node.ParentName;
        addOrUpdateTreeNode([node]);
        serverModal.hide();
    }

    function toAddVideoNode(selNode) {
        clearFullVideoInfo();
        videoModal.setData("parentid", selNode.Type == 1 ? selNode.Id : selNode.ParentId);
        videoModal.setData("nodeid", undefined);
        videoModal.setData("formtype", "add");
        videoModal.show();
    }

    function toModifyVideoNode(selNode) {
        if (videoMapper) {
            var videoInfo = videoMapper[selNode.ElementId];
            if (videoInfo) {
                updateFullVideoInfo(videoInfo);
                videoModal.setData("parentid", selNode.ParentId);
                videoModal.setData("nodeid", selNode.Id);
                videoModal.setData("videoId", selNode.ElementId);
                videoModal.setData("formtype", "update");
                videoModal.show();
            }
        }
    }

    function toDeleteVideoNode(selNode) {
        var tip = "确定删除视频: " + selNode.Name + " ?";
        showConfirm(tip, "warning", function (rst) {
            if (rst) {
                if (isDefaultTree()) {
                    var data = {
                        NodeId: selNode.Id,
                        VideoId: selNode.ElementId
                    };
                    $.ajax({
                        url: "api/VideoInfoNode/Delete",
                        type: "Post",
                        data: JSON.stringify(data),
                        contentType: "application/json",
                        success: function () {
                            reloadLogicalTree(selectedTree.LogicalName, selNode.ParentId);
                        },
                        error: function (jqXHR, status, err) {
                            showErrorResponseDialog("删除视频失败!", jqXHR, status, err);
                        }
                    });
                }
                else {
                    var url = "api/Hierarchy/" + selNode.Id + "/" + selectedTree.LogicalName;
                    $.ajax({
                        url: url,
                        type: "Delete",
                        contentType: "application/json",
                        success: function () {
                            reloadLogicalTree(selectedTree.LogicalName, selNode.ParentId);
                        },
                        error: function (jqXHR, status, err) {
                            showErrorResponseDialog("删除视频逻辑节点失败!", jqXHR, status, err);
                        }
                    });
                }
            }
        });
    }

    function doAddOrUpdateVideoNode() {
        var vinvm = {};
        vinvm.VideoInfo = getFullVideoInfo();
        vinvm.NodeId = videoModal.getData("nodeid");
        vinvm.ParentId = videoModal.getData("parentid");
        if (videoModal.getData("formtype") == "update") {
            modifyVideoInfo(videoModal.getData("videoId"), vinvm);
        }
        else {
            addVideoInfo(vinvm);
        }
        videoModal.hide();
    }

    function addVideoInfo(videoInfoNode) {
        var url = "api/VideoInfoNode";
        $.ajax({
            url: url,
            type: "Post",
            data: JSON.stringify(videoInfoNode),
            contentType: "application/json",
            success: function () {
                reloadLogicalTree(selectedTree.LogicalName, videoInfoNode.NodeId || videoInfoNode.ParentId);
            },
            error: function (jqXHR, status, err) {
                showErrorResponseDialog("保存逻辑树节点信息失败。", jqXHR, status, err);
            }
        });
    }

    function modifyVideoInfo(videoId, videoInfoNode) {
        var url = "api/VideoInfoNode/" + videoId;
        $.ajax({
            url: url,
            type: "Put",
            data: JSON.stringify(videoInfoNode),
            contentType: "application/json",
            success: function () {
                reloadLogicalTree(selectedTree.LogicalName, videoInfoNode.NodeId || videoInfoNode.ParentId);
            },
            error: function (jqXHR, status, err) {
                showErrorResponseDialog("修改逻辑树节点信息失败。", jqXHR, status, err);
            }
        });
    }

    function addOrUpdateTreeNode(nodes) {
        //添加或更新逻辑节点。
        $.ajax({
            url: "api/Hierarchies/" + selectedTree.LogicalName,
            type: "Post",
            data: JSON.stringify(nodes),
            contentType: "application/json",
            success: function () {
                reloadLogicalTree(selectedTree.LogicalName, nodes[0].Id || nodes[0].ParentId);
            },
            error: function (jqXHR, status, err) {
                showErrorResponseDialog("保存逻辑树节点信息失败。", jqXHR, status, err);
            }
        });
    }

    function toDeleteTreeNode(node) {
        var tip = "确定删除节点 " + node.Name;
        if (node.Type != 2)
            tip += " 及其子节点";
        tip += "?";
        showConfirm(tip, "warning", function (rst) {
            if (rst) {
                var url = "api/Hierarchy/" + node.Id + "/" + selectedTree.LogicalName;
                $.ajax({
                    url: url,
                    type: "Delete",
                    contentType: "application/json",
                    success: function () {
                        reloadLogicalTree(selectedTree.LogicalName, node.ParentId);
                    },
                    error: function (jqXHR, status, err) {
                        showErrorResponseDialog("删除节点失败!", jqXHR, status, err);
                    }
                });
            }
        });
    }

    function updateTreeComponent(rootNodes, selNodeId) {
        //从dom中移除旧节点并重新添加。
        var zNodes = createZNodes(rootNodes) || [];
        var settings = {
            edit: {
                enable: true,
                showRemoveBtn: false,
                showRenameBtn: false
            },
            callback: {
                beforeDrop: beforeDrop,
                onDrop: onDrop,
                onRightClick: onNodeRightClick
            },
            keep: {
                parent: true
            }
        }

        //初始化树
        var treeObj = $.fn.zTree.init($("#logicaltree-node-tree"), settings, zNodes);
        //选中目标节点。
        var selNode = treeObj.getNodeByParam("id", selNodeId);
        if (selNode) {
            treeObj.selectNode(selNode, false, false);
            treeObj.expandNode(selNode, true, false, true, true);
        }

        function beforeDrop(treeId, treeNodes, targetNode, moveType) {
            if (treeId != "logicaltree-node-tree") {
                return false;
            }
            if (targetNode) {
                if (targetNode.drop === false && moveType === "inner")
                    return false;
                //判断目标节点是否是当前结点的子节点。
                var isChild = false;
                for (var i = 0; i < treeNodes.length; i++) {
                    isChild |= checkIsChild(treeNodes[i], targetNode.id);
                }
                return !isChild;
            }
            return true;
        }

        function onDrop(event, treeId, treeNodes, targetNode, moveType, isCopy) {
            if (targetNode) {
                var pid;
                if (moveType === "next" || moveType === "prev")
                    pid = targetNode.realData.ParentId;
                else if (moveType === "inner")
                    pid = targetNode.realData.Id;

                var hInfos = [];
                for (var i = 0; i < treeNodes.length; i++) {
                    var realData = treeNodes[i].realData;
                    var hInfo = {
                        Id: realData.Id,
                        Name: realData.Name,
                        Type: realData.Type,
                        ElementId: realData.ElementId,
                        ParentId: pid
                    };
                    hInfos.push(hInfo);
                }
                //更新节点信息。
                addOrUpdateTreeNode(hInfos);
            }
        }

        function onNodeRightClick(event, treeId, treeNode) {
            if (treeNode) {
                var disableIds = [];
                if (!isDefaultTree())
                    disableIds = ["addvideo"];
                //从CCTV1适配过来的视频节点信息不能修改。
                if (treeNode.realData && treeNode.realData.Type != 1) {
                    var videoInfo = videoMapper[treeNode.realData.ElementId];
                    if (videoInfo && videoInfo.StaticInfo.Platform == 0)
                        disableIds.push("modifyitem");
                }
                menu.show(event, treeNode, disableIds);
            }
        }

        function checkIsChild(dNode, childId) {
            if (dNode.Id == childId)
                return true;
            if (dNode.Children && dNode.Children.length > 0) {
                for (var i = 0; i < dNode.Children.length; i++) {
                    var isChild = checkIsChild(dNode.Children[i], childId);
                    if (isChild)
                        return true;
                }
            }
            return false;
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

    function updateDefaultTreeCom(rootNodes, selNodeId) {
        //从dom中移除旧节点并重新添加。
        var zNodes = createZNodes(rootNodes) || [];
        var settings = {
            edit: {
                enable: true,
                showRemoveBtn: false,
                showRenameBtn: false,
                drag: {
                    isMove: false
                }
            },
            callback: {
                beforeDrop: beforeDrop,
                onDrop: onDrop,
            }
        }
        //初始化树
        var treeObj = $.fn.zTree.init($("#default-node-tree"), settings, zNodes);
        //选中目标节点。
        var selNode = treeObj.getNodeByParam("id", selNodeId);
        if (selNode) {
            treeObj.selectNode(selNode, false, false);
            treeObj.expandNode(selNode, true, false, true, true);
        }

        function beforeDrop(treeId, treeNodes, targetNode, moveType) {
            if (treeId == "default-node-tree") {
                return false;
            }
            if (targetNode && targetNode.drop === false && moveType === "inner") {
                return false;
            }
            return true;
        }

        function onDrop(event, treeId, treeNodes, targetNode, moveType, isCopy) {
            if (targetNode) {
                var pid;
                if (moveType === "next" || moveType === "prev")
                    pid = targetNode.realData.ParentId;
                else if (moveType === "inner")
                    pid = targetNode.realData.Id;

                var hInfos = [];
                buildHInfos(treeNodes, pid, hInfos);
                //更新节点信息。
                addOrUpdateTreeNode(hInfos);
            }
            function buildHInfos(dNodes, parentId, desArr) {
                for (var i = 0; i < dNodes.length; i++) {
                    var realData = dNodes[i].realData;
                    var hInfo = {
                        Id: createGuid(),
                        Name: realData.Name,
                        Type: realData.Type,
                        ElementId: realData.ElementId,
                        ParentId: parentId
                    };
                    desArr.push(hInfo);
                    if (dNodes[i].children)
                        buildHInfos(dNodes[i].children, hInfo.Id, desArr);
                }
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
}