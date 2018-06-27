//实例总参数
var FlowId = 0 //当前审批类别ID
var NodeId = 0 //审批节点ID
var TaskId = 0 //审批任务ID
var State = 0 //多异步辅助状态
var UrlObj = {} //url参数对象
var QueryObj = {} //获取url参数对象
var Id = 0 //自增task表的id
var UserList = [] //所有用户数据

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
    var newObj = {}
    for (var o in obj) {
        newObj[o]=obj[o]
    }
    return newObj
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
        disablePage: false,
        rules: {
            name: [
                { required: true, message: '名称不能为空', trigger: 'blur' },
                { min: 1, max: 5, message: '长度在 1 到 5 个字符', trigger: 'blur' }
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
            ]
        },
        pickerOptions: pickerOptions,
        showAddProject: false,
        currentPage: 1,
        totalRows: 0,
        pageSize: 5
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
        //添加项目
        addProject() {
            var that = this
            var param = {
                "ProjectName": this.ruleForm.inputProjectName,
                "CreateTime": _getTime(),
                "IsEnable": true,
                "IsFinish": false,
                "DeptName": "智慧工厂事业部",
                "ApplyMan": DingData.nickName,
                "ApplyManId": DingData.userid,
                "StartTime": _dateToString(this.ruleForm.Time[0]),
                "EndTime": _dateToString(this.ruleForm.Time[1]),
                "ProjectId": this.ruleForm.inputProjectId
            }
            console.log(param)
            $.ajax({
                url: "/Project/AddProject",
                type: "POST",
                dataType: "json",
                async: false,
                data: JSON.stringify(param),
                success: function (data) {
                    that.projestList.push({
                        ProjectId: that.ruleForm.inputProjectId,
                        ProjectName: that.ruleForm.inputProjectName
                    })
                    console.log(data);
                    that.showAddProject = false
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(XMLHttpRequest.status);
                }
            })
        },
        //获取节点数据GetNodeInfo
        getNodeInfo() {
            var that = this
            $.ajax({
                url: "/FlowInfo/GetNodeInfo?NodeId=0&FlowId=" + FlowId,
                type: "GET",
                dataType: "json",
                success: function (data) {
                    console.log("获取节点数据 GetNodeInfo")
                    console.log("/FlowInfo/GetNodeInfo?NodeId=0&FlowId=" + FlowId)
                    console.log(data)
                    that.nodeInfo = data[0]
                    that.preApprove = !data[0].IsNeedChose
                    that.preCopy = !data[0].IsSendChose
                    console.log(that.nodeInfo)
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(XMLHttpRequest.status);
                }
            })
            console.log("/Project/GetAllProJect?ApplyManId=" + DingData.userid)
            $.ajax({
                url: "/Project/GetAllProJect?ApplyManId=" + DingData.userid,
                type: "GET",
                dataType: "json",
                success: function (data) {
                    console.log("获取项目列表数据 GetAllProJect")
                    console.log(data)
                    that.projestList = data
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(XMLHttpRequest.status);
                }
            })
        },
        //获取审批/抄送 相关人员列表GetFlowProgress
        getNodeList() {
            var that = this
            $.ajax({
                url: "/FlowInfo/GetFlowProgress?FlowId=" + FlowId,
                type: "GET",
                dataType: "json",
                success: function (data) {
                    console.log("获取审批/抄送 相关人员列表ok")
                    for (let d of data) {
                        if (d.NodePeople)
                            d.NodePeople = d.NodePeople.split(',')
                        if (d.NodeId == 0)
                            d.NodePeople = [DingData.nickName]
                        d['AddPeople'] = []
                    }
                    console.log(data)
                    that.nodeList = data
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(XMLHttpRequest.status);
                }
            })
        },
        //审批所有流程通过，后续处理
        doneSubmit() {
            this.$alert('提交审批成功', '提交成功', {
                confirmButtonText: '确定',
                callback: action => {
                    loadPage('/main/Approval')
                }
            });
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
    props: ['preset', 'nodelist', 'type' ,'nodeid'],
    template: `<div>
                    <el-form-item v-if="type=='approve'" label="审批人" style="margin-bottom:0px;">
                        <span v-if="preset" class="hint">审批人已由管理员预置,并将自动去重</span>
                        <el-button v-else class="button-new-tag" size="small" v-on:click="addPeople">+ 添加审批人</el-button>
                    </el-form-item>
                    <el-form-item>
                        <template v-for="(node,index) in nodelist">
                            <el-tag type="warning" class="nodeTitle" style="width:100px;text-align:center;">
                                {{node.NodeName}}
                            </el-tag>
                            <template v-for="(p,a) in node.NodePeople">
                                <span v-if="a>0" style="margin-left:107px;">&nbsp;</span>
                                <el-tag :key="a"
                                        :closable="false"
                                        onclick="" v-if="node.NodePeople"
                                        :disable-transitions="false"
                                        :type="node.ApplyTime?'success':''"
                                        >
                                    {{p}}
                                </el-tag>
                                <p class='remark'>{{node.Remark}}</p>
                                <p class='applytime'>{{node.ApplyTime}}</p>
                                </br>
                            </template>
                            <template v-for="(ap,a) in node.AddPeople">
                                <span v-if="a>0" style="margin-left:97px;">&nbsp;</span>
                                <el-tag :key="a"
                                        :closable="true"
                                        v-on:close="deletePeople(ap.emplId)"
                                        onclick="" v-if="node.AddPeople.length>0"
                                        :disable-transitions="false"
                                        :type="node.ApplyTime?'success':''"
                                        >
                                    {{ap.name}}
                                </el-tag>
                                </br>
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
            inputVisible: false
        }
    },
    methods: {
        addNode() {

        },
        addPeople(nodeid) {
            var that = this
             console.log('addPeople')
            //for (let node of that.nodelist) {
            //    if (node.NodeId != that.nodeid + 1)
            //        continue
            //    node.AddPeople.push({
            //        emplId: "manager325",
            //        name: "詹姆斯"
            //    })
            //}
           
            DingTalkPC.biz.contact.choose({
                multiple: true, //是否多选： true多选 false单选； 默认true
                users: [], //默认选中的用户列表，员工userid；成功回调中应包含该信息
                corpId: DingData.CorpId, //企业id
                max: 10, //人数限制，当multiple为true才生效，可选范围1-1500
                onSuccess: function (data) {
                    console.log(that.nodeid)
                    for (let node of that.nodelist) {
                        if (node.NodeId != that.nodeid + 1) 
                            continue
                        for (let d of data) {
                            var dontExist = true
                            for (let a of node.AddPeople) {
                                if (a.emplId == d.emplId)
                                    dontExist = false
                            }
                            if (dontExist) node.AddPeople.push(d)
                        }
                    }
                    DingTalkPC.device.notification.alert({
                        message: JSON.stringify(data),
                        title: "提示",//可传空
                        buttonName: "收到",
                        onSuccess: function () {
                            /*回调*/
                        },
                        onFail: function (err) { }
                    });
                    console.log(data)
                    console.log(that.nodelist)
                },
                onFail: function (err) { }
            });
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