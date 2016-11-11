/**
 * Created by wyj on 2016/6/14.
 */
function initFormVideoPageEvent() {
    getAllServers();
    var csCType = new SCChosen($("#controltype"), {
        allowDeselect: true, //可选项。//在单选状态下，是否允许取消当前项选中，默认为false。
        changeHandler: function (value) { //当选中项改变后的回掉方法。 //可选项。
            var sels = csCType.getSelectedValues();
            $(".sc-group-toggle").addClass("hidden");
            switch (sels && sels[0]) {
                case "1":
                    $(".sc-group-dvr").removeClass("hidden");
                    break;
                case "2":
                    $(".sc-group-tdvr").removeClass("hidden");
                    break;
                case "3":
                    $(".sc-group-tcp").removeClass("hidden");
                    break;
            }
        }
    });
    var csServers = new SCChosen($("#PreferredServerId"), {
        allowDeselect: true
    });
    var csSerialType = new SCChosen($("#serialtype"));
    var csDevice = new SCChosen($("#devicetype"), {
        changeHandler: function (selVal) {
            var sv = csDevice.getSelectedValues();
            if (sv && sv.length > 0 && sv[0] == "3") {
                $("#deviceuser").val("root");
                $("#devicepwd").val("pass");
            }
            else {
                $("#deviceuser").val("admin");
                $("#devicepwd").val("12345");
            }
        }
    });
    var csImage = new SCChosen($("#imagetype"), {
        multiple: true
    });

    //初始化开关表。
    var actionList = {
        "editRow": editSwitchInfo,
        "deleteRow": deleteSwitchInfo
    };
    var switchDT = new SCSimpleTable($("#video-switch-table"), {"actionList": actionList});
    $("#video-switch-table").data("switchDT", switchDT);
    $("#video-btn-addswitch").click(videoAddSwitchBtnClick);

    function videoAddSwitchBtnClick() {
        var swInfo = formdataToObject($("#switchinfo-form").serializeArray());
        if (swInfo.Index) {
            var rowIndex = switchDT.exist("Index", swInfo.Index);
            if (rowIndex >= 0) {
                switchDT.update(rowIndex, swInfo);
            }
            else {
                switchDT.add(swInfo);
            }
        }
    }

    function editSwitchInfo(index) {
        var swInfo = switchDT.getData(index);
        updateFormData($("#switchinfo-form"), swInfo);
    }

    function deleteSwitchInfo(index) {
        switchDT.remove(index);
    }

    //初始化码流类型表。
    var actionList = {
        "editRow": editStreamInfo,
        "deleteRow": deleteStreamInfo
    };
    var streamDT = new SCSimpleTable($("#video-stream-table"), {"actionList": actionList});
    $("#video-stream-table").data("streamDT", streamDT);
    $("#video-btn-addstream").click(videoAddStreamBtnClick);

    function videoAddStreamBtnClick() {
        var stream = formdataToObject($("#streaminfo-form").serializeArray());

        if (stream.StreamType == 1)
            stream.StreamTypeName = "主码流";
        else if (stream.StreamType == 2)
            stream.StreamTypeName = "子码流";

        if (stream.Index) {
            var rowIndex = streamDT.exist("Index", stream.Index);
            if (rowIndex >= 0) {
                var oldStream = streamDT.getData(rowIndex);
                stream.Url = oldStream.Url;
                streamDT.update(rowIndex, stream);
            }
            else {
                streamDT.add(stream);
            }
        }
    }

    function editStreamInfo(index) {
        var stream = streamDT.getData(index);
        updateFormData($("#streaminfo-form"), stream);
    }

    function deleteStreamInfo(index) {
        streamDT.remove(index)
    }

    function getAllServers() {
        $.get("api/server").done(function (data, status, jqXHR) {
            updateAvailiableServers(data);
        }).fail(function (jqXHR, status, err) {
            console.error("获取节点服务器列表失败");
        });
    }

    function updateAvailiableServers(data) {
        csServers.update(data, "ServerId", "Name");
    }
}

function rebuildStreamTable(streams) {
    var streamDT = $("#video-stream-table").data("streamDT");
    $(streams).each(function (index, ele) {
        if (ele.StreamType == 1)
            ele.StreamTypeName = "主码流";
        else if (ele.StreamType == 2)
            ele.StreamTypeName = "子码流";
    })
    streamDT.init(streams);
}

function rebuildSwitchTable(switches) {
    var switchDT = $("#video-switch-table").data("switchDT");
    switchDT.init(switches);
}

function getAllStreamInfos() {
    var streamDT = $("#video-stream-table").data("streamDT");
    return streamDT.getAllData();
}

function getAllSwitchInfos() {
    var switchDT = $("#video-switch-table").data("switchDT");
    return switchDT.getAllData();
}

function clearStreamInfos() {
    var streamDT = $("#video-stream-table").data("streamDT");
    streamDT.clear();
}

function clearSwitchInfos() {
    var switchDT = $("#video-switch-table").data("switchDT");
    switchDT.clear();
}

function enumImageTypeToArr(imagetype) {
    var rtns = [];
    if (imagetype) {
        var seed = 1;
        for (var i = 0; i < 8; i++) {
            var v = imagetype & (seed << i);
            if (v)
                rtns.push(v);
        }
    }
    return rtns;
}

function arrImageTypeToEnum(types) {
    var it = 0;
    if (types) {
        for (var i = 0; i < types.length; i++) {
            it = it | types[i];
        }
    }
    return it;
}

function getFullVideoInfo() {
    var staticinfo = formdataToObject($("#baseinfo-form").serializeArray());
    staticinfo.Streams = getAllStreamInfos();
    staticinfo.ImageType = arrImageTypeToEnum($("#baseinfo-form select[name='ImageType']").scChosen().getSelectedValues());
    staticinfo.Platform = $("#baseinfo-form").data("platform");

    var control = formdataToObject($("#control-form").serializeArray());
    var switches = getAllSwitchInfos();
    if (switches && switches.length > 0)
        control.AuxSwitch = switches;

    var deviceinfo = formdataToObject($("#deviceinfo-form").serializeArray());
    // var cameralimits = formdataToObject($("#cameralimits-form").serializeArray());
    var videotrack = formdataToObject($("#videotrack-form").serializeArray());
    var videoanalyze = formdataToObject($("#analyze-form").serializeArray());
    var videoInfo = {};
    videoInfo.StaticInfo = staticinfo;
    if (isValidObject(deviceinfo)) {
        videoInfo.DeviceInfo = deviceinfo;
    }
    if (isValidObject(control))
        videoInfo.Control = control;
    // if (isValidObject(cameralimits))
    //     videoInfo.CameraLimits = cameralimits;
    if (isValidObject(videotrack))
        videoInfo.VideoTrack = videotrack;
    if (isValidObject(videoanalyze))
        videoInfo.VideoAnalyze = videoanalyze;
    return videoInfo;

    function isValidObject(obj) {
        var pNames = Object.getOwnPropertyNames(obj);
        for (var i = 0; i < pNames.length; i++) {
            if (obj[pNames[i]])
                return true;
        }
    }
}

function clearFullVideoInfo() {
    $("#baseinfo-form")[0].reset();
    $("#deviceinfo-form")[0].reset();
    $("#control-form")[0].reset();
    // $("#cameralimits-form")[0].reset();
    $("#videotrack-form")[0].reset();
    $("#analyze-form")[0].reset();
    $("#streaminfo-form")[0].reset();
    $("#switchinfo-form")[0].reset();
    $("#devicetype").scChosen().select(1);
    clearStreamInfos();
    clearSwitchInfos();
}

function updateFullVideoInfo(videoInfo) {
    clearFullVideoInfo();
    updateFormData($("#baseinfo-form"), videoInfo.StaticInfo);
    $("#baseinfo-form").data("platform", videoInfo.StaticInfo.Platform);
    $("#baseinfo-form select[name='ImageType']").scChosen().selectMulti(enumImageTypeToArr(videoInfo.StaticInfo.ImageType));

    updateFormData($("#deviceinfo-form"), videoInfo.DeviceInfo);
    updateFormData($("#control-form"), videoInfo.Control);

    if (videoInfo.StaticInfo && videoInfo.StaticInfo.Streams) {
        rebuildStreamTable(videoInfo.StaticInfo.Streams);
    }
    if (videoInfo.Control && videoInfo.Control.AuxSwitch) {
        rebuildSwitchTable(videoInfo.Control.AuxSwitch);
    }
    // updateFormData($("#cameralimits-form"), videoInfo.CameraLimits);
    updateFormData($("#videotrack-form"), videoInfo.VideoTrack);
    updateFormData($("#analyze-form"), videoInfo.VideoAnalyze);
}

