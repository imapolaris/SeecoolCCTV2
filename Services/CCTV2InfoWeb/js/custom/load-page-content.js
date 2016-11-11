/**
 * Created by wyj on 2016/6/6.
 */
$(function ($) {
    var loginInfo = $.cookie("loginUserInfo");
    if (loginInfo) {
        window.loginUser = JSON.parse(loginInfo);
        $(".user-info .login-username").text(window.loginUser.ChineseName);
        if (window.loginUser.IsAdmin) {
            $(".sc-adminonly").removeClass("hidden");
        }
        else {
            $(".sc-adminonly").addClass("hidden");
        }
    }
    else {
        window.location.href = "login.html";
    }
    if ("onhashchange" in window) {
        window.onhashchange = function () {
            loadSubPage(location.hash);
        }
    }
    else {
        $("a.linkaction[data-page]").click(function (event) {
            loadSubPage($(this).attr("href"));
        });
    }

    loadSubPage(location.hash);  //加载子页面。

    function loadSubPage(hash) {
        if (hash) {
            var page;  //页面
            var afterload; //加载后处理方法。
            if (hash.indexOf("#mgserver") == 0) {
                page = $("a[href='#mgserver']").attr("data-page");
                afterload = initServerManagerPage;
            }
            else if (hash.indexOf("#mgtree") == 0) {
                page = $("a[href='#mgtree']").attr("data-page");
                afterload = initLogicalTreePage;
            }
            else if (hash.indexOf("#uimanager") == 0) {
                page = $("a[href='#uimanager']").attr("data-page");
                afterload = initUserManagerPage;
            }
            // else if (hash.indexOf("#alarmstatus") == 0) {
            //     page = $("a[href='#alarmstatus']").attr("data-page");
            //     afterload = initAlarmPage;
            // }
            // else if (hash.indexOf("#bufang") == 0) {
            //     page = $("a[href='#bufang']").attr("data-page");
            //     afterload = initBufangPage;
            // }
            // else if (hash.indexOf("#chefang") == 0) {
            //     page = $("a[href='#chefang']").attr("data-page");
            //     afterload = initChefangPage;
            // }
            else if (hash.indexOf("#mggateway") == 0) {
                page = $("a[href='#mggateway']").attr("data-page");
                afterload = initGatewaySettingPage;
            }
            else if (hash.indexOf("#mgplatsuper") == 0) {
                page = $("a[href='#mgplatsuper']").attr("data-page");
                afterload = initPlatSuperManagerPage;
            }
            else if (hash.indexOf("#mgplatlower") == 0) {
                page = $("a[href='#mgplatlower']").attr("data-page");
                afterload = initPlatLowerManagerPage;
            }
            //加载
            $("#shared-content-div").load(page, function () {
                afterload();
            });
        }
    }

    $("#menu-logout").click(function () {
        $.cookie("loginUserInfo", null);
        window.location.href = "login.html";
    });
    // var model = {
    //     videoName: ko.observable("ceshi111")
    // };
    // ko.applyBindings(model, $(".page-content")[0]);
    //
    // window.setTimeout(function () {
    //     console.info(model.videoName);
    // },10000);
});
