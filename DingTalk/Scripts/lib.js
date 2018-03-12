
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
    name: '办公用品1',
    address: '笔记本'
}, {
    date: '2016-05-02',
    name: '办公用品2',
    address: '签字笔'
}, {
    date: '2016-05-04',
    name: '办公用品3',
    address: 'A4纸，透明胶'
}, {
    date: '2016-05-01',
    name: '用车1',
    address: '去基地'
}, {
    date: '2016-05-08',
    name: '用车2',
    address: '去软件园'
}, {
    date: '2016-05-06',
    name: '办公用品4',
    address: '垃圾桶'
}, {
    date: '2018-05-07',
    name: '加班1',
    address: '赶进度'
}, {
    date: '2017-05-07',
    name: '办公用品5',
    address: '尺子'
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