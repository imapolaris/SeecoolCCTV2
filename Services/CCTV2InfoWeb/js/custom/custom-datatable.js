if (!$.fn.scDataTable) {
    $.fn.scDataTable = function (options) {
        var table = $(this).data("scDataTable");
        if (table) {
            if (options) {
                table.destroy();
                table = new SCDataTable(this, options);
            }
        }
        else {
            table = new SCDataTable(this, options);
        }
        return table;
    }
}

if (!$.fn.scSimpleTable) {
    $.fn.scSimpleTable = function (options) {
        var table = $(this).data("scSimpleTable");
        if (table) {
            if (options) {
                table.destroy();
                table = new SCSimpleTable(this, options);
            }
        }
        else {
            table = new SCSimpleTable(this, options);
        }
        return table;
    }
}
/**
 *
 * @param dt table元素的jquery对象。
 * @param options 格式如下{
 *      autoWidth: //是否自动列宽。默认值为false。
 *      actionList: //单元格的点击事件列表。
 *      processList：//单元格值的预处理函数列表。
 * }
 * @constructor
 */
function SCDataTable(dt, options) {
    dt = $(dt);
    this._domTable = dt;
    this._dt = dt.DataTable({
        "autoWidth": options && (options.autoWidth === true),
        "aaSorting": [],
        "language": {
            "lengthMenu": "每页显示 _MENU_ 行",
            "zeroRecords": "未找到任何项 - 抱歉!",
            // "info": "当前 _PAGE_ / _PAGES_ 页",
            "info": "显示 _START_ 到 _END_ 共 _TOTAL_ 条",
            "infoEmpty": "没有可用的信息项",
            "infoFiltered": "(从 _MAX_ 记录中过滤信息)",
            "search": "搜索",
            "paginate": {
                "first": "首页",
                "last": "末页",
                "previous": "上一页",
                "next": "下一页"
            }
        }
        // "dom": "<'row'<'col-sm-6'l><'col-sm-6'f>>" +
        // "<'row'<'col-sm-12'tr>>" +
        // "<'row'<'col-sm-3'l><'col-sm-3'i><'col-sm-6'p>>"
    });
    this._actionList = options && options.actionList;
    this._processList = options && options.processList;

    this._rowTemplate = dt[0].children[1].children[0];

    var order = this._dt.order();
    //alert( 'Column '+order[0][0]+' is the ordering column' );

    this._dt.row(0).remove().draw();
    this._domTable.data("scDataTable", this);
}

SCDataTable.prototype._fill = function (template, data, $tr) {
    //如果$tr未定义，该方法的作用是创建新行,否则只修改原有行的内容。
    if (!$tr) {
        $tr = $("<tr>");
    }
    else {
        $tr.empty();
    }

    var re = /{{.+}}/gi;
    var exps = template.match(re);
    exps.forEach(function (exp) {
        var prop = exp.substring(2, exp.length - 2);
        //判断是否需要对数据值进行预处理
        var sIndex = prop.indexOf(":");
        var cellValue = data[prop] === undefined ? '' : data[prop];
        if (sIndex > 0 && sIndex < prop.length - 1) {
            var pName = prop.substring(0, sIndex);
            var pProcess = prop.substring(sIndex + 1);
            if (pProcess && this._processList && this._processList[pProcess]) {
                cellValue = this._processList[pProcess](data, pName);
            }
            else {
                cellValue = data[pName];
            }
        }
        template = template.replace('{{' + prop + '}}', cellValue);
    }.bind(this));

    //填充数据行内容。
    $tr.append($(template));
    $tr.data("rowData", data);
    var that = this;
    //为所有属于.table-click-action切包含data-clickaction属性的元素安装click事件
    //data-clickaction属性用于标记回掉方法名。
    $tr.find(".sc-datatable-action[data-clickaction]").click(function () {
        var actionName = $(this).attr("data-clickaction");
        if (actionName && that._actionList && that._actionList[actionName]) {
            var cIndex = that._dt.row($(this).closest("tr")).index();
            that._actionList[actionName](cIndex);
        }
    })
    return $tr;
    // return template;
}

SCDataTable.prototype.clear = function () {
    this.init([]);
}

SCDataTable.prototype.eachRow = function (callback) {
    if (callback) {
        var that = this;
        this._dt.rows().every(function (index, tableLoop, rowLoop) {
            var row = that._dt.row(index);
            if (row) {
                callback(index, $(row.node));
            }
        });
    }
}

SCDataTable.prototype.getFilteredIndices = function (filter) {
    var indices = [];
    if (filter) {
        var that = this;
        this._dt.rows().every(function (index, tableLoop, rowLoop) {
            var row = that._dt.row(index);
            if (row && filter($(row.node), $(row.node).data("rowData"))) {
                indices.push(index);
            }
        });
    }
    return indices;
}

SCDataTable.prototype.getAllData = function () {
    var datas = [];
    var that = this;
    this._dt.rows().every(function (index, tableLoop, rowLoop) {
        var row = that._dt.row(index);
        if (row) {
            var data = $(row.node).data("rowData");
            if (data)
                datas.push(data);
        }
    });
    return datas;
}

SCDataTable.prototype.getData = function (index) {
    var row = this._dt.row(index);
    if (row) {
        return $(row.node()).data("rowData");
    }
}

SCDataTable.prototype.init = function (data) {
    this._dt.clear();
    data.forEach(function (item) {
        var tr = this._fill(this._rowTemplate.innerHTML, item);
        this._dt.row.add(tr);
    }.bind(this))
    this._dt.draw();
}

SCDataTable.prototype.add = function (data) {
    var tr = this._fill(this._rowTemplate.innerHTML, data);
    var row = this._dt.row.add(tr).draw();
}

SCDataTable.prototype.remove = function (index) {
    var row = this._dt.row(index);
    if (row) {
        row.remove().draw();
    }
}

SCDataTable.prototype.update = function (index, data) {
    var row = this._dt.row(index);
    if (row) {
        var tr = row.node();
        this._fill(this._rowTemplate.innerHTML, data, $(tr));
    }
}

SCDataTable.prototype.destroy = function () {
    this._dt.destroy();
    this._domTable.find("tbody").empty().append($(this._rowTemplate));
    this._domTable.removeData("scDataTable");
}

//========================================//
//以下为简单表格类型
//========================================//
function SCSimpleTable($dt, options) {
    this._domTable = $dt;
    this._tbody = $($dt.find("tbody"));
    this._actionList = options && options.actionList;
    this._processList = options && options.processList;
    this._rowTemplate = $dt[0].children[1].children[0];
    this._index = 0;
    this._dataMap = {};
    $(this._rowTemplate).remove();
    this._domTable.data("scSimpleTable", this);
}

SCSimpleTable.prototype._fill = function (template, data, $tr) {
    //如果$tr未定义，该方法的作用是创建新行,否则只修改原有行的内容。
    var index = -1;
    if (!$tr) {
        $tr = $("<tr>");
        index = this._index++;
    }
    else {
        $tr.empty();
        index = $tr.data("rowData").index;
    }
    this._dataMap[index] = {
        "index": index,
        "data": data,
        "tableRow": $tr
    };

    var re = /{{.+}}/gi;
    var exps = template.match(re);
    exps.forEach(function (exp) {
        var prop = exp.substring(2, exp.length - 2);
        //判断是否需要对数据值进行预处理
        var sIndex = prop.indexOf(":");
        var cellValue = data[prop] === undefined ? '' : data[prop];
        if (sIndex > 0 && sIndex < prop.length - 1) {
            var pName = prop.substring(0, sIndex);
            var pProcess = prop.substring(sIndex + 1);
            if (pProcess && this._processList && this._processList[pProcess]) {
                cellValue = this._processList[pProcess](data, pName);
            }
            else {
                cellValue = data[pName];
            }
        }
        template = template.replace('{{' + prop + '}}', cellValue);
    }.bind(this));

    //填充数据行内容。
    $tr.append($(template));
    $tr.data("rowData", {
        "data": data,
        "index": index
    });
    var that = this;
    //为所有属于.table-click-action切包含data-clickaction属性的元素安装click事件
    //data-clickaction属性用于标记回掉方法名。
    $tr.find(".sc-datatable-action[data-clickaction]").click(function () {
        var actionName = $(this).attr("data-clickaction");
        if (actionName && that._actionList && that._actionList[actionName]) {
            var cIndex = $(this).closest("tr").data("rowData").index;
            that._actionList[actionName](cIndex);
        }
    })
    return $tr;
    // return template;
}


SCSimpleTable.prototype.exist = function (key, value) {
    for (var pname in this._dataMap) {
        var temp = this._dataMap[pname];
        if (temp && temp.data[key] == value)
            return temp.index;
    }
    return -1;
}

SCSimpleTable.prototype.clear = function () {
    this.init([]);
}

SCSimpleTable.prototype.eachRow = function (callback) {
    if (callback) {
        for (var pname in this._dataMap) {
            var temp = this._dataMap[pname];
            if (temp) {
                callback(temp.index, temp.tableRow);
            }
        }
    }
}

SCSimpleTable.prototype.getAllData = function () {
    var datas = [];
    for (var pname in this._dataMap) {
        var temp = this._dataMap[pname];
        if (temp) {
            datas.push(temp.data);
        }
    }
    return datas;
}

SCSimpleTable.prototype.getData = function (index) {
    var row = this._dataMap[index + ""];
    if (row) {
        return row.data;
    }
}

SCSimpleTable.prototype.getFilteredIndices = function (filter) {
    var indices = [];
    if (filter) {
        for (var pname in this._dataMap) {
            var temp = this._dataMap[pname];
            if (temp && filter(temp.tableRow, temp.data)) {
                indices.push(pname);
            }
        }
    }
    return indices;
}

SCSimpleTable.prototype.init = function (data) {
    this._tbody.empty();
    this._dataMap = [];
    this._index = 0;
    data.forEach(function (item) {
        var tr = this._fill(this._rowTemplate.innerHTML, item);
        this._tbody.append(tr);
    }.bind(this))
}

SCSimpleTable.prototype.add = function (data) {
    var tr = this._fill(this._rowTemplate.innerHTML, data);
    this._tbody.append(tr);
}

SCSimpleTable.prototype.remove = function (index) {
    var row = this._dataMap[index + ""];
    if (row) {
        row.tableRow.remove();
        delete this._dataMap[index + ""];
    }
}

SCSimpleTable.prototype.update = function (index, data) {
    var row = this._dataMap[index + ""];
    if (row) {
        var tr = row.tableRow;
        this._fill(this._rowTemplate.innerHTML, data, tr);
    }
}

SCSimpleTable.prototype.destroy = function () {
    this._tbody.empty().append($(this._rowTemplate));
    this._domTable.removeData("scSimpleTable");
    this._dataMap = null;
}