//实例总参数
console.log('lib load success~~~~~~~~~~~~~~~~~~~~~~~~~~~~1.01')
var FlowId = 0 //当前审批流程ID
var FlowName = '' //当前审批流程名称
var NodeId = 0 //审批节点ID
var TaskId = 0 //审批任务ID
var state = ''//流程状态
var State = '未完成' //多异步辅助状态
var Index = 0 //审批列表类型参数 0-带我审批 1-我已审批 2-我发起的 3-抄送我的
var UrlObj = {} //url参数对象
var QueryObj = {} //获取url参数对象
var Id = 0 //自增task表的id
var UserList = [] //所有用户数据
var menu = []//菜单列表
let rolelist = []//角色列表
var ReApprovalTempData = {} //重新发起审批保存的临时数据
let slParam = ['ruleForm', 'tableForm', 'imageList', 'pdfList', 'nodeList', 'dataArr', 'purchaseList', 'fileList', 'items']//需要临时保存的字段
var imageListOrigin = []
var fileListOrigin = []
var pdfListOrigin = []
var imageList = []
var fileList = []
var pdfList = []
let jinDomarn = 'http://wuliao5222.55555.io:35705/api/'
//let serverUrl = 'http://17e245o364.imwork.net:49415/'
let serverUrl = 'http://' + window.location.host +'/'
let ProjectTypes = ['自研项目', '纵向项目', '横向项目', '测试项目']
let PTypes = [
    { label: '研发类', value: '研发类', children: [{ value: '自研', label: '自研' }, { value: '横向', label: '横向' }, { value: '纵向', label: '纵向' }, { value: '测试', label: '测试' }] },
    { label: '产品类', value: '产品类', children: [{ value: '产品', label: '产品' }, { value: '测试', label: '测试' }] },
    { label: '教育类', value: '教育类', children: [{ value: '教育', label: '教育' }, { value: '测试', label: '测试' }] }
]
let status = ["在研", "已完成", "终止"]
let Depts = []
let DeptNames = []
let CompanyNames = ['泉州华中科技大学智能制造研究院', '泉州华数机器人有限公司']

//原型方法
Array.prototype.removeByValue = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) {
            this.splice(i, 1);
            break;
        }
    }
}
//库方法
function getLocalObj(name) {
    return JSON.parse(localStorage.getItem(name))
}

function setLocalObj(name, obj) {
    localStorage.setItem(name,JSON.stringify(obj))
}

function logout() {
    localStorage.clear()
    _delCookie('UserName')
    location.reload()
}

function loadPage(url) {
    TaskId = 0
    NodeId = 0

    var param = url.split('?')[1]
    if (param) {
        var paramArr = param.split('&')
        for (let p of paramArr) {
            UrlObj[p.split('=')[0]] = p.split('=')[1]
        }
    }
    $("#tempPage").load(url)
}
function goHome() {
    GetData('/FlowInfoNew/GetFlowStateDetail?Index=1&OnlyReturnCount=true&ApplyManId=' + DingData.userid, (res) => {
        if (res.ApproveCount) {
            loadPage('/Main/approval')
        } else {
            loadPage('/Main/approval_list?Index=0')
        }
    })
    return true
}
function goError() {
    if (!DingData.nickName || DingData.nickName == 'undefined') {
        loadPage('/Login/Error?errorStr=免登失败，重新打开页面试试')
        return true
    }
}

function loadHtml(parentId,childId) {
    $("#" + parentId).html('')
    $("#" + parentId).append($("#" + childId))
}

function _cloneObj(obj) {
     return $.extend(true, {}, obj)
}

function _cloneArr(arr) {
    var newArr = []
    for (var a = 0; a < arr.length; a++) {
        if (typeof (arr[a]) == 'object') {
            if (arr[a].length >= 0)
                newArr.push($.extend(true, [], arr[a]))
            else
                newArr.push($.extend(true, {}, arr[a]))
        }
        else newArr.push(arr[a])
    }
    return newArr
}

function _mergeObjectArr(arr1, arr2, prop) {
    for (var a = 0; a < arr1.length; a++) {
        for (var b = 0; b < arr2.length; b++) {
            if (arr1[a][prop] == arr2[b][prop]) {
                for (var p in arr2[b]) {
                    arr1[a][p] = arr2[b][p]
                }
            }
        }
    }
    return arr1
}

function _formatQueryStr(obj) {
    var queryStr = '?'
    for (var o in obj) {
        //if (obj[o] == null || obj[o] == undefined || obj[o] == '')
        //    continue
        queryStr = queryStr + o + '=' + obj[o] + '&'
    }
    return queryStr.substring(0, queryStr.length - 1)
}

function _delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = _getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}

function _dateToString(date, split) {
    if(!split) split = "-"
    var d = new Date(date)
    var year = d.getFullYear()
    var month = d.getMonth() + 1
    var day = d.getDate()
    if (month < 10) month = '0' + month
    if (day < 10) day = '0' + day
    return year + split + month + split + day
}

function _timeToString(date, split) {
    if (!split) split = "-"
    var d = new Date(date)
    var year = d.getFullYear()
    var month = d.getMonth() + 1
    var day = d.getDate()
    var hour = d.getHours()
    var minute = d.getMinutes()
    var second = d.getSeconds()
    if (month < 10) month = '0' + month
    if (day < 10) day = '0' + day
    if (hour < 10) hour = '0' + hour
    if (minute < 10) minute = '0' + minute
    if (second < 10) second = '0' + second
    return year + split + month + split + day + ' ' + hour + ':' + minute + ':' + second
}

function _stringToDate(str) {
    str = str.replace(/-/g, '/'); 
    var date = new Date(str);
    date.setDate(date.getDate() + 1);
    return date
}

function _stringToTime(str) {
    var tempStrs = str.split(" ");
    var dateStrs = tempStrs[0].split("-");
    var year = parseInt(dateStrs[0], 10);
    var month = parseInt(dateStrs[1], 10) - 1;
    var day = parseInt(dateStrs[2], 10);
    if (tempStrs[1]) {
        var timeStrs = tempStrs[1].split(":");
        var hour = parseInt(timeStrs[0], 10);
        var minute = parseInt(timeStrs[1], 10);
        var second = parseInt(timeStrs[2], 10);
        var date = new Date(year, month, day, hour, minute, second);
    } else {
        var date = new Date(year, month, day);
    }
    console.log('new date')
    console.log(date)
    return date;
}


function _getTime() {
    var split = "-"
    var d = new Date()
    var year = d.getFullYear()
    var month = d.getMonth() + 1
    var day = d.getDate()
    var hour = d.getHours()
    var minute = d.getMinutes()
    var second = d.getSeconds()
    if (month < 10) month = '0' + month
    if (day < 10) day = '0' + day
    if (hour < 10) hour = '0' + hour
    if (minute < 10) minute = '0' + minute
    if (second < 10) second = '0' + second
    return year + split + month + split + day + ' ' + hour + ':' + minute + ':' + second
}

function _getDate(split,noZero) {
    var d = new Date()
    var year = d.getFullYear()
    var month = d.getMonth() + 1
    var day = d.getDate()
    if (month < 10 && !noZero) month = '0' + month
    if (day < 10 && !noZero) day = '0' + day
    if (split)
        return year + split + month + split + day
    else
        return year + '年' + month + '月' + day + '日' 
}

function _computeDurTime(startTime, endTime, type) {
    var durDate = endTime.getTime() - startTime.getTime()
    var days = Math.floor(durDate / (24 * 3600 * 1000))
    var hours = Math.floor(durDate / (3600 * 1000))
    var minutes = Math.floor(durDate / (60 * 1000))
    var seconds = Math.floor(durDate / (1000))

    //去掉周末计算天数
    let first = startTime.getTime();
    let last = endTime.getTime();
    let days2 = 0;
    for (let i = first; i <= last; i += 24 * 3600 * 1000) {
        let d = new Date(i);
        if (d.getDay() >= 1 && d.getDay() <= 5) {
            days2++;
        }
    }
    switch (type) {
        case 'd': return days; break;
        case 'd2': return days2; break;
        case 'h': return hours; break;
        case 'm': return minutes; break;
        default: return seconds;
    }
}

function _computedTime(startHour, startMinute, endHour, endMinute) {
    if (startMinute > endMinute) {
        endMinute += 60
        endHour -- 
    }
    if (endHour < startHour) return '0:0'
    return (endHour - startHour) + ':' + (endMinute - startMinute)
}

function isArray(o) {
    return Object.prototype.toString.call(o) == '[object Array]';
}

function _getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

function checkRate(input) {
    var re = /^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 //判断正整数 /^[1-9]+[0-9]*]*$/ 
    var nubmer = input
    if (!re.test(nubmer)) {
        return false;
    }
}


function getFormData_done(res) {

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
        
var checkProjectId = (rule, value, callback) => {
    if (!value) {
        return callback(new Error('项目编号不能为空'));
    }
    if (value == '2015ZL-XN000') callback();
    setTimeout(() => {
        let reg1 = /^[0-9]{4}[a-zA-Z]{2,3}[0-9]{3}$/
        if (!reg1.test(value)) {
            callback(new Error('请输入正确格式，例：1234**567或者1234***567,*为字母'));
        } else {
            callback();
        }
    }, 500);
};
let commonInput = [{ required: true, message: '该项不能为空', trigger: 'blur' }, { min: 0, max: 50, message: '长度在 50 个字符以内', trigger: 'blur' }]
var mixin = {
    data: {
        user: {},
        specialRoles: [
            {
                name: '图纸校对人员',
                intrudations: ['保证所校对的图样（文件）与技术（设计）任务或技术协议书要求的一致性与合理性。并应承担一定的设计技术责任'],
                label: '校对人员的责任',
                members: []
            },
            {
                name: '图纸设计审核人员',
                intrudations: ['a)产品设计方案合理、可行，能满足技术（设计）任务书或技术协议书的要求；', '  b)产品图样和设计文件的内容正确，数据、尺寸准确；','  c)设计人员不在时，应承担设计的技术责任。'],
                label: '设计审核人员的责任',
                members: []
            }
        ],
        ruleForm: {
            Title: ''
        },
        menu: [],
        specialRole: [],
        specialRoleNames: [],
        tableForm:{},
        DingData: {},
        nodeList: [],
        nodeInfo: {},
        NodeIds: [],
        data: [],
        tableData: [],
        fileList: [],
        pdfList: [],
        imageList: [],
        excelList: [],
        mediaList: [],
        specialRole: [],
        preApprove: true,
        isBack: false,
        projectList: [],
        disablePage: false,
        preUrl: '',//预览图片
        showPre: false,
        rolelist: rolelist,//角色列表
        rules: {
            name: [
                { required: true, message: '名称不能为空', trigger: 'blur' },
                { min: 1, max: 15, message: '长度在 1 到 15 个字符', trigger: 'blur' }
            ],
            string: [
                { required: true, message: '内容不能为空', trigger: 'blur' },
                { min: 1, max: 25, message: '长度在 1 到 25 个字符', trigger: 'blur' }
            ],
            ProjectId: [
                { required: true, validator: checkProjectId, trigger: 'blur' },
                { required: true, message: '请输入正确格式，例：1234**567或者1234***567,*为字母', trigger: 'change' }
            ],
            Title: [
                { required: true, message: '标题内容不能为空！', trigger: 'change' },
                { min: 0, max: 60, message: '长度在 60 个字符以内', trigger: 'blur' }
            ],
            ImageUrl: [
                { required: true, message: '图片不能为空！', trigger: 'change' }
            ],
            FilePDFUrl: [
                { required: true, message: 'PDF文件不能为空！', trigger: 'change' }
            ],
            FileUrl: [
                { required: true, message: '文件不能为空！', trigger: 'change' }
            ],
            ProjectType: [
                { required: true, message: '图纸用途不能为空！', trigger: 'change' }
            ],
            Type: [
                { required: true, message: '类别不能为空！', trigger: 'change' }
            ],
            counts: [
                { required: true, message: '套数不能为空！', trigger: 'change' }
            ],
            tpName: [
                { required: true, message: '图纸设计人员不能为空！', trigger: 'change' }
            ],
            Name: [
                { required: true, message: '名称不能为空！', trigger: 'change' },
                { min: 0, max: 15, message: '长度在 15 个字符以内', trigger: 'blur' }
            ],
            CarNumber: [
                { required: true, message: '车牌号不能为空！', trigger: 'change' },
                { min: 0, max: 15, message: '长度在 15 个字符以内', trigger: 'blur' }
            ],
            UnitPricePerKilometre: [
                { required: true, message: '每公里单价不能为空！', trigger: 'change' },
                { min: 0, max: 15, message: '长度在 15 个字符以内', trigger: 'blur' }
            ],
            Abstract: [
                { required: true, message: '内容简介不能为空！', trigger: 'change' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            ProjectName: [
                { required: true, message: '内容不能为空！', trigger: 'change' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            inputProjectId: [
                { required: true, validator: checkProjectId, trigger: 'blur' },
                { min: 0, max: 30, message: '请输入正确格式，例：1234**567或者1234***567,*为字母', trigger: 'blur' }
            ],
            inputProjectName: [
                { required: true, message: '内容不能为空!', trigger: 'change' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ], 
            Price: [
                { required: true, message: '价格不能为空！', trigger: 'change' },
                { type: 'number', message: '必须为数字值' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            Unit: [
                { required: true, message: '单位不能为空！', trigger: 'change' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            Count: [
                { required: true, message: '数量不能为空！', trigger: 'change' },
                { type: 'number', message: '必须为数字值' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            time: [
                { required: true, message: '时长不能为空！', trigger: 'change' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            DateTime: [
                { required: true, message: '日期不能为空！', trigger: 'change' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            StartTime: [
                { required: true, message: '开始时间不能为空！', trigger: 'change' }
            ], 
            EndTimeTime: [
                { required: true, message: '结束时间不能为空！', trigger: 'change' }
            ], 
            UseTime: [
                { required: true, message: '时长不能为空！', trigger: 'change' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            OverTimeContent: [
                { required: true, message: '不能为空！', trigger: 'change' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ], 
            EffectiveTime: [
                { required: true, message: '有效时间不能为空！', trigger: 'change' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            Purpose: [
                { required: true, message: '用途不能为空！', trigger: 'change' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            //文件阅办单表单
            MainContent: [
                { required: true, message: '文件标题不能为空！', trigger: 'blur' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            ReceivingUnit: [
                { required: true, message: '来文单位不能为空！', trigger: 'blur' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            ReceivingTime: [
                { required: true, message: '时间不能为空！', trigger: 'blur' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ], 
            MainIdea: [
                { required: true, message: '主要内容不能为空！', trigger: 'blur' },
                { min: 0, max: 140, message: '长度在 140 个字符以内', trigger: 'blur' }
            ], 
            Suggestion: [
                { required: true, message: '拟办意见不能为空！', trigger: 'blur' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            Leadership: [
                { required: true, message: '领导阅示不能为空！', trigger: 'blur' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ], 
            Review: [
                { required: true, message: '部门阅办情况不能为空！', trigger: 'blur' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
            HandleImplementation: [
                { required: true, message: '办理落实情况不能为空！', trigger: 'blur' },
                { min: 0, max: 30, message: '长度在 30 个字符以内', trigger: 'blur' }
            ],
        },
        pickerOptions: pickerOptions,
        CompanyNames: CompanyNames,
        dialogFormVisible:false,
        currentPage: 1,
        totalRows: 0,
        pageSize: 5,
        dingList: [],
        PTypes: PTypes,
        project: {},
        purchaseList:[],
        noList: [],

        groupPeople:[],

        date: _getDate(),
        //审批页面参数
        FlowId: '',
        FlowName: '',
        NodeId: '',
        TaskId: '',
        State: '',
        Index: '',
    },
    methods: {
        doWithErrcode(error,url, errorFunc) {
            if (!error) {
                return 1
            }
            if (error && error.errorCode != 0) {
                if (errorFunc) {
                    if(errorFunc() === false) return 1
                }
                this.elementAlert('报错信息', error.errorMessage)
                //报错日志
                $.ajax({
                    url: 'ContextError/Read',
                    type: 'GET',
                    success: function (res) {
                        if (typeof (res) == 'string') res = JSON.parse(res)
                        console.log('ContextError/Read')
                        console.log(url)
                        console.log(res)
                    },
                    error: function (err) {
                        console.error(url)
                        console.log('ContextError/Read err')
                        console.error(err)
                    }
                })
                return 1
            }
            return 0
        },
        GetData(url, succe, showLoading = false, errorFunc) {
            const loading = null
            if (showLoading) {
                loading = this.$loading({
                    lock: true,
                    text: '数据获取中，请耐心等待~',
                    spinner: 'el-icon-loading',
                    background: 'rgba(0, 0, 0, 0.7)'
                });
            }
            var that = this
            $.ajax({
                url: url,
                type: 'GET',
                success: function (res) {
                    if (showLoading) { loading.close() }
                    if (typeof (res) == 'string') res = JSON.parse(res)
                    if (url.indexOf('GetFlowStateDetail') <= 0) {
                        console.log(url)
                        console.log(res)
                    }
                    
                    if (that.doWithErrcode(res.error, url, errorFunc)) {
                        return
                    }
                    res.count ? succe(res.data, res.count) : succe(res.data)
                },
                error: function (err) {
                    if (showLoading) { loading.close() }
                    console.error(url)
                    console.error(err)
                }
            })
        },
        PostData(url, param, succe, errorFunc, showLoading = false) {
            param = JSON.stringify(param).replace(/null/g, '""')
            let loading = null
            if (showLoading) {
                loading = this.$loading({
                    lock: true,
                    text: '数据获取中，请耐心等待~',
                    spinner: 'el-icon-loading',
                    background: 'rgba(0, 0, 0, 0.7)'
                });
            }
            var that = this
            $.ajax({
                url: url,
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                data: param,
                success: function (res) {
                    if (showLoading) { loading.close() }
                    if (typeof (res) == 'string') res = JSON.parse(res) 
                    console.log(url)
                    console.log(JSON.parse(param))
                    console.log(res)
                    if (that.doWithErrcode(res.error, url, errorFunc)) {
                        return
                    }
                    if (res.data == null) {
                        succe([])
                        return
                    }
                    res.data = JSON.stringify(res.data).replace(/null/g, '""')
                    succe(JSON.parse(res.data))
                },
                error: function (err) {
                    if (showLoading) { loading.close() }
                    if (errorFunc) errorFunc()
                    console.error(url)
                    console.error(err)
                }
            })
        },
        //初始化方法
        initStart(callBack = function () { }) {
            Index = ''
            State = "未完成"
            this.doneloadTmp = false 
            this.DingData = DingData
            this.DeptNames = DeptNames
            this.data = []
            this.tableData = []
            this.nodeList = []
            this.nodeInfo = {}
            this.pdfList = []
            this.excelList = []
            this.fileList = []
            this.imageList = []
            this.mediaList = []
            this.mediaPdfList = []
            this.FlowName = FlowName
            this.purchaseList = []
            this.noList = []
            this.pageSize = 5
            this.items = {}
            this.ruleForm = {
                ApplyMan: DingData.nickName,
                ApplyManId: DingData.userid,
                Dept: DingData.dept[0],
                remark: '',
                ImageUrl: '',
                OldImageUrl: '',
                FileUrl: '',
                OldFilePDFUrl: '',
                FilePDFUrl: '',
                OldFileUrl: '',
                MediaId: '',
                MediaIdPDF: '',
                ProjectName: '',
                ProjectId: '',
                ProjectType: '',
                counts: '',
                tpName: '',
                NodeId: '0',
                //ApplyTime: _getTime(),
                IsEnable: '1',
                FlowId: FlowId,
                IsSend: false,
                State: '1', 
                Title: FlowName,
            }
            this.tableForm = {}
            if (DingData.dept && DingData.dept[0]) this.ruleForm.Dept = DingData.dept[0]
            this.getProjects()
            this.getNodeInfo(true, callBack)
            loadHtml("mainPage", "partPage")
        },
        initEnd(callBack = function () { }) {
            if (UrlObj.flowid) {
                FlowId = UrlObj.flowid
                FlowName = UrlObj.flowName
                NodeId = parseInt(UrlObj.nodeid)
                TaskId = UrlObj.taskid
                State = UrlObj.state
                Id = UrlObj.id
                Index = UrlObj.Index
            }
            this.FlowId = FlowId
            this.FlowName = FlowName
            this.NodeId = NodeId
            this.TaskId = TaskId
            this.state = State
            this.State = State
            this['Id'] = Id
            this.index = Index
            this.Index = Index
               
            this.DingData = DingData
            this.DeptNames = DeptNames
            this.data = []
            this.tableData = []
            this.nodeList = []
            this.nodeInfo = {}
            this.pdfList = []
            this.excelList = []
            this.fileList = []
            this.imageList = []
            this.mediaList = []
            this.mediaPdfList = []
            this.FlowName = FlowName
            this.purchaseList = []
            this.noList = []
            this.pageSize = 5
            this.rebackAble = true
            this.ruleForm = {
                ApplyMan: DingData.nickName,
                ApplyManId: DingData.userid,
                Dept: DingData.dept[0],
                remark: '',
                ImageUrl: '',
                OldImageUrl: '',
                FileUrl: '',
                OldFilePDFUrl: '',
                FilePDFUrl: '',
                OldFileUrl: '',
                MediaId: '',
                MediaIdPDF: '',
                ProjectName: '',
                ProjectId: '',
                ProjectType: '',
                counts: '',
                tpName: '',
                NodeId: '0',
                //ApplyTime: _getTime(),
                IsEnable: '1',
                FlowId: FlowId + '',
                IsSend: false,
                State: '1',
                Title: FlowName,
            }
           
            if (DingData.dept && DingData.dept[0]) this.ruleForm.Dept = DingData.dept[0]
            this.GetDingList(TaskId)
            this.getNodeInfo(false, callBack)
            this.getFormData()
            loadHtml("mainPage", "partPage")
        },
        //提交审批
        approvalSubmit(callBack = function () { }) {
            if (!DingData.userid) return
            var that = this
            this.fileListToUrl()
            this.$refs['ruleForm'].validate((valid) => {
                if (valid && that.ruleForm.Title) {
                    that.disablePage = true
                    for (let r in that.ruleForm) {
                        if (that.ruleForm[r] == '' || that.ruleForm[r] == null) delete that.ruleForm[r]
                    }
                    var paramArr = [that.ruleForm]
                    let mustList = []
                    let choseList = []
                    if (that.nodeInfo.IsMandatory) mustList = that.nodeInfo.IsMandatory.split(',')
                    if (that.nodeInfo.ChoseNodeId) choseList = that.nodeInfo.ChoseNodeId.split(',') 
                    for (let node of that.nodeList) {
                        if ((that.nodeInfo.IsNeedChose && choseList.indexOf(node.NodeId + '') >= 0)
                            || (that.addPeopleNodes && that.addPeopleNodes.indexOf(node.NodeId) >= 0)
                            || (node.NodeName.indexOf('申请人') >= 0 && node.NodeId > 0)) {
                            if ((node.AddPeople.length == 0 && mustList[choseList.indexOf(node.NodeId + '')] == '1') ||
                                (node.AddPeople.length == 0 && (that.addPeopleNodes && that.addPeopleNodes.indexOf(node.NodeId) >= 0))){
                                that.$alert(' 审批人不允许为空，请输入！', '提交失败', {
                                    confirmButtonText: '确定',
                                    callback: action => {

                                    }
                                });
                                that.disablePage = false
                                return
                            }
                            for (let a of node.AddPeople) {
                                let tmpParam = {
                                    "ApplyMan": a.name,
                                    "ApplyManId": a.emplId,
                                    "IsEnable": 1,
                                    "FlowId": FlowId + '',
                                    "NodeId": node.NodeId + '',
                                    "IsSend": node.IsSend,
                                    "State": 0,
                                    "OldFileUrl": null,
                                    "IsBack": null
                                }
                                paramArr.push(tmpParam)
                            }
                        }
                    }
                    //选人去重
                    let compareObj = {}
                    paramArr = paramArr.reduce((item, next) => {
                        compareObj[next.NodeId + next.ApplyManId] ? '' : compareObj[next.NodeId + next.ApplyManId] = true && item.push(next);
                        return item;
                    }, [])

                    console.log(JSON.stringify(paramArr))
                    that.PostData('FlowInfoNew/CreateTaskInfo', paramArr, (res) => {
                        callBack(res)
                    })
                } else {
                    that.$alert('表单数据不全或有误', '提交错误', {
                        confirmButtonText: '确定'
                    });
                    console.log('error submit!!');
                    return false;
                }
            });
        },
        //同意审批
        aggreSubmit() {
            if (!DingData.userid) return
            this.disablePage = true
            var paramArr = []
            var that = this
            if (this.fileList.length > 0) this.fileList.splice(0, fileListOrigin.length)
            if (this.imageList.length > 0) this.imageList.splice(0, imageListOrigin.length)
            if (this.pdfList.length > 0) this.pdfList.splice(0, pdfListOrigin.length)
            this.fileListToUrl()
            paramArr.push({
                "Id": this.ruleForm.Id,
                "Remark": this.ruleForm.Mark,
                "ImageUrl": this.ruleForm.ImageUrl || '',
                "OldImageUrl": this.ruleForm.OldImageUrl || '',
                "FileUrl": this.ruleForm.FileUrl || '',
                "MediaId": this.ruleForm.MediaId  || '',
                "OldFileUrl": this.ruleForm.OldFileUrl || '',
                "FilePDFUrl": this.ruleForm.FilePDFUrl || '',
                "MediaIdPDF": this.ruleForm.MediaIdPDF  || '',
                "OldFilePDFUrl": this.ruleForm.OldFilePDFUrl || '',
                "ProjectId": this.ruleForm.ProjectId || '',
                "ProjectName": this.ruleForm.ProjectName || '',
                "ProjectType": this.ruleForm.ProjectType || '',
                "counts": this.ruleForm.counts || '',
                "tpName": this.ruleForm.tpName || '',
                "TaskId": TaskId,
                "ApplyMan": DingData.nickName,
                "ApplyManId": DingData.userid,
                "Dept": this.ruleForm.Dept || '',
                "NodeId": NodeId,
                "ApplyTime": _getTime(),
                "IsEnable": "1",
                "FlowId": FlowId,
                "IsSend": "false",
                "State": "1",

            })
            for (let r in paramArr[0]) {
                if (paramArr[0][r] == '' || paramArr[0][r] == null) delete paramArr[0][r]
            }
            let mustList = []
            let choseList = []
            if (that.nodeInfo.IsMandatory) mustList = that.nodeInfo.IsMandatory.split(',') 
            if (that.nodeInfo.ChoseNodeId) choseList = that.nodeInfo.ChoseNodeId.split(',') 
            for (let node of this.nodeList) {
                if ((that.nodeInfo.IsNeedChose && choseList.indexOf(node.NodeId + '') >= 0)
                    || (that.addPeopleNodes && that.addPeopleNodes.indexOf(node.NodeId) >= 0)) {
                    if ((node.AddPeople.length == 0 && mustList[choseList.indexOf(node.NodeId + '')] == '1') ||
                        (node.AddPeople.length == 0 && (that.addPeopleNodes && that.addPeopleNodes.indexOf(node.NodeId) >= 0))) {
                        this.$alert(' 审批人不允许为空，请输入！', '提交失败', {
                            confirmButtonText: '确定',
                            callback: action => {

                            }
                        });
                        that.disablePage = false
                        return
                    }
                    for (let a of node.AddPeople) {
                        let tmpParam = {
                            "ApplyMan": a.name,
                            "ApplyManId": a.emplId,
                            "TaskId": TaskId,
                            //"ApplyTime": null,
                            "IsEnable": 1,
                            "FlowId": FlowId,
                            "NodeId": node.NodeId,
                            "Remark": null,
                            "IsSend": node.IsSend,
                            "State": 0,
                            "ImageUrl": null,
                            "FileUrl": null,
                            "IsPost": false,
                            "OldImageUrl": null,
                            "OldFileUrl": null,
                            "IsBack": null
                        }
                        paramArr.push(tmpParam)
                    }
                }
            }

            //选人去重
            let compareObj = {}
            paramArr = paramArr.reduce((item, next) => {
                compareObj[next.NodeId + next.ApplyManId] ? '' : compareObj[next.NodeId + next.ApplyManId] = true && item.push(next);
                return item;
            }, [])

            console.log(JSON.stringify(paramArr))
            this.PostData("/FlowInfoNew/SubmitTaskInfo", paramArr, (res) => {
                this.$alert('审批成功', '提示信息', {
                    confirmButtonText: '确定',
                    callback: action => {
                        loadPage('/main/Approval_list')
                    }
                });
            })
        },
        //退回审批 
        returnBk() {
            this.$confirm('是否确认退回申请？', '提示信息')
                .then(_ => {
                    let param = {
                        "Id": this.ruleForm.Id,
                        "Remark": this.ruleForm.Mark
                    }
                    this.returnSubmit(param, '退回')
                })
                .catch(_ => {

                });
        },
        //撤回审批
        rebackSubmit() {
            this.$confirm('是否确认撤回申请？','提示信息')
                .then(_ => {
                    this.disablePage = true
                    var param = {
                        "Id": this.ruleForm.Id,
                        "NodeId": 0,
                        "Remark": this.ruleForm.Mark
                    }
                    this.returnSubmit(param,'撤回')
                })
                .catch(_ => {

                });
        },
        returnSubmit(option,type) {
            this.disablePage = true
            var param = {
                "TaskId": TaskId,
                "ApplyMan": DingData.nickName,
                "ApplyManId": DingData.userid,
                "Dept": DingData.departName,
                "NodeId": NodeId,
                //"ApplyTime": _getTime(),
                "IsEnable": "1",
                "FlowId": FlowId,
                "IsSend": "false",
                "State": "1",
                "BackNodeId": this.nodeInfo.BackNodeId
            }
            for (let o in option) {
                param[o] = option[o]
            }
            this.PostData("/FlowInfoNew/FlowBack", param, (res) => {
                this.$alert('审批已' + type, '提示信息', {
                    confirmButtonText: '确定',
                    callback: action => {
                        loadPage('/main/Approval_list')
                    }
                });
            })
        },
        resetForm(formName) {
            this.$refs[formName].resetFields();
        },
        
        //显示临时保存数据
        saveTempData() {
            for (let p of slParam) {
                if (this[p]) {
                    console.log(p)
                    console.log(this[p])
                    this.saveData(p, this[p])
                }
            }
            this.$message({ type: 'success', message: `临时保存成功，下次打开本页面有效` });
        },
        loadTempData() {
            for (let p of slParam) {
                let data = this.loadData(FlowId + '-' + p)
                if (getLocalObj(FlowId + '-' + p)) {
                    data = getLocalObj(FlowId + '-' + p)
                }
                if (data) {
                    this[p] = data
                    this.saveData(p, null)
                }
            }
            localStorage.clear()
        },
        saveData(key,value) {
            var Days = 7;
            var exp = new Date();
            exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
            if (JSON.stringify(value).length > 1500) {
                console.warn(FlowId + "-" + key + ' is too large~~~~~~~~~~!!!!!!!!!!')
                setLocalObj(FlowId + "-" + key, value)
                return
            }
            document.cookie = FlowId + "-" + key + "=" + JSON.stringify(value) + ";expires=" + exp.toGMTString();
            
            //document.cookie = FlowId + "-file=" + JSON.stringify(files) + ";expires=" + exp.toGMTString();
            //document.cookie = FlowId + "-purchaseList=" + JSON.stringify(purchaseList) + ";expires=" + exp.toGMTString();
            //document.cookie = FlowId + "-dataArr=" + JSON.stringify(purchaseList) + ";expires=" + exp.toGMTString();
        },
        loadData(cookieName) {
            let arr, reg = new RegExp("(^| )" + cookieName + "=([^;]*)(;|$)");
            arr = document.cookie.match(reg)
            if (arr && arr[2] && arr[2] != 'null' && arr[2] != '[]') {
                console.log('获取临时保存数据 -- ' + cookieName + ' 成功----------!')
                console.log(arr[2])
                return JSON.parse(arr[2])
            }
        },
        //重新发起审批
        reApproval() {
            this.disablePage = true
            State = '未完成'
            var tmpPdfList = []
            for (let pdf of this.pdfList) {
                if (pdf.state == '1') tmpPdfList.push(pdf)
            }
            if (!this.tableForm) this.tableForm = {}
            this.ruleForm.IsBacked = false
            this.ruleForm.IsPost = false
            this.ruleForm.NodeId = '0'
            delete this.ruleForm.Id
            delete this.ruleForm.TaskId
            delete this.ruleForm.ApplyTime
            ReApprovalTempData = {
                valid: true,
                dataArr: this.dataArr,
                imageList: this.imageList,
                fileList: this.fileList,
                pdfList: tmpPdfList,
                ruleForm: this.ruleForm,
                items: this.items,
                nodeList: this.nodeList
            }
            ReApprovalTempData['tableForm'] = this.tableForm || {}
            if (this.data && this.data.length>0) {
                ReApprovalTempData['dataArr'] = this.data
            } else if (this.tableData && this.tableData.length > 0) {
                ReApprovalTempData['dataArr'] = this.tableData
            }
            //if(items) ReApprovalTempData['items'] = items
            console.log(ReApprovalTempData)
            for (let m of menu) {
                for (let f of m.flows) {
                    if (f.FlowId == FlowId) {
                        loadPage(f.PcUrl)
                    }
                }
            }
        },
        //加载重新发起审批传递的数据
        loadReApprovalData() {
            if (!ReApprovalTempData.valid) return
            //重新发起审批节点判断
            for (let i = 1; i < this.nodeList.length; i++) {
                if (!this.nodeList[i].ApplyMan && ReApprovalTempData.nodeList[i].ApplyMan) {
                    this.nodeList[i]['AddPeople'] = [{
                        avatar: "",
                        emplId: ReApprovalTempData.nodeList[i].ApplyManId,
                        name: ReApprovalTempData.nodeList[i].ApplyMan
                    }]
                }
            }
            this.ruleForm = ReApprovalTempData.ruleForm
            this.tableForm = ReApprovalTempData.tableForm
            this.dataArr = ReApprovalTempData.dataArr
            this.imageList = ReApprovalTempData.imageList
            this.fileList = ReApprovalTempData.fileList
            this.pdfList = ReApprovalTempData.pdfList
            this.items = ReApprovalTempData.items
            //this.nodeList = ReApprovalTempData.nodeList
            ReApprovalTempData.valid = false
            this['purchaseList'] = ReApprovalTempData.dataArr
            this.loadReApprovalData_done()
        },
        //翻頁相關事件
        //获取全部方法
        getData() {
            var start = this.pageSize * (this.currentPage - 1)
            this.tableData = this.data.slice(start, start + this.pageSize)
        },
        handleSizeChange: function (val) {
            this.currentPage = 1
            this.pageSize = val
            this.getData()
        },
        handleCurrentChange: function (val) {
            this.currentPage = val
            this.getData()
        },
        //分页获取方法
        handleSizeChange2: function (val) {
            this.currentPage = 1
            this.pageSize = val
            this.getData2()
        },
        handleCurrentChange2: function (val) {
            this.currentPage = val
            this.getData2()
        },
        //下拉框选择项目
        selectProject(id) {
            console.log(id)
            for (var proj of this.projectList) {
                if (proj.ProjectId == id) {
                    this.ruleForm.ProjectId = proj.ProjectId
                    this.ruleForm.ProjectName = proj.ProjectName
                    if(FlowId != 6)this.ruleForm.ProjectType = proj.ProjectType
                    this.project = proj
                    this.ruleForm.Title = proj.ProjectId + ' - ' + proj.ProjectName

                    for (let i = 0; i < this.nodeList.length; i++) {
                        if (this.nodeList[i].NodeName.indexOf('项目负责人') >= 0) {
                            this.nodeList[i].AddPeople = [{
                                name: proj.ResponsibleMan,
                                emplId: proj.ResponsibleManId
                            }]
                            $("." + i).remove()
                            $("#" + i).after('<span class="el-tag ' + i + '" style="width: 60px; text-align: center; ">' + proj.ResponsibleMan.substring(0, 3) + '</span >')
                        }
                    }
                }
            }
            
        },
        //获取特殊角色详细信息
        getSpecialRoleInfo: function (roleName) {
            var that = this
            var url = '/Role/GetRoleInfo?RoleName=' + roleName
            $.ajax({
                url: url,
                success: function (data) {
                    console.log('获取特殊角色详细信息')
                    console.log(url)
                    console.log(data)
                    for (let s of that.specialRoles) {
                        if (data[0].RoleName == s.name) {
                            s.members = data
                        }
                    }
                }
            })
        },
        //获取审批页面基本信息
        getFormData() {
            var url = "/FlowInfoNew/GetApproveInfo?TaskId=" + TaskId + "&ApplyManId=" + DingData.userid
            this.GetData(url, (res) => {
                imageList = []
                fileList = []
                pdfList = []
                this.handleUrlData(res)
                taskId = res.TaskId
                this.ruleForm = res
                getFormData_done(res)
            })
        },
        handleUrlData(data) {
            imageList = []
            fileList = []
            pdfList = []
            if (data.ImageUrl && data.ImageUrl.length > 5) {
                var urlList = data.ImageUrl.split(',')
                var oldUrlList = []
                data.OldImageUrl ? oldUrlList = data.OldImageUrl.split(',') : oldUrlList = []
                //var oldUrlList = data.OldImageUrl.split(',')
                for (let i = 0; i < urlList.length; i++) {
                    imageList.push({
                        response: { Content: urlList[i] },
                        name: oldUrlList[i] || '图片' + i,
                        url: document.location + (urlList[i].substring(2)).replace(/\\/g, "/")
                    })
                }
                imageListOrigin = _cloneArr(imageList)
                this.imageList = imageList
            }
            if (data.FileUrl && data.FileUrl.length > 5) {
                FileUrl = data.FileUrl
                var urlList = data.FileUrl.split(',')
                var oldUrlList = data.OldFileUrl.split(',')
                var MediaIdList = data.MediaId ? data.MediaId.split(',') : []
                for (let i = 0; i < urlList.length; i++) {
                    fileList.push({
                        response: { Content: urlList[i] },
                        name: oldUrlList[i],
                        path: urlList[i].replace(/\\/g, "/"),
                        url: document.location + (urlList[i].substring(2)).replace(/\\/g, "/"),
                        mediaId: MediaIdList[i]
                    })
                }
                fileListOrigin = _cloneArr(fileList)
                this.fileList = fileList
            }
            if (data.FilePDFUrl && data.FilePDFUrl.length > 5) {
                FilePDFUrl = data.FilePDFUrl
                var urlList = data.FilePDFUrl.split(',')
                var oldUrlList = data.OldFilePDFUrl.split(',')
                var MediaIdList = data.MediaIdPDF ? data.MediaIdPDF.split(',') : []
                var stateList = data.PdfState ? data.PdfState.split(',') : []
                
                for (let i = 0; i < urlList.length; i++) {
                    pdfList.push({
                        response: { Content: urlList[i] },
                        name: oldUrlList[i],
                        url: document.location + (urlList[i].substring(2)).replace(/\\/g, "/"),
                        mediaId: MediaIdList[i],
                        state: stateList[i] || '1'
                    })
                }
                pdfListOrigin = _cloneArr(pdfList)
                this.pdfList = pdfList
            }
        },
        //获取审批/抄送 相关人员列表
        getNodeList(ifStart, callBack = function () { }) {
            var url = "/FlowInfoNew/GetSign?FlowId=" + FlowId + "&TaskId=" + TaskId
            this.GetData(url, (res) => {
                this.isBack = res[0].IsBack
                this.nodeList = _cloneArr(res)
                let lastNode = {}
                let tempNodeList = []
                //审批人分组
                for (let node of this.nodeList) {
                    if (lastNode.NodeName == node.NodeName && !lastNode.ApplyTime && !node.ApplyTime) {
                        tempNodeList[tempNodeList.length - 1].ApplyMan = tempNodeList[tempNodeList.length - 1].ApplyMan + ',' + node.ApplyMan
                    }
                    else {
                        tempNodeList.push(node)
                    }
                    lastNode = node
                    //判断是否可以撤回
                    if (this.rebackAble && node.IsSend == false && node.ApplyTime && node.ApplyManId && node.ApplyManId != DingData.userid) {
                        this.rebackAble = false
                    }
                }
                this.nodeList = _cloneArr(tempNodeList)

                for (let node of this.nodeList) {
                    node['AddPeople'] = []
                    //抄送人分组
                    if (node.ApplyMan && node.ApplyMan.length > 0)
                        node.NodePeople = node.ApplyMan.split(',')
                    //申请人设置当前人信息
                    if (node.NodeName.indexOf('申请人') >= 0 && !node.ApplyMan) {
                        node.ApplyMan = DingData.nickName
                        node.AddPeople = [{
                            name: DingData.nickName,
                            emplId: DingData.userid
                        }]
                    }
                }
                //设置当前角色正确节点
                var needChoose = []
                var chooseType = []
                var roleName = []
                if (this.nodeInfo.ChoseNodeId) needChoose = this.nodeInfo.ChoseNodeId.split(',')
                if (this.nodeInfo.ChoseType) chooseType = this.nodeInfo.ChoseType.split(',')
                if (this.nodeInfo.RoleNames) roleName = this.nodeInfo.RoleNames.split(',')
                if (this.index && this.index != '0' && this.index != '2') {
                    for (let i = this.nodeList.length - 1; i >= 0; i--) {
                        if (this.nodeList[i].ApplyManId == DingData.userid) {
                            this.NodeIds.push(this.nodeList[i].NodeId)
                        }
                    }
                }
                //角色选人赋值数据
                let i = 1
                for (let node of this.nodeList) {
                    let index = needChoose.indexOf(node.NodeId + '')
                    if (index >= 0 && chooseType[index] == '1' && roleName[index]) {
                        for (let r in rolelist) {
                            if (r == roleName[index]) {
                                node['roles'] = []
                                for (let user of rolelist[r]) {
                                    node['roles'].push({
                                        emplId: user.UserId,
                                        name: user.UserName
                                    })
                                }
                                //node['roles'] = rolelist[r]
                                node['roleid'] = i
                                i++
                            }
                        }
                    }
                }

                this.getNodeInfo_done(this.nodeList)
                ////我已审批页面重新判断NodeId
                if (Index == 1) {
                    for (let i = this.nodeList.length - 2; i > 0; i--) {
                        if (this.nodeList[i].ApplyManId == DingData.userid) {
                            NodeId = this.nodeList[i].NodeId
                            this.NodeId = this.nodeList[i].NodeId
                            break
                        }
                    }
                }
                //发起页面获取临时保存数据和重新发起数据
                if (ifStart) {
                    this.loadTempData()
                    this.loadReApprovalData()
                    this.doneloadTmp = true
                }
                callBack()
            })
            
        },
        loadReApprovalData_done() {
            //重新发起获取数据后加载
        },
        getNodeInfo_done(nodeList) {

        },
        //获取项目数据
        getProjects() {
            var that = this
            $.ajax({
                url: "/Project/GetAllProJect",
                type: "GET",
                dataType: "json",
                success: function (data) {
                    console.log("获取项目列表数据 GetAllProJect")
                    console.log(data)
                    that.projectList = data
                },
                error: function (err) {
                    console.log(err);
                }
            })
        },
        //获取審批節點數據
        getNodeInfo(ifStart,callBack) {
            var url = "/FlowInfoNew/getnodeinfo?FlowId=" + FlowId + "&nodeid=" + NodeId
            this.GetData(url, (res) => {
                this.nodeInfo = res[0]
                NodeId = res[0].NodeId
                this.getNodeList(ifStart, callBack)
            })
        },
        //审批所有流程通过，后续处理
        doneSubmit(text) {
            if (!text) text = '提交审批成功'
            this.$alert(text, '提示信息', {
                confirmButtonText: '确定',
                callback: action => {
                    loadPage('/main/Approval')
                }
            });
        },
        //钉钉推送文件
        downloadFile(mediaId) {
            this.disablePage = true
            var that = this
            var param = {
                UserId: DingData.userid,
                Media_Id: mediaId
            }
            $.ajax({
                url: '/DingTalkServers/sendFileMessage',
                type: 'POST',
                data: param,
                success: function (data) {
                    data = JSON.parse(data)
                    console.log('钉钉推送文件')
                    console.log(param)
                    console.log(data)
                    if (data.errmsg == 'ok') {
                        that.$alert(data.errorMessage, '获取文件成功', {
                            confirmButtonText: '确定'
                        });
                    }
                    else {
                        that.$alert(data.errorMessage, '获取文件失败', {
                            confirmButtonText: '确定'
                        });
                    }
                    that.disablePage = false
                }
            })
        },
        //element彈窗
        elementAlert(title, text) {
            var that = this
            that.$alert(text, title, {
                confirmButtonText: '确定',
            });
        },
        //选单人
        addMan() {
            var that = this
            dd.biz.contact.choose({
                multiple: false, //是否多选： true多选 false单选； 默认true
                users: [], //默认选中的用户列表，员工userid；成功回调中应包含该信息
                corpId: DingData.CorpId, //企业id
                max: 10,
                onSuccess: function (data) {
                    that.ResponsibleMan = data
                    console.log(data)
                },
                onFail: function (err) { }
            });
        },
        //选多人
        addGroup(users) {
            var that = this
            console.log(users)
            dd.biz.contact.choose({
                multiple: true, //是否多选： true多选 false单选； 默认true
                users: users||[], //默认选中的用户列表，员工userid；成功回调中应包含该信息
                corpId: DingData.CorpId, //企业id
                onSuccess: function (data) {
                    that.groupPeople = data
                    console.log(data)
                },
                onFail: function (err) { }
            });
        },
        //编辑添加人，删除人
        handleClose(tag) {
            for (let i = 0; i < this.groupPeople.length; i++) {
                if (this.groupPeople[i].emplId == tag.emplId) {
                    this.groupPeople.splice(i, 1)
                    break
                }
            }
        },
        addGroup() {
            var that = this
            dd.biz.contact.choose({
                multiple: true, //是否多选： true多选 false单选； 默认true
                users: [], //默认选中的用户列表，员工userid；成功回调中应包含该信息
                corpId: DingData.CorpId, //企业id\
                max: 10,
                onSuccess: function (data) {
                    console.log(data)
                    for (let d of data) {
                        let isAdd = true
                        for (let g of that.groupPeople) {
                            if (d.emplId == g.emplId) isAdd = false
                        }
                        if (isAdd) that.groupPeople.push(d)
                    }
                    //that.groupPeople = that.groupPeople.concat(data)
                },
                onFail: function (err) { }
            });
        },
        //搜索物料列表
        searchCode(formName) {
            var that = this
            if (!this.searchForm.name) {
                this.$message({ type: 'error', message: `请输入关键字` });
                return
            }
            var url = '/Purchase/GetICItem?Key=' + that.searchForm.name
            $.ajax({
                url: url,
                success: function (data) {
                    console.log(url)
                    data = JSON.parse(data)
                    if (data.length == 0) this.$message({ type: 'warning', message: `获取数据数为0` })
                    console.log(data)
                    that.data = data
                    that.totalRows = data.length
                    that.getData()
                }
            })
        },
        //删除申请物料
        deleteGood(index, good) {
            this.purchaseList.splice(index, 1)
            if (this.noList) { this.noList.splice(index, 1) }
        },
        //编辑审批物料
        showEditGood(index, good) {
            this.dialogFormVisible = true
            this.good = $.extend({}, good)
            this.good.index = index
        },
        editGood() {
            this.dialogFormVisible = false
            let tmpGood = $.extend({}, this.good)
            if (tmpGood.UrgentDate) tmpGood.UrgentDate = _dateToString(tmpGood.UrgentDate)
            this.purchaseList[this.good.index] = tmpGood
            let tmpArr = _cloneArr(this.purchaseList)
            this.purchaseList = tmpArr
            console.log('done eidt')
            console.log(this.good.index)
            console.log(this.purchaseList)
        },



        //图片上传事件
        beforePictureUpload(file) {
            if (!file.size) {
                this.$message.error('文件大小（以字节为单位）为0!请上传有效文件')
                return false
            }
            file.name = 'helloWorld'
            const isJPG = file.type === 'image/jpeg'
            const isPNG = file.type === 'image/png'
            const isLt2M = file.size / 1024 / 1024 < 10
            if (!(isJPG || isPNG)) {
                this.$message.error('上传图片只能是 JPG 或 PNG 格式!')
                return false
            }
            if (!isLt2M) {
                this.$message.error('上传图片大小不能超过 10MB!')
                return false
            }
            return true
        },
        handlePictureCardPreview(file) {
            console.warn(file)
            this.dialogImageUrl = file.url;
            this.dialogVisible = true;
        },
        handlePictureCardPreview2(file) {
            console.warn(file)
            this.preUrl = file.url;
            this.showPre = true;
        },
        handlePictureRemove(file, fileList) {
            this.imageList = _cloneArr(fileList)
        },
        handlePictureCardSuccess(response, file, fileList) {
            this.imageList = _cloneArr(fileList)
        },
        judgeRole(roleName) {
            var url = '/Role/GetRoleInfo?RoleName=' + roleName
            var that = this
            $.ajax({
                url: url,
                success: function (data) {
                    console.log(url)
                    console.log(data)
                    for (let d of data) {
                        console.log(d)
                        if (d.emplId == DingData.userid) {
                            that.isRole = true
                            break
                        }
                    }
                }
            })
        },
        //文件上传处理方法
        HandleFileExceed() {
            this.$message({ type: 'error', message: `超出文件个数限制！` });
            return false
        },
        errUpload: function (err, file, fileList) {
            console.warn(err)
            console.warn(file)
            console.warn(fileList)
        },
        BeforeFileUpload(file) {
            if (file.name.indexOf('.')<0) {
                this.$message.error('文件类型不正确，请重新选择！  ')
                return false
            }
            if (!file.size) {
                this.$message.error('文件大小（以字节为单位）为0!请上传有效文件')
                return false
            }
            for (let p of this.fileList) {
                if (file.name == p.name) {
                    this.$message.error('已存在相同文件名文件!')
                    return false
                }
            }
            file.name = 'helloWorld'
            isPdf = false
            const isLt2M = file.size / 1024 / 1024 < 9
            if (!isLt2M) {
                this.$message.error('文件大小不允许超过9M，请重新选择!')
                return false
            }
            return true
        },
        beforeExcelUpload(file) {
            if (!file.size) {
                this.$message.error('文件大小（以字节为单位）为0!请上传有效文件')
                return false
            }
            for (let p of this.excelList) {
                if (file.name == p.name) {
                    this.$message.error('已存在相同文件名文件!')
                    return false
                }
            }
            const isExcel = (file.name.substr(-3) == 'xls' || file.name.substr(-4) == 'xlsx')
            const isLt2M = file.size / 1024 / 1024 < 4
            if (!isExcel) {
                this.$message.error('只能上传 excel 文件!')
                return false
            }
            if (!isLt2M) {
                this.$message.error('文件大小不允许超过4M，请重新选择!')
                return false
            }
            return true
        },
        HandleFileRemove(file, fileList) {
            this.fileList = _cloneArr(fileList)
            this.mediaList = []
            for (let f of fileList) {
                if (f.mediaid) this.mediaList.push(f.mediaid)
            }
        },
        HandleFileSuccess(response, file, fileList) {
            var that = this
            this.mediaList = []
            const loading = this.$loading({
                lock: true,
                text: '数据获取中，请耐心等待~',
                spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.7)'
            });
            var paramObj = {
                "": file.response.Content
            }
            $.ajax({
                url: '/DingTalkServers/UploadMedia/',
                type: 'POST',
                data: paramObj,
                success: function (data) {
                    data = JSON.parse(data)
                    console.log('上传文件到钉盘')
                    console.log(fileList)
                    if (data.media_id) {
                        console.log(data.media_id)
                        for (let f of fileList) {
                            if (f.uid == file.uid) {
                                f['mediaid'] = data.media_id
                            }
                        }
                    } else {
                        console.log('无media_di')
                    }
                    for (let f of fileList) {
                        if (f.mediaid) that.mediaList.push(f.mediaid)
                    }
                    that.fileList = _cloneArr(fileList)
                    loading.close()
                }
            })
        },
        //下载文件
        downloadServerFile: function (path, row) {
            console.log(serverUrl + 'ProjectNew/DownLoad?path=' + path)
            location.href = serverUrl + 'ProjectNew/DownLoad?path=' + path
        },
        fileListToUrl() {
            this.ruleForm['ImageUrl'] = ''
            this.ruleForm['OldImageUrl'] = ''

            this.ruleForm['FilePDFUrl'] = ''
            this.ruleForm['OldFilePDFUrl'] = ''
            this.ruleForm['MediaIdPDF'] = ''

            this.ruleForm['FileUrl'] = ''
            this.ruleForm['OldFileUrl'] = ''
            this.ruleForm['MediaId'] = ''

            if (this.imageList[0] && this.imageList[0].response && this.imageList[0].response.Content ) {
                for (let i = 0; i < this.imageList.length; i++) {
                    this.ruleForm.ImageUrl += this.imageList[i].response.Content
                    this.ruleForm.OldImageUrl += this.imageList[i].name
                    if (i == this.imageList.length - 1) break
                    this.ruleForm.ImageUrl += ','
                    this.ruleForm.OldImageUrl += ','
                }
            }
            //this.ruleForm.ImageUrl = this.ruleForm.ImageUrl.replace(/[\(]/g, "（").replace(/[\)]/g, "）")
            if (this.pdfList[0] && this.pdfList[0].response && this.pdfList[0].response.Content) {
                for (let i = 0; i < this.pdfList.length; i++) {
                    this.ruleForm.FilePDFUrl += this.pdfList[i].response.Content
                    this.ruleForm.OldFilePDFUrl += this.pdfList[i].name
                    this.pdfList[i].mediaid ? this.ruleForm.MediaIdPDF += this.pdfList[i].mediaid : this.ruleForm.MediaIdPDF += this.pdfList[i].mediaId
                    if (i == this.pdfList.length - 1) break
                    this.ruleForm.FilePDFUrl += ','
                    this.ruleForm.OldFilePDFUrl += ','
                    this.ruleForm.MediaIdPDF += ','
                }
            }
            if (this.fileList[0] && this.fileList[0].response && this.fileList[0].response.Content) {
                for (let i = 0; i < this.fileList.length; i++) {
                    this.ruleForm.FileUrl += this.fileList[i].response.Content
                    this.ruleForm.OldFileUrl += this.fileList[i].name
                    this.fileList[i].mediaid ? this.ruleForm.MediaId += this.fileList[i].mediaid : this.ruleForm.MediaId += this.fileList[i].mediaId
                    if (i == this.fileList.length - 1) break
                    this.ruleForm.FileUrl += ','
                    this.ruleForm.OldFileUrl += ','
                    this.ruleForm.MediaId += ','
                }
            }
            return true
        },
        handleExceed(files, fileList) {
            this.$message.warning(`当前限制选择 1 个文件，本次选择了 ${files.length} 个文件，共选择了 ${files.length + fileList.length} 个文件`);
        },
        //PDF上传处理方法
        beforePdfFileUpload(file) {
            console.log('before pdf')
            console.log(file)
            for (let p of this.pdfList) {
                if (file.name == p.name) {
                    this.$message.error('已存在相同文件名文件!')
                    return false
                }
            }
            if (file.name.indexOf(' ') >= 0) {
                this.$alert('文件名不能带空格', '上传失败', {
                    confirmButtonText: '确定',
                });
                return false
            }
            file.name = 'helloWorld'
            const isPdfType = file.type === 'application/pdf'
            const isLt2M = file.size / 1024 / 1024 < 9
            if (!isPdfType) {
                this.$message.error('上传文件只能是 PDF 格式!')
                return false
            }
            if (!isLt2M) {
                this.$message.error('上传文件大小不能超过 9MB!')
                return false
            }
            return true
        },
        HandlePdfFileRemove(file, fileList) {
            this.pdfList = _cloneArr(fileList) 
            this.mediaPdfList = []
            for (let f of fileList) {
                if (f.mediaid) this.mediaPdfList.push(f.mediaid)
            }
        },
        handlePdfFileSuccess(response, file, fileList) {
            var that = this
            const loading = this.$loading({
                lock: true,
                text: '数据获取中，请耐心等待~',
                spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.7)'
            });
            var paramObj = {
                "": file.response.Content
            }
            $.ajax({
                url: '/DingTalkServers/UploadMedia/',
                type: 'POST',
                data: paramObj,
                success: function (data) {
                    data = JSON.parse(data)
                    console.log('/api/dt/uploadMedia/')
                    console.log(paramObj)
                    if (data.media_id) {
                        console.log(data.media_id)
                        that.mediaPdfList.push(data.media_id)
                    } else {
                        console.log('无media_di')
                    }
                    for (let i = 0; i < that.mediaPdfList.length; i++) {
                        fileList[i]['mediaid'] = that.mediaPdfList[i]
                    }
                    that.pdfList = _cloneArr(fileList)
                    loading.close()
                }
            })
        },

        //选时间操作
        selectTime(value) {
            if (!value[0]) return
            this.ruleForm.StartTime = _timeToString(value[0])
            this.ruleForm.EndTime = _timeToString(value[1])
        },
        selectActualTime(value) {
            if (!value[0]) return
            this.ruleForm.ActualCycleStart = _timeToString(value[0])
            this.ruleForm.ActualCycleEnd = _timeToString(value[1])
        },
        //根据taskId获取下一个需要审批的人，即要钉的人
        GetDingList(taskId) {
            this.GetData('/DingTalkServers/Ding' + _formatQueryStr({ taskId: taskId }), (res) => {
                if (!res) return
                this.dingList = []
                this.dingList.push(res.ApplyManId)
            })
        },

        //get获取接口数据通用方法
        _getData(url, callBack, param = {}, alertStr, alertTitle = '提示信息') {
            var that = this
            url = url += _formatQueryStr(param)
            $.ajax({
                url: url,
                dataType: "json",
                headers: { Accept: 'application/json'},
                success: function (data) {
                    if (typeof (data) == 'string') data = JSON.parse(data) 
                    console.log(url)
                    console.log(param)
                    console.log(data)
                    if (data.error && data.error.errorCode != 0) {
                        that.$message({ type: 'error', message: data.error.errorMessage })
                        return
                    }
                    if (alertStr) {
                        that.$alert(alertStr.length > 2 ? alertStr : data.error.errorMessage, alertTitle, {
                            confirmButtonText: '确定',
                            callback: action => {
                                if (callBack) callBack(data)
                            }
                        })
                    } else {
                        console.log(data)
                        callBack(data)
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(url)
                    console.log(textStatus)
                    console.log(errorThrown)
                    console.log(XMLHttpRequest);
                }
            })
        },
        //post提交接口数据通用方法
        _postData(url, callBack, param = {}, alertStr, alertTitle = '提示信息') {
            var that = this
            console.log(url)
            console.log(param)
            $.ajax({
                url: url,
                contentType: "application/json; charset=utf-8",
                type: "POST",
                data: JSON.stringify(param),
                dataType: "json",
                success: function (data) {
                    if (typeof (data) == 'string') data = JSON.parse(data) 
                    console.log(data)
                    if (data.error && data.error.errorCode != 0) {
                        that.$message({ type: 'error', message: data.error.errorMessage })
                        return
                    }
                    if (alertStr) {
                        that.$alert(alertStr.length > 2 ? alertStr : data.error.errorMessage, alertTitle, {
                            confirmButtonText: '确定',
                            callback: action => {
                                if (callBack) callBack(data)
                            }
                        })
                    } else {
                        callBack(data)
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(XMLHttpRequest);
                }
            })
        }
    },
    filters: {
        boolean: function (value) {
            if (value == true || value == 'true') {
                return '是'
            } else {
                return '否'
            }
        }
    }
}

function doWithErrcode(error, demo) {
    //if (!error) {
    //    return 1
    //}
    if (error && error.errorCode != 0) {
        console.error(error.errorMessage)
        demo.elementAlert('报错信息', error.errorMessage)
        return 1
    }
    return 0
}
function GetData(url, succe, demo) {
    $.ajax({
        url: url,
        type: 'GET',
        success: function (res) {
            if (typeof (res) == 'string') res = JSON.parse(data)
            console.log(url)
            console.log(res)
            if (doWithErrcode(res.error, demo)) {
                return
            }
            res.count ? succe(res.data, res.count) : succe(res.data)
        },
        error: function (err) {
            console.error(url)
            console.error(err)
        }
    })
}

function PostData(url, param, succe, demo) {
    console.warn('from out this')
    $.ajax({
        url: url,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(param),
        success: function (res) {
            if (typeof (res) == 'string') res = JSON.parse(res) 
            console.log(url)
            console.log(param)
            console.log(res)
            if (doWithErrcode(res.error, demo)) {
                return
            }
            succe(res.data)
        },
        error: function (err) {
            if(!error(err)) return
            console.error(url)
            console.error(err)
        }
    })
}
//表单限制输入，返回对象函数
function lengthLimit(min, max) {
    return {
        min: min, max: max, message: '长度在 ' + min + ' 到 ' + max + ' 个字符', trigger: 'blur'
    }
}

//选择项目控件
Vue.component('sam-dropdown', {
    props: ['str', 'arr'],
    template: ` 
                <el-select v-model="ruleForm.ProjectId" placeholder="请选择" style="width:400px;" v-on:change="selectProject" filterable>
                    <el-option v-for="item in projectList"
                               :key="item.ProjectId"
                               :label="item.ProjectId + '-' + item.ProjectName"
                               :value="item.ProjectId">
                        <span style="float: left"> {{item.ProjectId}}-{{ item.ProjectName }} </span>
                        <span style="float: right; color: #8492a6; font-size: 13px"></span>
                    </el-option>
                </el-select>
              `,
    data: function () {
        return {
            value: this.str ? this.str.split(',') : [],
        }
    },
    methods: {
        handleChange(arr) {
            console.log(arr)
            this.$emit('update:str', arr.join(','))
        },
    },
})
//多选控件
Vue.component('sam-checkbox', {
    props: ['str','arr','onchange'],
    template: ` <el-checkbox-group v-model="value" v-on:change="handleChange">
                    <el-checkbox v-for="a in arr" :label="a" :key="a"></el-checkbox>
                </el-checkbox-group>`,
    data: function () {
        return {
            value: this.str ? this.str.split(',') : [],
        }
    },
    methods: {
        handleChange(arr) {
            let trr = []
            for (let a of this.arr) {
                if (arr.indexOf(a) >= 0) {
                    trr.push(a)
                }
            }
            this.$emit('update:str', trr.join(','))
            if (this.onchange) {
                this.onchange(trr)
            }
        },
    },
})
//选择小组组件
Vue.component('sam-group', {
    props: ['names', 'ids', 'single', 'onchange','disable'],
    template: `  <div v-if="names">
                    <el-tag :key="tag" v-for="tag in names.split(',')" closable
                            :disable-transitions="false" v-on:close="handleClose(tag)">
                        {{tag}}
                    </el-tag>
                    <el-button class="button-new-tag" size="small" v-on:click="addGroup">+ 添加</el-button>
                 </div>
                <div v-else>
                    <el-tag :key="tag" v-for="tag in []" closable
                            :disable-transitions="false" v-on:close="handleClose(tag)">
                        {{tag}}
                    </el-tag>
                    <el-button :disabled='disable' class="button-new-tag" size="small" v-on:click="addGroup">+ 添加</el-button>
                 </div>`,
    data: function () {
        return {
            tags: [],
            tids: []
        }
    },
    methods: {
        addGroup() {
            var that = this
            this.tags = this.names ? this.names.split(',') : []
            this.tids = this.ids ? this.ids.split(',') : []
            if (this.single) {
                this.tags = []
                this.tids = []
            }
            dd.biz.contact.choose({
                multiple: this.single ? false :true, //是否多选： true多选 false单选； 默认true
                users: this.tids, //this.tids 默认选中的用户列表，员工userid；成功回调中应包含该信息
                corpId: DingData.CorpId, //企业id
                max:10,
                onSuccess: function (data) {
                    console.log(data)
                    for (let d of data) {
                        if (that.tids.indexOf(d.emplId)>=0) continue
                        that.tags.push(d.name)
                        that.tids.push(d.emplId)
                    }
                    that.$emit('update:names', that.tags.join(','))
                    that.$emit('update:ids', that.tids.join(','))
                    if (that.onchange) that.onchange(that.tags, that.tids)
                },
                onFail: function (err) { }
            });
        },
        handleClose(tag) {
            this.tags = this.names ? this.names.split(',') : []
            this.tids = this.ids ? this.ids.split(',') : []
            for (let i = 0; i < this.tags.length; i++) {
                if (this.tags[i] == tag) {
                    this.tags.splice(i, 1)
                    this.tids.splice(i, 1)
                    break
                }
            }
            this.$emit('update:names', this.tags.join(','))
            this.$emit('update:ids', this.tids.join(','))
            if (this.onchange) this.onchange(this.tags, this.tids)
        },
    },
})

//输入框组件
Vue.component('custom-input', {
    props: ['value'],
    template: `
    <input
      v-bind:value="value"
      v-on:input="$emit('input', $event.target.value)"
    >
  `
})
Vue.component('sam-input', {
    props: ['value', 'required', 'type', 'minlength', 'maxlength', 'callBack', 'max', 'min', 'placeholder','disabled'],
    template: `<el-input v-model=value :value=value show-word-limit  :type="type||'input'" :placeholder = "placeholder || ''"
                        :minlength = "minlength||0" :maxlength = "maxlength||50" v-on:blur="onBlur" :disabled='disabled'
                        :class="{ redborder:(value =='' && required)}">
                   </el-input>`,
    data: function () {
        return {
            Index: Index,
            value2:this.value
        }
    },
    methods: {
        onBlur(e) {
            let value = e.target.value//.replace(/(^\s*)|[\/&]s*|(\s*$)/g, '')
            //return
            if (this.type == 'number') {
                value = parseFloat(value)
                let max = parseInt(this.max) || 1000000
                let min = parseInt(this.min) || 0
                if (value > max) value = max
                if (value < min) value = min
                value = value + ''
            }
            e.target.value = value
            this.$emit('update:value', value)
        }
    },
})
//时间区间选择器组件
Vue.component('sam-timerange', {
    props: ['value1', 'value2', 'required', 'onchange','date'],
    template: `<div>
                 <el-date-picker
                      v-model="value"
                      :type="type"
                      :value-format="format"
                      :picker-options="pickerOptions"
                      :class="{ redborder: value1 =='' && !required}"
                      range-separator="至"
                      start-placeholder="开始日期"
                      end-placeholder="结束日期"
                      v-on:change='onChange'
                      align="right">
                </el-date-picker>
               </div>
               `,
    data: function () {
        return {
            value: this.value1 ? [this.value1, this.value2] : null,
            format: this.date ? 'yyyy-MM-dd' : 'yyyy-MM-dd HH:mm:ss',
            type: this.date ? 'daterange' : 'datetimerange',
            pickerOptions: {
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
                }]
            },
        }
    },
    methods: {
        onChange(data) {
            console.log('onchange1')
            if (!data) data = ['', '']
            console.log(data)
            this.$emit('update:value1', data[0])
            this.$emit('update:value2', data[1])
            if (this.onchange) {
                this.onchange([_stringToTime(data[0]), _stringToTime(data[1])])
            }
        }
    },
})

//钉钉审批组件
Vue.component('sam-approver-list', {
    props: ['nodedata', 'nodelist', 'specialRoles', 'sprolenames'],
    template: `<div>
                    <el-form-item :label="'审批人'" style="margin-bottom:0px;">
                        <h5></h5>
                    </el-form-item>
                    <el-form-item>
                        <template v-for="(node,index) in nodelist">
                            <el-tag :key="node.NodeName" type="warning" class="nodeTitle" style="width:130px;text-align:center;" :id="node.NodeId">
                                {{node.NodeName}}
                            </el-tag>

                            <template v-for="(p,a) in node.NodePeople">
                                <span v-if="a>0 && node.NodeName!='抄送' && node.ApplyTime" style="margin-left:137px;">&nbsp;</span>
                                <el-tag :key="a"
                                        :closable="false"
                                        onclick="" v-if="node.NodePeople"
                                        :disable-transitions="false"
                                        :type="node.ApplyTime?'success':''" 
                                        :class="{'el-tag--danger':node.IsBack}"
                                        style="text-align:center;"
                                        >
                                    {{p}}
                                </el-tag>
                                
                                <span v-if="(node.NodeName=='抄送' || !node.ApplyTime) && a < node.NodePeople.length-1">,</span>
                                <template v-else>
                                    <p class='applytime'>{{node.ApplyTime}}</p>
                                    <p class='remark'>{{node.Remark}}</p>
                                </template>
                            </template>
                            
                            <template v-for="(ap,b) in node.AddPeople">
                                <span v-if="b>0" style="margin-left:97px;">&nbsp;</span>
                                <el-tag :class="node.NodeId+''" :key="ap.emplId"
                                        :closable="false"
                                        v-on:close="deletePeople(ap.emplId)"
                                        v-if="node.AddPeople.length>0"
                                        :disable-transitions="false"
                                        :type="node.ApplyTime?'success':''"
                                        style="text-align:center;"
                                        >
                                    {{ap.name}}
                                </el-tag>
                            </template>

                           <template v-if="nodedata.IsNeedChose && nodedata.ChoseNodeId && nodedata.ChoseNodeId.indexOf(node.NodeId) >= 0 && State == '未完成' && (Index == '0' || !Index)" >
                                <el-select placeholder="请选择审批人" v-if="node.roles && node.roles.length > 0 && node.roleid == 1" v-model="member1"
                                 style="margin-left:10px;" size="small" v-on:change="selectSpecialMember(node,member1)">
                                    <el-option
                                      v-for="role in node.roles"
                                      :key="role.emplId"
                                      :label="role.name"
                                      :value="role.emplId">
                                    </el-option>
                                </el-select>
                                <el-select placeholder="请选择审批人" v-else-if="node.roles && node.roles.length > 0 && node.roleid == 2" v-model="member2"
                                 style="margin-left:10px;" size="small" v-on:change="selectSpecialMember(node,member2)">
                                    <el-option
                                      v-for="role in node.roles"
                                      :key="role.emplId"
                                      :label="role.name"
                                      :value="role.emplId">
                                    </el-option>
                                </el-select>
                                <el-button class="button-new-tag" v-else size="small" v-on:click="addMember(node)">+ 选人</el-button>
                            </template>

                            <div v-if="index<nodelist.length-1" style="line-height:1px;">
                                <i class="el-icon-arrow-down approve-arrow"  type="primary"></i>
                                </br>
                            </div>
                        </template>

                    </el-form-item></div>`,
    data: function () {
        return {
            State: State,
            Index:Index,
            member1: '',
            member2: '',
        }
    },
    methods: {
        //选人控件添加
        addMember(node) {
            //设置默认选人
            let nodeId = node.NodeId
            let selectMoreList = this.nodedata.IsSelectMore.split(',')
            let choseList = this.nodedata.ChoseNodeId.split(',')
            var that = this
            let choosed = []
            for (let p of node.AddPeople) {
                choosed.push(p.emplId)
            }
            dd.biz.contact.choose({
                multiple: selectMoreList[choseList.indexOf(nodeId + '')] == '1'?true:false, //是否多选： true多选 false单选； 默认true
                users: choosed, //默认选中的用户列表，员工userid；成功回调中应包含该信息
                corpId: DingData.CorpId, //企业id
                max: 10, //人数限制，当multiple为true才生效，可选范围1-1500
                onSuccess: function (data) {
                    console.log(data)
                    var url2 = '/DingTalkServers/getUserDetail?userId=' + data[0].emplId
                    $.ajax({
                        url: url2,
                        dataType: 'json',
                        type: 'POST',
                        data: {},
                        contentType: "application/json; charset=utf-8",
                        success: function (data2) {
                            if (typeof (data2) == 'string') data2 = JSON.parse(data2)
                            for (let node of that.nodelist) {
                                if (node.NodeId == nodeId) {
                                    $("." + nodeId).remove()
                                    data[0].name = data2.name
                                    node.AddPeople = data
                                    for (let d of data) {
                                        $("#" + nodeId).after('<span class="el-tag ' + nodeId + '" style="width: 60px; text-align: center; ">' + d.name.substring(0, 4) + '</span >')
                                    }
                                }
                            }
                        },
                        error: function (err) {
                            console.error(err)
                        }
                    })

                    for (let node of that.nodelist) {
                        if (node.NodeId == nodeId) {
                            $("." + nodeId).remove()
                            node.AddPeople = data
                            for (let d of data) {
                                $("#" + nodeId).after('<span class="el-tag ' + nodeId + '" style="width: 60px; text-align: center; ">' + d.name.substring(0, 3) + '</span >')
                            }
                        }
                    }
                },
                onFail: function (err) { }
            });
        },
        //下拉框选人添加
        selectSpecialMember(node,userid) {
            console.log(node)
            for (let role of node.roles) {
                if (role.emplId == userid) {
                    node.AddPeople = [role]
                }
            }
            return
            console.log(userName)
            console.log(nodeId)
            for (let node of this.nodelist) {
                if (node.NodeId != nodeId)
                    continue
                node.AddPeople = [{
                    emplId: userid,
                    name: userName
                }]
            }
        },

        deletePeople(emplId) {
            for (let node of this.nodelist) {
                for (let a of node.AddPeople) {
                    if (a.emplId == emplId) {
                        node.AddPeople.splice(node.AddPeople.indexOf(a), 1);
                    }
                }
            }
        },
    },
    computed: {
        
    }
})

//钉钉----钉一下功能组件
Vue.component('Ding2', {
    props: ['dinglist'],
    template: `
            <div v-show="dinglist && dinglist.length && dinglist.length>0" style="display:inline-block;">
                <el-button type="primary" v-on:click="Ding">钉一下</el-button>
            </div>`,
    data: function () {
        return {
            UserList: [],
            formLabelWidth: '120px'
        }
    },
    methods: {
        Ding() {
            let param = {
                userId: this.dinglist?this.dinglist[0]:'',
                title: '请帮我审核一下流水号为 ' + TaskId + ' 的流程',
                flowName: FlowName,
                taskId: TaskId,
                linkUrl: "eapp://page/approve/approve?index=0"
            }
            let url = 'DingTalkServers/sendOaMessage' + _formatQueryStr(param)

            $.ajax({
                url: url,
                contentType: "application/json; charset=utf-8",
                type: "POST",
                //data: JSON.stringify(param),
                dataType: "json",
                success:  (res) => {
                    console.log(url)
                    console.log(param)
                    console.log(res)
                    dd.device.notification.alert({ message: '已为你催办~', title: '提示信息' })
                },
                error: function (err) {
                    dd.device.notification.alert({ message: '催办失败~', title: '提示信息' })
                    console.error(url)
                    console.log(param)
                    console.error(err)
                }
            })
        }
    }
})
Vue.component('ding', {
    props: ['dinglist'],
    template: `
            <div v-show="dinglist && dinglist.length && dinglist.length>0" style="display:inline-block;">
                <el-button type="primary" v-on:click="Ding">钉一下</el-button>
                <el-dialog title="编辑钉一下内容" :visible.sync="dialogFormVisible">
                  <el-form v-on:submit.native.prevent :model="form">
                    <el-form-item label="钉信息" :label-width="formLabelWidth">
                      <el-input v-model="form.text" auto-complete="off"></el-input>
                    </el-form-item>
                    <el-form-item label="钉类型" :label-width="formLabelWidth">
                      <el-select v-model="form.alertType" placeholder="请选择钉类型">
                        <el-option label="应用内" value="2"></el-option>
                        <el-option label="短信" value="1"></el-option>
                        <el-option label="电话" value="0"></el-option>
                      </el-select>
                    </el-form-item>
                    <el-form-item label="钉时间" :label-width="formLabelWidth">
                      <div class="block">
                        <span class="demonstration">默认</span>
                        <el-date-picker :ediTable="false"
                          v-model="form.alertDate"
                          type="datetime"
                          value-format="yyyy-MM-dd HH:mm"
                          placeholder="选择日期时间">
                        </el-date-picker>
                      </div>
                    </el-form-item>
                  </el-form>
                  <div slot="footer" class="dialog-footer">
                    <el-button @click="dialogFormVisible = false">取 消</el-button>
                    <el-button type="primary" @click="Ding">确 定</el-button>
                  </div>
                </el-dialog>
            </div>`,
    data: function () {
        return {
            DingVisiable: true,
            dialogFormVisible: false,
            form: {
                alertType: '2',
                alertDate: '',
                text: '', 
            },
            UserList:[],
            formLabelWidth: '120px'
        }
    },
    methods: {
        Ding() {
            console.log('你开始钉了')
            var that = this
            var param = {
                users: this.dinglist,//用户列表，userid
                corpId: DingData.CorpId, //加密的企业id
                type: 1, //钉类型 1：image  2：link
                alertType: parseInt(that.form.alertType),
                //alertDate: { "format": "yyyy-MM-dd HH:mm", "value": that.form.alertDate },
                //attachment: {
                //    images: ['~/Content/images/单位LOGO.jpg'],
                //},
                //text: that.form.text,
            }
            console.log(param)
            dd.biz.ding.post(param)
        }
    }
})


Vue.component('sam-addapprover', {
    props: ['preset', 'approvers','type'],
    template: `<div>
                    <el-form-item v-if="type=='approve'" label="审批人" style="margin-bottom:0px;">
                        <span v-if="preset" class="hint">审批人已由管理员预置,并将自动去重</span>
                        <el-button v-else class="button-new-tag" size="small" v-on:click="showInput">+ 添加审批人</el-button>
                    </el-form-item>
                    <el-form-item v-else label="抄送人" style="margin-bottom:0px;">
                        <span v-if="preset" class="hint">抄送人已由管理员预置,并将自动去重</span>
                        <el-button v-else class="button-new-tag" size="small" v-on:click="showInput">+ 添加抄送人</el-button>
                    </el-form-item>
                    <el-form-item>
                        <template v-for="(tag,index) in approvers">
                            <template v-if="index>0 && index< approvers.length+1">
                                <span> , </span>
                            </template>
                            <el-tag :key="tag"
                                    :closable="!preset"
                                    onclick=""
                                    :disable-transitions="false"
                                    v-on:close="handleClose(tag)">
                                {{tag}}
                            </el-tag>
                        </template>
                        <el-input class="input-new-tag"
                                    v-if="inputVisible"
                                    v-model="inputValue"
                                    ref="saveTagInput"
                                    size="small"
                                    v-on:keyup.enter.native="handleInputConfirm"
                                    v-on:blur="handleInputConfirm">
                        </el-input>
                    </el-form-item></div>`,
    data: function () {
        return {
            inputValue: '',
            inputVisible: false
        }
    },
    methods: {
        showInput() {
            this.inputVisible = true;
            this.$nextTick(_ => {
                this.$refs.saveTagInput.$refs.input.focus();
            });
        },
        handleClose(tag) {
            this.approvers.splice(this.approvers.indexOf(tag), 1);
        },
        handleInputConfirm() {
            let inputValue = this.inputValue;
            if (inputValue) {
                this.approvers.push(inputValue);
            }
            this.inputVisible = false;
            this.inputValue = '';
        }
    },
    computed: {
    }
})

//钉钉审批编辑组件
Vue.component('sam-approver-edit', {
    props: ['nodelist', 'dingdata', 'addable','rolelist','flowid','tpthis'],
    template: `<div>
                    <el-form-item label="审批人" style="margin-bottom:0px;">
                        <h5></h5>
                    </el-form-item>
                    <el-form-item>
                        <template v-for="(node,index) in nodelist">
                            <template v-if="addable">
                                <el-tag type="warning" class="nodeTitle" style="width:130px;text-align:center;" 
                                    :id="node.NodeId" >
                                    <span v-on:click="editNode(node,index)">{{node.NodeName}}</span>
                                </el-tag>
                                <i v-if="index>0&&index<nodelist.length-1" v-on:click="deleteNode(node,index)" style="margin-left:-16px;" class="el-icon-circle-close"></i>
                            </template>
                            <template v-else>
                                <el-tag type="warning" class="nodeTitle" style="width:130px;text-align:center;" :id="node.NodeId">
                                    {{node.NodeName}}
                                </el-tag>
                            </template>
                            <template v-for="(p,a) in node.NodePeople">
                                <span v-if="a>0 && node.NodeName!='抄送' && node.ApplyTime" style="margin-left:137px;">&nbsp;</span>
                                <el-tag :key="a"
                                        :closable="false"
                                        onclick="" v-if="node.NodePeople"
                                        :disable-transitions="false"
                                        :class="{'el-tag--danger':dingdata.userid == node.PeopleId[a]}"
                                        style="text-align:center;"
                                        >
                                    {{p}}
                                </el-tag>
                                <span v-if="a < node.NodePeople.length-1">,</span>
                            </template>
                            
                            <template v-for="(ap,a) in node.AddPeople">
                                <span v-if="a>0" style="margin-left:97px;">&nbsp;</span>
                                <el-tag :key="a" 
                                        :closable="false"
                                        v-if="node.AddPeople.length>0"
                                        :disable-transitions="false"
                                        style="text-align:center;"
                                        >
                                    {{ap.name}}
                                </el-tag>
                            </template>

                           <template v-if="node.NodeId >0 && node.NodeName != '结束' && !addable">
                                <el-button class="button-new-tag" size="small" v-on:click="addMember(node)">+ 选人</el-button>
                            </template>

                            <div v-if="index<nodelist.length-1" style="line-height:1px;">
                                <template v-if="addable">
                                    <span style="color:#CACACA;margin-left:60px;font-size:14px;">|</span>
                                    </br>
                                    <i v-on:click="addNode(node)" class="el-icon-circle-plus approve-arrow"  type="primary"></i>
                                    </br>
                                    <span style="color:#CACACA;margin-left:59px;font-size:14px;">⤓</span>
                                    </br>
                                </template>
                                <i v-else class="el-icon-arrow-down approve-arrow"  type="primary"></i>
                                </br>
                            </div>
                        </template>
                    </el-form-item>
                    <el-button type="primary" v-on:click="save">保存节点配置</el-button>
                    <el-dialog :title="dialogName" :visible.sync="dialogFormVisible">
                            <el-form v-on:submit.native.prevent :model="form" :rules="rules" ref="form" label-width="120px" class="demo-ruleForm"
                                     enctype="multipart/form-data">
                                <template>
                                    <el-form-item v-if="form.NodeName != '申请人发起'" label="节点名称" prop="NodeName">
                                        <sam-input :value.sync="form.NodeName" :maxlength="8"></sam-input>
                                    </el-form-item>
                                    <el-form-item v-if="form.NodeName != '申请人发起'" label="审批人配置" required="required">
                                        <sam-group :names.sync="form.NodePeople" :ids.sync="form.PeopleId" :single="!form.IsSend"></sam-group>
                                    </el-form-item>
                                    <el-form-item label="是否可以退回">
                                        <el-radio-group v-model="form.IsBack" :disabled="disabled1">
                                            <el-radio :label="true">是</el-radio>
                                            <el-radio :label="false">否</el-radio>
                                        </el-radio-group>
                                    </el-form-item>
                                    <el-form-item label="退回节点Id">
                                        <sam-input :value.sync="form.BackNodeId" type="number"></sam-input>
                                    </el-form-item>
                                    <el-form-item label="是否是抄送节点">
                                        <el-radio-group v-model="form.IsSend" v-on:change="isSend">
                                            <el-radio :label="true">是</el-radio>
                                            <el-radio :label="false">否</el-radio>
                                        </el-radio-group>
                                    </el-form-item>
                                    <el-form-item label="是否需要选人">
                                        <el-radio-group v-model="form.IsNeedChose">
                                            <el-radio :label="true">是</el-radio>
                                            <el-radio :label="false">否</el-radio>
                                        </el-radio-group>
                                    </el-form-item>
                                    <template v-if="form.IsNeedChose">
                                        <el-form-item label="需要选择的节点">
                                            <sam-checkbox :str.sync="form.ChoseNodeId" :arr="chooseArr" :onchange="onchange"></sam-checkbox>
                                        </el-form-item>
                                        <el-form-item label="需要多选的节点">
                                            <sam-checkbox :str.sync="chooseMore" :arr="needChooseArr"></sam-checkbox>
                                        </el-form-item>
                                        <el-form-item label="必选的节点">
                                            <sam-checkbox :str.sync="chooseMust" :arr="needChooseArr"></sam-checkbox>
                                        </el-form-item>
                                        <el-form-item label="角色选人节点">
                                            <sam-checkbox :str.sync="chooseType" :arr="needChooseArr"></sam-checkbox>
                                        </el-form-item>
                                        <el-form-item v-if="chooseType!=''" v-for="(r,i) of chooseType.split(',')" :key="r" :label="'节点 '+ r +' 选人角色'">
                                            <el-select v-model="chooseRole[i]" style="width:300px;">
                                                <el-option v-for="(v,k) of rolelist" :label="k" :value="k" :key="k"></el-option>
                                            </el-select>
                                        </el-form-item>
                                    </template>
                                    <hr />
                                    <el-form-item>
                                        <el-button v-if="dialogName == '添加节点'" type="primary" v-on:click="onAddSubmit">添加</el-button>
                                        <el-button v-else type="primary" v-on:click="onEditSubmit">编辑</el-button>
                                        <el-button v-on:click="dialogFormVisible = false">取 消</el-button>
                                    </el-form-item>
                                </template>
                            </el-form>
                        </el-dialog>
                </div>`,
    data: function () {
        return {
            inputValue: '',
            NodeId: 0,
            member1: '',
            member2: '',
            inputVisible: false,

            //编辑相关
            disabled1: false,
            dialogName: '编辑节点',
            dialogFormVisible: false,
            form: {
                IsBack: true,
                IsNeedChose: false,
                IsSend: false,
            },
            chooseArr: [],//后续节点
            needChooseArr:[],//后续且需要选人的节点
            chooseMore: '',
            chooseMust: '',
            chooseType: '',
            chooseRole: ['', '', '', '', ''],
            //rolelist: ['管理员', '行政人员'],
            rules: {
                NodeName: { required: true, message: '该项不能为空', trigger: 'blur' }
            }
        }
    },
    methods: {
        //选择需要选人的节点事件
        onchange(arr) {
            console.log('选择需要选人的节点事件')
            console.log(arr)
            this.needChooseArr = arr

        },
        //抄送节点不能退回
        isSend(value) {
            console.log(value)
            if (value) {
                this.form.IsBack = false
                this.disabled1 = true
            } else {
                this.disabled1 = false
            }
        },
        //选人控件添加
        addMember(node) {
            var that = this
            let nodeId = node.NodeId
            let choosed = []
            for (let p of node.AddPeople) {
                choosed.push(p.emplId)
            }
            dd.biz.contact.choose({
                multiple: true, //是否多选： true多选 false单选； 默认true
                users: choosed, //默认选中的用户列表，员工userid；成功回调中应包含该信息
                corpId: DingData.CorpId, //企业id
                max: 10, //人数限制，当multiple为true才生效，可选范围1-1500
                onSuccess: function (data) {
                    console.log(data)
                    var url2 = '/DingTalkServers/getUserDetail?userId=' + data[0].emplId
                    $.ajax({
                        url: url2,
                        dataType: 'json',
                        type: 'POST',
                        data: {},
                        contentType: "application/json; charset=utf-8",
                        success: function (data2) {
                            if (typeof (data2) == 'string') data2 = JSON.parse(data2)
                            for (let node of that.nodelist) {
                                if (node.NodeId == nodeId) {
                                    $("." + nodeId).remove()
                                    data[0].name = data2.name
                                    node.AddPeople = data
                                }
                            }
                        },
                        error: function (err) {
                            console.error(err)
                        }
                    })
                },
                onFail: function (err) { }
            });
        },
        deleteNode(node, index) {
            console.log(this.nodelist)
            console.log(index)
            this.nodelist.splice(index, 1)
            for (let i = 0; i < this.nodelist.length - 1; i++) {
                if (i < index) continue
                this.nodelist[i].PreNodeId = parseInt(this.nodelist[i].PreNodeId) - 1 + ''
                this.nodelist[i].NodeId --
            }
            this.nodelist[this.nodelist.length-1].NodeId--
        },
        //编辑节点提交
        editNode(node, index) {
            
            if (node.NodeName == '结束') return
            this.dialogName = "编辑节点"
            this.chooseArr = []//后续节点
            this.needChooseArr = []//后续且需要选人的节点
            this.chooseMore = ''
            this.chooseMust = ''
            this.chooseType = ''
            this.chooseRole = ['', '', '', '', '']

            this.form.NodePeople = ''
            this.form.PeopleId = ''

            for (let i = node.NodeId + 1; i < this.nodelist.length-1; i++) {
                this.chooseArr.push(i + '')
            }
            let needChooseArr = []
            if (node.ChoseNodeId) needChooseArr = node.ChoseNodeId.split(',')
            if (needChooseArr.length > 0) {
                let moreArr = node.IsSelectMore.split(',')
                let mustArr = node.IsMandatory.split(',')
                let typeArr = node.ChoseType.split(',')
                this.needChooseArr = needChooseArr
                for (let i = 0; i < needChooseArr.length; i++) {
                    if (moreArr[i] == '1') this.chooseMore = this.chooseMore + needChooseArr[i] + ','
                    if (mustArr[i] == '1') this.chooseMust = this.chooseMust + needChooseArr[i] + ','
                    if (typeArr[i] == '1') this.chooseType = this.chooseType + needChooseArr[i] + ','
                }
                this.chooseMore = this.chooseMore.substr(0, this.chooseMore.length - 1)
                this.chooseMust = this.chooseMust.substr(0, this.chooseMust.length - 1)
                this.chooseType = this.chooseType.substr(0, this.chooseType.length - 1)
                if (node.RoleNames) this.chooseRole = [node.RoleNames.split(','),'', '', '', '']
            }
            this.form = _cloneObj(node)
            this.form.NodePeople = ''
            this.form.PeopleId = ''
            if (node.NodePeople) this.form.NodePeople = node.NodePeople.join(',')
            if (node.PeopleId) this.form.PeopleId = node.PeopleId.join(',')
    
            console.log(JSON.stringify(this.form))
            this.dialogFormVisible = true
        },
        onEditSubmit() {
            this.dowithForm()
            let nodes = []
            for (let node of this.nodelist) {
                if (node.NodeId < this.form.NodeId) {
                    nodes.push(node)
                }
            }
            let tmpForm = _cloneObj(this.form)
            if (tmpForm.NodePeople) {
                tmpForm.NodePeople = tmpForm.NodePeople.split(',')
                tmpForm.PeopleId = tmpForm.PeopleId.split(',')
            } 
            nodes.push(tmpForm)
            for (let node of this.nodelist) {
                if (node.NodeId > this.form.NodeId) {
                    nodes.push(node)
                }
            }
            console.log(nodes)
            this.$emit('update:nodelist', _cloneArr(nodes))
            this.dialogFormVisible = false
        },
        //保存节点配置
        save() {
            let param = []
            //for (let n of this.nodelist) {
            //    let node = _cloneObj(n)
            //    if (node.NodePeople) node.NodePeople = node.NodePeople.join(',')
            //    if (node.PeopleId) node.PeopleId = node.PeopleId.join(',')
            //    param.push(node)
            //}
            for (let i = 0; i < this.nodelist.length; i++) {
                let node = _cloneObj(this.nodelist[i])
                if (node.NodePeople) node.NodePeople = node.NodePeople.join(',')
                if (node.PeopleId) node.PeopleId = node.PeopleId.join(',')
                node.NodeId = i
                node.PreNodeId = i + 1 + ''
                node.FlowId = this.flowid
                param.push(node)
            }
            this.tpthis.PostData('FlowInfoNew/UpdateNodeInfos', param, (res) => {
                this.$message({ type: 'success', message: `修改成功` }, tpthis);
            })
        },
        //添加节点提交
        dowithForm() {
            console.log(_cloneObj(this.form))
            this.form.IsSelectMore = ''
            this.form.IsMandatory = ''
            this.form.ChoseType = ''
            this.form.RoleNames = ''
            for (let id of this.form.ChoseNodeId.split(',')) {
                this.chooseMore.indexOf(id) >= 0 ? this.form.IsSelectMore += '1' : this.form.IsSelectMore += '0'
                this.form.IsSelectMore += ','
                this.chooseMust.indexOf(id) >= 0 ? this.form.IsMandatory += '1' : this.form.IsMandatory += '0'
                this.form.IsMandatory += ','
                this.chooseType.indexOf(id) >= 0 ? this.form.ChoseType += '1' : this.form.ChoseType += '0'
                this.form.ChoseType += ','
            }
            this.form.IsSelectMore = this.form.IsSelectMore.substr(0, this.form.IsSelectMore.length - 1)
            this.form.IsMandatory = this.form.IsMandatory.substr(0, this.form.IsMandatory.length - 1)
            this.form.ChoseType = this.form.ChoseType.substr(0, this.form.ChoseType.length - 1)
            for (let i = 0; i < this.chooseType.split(',').length; i++) {
                this.form.RoleNames += this.chooseRole[i]
                this.form.RoleNames += ','
            }
            this.form.RoleNames = this.form.RoleNames.substr(0, this.form.RoleNames.length - 1)
            console.log(_cloneObj(this.form))
        },
        addNode(node) {
            //初始化参数
            this.dialogName = "添加节点"
            this.chooseArr = []//后续节点
            this.needChooseArr = []//后续且需要选人的节点
            this.chooseMore = ''
            this.chooseMust = ''
            this.chooseType = ''
            this.chooseRole = ['', '', '', '', '']
            this.form.NodeId = node.NodeId + 1
            this.form.FlowId = node.FlowId
            this.form.PreNodeId = node.NodeId + 2 + ''
            this.form.IsBack = true
            this.form.IsNeedChose = false
            this.form.IsSend = false
            this.form.NodePeople = ''
            this.form.PeopleId = ''
            this.form.BackNodeId = '0'
            this.form.ChoseNodeId = ''
            this.form.IsSelectMore = ''
            this.form.IsMandatory = ''
            this.form.ChoseType = ''
            this.form.RoleNames = ''
            console.log(this.form)
            for (let i = node.NodeId + 2; i < this.nodelist.length; i++) {
                this.chooseArr.push(i + '')
            }
            this.dialogFormVisible = true

        },
        onAddSubmit() {
            this.dowithForm()
            let nodes = []
            for (let node of this.nodelist) {
                if (node.NodeId < this.form.NodeId) {
                    nodes.push(node)
                } 
            }
            let tmpForm = _cloneObj(this.form)
            if (tmpForm.NodePeople) {
                tmpForm.NodePeople = tmpForm.NodePeople.split(',')
                tmpForm.PeopleId = tmpForm.PeopleId.split(',')
            } 
            nodes.push(tmpForm)
            for (let node of this.nodelist) {
                if (node.NodeId >= this.form.NodeId) {
                    node.NodeId++
                    node.PreNodeId = node.NodeId + 1 + ''
                    nodes.push(node)
                }
            }
            console.log(nodes)
            this.$emit('update:nodelist', _cloneArr(nodes))
            this.dialogFormVisible = false
        }
    },
    computed: {

    }
})

//流程配置相关
let editFlow = {}
let editSort = {}