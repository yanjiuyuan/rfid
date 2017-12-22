
function getLocalObj(name) {
    return JSON.parse(localStorage.getItem(name))
}

function setLocalObj(name, obj) {
    localStorage.setItem(name,JSON.stringify(obj))
}

function logout() {
    localStorage.clear()
    location.reload()
}

function loadPage(url) {
    $("#tempPage").load(url)
}

function loadHtml(parentId,childId) {
    $("#" + parentId).html('')
    $("#" + parentId).append($("#" + childId))
}

function _cloneObj(obj) {
    var newObj = {}
    for (var o in obj) {
        newObj[o]=obj[o]
    }
    return newObj
}

function isArray(o) {
    return Object.prototype.toString.call(o) == '[object Array]';
}
//时间选择器插件参数
var pickerOptions = {
    shortcuts: [{
        text: '最近一周',
        onClick(picker) {
            const end = new Date();
            const start = new Date();
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
            picker.$emit('pick', [start, end]);
        }
    }, {
        text: '最近一个月',
        onClick(picker) {
            const end = new Date();
            const start = new Date();
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
            picker.$emit('pick', [start, end]);
        }
    }, {
        text: '最近三个月',
        onClick(picker) {
            const end = new Date();
            const start = new Date();
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
            picker.$emit('pick', [start, end]);
        }
    }]
}
//实例总参数
var mixin = {
    data: {
        user: {},
        pickerOptions: pickerOptions
    },
    methods: {

    }
}

var tableData = [{
    date: '2017-05-03',
    name: '王小虎',
    address: '上海市'
}, {
    date: '2016-05-02',
    name: '李小龍',
    address: '美國'
}, {
    date: '2016-05-04',
    name: '趙小剛',
    address: '北京市'
}, {
    date: '2016-05-01',
    name: '老王',
    address: '上海'
}, {
    date: '2016-05-08',
    name: '小羅',
    address: '荊州'
}, {
    date: '2016-05-06',
    name: '朱迪',
    address: '江蘇'
}, {
    date: '2018-05-07',
    name: '小明',
    address: '浙江'
}, {
    date: '2017-05-07',
    name: '小明',
    address: '浙江'
}]

var menbers = [
    {
        id: 1,
        name:'黄龙贤'
    },
    {
        id: 2,
        name: '蔡兴桐'
    },
    {
        id: 3,
        name: '黄浩炜'
    },
    {
        id: 4,
        name: '肖民生'
    },
    {
        id: 5,
        name: '熊肖'
    },
    {
        id: 6,
        name: '袁观福'
    }, {
        id: 7,
        name: '张鹏辉'
    }, {
        id: 8,
        name: '石威'
    }
]

var approval_list = []

var approval_type = [
    {
        id: 1,
        name: '办公用品',
        approvers:'7|1'
    },
    {
        id: 2,
        name: '绩效报表',
        approvers:'1'
    }
]