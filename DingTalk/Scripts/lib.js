﻿//实例总参数
var FlowId = 0 //当前审批类别ID
var NodeId = 0 //审批节点ID
var TaskId = 0 //审批任务ID
var state = ''//流程状态
var State = 0 //多异步辅助状态
var Index = 0 //审批列表类型参数 0-带我审批 1-我已审批 2-我发起的 3-抄送我的
var UrlObj = {} //url参数对象
var QueryObj = {} //获取url参数对象
var Id = 0 //自增task表的id
var UserList = [] //所有用户数据
var imgConfig = [] //审批主页图片和路由配置
var ReApprovalTempData = {} //重新发起审批保存的临时数据
var imageList = []
var fileList = []
var pdfList = []
let jinDomarn = 'http://wuliao5222.55555.io:35705/api/'
//let serverUrl = 'http://17e245o364.imwork.net:49415/'
let serverUrl = 'http://47.96.172.122:8093/'
let ProjectTypes = ['自研项目', '纵向项目', '横向项目', '测试项目']
let PTypes = [
    { label: '研发类', value: '研发类', children: [{ value: '自研', label: '自研' }, { value: '横向', label: '横向' }, { value: '纵向', label: '纵向' }, { value: '测试', label: '测试' }] },
    { label: '产品类', value: '产品类', children: [{ value: '产品', label: '产品' }, { value: '测试', label: '测试' }] },
    { label: '教育类', value: '教育类', children: [{ value: '教育', label: '教育' }, { value: '测试', label: '测试' }] }
]
let status = ["在研", "已完成", "终止"]
let DeptNames = ['', '智慧工厂事业部', '数控一代事业部', '机器人事业部', '行政部', '财务部', '制造试验部', '项目推进部', '自动化事业部']
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
    GetData('/FlowInfoNew/GetFlowStateCounts?ApplyManId=' + DingData.userid, (res) => {
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

function _getDate(split) {
    var d = new Date()
    var year = d.getFullYear()
    var month = d.getMonth() + 1
    var day = d.getDate()
    if (month < 10) month = '0' + month
    if (day < 10) day = '0' + day
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
    switch (type) {
        case 'd': return days; break;
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

//获取审批表单信息
function getFormData(demo) {
    var url = "/FlowInfoNew/GetApproveInfo?TaskId=" + TaskId + "&ApplyManId=" + DingData.userid
    GetData(url, (res) => {
        imageList = []
        fileList = []
        pdfList = []
        handleUrlData(res, demo)
        taskId = res.TaskId
        allData = res
        demo.ruleForm = res
        demo.getNodeInfo()
        demo.GetDingList(taskId)
        demo.getApproInfo()
    },demo)
}
function handleUrlData(data,demo) {
    imageList = []
    fileList = []
    pdfList = []
    if (data.ImageUrl && data.ImageUrl.length > 5) {
        var tempList = data.ImageUrl.split(',')
        for (let img of tempList) {
            imageList.push({
                name: 'hello.jpg',
                url: document.location + (img.substring(2)).replace(/\\/g, "/")
            })
        }
        demo.imageList = imageList
    }
    if (data.FileUrl && data.FileUrl.length > 5) {
        FileUrl = data.FileUrl
        var urlList = data.FileUrl.split(',')
        var oldUrlList = data.OldFileUrl.split(',')
        var MediaIdList = data.MediaId ? data.MediaId.split(',') : []
        for (var i = 0; i < urlList.length; i++) {
            fileList.push({
                name: oldUrlList[i],
                path: urlList[i].replace(/\\/g, "/"),
                url: document.location + (urlList[i].substring(2)).replace(/\\/g, "/"),
                mediaId: MediaIdList[i]
            })
        }
        demo.fileList = fileList
    }
    if (data.FilePDFUrl && data.FilePDFUrl.length > 5) {
        FilePDFUrl = data.FilePDFUrl
        var urlList = data.FilePDFUrl.split(',')
        var oldUrlList = data.OldFilePDFUrl.split(',')
        var MediaIdList = data.MediaIdPDF ? data.MediaIdPDF.split(',') : []
        var stateList = data.PdfState ? data.PdfState.split(',') : []
        for (var i = 0; i < urlList.length; i++) {
            pdfList.push({
                name: oldUrlList[i],
                url: document.location + (urlList[i].substring(2)).replace(/\\/g, "/"),
                mediaId: MediaIdList[i],
                state: stateList[i]
            })
        }
        demo.pdfList = pdfList
    }
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
        nodeList: [],
        nodeInfo: {},
        data: [],
        tableData: [],
        fileList: [],
        pdfList: [],
        mediaList: [],
        specialRoleNames: [],
        ruleForm: {},
        preApprove: true,
        isBack: false,
        projectList: [],
        disablePage: false,
        preUrl: '',//预览图片
        showPre: false,
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
                { required: true, message: '内容不能为空！', trigger: 'change' }
            ],
            Title: [
                { required: true, message: '标题内容不能为空！', trigger: 'change' }
            ],
            Type: [
                { required: true, message: '类别不能为空！', trigger: 'change' }
            ],
            Name: [
                { required: true, message: '名称不能为空！', trigger: 'change' }
            ],
            CarNumber: [
                { required: true, message: '车牌号不能为空！', trigger: 'change' }
            ],
            UnitPricePerKilometre: [
                { required: true, message: '每公里单价不能为空！', trigger: 'change' }
            ],
            Abstract: [
                { required: true, message: '内容简介不能为空！', trigger: 'change' }
            ],
            ProjectName: [
                { required: true, message: '内容不能为空！', trigger: 'change' }
            ],
            inputProjectId: [
                { required: true, message: '内容不能为空！', trigger: 'change' }
            ],
            inputProjectName: [
                { required: true, message: '内容不能为空！', trigger: 'change' }
            ], 
            Price: [
                { required: true, message: '价格不能为空！', trigger: 'change' },
                { type: 'number', message: '必须为数字值' }
            ],
            Unit: [
                { required: true, message: '单位不能为空！', trigger: 'change' }
            ],
            Count: [
                { required: true, message: '数量不能为空！', trigger: 'change' },
                { type: 'number', message: '必须为数字值' }
            ],
            time: [
                { required: true, message: '时长不能为空！', trigger: 'change' }
            ],
            DateTime: [
                { required: true, message: '日期不能为空！', trigger: 'change' }
            ],
            StartTime: [
                { required: true, message: '开始时间不能为空！', trigger: 'change' }
            ], 
            EndTimeTime: [
                { required: true, message: '结束时间不能为空！', trigger: 'change' }
            ], 
            UseTime: [
                { required: true, message: '时长不能为空！', trigger: 'change' }
            ],
            OverTimeContent: [
                { required: true, message: '不能为空！', trigger: 'change' }
            ], 
            EffectiveTime: [
                { required: true, message: '有效时间不能为空！', trigger: 'change' }
            ],
            Purpose: [
                { required: true, message: '用途不能为空！', trigger: 'change' }
            ],
            //文件阅办单表单
            MainContent: [
                { required: true, message: '文件标题不能为空！', trigger: 'blur' }
            ],
            ReceivingUnit: [
                { required: true, message: '来文单位不能为空！', trigger: 'blur' }
            ],
            ReceivingTime: [
                { required: true, message: '时间不能为空！', trigger: 'blur' }
            ], 
            MainIdea: [
                { required: true, message: '主要内容不能为空！', trigger: 'blur' }
            ], 
            Suggestion: [
                { required: true, message: '拟办意见不能为空！', trigger: 'blur' }
            ],
            Leadership: [
                { required: true, message: '领导阅示不能为空！', trigger: 'blur' }
            ], 
            Review: [
                { required: true, message: '部门阅办情况不能为空！', trigger: 'blur' }
            ],
            HandleImplementation: [
                { required: true, message: '办理落实情况不能为空！', trigger: 'blur' }
            ],
        },
        pickerOptions: pickerOptions,
        CompanyNames: CompanyNames,
        showAddProject: false,
        currentPage: 1,
        totalRows: 0,
        pageSize: 5,
        dingList: [],
        PTypes: PTypes
    },
    created:function() {
        
    },
    methods: {
        doWithErrcode(error) {
            if (!error) {
                return 1
            }
            if (error && error.errorCode != 0) {
                this.elementAlert('报错信息',error.errorMessage)
                return 1
            }
            return 0
        },
        GetData(url, succe) {
            var that = this
            $.ajax({
                url: url,
                type: 'GET',
                success: function (res) {
                    console.log(url)
                    console.log(res)
                    if (that.doWithErrcode(res.error)) {
                        return
                    }
                    succe(res.data)
                },
                error: function (err) {
                    console.error(url)
                    console.error(err)
                }
            })
        },
        PostData(url, param, succe) {
            var that = this
            $.ajax({
                url: url,
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(param),
                success: function (res) {
                    console.log(url)
                    console.log(param)
                    console.log(res)
                    if (that.doWithErrcode(res.error)) {
                        return
                    }
                    succe(res.data)
                },
                error: function (err) {
                    console.error(url)
                    console.error(err)
                }
            })
        },
        //提交审批
        approvalSubmit(formName, param, callBack, param2 = {}) {
            if (!DingData.userid) return
            var that = this
            this.$refs[formName].validate((valid) => {
                if (valid) {
                    that.disablePage = true
                    var paramArr = []
                    var applyObj = {
                        "ApplyMan": DingData.nickName,
                        "ApplyManId": DingData.userid,
                        "Dept": DingData.departName,
                        "NodeId": "0",
                        "ApplyTime": _getTime(),
                        "IsEnable": "1",
                        "FlowId": FlowId + '',
                        "IsSend": false,
                        "State": "1",
                    }
                    for (let p in param) {
                        applyObj[p] = param[p]
                    }
                    paramArr.push(applyObj)
                    for (let node of that.nodeList) {
                        if ((that.nodeInfo.IsNeedChose && that.nodeInfo.ChoseNodeId && that.nodeInfo.ChoseNodeId.indexOf(node.NodeId) >= 0 ) || (that.addPeopleNodes && that.addPeopleNodes.indexOf(node.NodeId) >= 0)|| (node.NodeName.indexOf('申请人') >= 0 && node.NodeId>0)) {
                            if (node.AddPeople.length == 0) {
                                this.$alert('您尚未选择审批人', '提交错误', {
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
                                    "IsSend": false,
                                    "State": 0,
                                    "OldFileUrl": null,
                                    "IsBack": null
                                }
                                for (let p2 in param2) {
                                    tmpParam[p2] = param2[p2]
                                }
                                paramArr.push(tmpParam)
                            }
                        }
                    }
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
        aggreSubmit(param, param2 = {}) {
            if (!DingData.userid) return
            this.disablePage = true
            var paramArr = []
            var that = this
            paramArr.push({
                "TaskId": TaskId,
                "ApplyMan": DingData.nickName,
                "ApplyManId": DingData.userid,
                "Dept": DingData.departName,
                "NodeId": NodeId,
                "ApplyTime": _getTime(),
                "IsEnable": "1",
                "FlowId": FlowId,
                "IsSend": "false",
                "State": "1",
            })
            for (let p in param) {
                paramArr[0][p] = param[p]
            }
            for (let node of this.nodeList) {
                if ( (that.nodeInfo.IsNeedChose && that.nodeInfo.ChoseNodeId && that.nodeInfo.ChoseNodeId.indexOf(node.NodeId) >= 0) || (that.addPeopleNodes && that.addPeopleNodes.indexOf(node.NodeId) >= 0) ) {
                    if (node.AddPeople.length == 0) {
                        this.$alert('您尚未选择审批人', '提交错误', {
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
                            "ApplyTime": null,
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
                        //if (this.FlowId == 31) tmpParam.IsPost = true
                        for (let p2 in param2) {
                            tmpParam[p2] = param2[p2]
                        }
                        paramArr.push(tmpParam)
                    }
                }
            }
            console.log(JSON.stringify(paramArr))
            //return
            this.PostData("/FlowInfoNew/SubmitTaskInfo", paramArr, (res) => {
                this.$alert('审批成功', '操作成功', {
                    confirmButtonText: '确定',
                    callback: action => {
                        loadPage('/main/Approval_list')
                    }
                });
            })
        },
        //退回审批 
        returnBk() {
            let param = {
                "Id": this.ruleForm.Id,
                "Remark": this.ruleForm.Mark
            }
            this.returnSubmit(param)
        },
        //撤回审批
        rebackSubmit() {
            this.disablePage = true
            var param = {
                "Id": this.ruleForm.Id,
                "NodeId": 0,
                "Remark": this.ruleForm.Mark
            }
            this.returnSubmit(param)
        },
        returnSubmit(option) {
            this.disablePage = true
            var param = {
                "TaskId": TaskId,
                "ApplyMan": DingData.nickName,
                "ApplyManId": DingData.userid,
                "Dept": DingData.departName,
                "NodeId": NodeId,
                "ApplyTime": _getTime(),
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
                this.$alert('审批成功', '操作成功', {
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
        //重新发起审批
        reApproval() {
            this.disablePage = true
            var tmpPdfList = []
            for (let pdf of this.pdfList) {
                if (pdf.state == '1') tmpPdfList.push(pdf)
            }
            ReApprovalTempData = {
                valid: true,
                data: this.data,
                dataArr: this.dataArr,
                ProjectId: this.ruleForm.ProjectId,
                Title: this.ruleForm.Title,
                imageList: this.imageList,
                fileList: this.fileList,
                pdfList: tmpPdfList,
                items: items
            }
            for (let img of imgConfig) {
                if (img.flowId == FlowId) {
                    loadPage(img.url)
                }
            }
        },
        
        //翻頁相關事件
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
        //下拉框选择项目
        selectProject(id) {
            console.log(id)
            let project = {}
            for (var proj of this.projectList) {
                if (proj.ProjectId == id) {
                    this.ruleForm.ProjectName = proj.ProjectName
                    project = proj
                }
            }
            //this.ruleForm.Title = project.ProjectName + ' - 编号：' + project.ProjectId
            for (let i = 0; i < this.nodeList.length; i++) {
                if (this.nodeList[i].NodeName.indexOf('项目负责人') >= 0) {
                    this.nodeList[i].AddPeople = [{
                        name: project.ResponsibleMan,
                        emplId: project.ResponsibleManId
                    }]
                    $("." + i).remove()
                    $("#" + i).after('<span class="el-tag ' + i + '" style="width: 60px; text-align: center; ">' + project.ResponsibleMan.substring(0, 3) + '</span >')
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

        //获取审批/抄送 相关人员列表
        getNodeInfo() {
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
            })
            
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
        getApproInfo() {
            var url = "/FlowInfoNew/getnodeinfo?FlowId=" + FlowId + "&nodeid=" + NodeId
            this.GetData(url, (res) => {
                this.nodeInfo = res[0]
                NodeId = res[0].NodeId
                this.preApprove = !res[0].IsNeedChose
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
            DingTalkPC.biz.contact.choose({
                multiple: false, //是否多选： true多选 false单选； 默认true
                users: [], //默认选中的用户列表，员工userid；成功回调中应包含该信息
                corpId: DingData.CorpId, //企业id
                onSuccess: function (data) {
                    that.ResponsibleMan = data
                    console.log(data)
                },
                onFail: function (err) { }
            });
        },
        //选多人
        addGroup() {
            var that = this
            console.log('addGroup')
            DingTalkPC.biz.contact.choose({
                multiple: true, //是否多选： true多选 false单选； 默认true
                users: [], //默认选中的用户列表，员工userid；成功回调中应包含该信息
                corpId: DingData.CorpId, //企业id
                onSuccess: function (data) {
                    that.groupPeople = data
                    console.log(data)
                },
                onFail: function (err) { }
            });
        },
        //搜索物料列表
        searchCode(formName) {
            var that = this
            if (!this.searchForm.name) return
            var url = '/Purchase/GetICItem?Key=' + that.searchForm.name
            $.ajax({
                url: url,
                success: function (data) {
                    console.log(url)
                    data = JSON.parse(data)
                    console.log(data)
                    that.data = data
                    that.totalRows = data.length
                    that.getData()
                }
            })
        },
        //图片上传事件
        beforePictureUpload(file) {
            console.log('before file')
            console.log(file)
            file.name = 'helloWorld'
            const isJPG = file.type === 'image/jpeg'
            const isPNG = file.type === 'image/png'
            const isLt2M = file.size / 1024 / 1024 < 2
            if (!(isJPG || isPNG)) {
                this.$message.error('上传图片只能是 JPG 或 PNG 格式!')
                return false
            }
            if (!isLt2M) {
                this.$message.error('上传图片大小不能超过 2MB!')
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
            this.changePictureList(fileList)
        },
        handlePictureCardSuccess(response, file, fileList) {
            this.changePictureList(fileList)
        },
        changePictureList(fileList) {
            console.log(fileList)
            this.ruleForm['ImageUrl'] = ''
            this.ruleForm['OldImageUrl'] = ''
            for (var i = 0; i < fileList.length; i++) {
                this.ruleForm.ImageUrl += fileList[i].response.Content
                this.ruleForm.OldImageUrl += fileList[i].name
                if (i == fileList.length - 1) break
                this.ruleForm.ImageUrl += ','
                this.ruleForm.OldImageUrl += ','
            }
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
        BeforeFileUpload(file) {
            file.name = 'helloWorld'
            isPdf = false
            const isLt2M = file.size / 1024 / 1024 < 10
            if (!isLt2M) {
                this.$message.error('上传图片大小不能超过 10MB!')
                return false
            }
            return true
        },
        beforeExcelUpload(file) {
            const isExcel = (file.name.substr(-3) == 'xls' || file.name.substr(-4) == 'xlsx')
            const isLt2M = file.size / 1024 / 1024 < 4
            if (!isExcel) {
                this.$message.error('只能上传 excel 文件!')
                return false
            }
            if (!isLt2M) {
                this.$message.error('上传文件大小不能超过 4MB!')
                return false
            }
            return true
        },
        HandleFileRemove(file, fileList) {
            this.fileList = _cloneArr(fileList)
        },
        HandleFileSuccess(response, file, fileList) {
            var that = this
            const loading = this.$loading({
                lock: true,
                text: 'Loading',
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
                    if (data.media_id) {
                        console.log(data.media_id)
                        that.mediaList.push(data.media_id)
                        fileList[fileList.length - 1]['mediaid'] = data.media_id
                        //that.ruleForm
                    } else {
                        console.log('无media_di')
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
            this.ruleForm.FilePDFUrl = ''
            this.ruleForm.OldFilePDFUrl = ''
            this.ruleForm.FileUrl = ''
            this.ruleForm.OldFileUrl = ''
            this.ruleForm.MediaId = ''
            if (this.pdfList) {
                for (var i = 0; i < this.pdfList.length; i++) {
                    this.ruleForm.FilePDFUrl += this.pdfList[i].response.Content
                    this.ruleForm.OldFilePDFUrl += this.pdfList[i].name
                    this.ruleForm.MediaIdPDF += this.pdfList[i].mediaid
                    if (i == this.pdfList.length - 1) break
                    this.ruleForm.FilePDFUrl += ','
                    this.ruleForm.OldFilePDFUrl += ','
                    this.ruleForm.MediaIdPDF += ','
                }
            }
            if (this.fileList) {
                for (var i = 0; i < this.fileList.length; i++) {
                    this.ruleForm.FileUrl += this.fileList[i].response.Content
                    this.ruleForm.OldFileUrl += this.fileList[i].name
                    this.ruleForm.MediaId += this.fileList[i].mediaid
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
            if (file.name.indexOf(' ') >= 0) {
                this.$alert('文件名不能带空格', '上传失败', {
                    confirmButtonText: '确定',
                });
                return false
            }
            file.name = 'helloWorld'
            const isPdfType = file.type === 'application/pdf'
            const isLt2M = file.size / 1024 / 1024 < 10
            if (!isPdfType) {
                this.$message.error('上传图片只能是 PDF 格式!')
                return false
            }
            if (!isLt2M) {
                this.$message.error('上传文件大小不能超过 10MB!')
                return false
            }
            return true
        },
        HandlePdfFileRemove(file, fileList) {
            this.pdfList = _cloneArr(fileList) 
        },
        handlePdfFileSuccess(response, file, fileList) {
            var that = this
            const loading = this.$loading({
                lock: true,
                text: 'Loading',
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
                        fileList[fileList.length - 1]['mediaid'] = data.media_id
                    } else {
                        console.log('无media_di')
                    }
                    that.pdfList = _cloneArr(fileList)
                    loading.close()
                }
            })
        },

        //根据taskId获取下一个需要审批的人，即要钉的人
        GetDingList(taskId) {
            this.GetData('/DingTalkServers/Ding' + _formatQueryStr({ taskId: taskId }), (res) => {
                if (!res) return
                this.dingList = []
                this.dingList.push(res.applyManId)
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
                    if (data.error && data.error.errorCode != 0) {
                        that.elementAlert('报错信息', data.error.errorMessage)
                        return
                    }
                    console.log(url)
                    console.log(param)
                    console.log(data)
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
                        that.elementAlert('报错信息', data.error.errorMessage)
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
    }
}

function doWithErrcode(error,demo) {
    if (!error) {
        return 1
    }
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
            console.log(url)
            console.log(res)
            if (doWithErrcode(res.error,demo)) {
                return
            }
            succe(res.data)
        },
        error: function (err) {
            console.error(url)
            console.error(err)
        }
    })
}

function PostData(url, param, succe) {
    $.ajax({
        url: url,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(param),
        success: function (res) {
            console.log(url)
            console.log(param)
            console.log(res)
            if (doWithErrcode(res.error)) {
                return
            }
            succe(res.data)
        },
        error: function (err) {
            console.error(url)
            console.error(err)
        }
    })
}

//钉钉审批组件
Vue.component('sam-approver-list', {
    props: ['nodedata', 'nodelist', 'single', 'specialRoles', 'specialRoleNames'],
    template: `<div>
                    <el-form-item label="审批人" style="margin-bottom:0px;">
                        <h5></h5>
                    </el-form-item>
                    <el-form-item>
                        <template v-for="(node,index) in nodelist">
                            <el-tag type="warning" class="nodeTitle" style="width:130px;text-align:center;" :id="node.NodeId">
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
                            
                            <template v-for="(ap,a) in node.AddPeople">
                                <span v-if="a>0" style="margin-left:97px;">&nbsp;</span>
                                <el-tag :key="a" 
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

                           <template v-if="nodedata.IsNeedChose && nodedata.ChoseNodeId && nodedata.ChoseNodeId.indexOf(node.NodeId) >= 0">
                                <el-button class="button-new-tag" v-if="!specialRoles || specialRoles.length==0" size="small" v-on:click="addMember(node.NodeId,node.NodeName)">+ 选人</el-button>
                                <el-select placeholder="请选择审批人" v-for="role in specialRoles" :key="role.name" v-if="role.name == specialRoleNames[0] && role.name == node.NodeName" v-model="member1"
                                 style="margin-left:10px;" size="small" v-on:change="selectSpecialMember(member1,node.NodeId)">
                                    <el-option
                                      v-for="member in role.members"
                                      :key="member.emplId"
                                      :label="member.name"
                                      :value="JSON.stringify(member)">
                                    </el-option>
                                </el-select>
                                <el-select placeholder="请选择审批人" v-for="role in specialRoles" :key="role.name" v-if="role.name == specialRoleNames[1] && role.name == node.NodeName"" v-model="member2"
                                 style="margin-left:10px;" size="small" v-on:change="selectSpecialMember(member2,node.NodeId)">
                                    <el-option
                                      v-for="member in role.members"
                                      :key="member.emplId"
                                      :label="member.name"
                                      :value="JSON.stringify(member)">
                                    </el-option>
                                </el-select>
                            </template>

                            <div v-if="index<nodelist.length-1" style="line-height:1px;">
                                <i class="el-icon-arrow-down approve-arrow"  type="primary"></i>
                                </br>
                            </div>
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
            NodeId: 0,
            member1: '',
            member2: '',
            inputVisible: false
        }
    },
    methods: {
        addNode() {

        },
        //选人控件添加
        addMember(nodeId, nodename) {
            var that = this
            DingTalkPC.biz.contact.choose({
                multiple: !that.single, //是否多选： true多选 false单选； 默认true
                users: [], //默认选中的用户列表，员工userid；成功回调中应包含该信息
                corpId: DingData.CorpId, //企业id
                max: 10, //人数限制，当multiple为true才生效，可选范围1-1500
                onSuccess: function (data) {
                    console.log(data)
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
        selectSpecialMember(userInfo, nodeId) {
            console.log(userInfo)
            userInfo = JSON.parse(userInfo)
            console.log(userInfo)
            console.log(nodeId)
            for (let node of this.nodelist) {
                if (node.NodeId != nodeId)
                    continue
                node.AddPeople = [userInfo]
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
        handleClose(emplId) {
            for (let node of this.nodelist) 
                for (var i = 0; i < node.AddPeople.length; i++) {
                    if (node.AddPeople[i].emplId == emplId)
                        node.AddPeople.splice(i, 1)
                }
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

//钉钉----钉一下功能组件
Vue.component('Ding2', {
    props: ['dinglist', 'userid','flowid'],
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
                userId: this.dinglist[0],
                title: '请帮我审核一下流水号为 ' + this.flowid + ' 的流程',
                applyMan: this.userid,
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
                    DingTalkPC.device.notification.alert({ message: '已为你催办~', title: '提示信息' })
                },
                error: function (err) {
                    DingTalkPC.device.notification.alert({ message: '催办失败~', title: '提示信息' })
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
                  <el-form :model="form">
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
                        <el-date-picker
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
            DingTalkPC.biz.ding.post(param)
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


