//实例总参数
var FlowId = 0 //当前审批类别ID
var NodeId = 0 //审批节点ID
var TaskId = 0 //审批任务ID
var State = 0 //多异步辅助状态
var UrlObj = {} //url参数对象
var QueryObj = {} //获取url参数对象
var Id = 0 //自增task表的id
var UserList = [] //所有用户数据
var imgConfig = [] //审批主页图片和路由配置
var ReApprovalTempData = {} //重新发起审批保存的临时数据

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

function _getTime() {
    var split = "-"
    var d = new Date()
    var year = d.getFullYear()
    var month = d.getMonth() + 1
    var day = d.getDate()
    var hour = d.getHours()
    var minute = d.getMinutes()
    if (month < 10) month = '0' + month
    if (day < 10) day = '0' + day
    return year + split + month + split + day + ' ' + hour + ':' + minute
}

function isArray(o) {
    return Object.prototype.toString.call(o) == '[object Array]';
}

function _getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
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
        specialRoleNames: [],
        preApprove: true,
        isBack: false,
        disablePage: false,
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
            ]
        },
        pickerOptions: pickerOptions,
        showAddProject: false,
        currentPage: 1,
        totalRows: 0,
        pageSize: 5
    },
    created:function() {
        
    },
    methods: {
        resetForm(formName) {
            this.$refs[formName].resetFields();
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
            var that = this
            var url = "/FlowInfo/GetSign?FlowId=" + FlowId + "&TaskId=" + TaskId
            if (TaskId == 0 || TaskId == '0') {
                url = "/FlowInfo/GetSign?FlowId=" + FlowId
            }
            $.ajax({
                url: url,
                type: "GET",
                success: function (result) {
                    console.log("获取节点数据 GetSign nodeList")
                    result = JSON.parse(result)
                    console.log(url)
                    console.log(result)
                    that.isBack = result[0].IsBack
                    that.nodeList = _cloneArr(result)
                    for (let node of that.nodeList) {
                        if (node.NodeId == 0)
                            node.NodePeople = [DingData.nickName]
                        if (node.ApplyMan && node.ApplyMan.length > 0)
                            node.NodePeople = node.ApplyMan.split(',')
                        node['AddPeople'] = []
                        }
                    
                    //that.preApprove = !data[0].IsNeedChose
                },
                error: function (err) {
                    console.log(err);
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
                    that.projestList = data
                },
                error: function (err) {
                    console.log(err);
                }
            })
        },
        //获取審批節點數據
        getApproInfo() {
            var that = this
            var url = "/FlowInfo/getnodeinfo?FlowId=" + FlowId + "&nodeid=" + NodeId
            $.ajax({
                url: url,
                dataType: "json",
                success: function (data) {
                    console.log("當前節點信息")
                    console.log(url)
                    console.log(data)
                    that.nodeInfo = data[0]
                    NodeId = data[0].NodeId
                    that.preApprove = !data[0].IsNeedChose
                },
                error: function (err) {
                    console.log(err);
                }
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
                confirmButtonText: '确定'
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
        //get获取接口数据通用方法
        _getData(url, callBack, param = {}, alertStr, alertTitle = '提示信息') {
            var that = this
            url = url += _formatQueryStr(param)
            console.log(url)
            console.log(param)
            $.ajax({
                url: url,
                dataType: "json",
                success: function (data) {
                    console.log(data)
                    if (alertStr) {
                        that.$alert(alertStr.length > 2 ? alertStr : data.errorMessage, alertTitle, {
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
        },
        //post提交接口数据通用方法
        _postData(url, callBack, param = {}, alertStr, alertTitle = '提示信息') {
            var that = this
            console.log(url)
            console.log(param)
            $.ajax({
                url: url,
                contentType: "application/json",
                type: "POST",
                data: param,
                dataType: "json",
                success: function (data) {
                    console.log(data)
                    if (alertStr) {
                        that.$alert(alertStr.length > 2 ? alertStr : data.errorMessage, alertTitle, {
                            confirmButtonText: '确定',
                            callback: action => {
                                if (callBack) callBack()
                            }
                        })
                    } else {
                        callBack()
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(XMLHttpRequest);
                }
            })
        }
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



//钉钉审批组件
Vue.component('sam-approver-list', {
    props: ['preset', 'nodelist', 'type', 'nodeid', 'single', 'specialRoles', 'specialRoleNames'],
    template: `<div>
                    <el-form-item label="审批人" style="margin-bottom:0px;">
                    </el-form-item>
                    <el-form-item>
                        <template v-for="(node,index) in nodelist">
                            <el-tag type="warning" class="nodeTitle" style="width:130px;text-align:center;" :id="node.NodeId">
                                {{node.NodeName}}
                            </el-tag>

                            <template v-for="(p,a) in node.NodePeople">
                                <span v-if="a>0 && node.NodeName!='抄送'" style="margin-left:137px;">&nbsp;</span>
                                <el-tag :key="a"
                                        :closable="false"
                                        onclick="" v-if="node.NodePeople"
                                        :disable-transitions="false"
                                        :type="node.ApplyTime?'success':''"
                                        :class="{'el-tag--danger':node.IsBack}"
                                        style="width:60px;text-align:center;"
                                        >
                                    {{p}}
                                </el-tag>
                                
                                <span v-if="node.NodeName=='抄送' && a < node.NodePeople.length-1">,</span>
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
                                        style="width:60px;text-align:center;"
                                        >
                                    {{ap.name}}
                                </el-tag>
                            </template>

                           <template v-if="!preset && !node.NodePeople && node.NodeName!='结束'">
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
                    console.log(nodeId)
                    console.log(data)
                    for (let node of that.nodelist) {
                        if (node.NodeId == nodeId) {
                            node.AddPeople = data
                            for (let d of data) {
                                $("#" + nodeId).after('<span class="el-tag" style="width: 60px; text-align: center; ">' + d.name + '</span >')
                            }
                        }
                    }
                    console.log(that.nodelist)
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


  //  < template v-for="(p,b) in node.AddPeople" >
  //      <el-tag :key="b"
  //                                      :closable="true"
  //          onclick="" 
  //                                      :disable-transitions="false"
  //          v-on:close="handleClose(p.emplId)">
  //          {{ p.name }}
  //      </el-tag>
  //</template >