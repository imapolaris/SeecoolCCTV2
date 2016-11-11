/**
 * Created by wyj on 2016/5/31.
 */
var DataSourceTree = function (options) {
    this._data = options.data;
    this._delay = options.delay;
}

DataSourceTree.prototype.data = function (options, callback) {
    var self = this;
    var $data = null;

    if (!("name" in options) && !("type" in options)) {
        $data = this._data;//the root tree
        callback({data: $data});
        return;
    }
    else if ("type" in options && options.type == "folder") {
        if ("additionalParameters" in options && "children" in options.additionalParameters)
            $data = options.additionalParameters.children;
        else $data = {}//no data
    }

    if ($data != null)//this setTimeout is only for mimicking some random delay
        setTimeout(function () {
            callback({data: $data});
        }, parseInt(Math.random() * 500) + 200);

    //we have used static data here
    //but you can retrieve your data dynamically from a server using ajax call
    //checkout examples/treeview.html and examples/treeview.js for more info
};

function genTreeData(node) {
    // return new DataSourceTree({data: createHierarchy(node)});
    return new DataSourceTree({data: createTreeData(new Array(node))});
}

function createTreeData(nodes) {
    var data = {};
    for (var i = 0; i < nodes.length; i++) {
        var ele = nodes[i];
        data[ele.VideoId] = {
            name: ele.VideoName,
            type: "item",
            additionalParameters:{
                'item-selected': ele.StorageOn
            },
            cctvid: ele.VideoId
        };
        console.info(ele.VideoName+"__"+ele.VideoId);
        if (ele.Children.length > 0) {
            var sub = data[ele.VideoId];
            sub.additionalParameters.children = createTreeData(ele.Children);
            sub.type = "folder";
        }
    }
    return data;
}
