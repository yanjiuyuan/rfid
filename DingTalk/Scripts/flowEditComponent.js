Vue.component('nodewrap', {
    props: ['nodeconfig', 'flowpermission', 'directormaxlevel', 'istried', 'tableid'],
    template: ` 
    <div>
        <div class="node-wrap" v-if="nodeconfig.type!=4">
            <div class="node-wrap-box" :class="(nodeconfig.type==0?'start-node ':'')+(istried&&nodeconfig.error?'active error':'')">
                <div>
                    <div class="title" :style="'background: rgb('+ ['87, 106, 149','255, 148, 62','50, 150, 250'][nodeconfig.type] +');'">
                        <span class="iconfont" v-show="nodeconfig.type==1"></span>
                        <span class="iconfont" v-show="nodeconfig.type==2"></span>
                        <span v-if="nodeconfig.type==0">{{nodeconfig.nodeName}}</span>
                        <input type="text" class="ant-input editable-title-input" v-if="nodeconfig.type!=0&&isInput"
                        v-on:blur="blurEvent()" v-on:focus="$event.currentTarget.select()" v-focus
                        v-model="nodeconfig.nodeName" :placeholder="placeholderList[nodeconfig.type]">
                        <span class="editable-title" v-on:click="clickEvent()" v-if="nodeconfig.type!=0&&!isInput">{{nodeconfig.nodeName}}</span>
                        <i class="anticon anticon-close close" v-if="nodeconfig.type!=0" v-on:click="delNode()"></i>
                    </div>
                    <div class="content" v-on:click="setPerson">
                        <div class="text" v-if="nodeconfig.type==0">{{arrToStr(flowpermission)?arrToStr(flowpermission):'所有人'}}</div>
                        <div class="text" v-if="nodeconfig.type==1">
                            <span class="placeholder" v-if="!setApproverStr(nodeconfig)">请选择{{placeholderList[nodeconfig.type]}}</span>
                            {{setApproverStr(nodeconfig)}}
                        </div>
                        <div class="text" v-if="nodeconfig.type==2">
                            <span class="placeholder" v-if="!copyerStr(nodeconfig)">请选择{{placeholderList[nodeconfig.type]}}</span>
                            {{copyerStr(nodeconfig)}}
                        </div>
                        <i class="anticon anticon-right arrow"></i>
                    </div>
                    <div class="error_tip" v-if="istried&&nodeconfig.error">
                        <i class="anticon anticon-exclamation-circle" style="color: rgb(242, 86, 67);"></i>
                    </div>
                </div>
            </div>
            <addNode :childNodeP.sync="nodeconfig.childNode"></addNode>
        </div>
        <div class="branch-wrap" v-if="nodeconfig.type==4">
            <div class="branch-box-wrap">
                <div class="branch-box">
                    <button class="add-branch" v-on:click="addTerm">添加条件</button>
                    <div class="col-box" v-for="(item,index) in nodeconfig.conditionNodes" :key="index">
                        <div class="condition-node">
                            <div class="condition-node-box">
                                <div class="auto-judge" :class="istried&&item.error?'error active':''">
                                    <div class="sort-left" v-if="index!=0" v-on:click="arrTransfer(index,-1)">&lt;</div>
                                    <div class="title-wrapper">
                                        <input type="text" class="ant-input editable-title-input" v-if="isInputList[index]"
                                        v-on:blur="blurEvent(index)" v-on:focus="$event.currentTarget.select()" v-focus v-model="item.nodeName">
                                        <span class="editable-title" v-on:click="clickEvent(index)" v-if="!isInputList[index]">{{item.nodeName}}</span>
                                        <span class="priority-title" v-on:click="setPerson(item.priorityLevel)">优先级{{item.priorityLevel}}</span>
                                        <i class="anticon anticon-close close" v-on:click="delTerm(index)"></i>
                                    </div>
                                    <div class="sort-right" v-if="index!=nodeconfig.conditionNodes.length-1"
                                        v-on:click="arrTransfer(index)">&gt;</div>
                                    <div class="content" v-on:click="setPerson(item.priorityLevel)">{{conditionStr(item,index)}}</div>
                                    <div class="error_tip" v-if="istried&&item.error">
                                        <i class="anticon anticon-exclamation-circle" style="color: rgb(242, 86, 67);"></i>
                                    </div>
                                </div>
                                <addNode :childNodeP.sync="item.childNode"></addNode>
                            </div>
                        </div>
                        <nodewrap v-if="item.childNode && item.childNode" :nodeconfig.sync="item.childNode" :tableid="tableid"
                        :istried.sync="istried" :directormaxlevel="directormaxlevel"></nodewrap>
                        <div class="top-left-cover-line" v-if="index==0"></div>
                        <div class="bottom-left-cover-line" v-if="index==0"></div>
                        <div class="top-right-cover-line" v-if="index==nodeconfig.conditionNodes.length-1"></div>
                        <div class="bottom-right-cover-line" v-if="index==nodeconfig.conditionNodes.length-1"></div>
                    </div>
                </div>
                <addNode :childNodeP.sync="nodeconfig.childNode"></addNode>
            </div>
        </div>
     




        <nodewrap v-if="nodeconfig.childNode && nodeconfig.childNode" :nodeconfig.sync="nodeconfig.childNode" :tableid="tableid"
        :istried.sync="istried" :directormaxlevel="directormaxlevel"></nodewrap>
    </div>
              `,
    /** 
     


     *  **/
    data: function () {
        return {
            placeholderList: ["发起人", "审核人", "抄送人"],
            isInputList: [],
            isInput: false,
            promoterVisible: false,
            promoterDrawer: false,
            departments: {},
            checkedDepartmentList: [],
            checkedEmployessList: [],
            promoterSearchName: "",
            flowPermission1: this.flowpermission,
            approverDrawer: false,
            approverVisible: false,
            approverRoleVisible: false,
            approverConfig: {},
            approverEmplyessList: [],
            approverSearchName: "",
            roles: [],
            roleList: [],
            approverRoleSearchName: "",
            copyerDrawer: false,
            copyerVisible: false,
            copyerConfig: {},
            copyerSearchName: "",
            activeName: "1",
            copyerEmployessList: [],
            copyerRoleList: [],
            ccSelfSelectFlag: [],
            conditionDrawer: false,
            conditionVisible: false,
            conditionConfig: {},
            conditionsConfig: {
                conditionNodes: [],
            },
            bPriorityLevel: "",
            conditions: [],
            conditionList: [],
            conditionRoleVisible: false,
            conditionRoleSearchName: "",
            conditionDepartmentList: [],
            conditionEmployessList: [],
            conditionRoleList: [],
        }
    },
    mounted() {
        if (this.nodeconfig.type == 1) {
            this.nodeconfig.error = !this.setApproverStr(this.nodeconfig)
        } else if (this.nodeconfig.type == 2) {
            this.nodeconfig.error = !this.copyerStr(this.nodeconfig)
        } else if (this.nodeconfig.type == 4) {
            for (var i = 0; i < this.nodeconfig.conditionNodes.length; i++) {
                this.nodeconfig.conditionNodes[i].error = this.conditionStr(this.nodeconfig.conditionNodes[i], i) == "请设置条件" && i != this.nodeconfig.conditionNodes.length - 1
            }
        }
    },
    methods: {
        clickEvent(index) {
            if (index || index === 0) {
                this.$set(this.isInputList, index, true)
            } else {
                this.isInput = true;
            }
        },
        blurEvent(index) {
            if (index || index === 0) {
                this.$set(this.isInputList, index, false)
                this.nodeconfig.conditionNodes[index].nodeName = this.nodeconfig.conditionNodes[index].nodeName ? this.nodeconfig.conditionNodes[index].nodeName : "条件"
            } else {
                this.isInput = false;
                this.nodeconfig.nodeName = this.nodeconfig.nodeName ? this.nodeconfig.nodeName : this.placeholderList[this.nodeconfig.type]
            }
        },
        conditionStr(item, index) {
            var { conditionList, nodeUserList } = item;
            if (conditionList.length == 0) {
                return (index == this.nodeconfig.conditionNodes.length - 1) && this.nodeconfig.conditionNodes[0].conditionList.length != 0 ? '其他条件进入此流程' : '请设置条件'
            } else {
                let str = ""
                for (var i = 0; i < conditionList.length; i++) {
                    var { columnId, columnType, showType, showName, optType, zdy1, opt1, zdy2, opt2, fixedDownBoxValue } = conditionList[i];
                    if (columnId == 0) {
                        if (nodeUserList.length != 0) {
                            str += '发起人属于：'
                            str += nodeUserList.map(item => { return item.name }).join("或") + " 并且 "
                        }
                    }
                    if (columnType == "String" && showType == "3") {
                        if (zdy1) {
                            str += showName + '属于：' + this.dealStr(zdy1, JSON.parse(fixedDownBoxValue)) + " 并且 "
                        }
                    }
                    if (columnType == "Double") {
                        if (optType != 6 && zdy1) {
                            var optTypeStr = ["", "<", ">", "≤", "=", "≥"][optType]
                            str += `${showName} ${optTypeStr} ${zdy1} 并且 `
                        } else if (optType == 6 && zdy1 && zdy2) {
                            str += `${zdy1} ${opt1} ${showName} ${opt2} ${zdy2} 并且 `
                        }
                    }
                }
                return str ? str.substring(0, str.length - 4) : '请设置条件'
            }
        },
        dealStr(str, obj) {
            let arr = [];
            let list = str.split(",");
            for (var elem in obj) {
                list.map(item => {
                    if (item == elem) {
                        arr.push(obj[elem].value)
                    }
                })
            }
            return arr.join("或")
        },
        addConditionRole() {
            this.conditionRoleSearchName = "";
            this.conditionRoleVisible = true;
            this.activeName = "1";
            this.getDepartmentList();
            this.conditionDepartmentList = [];
            this.conditionEmployessList = [];
            this.conditionRoleList = [];
            for (var i = 0; i < this.conditionConfig.nodeUserList.length; i++) {
                var { type, name, targetId } = this.conditionConfig.nodeUserList[i];
                if (type == 1) {
                    this.conditionEmployessList.push({
                        employeeName: name,
                        id: targetId
                    });
                } else if (type == 2) {
                    this.conditionRoleList.push({
                        roleName: name,
                        roleId: targetId
                    });
                } else if (type == 3) {
                    this.conditionDepartmentList.push({
                        departmentName: name,
                        id: targetId
                    });
                }
            }
        },
        sureConditionRole() {
            this.conditionConfig.nodeUserList = [];
            this.conditionRoleList.map(item => {
                this.conditionConfig.nodeUserList.push({
                    type: 2,
                    targetId: item.roleId,
                    name: item.roleName
                })
            });
            this.conditionDepartmentList.map(item => {
                this.conditionConfig.nodeUserList.push({
                    type: 3,
                    targetId: item.id,
                    name: item.departmentName
                })
            });
            this.conditionEmployessList.map(item => {
                this.conditionConfig.nodeUserList.push({
                    type: 1,
                    targetId: item.id,
                    name: item.employeeName
                })
            });
            this.conditionRoleVisible = false;
        },
        addCondition() {
            this.conditionList = [];
            this.conditionVisible = true;
            this.$axios.get("/conditions.json?tableid=" + this.tableid).then(res => {
                this.conditions = res.data;
                if (this.conditionConfig.conditionList) {
                    for (var i = 0; i < this.conditionConfig.conditionList.length; i++) {
                        var { columnId } = this.conditionConfig.conditionList[i]
                        if (columnId == 0) {
                            this.conditionList.push({ columnId: 0 })
                        } else {
                            this.conditionList.push(this.conditions.filter(item => { return item.columnId == columnId; })[0])
                        }
                    }
                }
            })
        },
        changeOptType(item) {
            if (item.optType == 1) {
                item.zdy1 = 2;
            } else {
                item.zdy1 = 1;
                item.zdy2 = 2;
            }
        },
        sureCondition() {
            //1.弹窗有，外面无+
            //2.弹窗有，外面有不变
            for (var i = 0; i < this.conditionList.length; i++) {
                var { columnId, showName, columnName, showType, columnName, columnType, fixedDownBoxValue } = this.conditionList[i];
                if (this.toggleClass(this.conditionConfig.conditionList, this.conditionList[i], "columnId")) {
                    continue;
                }
                if (columnId == 0) {
                    this.conditionConfig.nodeUserList == [];
                    this.conditionConfig.conditionList.push({
                        "type": 1,
                        "columnId": columnId,
                        "showName": '发起人'
                    });
                } else {
                    if (columnType == "Double") {
                        this.conditionConfig.conditionList.push({
                            "showType": showType,
                            "columnId": columnId,
                            "type": 2,
                            "showName": showName,
                            "optType": "1",
                            "zdy1": "2",
                            "opt1": "<",
                            "zdy2": "",
                            "opt2": "<",
                            "columnDbname": columnName,
                            "columnType": columnType,
                        })
                    } else if (columnType == "String" && showType == "3") {
                        this.conditionConfig.conditionList.push({
                            "showType": showType,
                            "columnId": columnId,
                            "type": 2,
                            "showName": showName,
                            "zdy1": "",
                            "columnDbname": columnName,
                            "columnType": columnType,
                            "fixedDownBoxValue": fixedDownBoxValue
                        })
                    }
                }
            }
            ////3.弹窗无，外面有-
            for (var i = this.conditionConfig.conditionList.length - 1; i >= 0; i--) {
                if (!this.toggleClass(this.conditionList, this.conditionConfig.conditionList[i], "columnId")) {
                    this.conditionConfig.conditionList.splice(i, 1);
                }
            }
            this.conditionConfig.conditionList.sort(function (a, b) { return a.columnId - b.columnId; });
            this.conditionVisible = false;
        },
        saveCondition() {
            this.conditionDrawer = false;
            var a = this.conditionsConfig.conditionNodes.splice(this.bPriorityLevel - 1, 1)//截取旧下标
            this.conditionsConfig.conditionNodes.splice(this.conditionConfig.priorityLevel - 1, 0, a[0])//填充新下标
            this.conditionsConfig.conditionNodes.map((item, index) => {
                item.priorityLevel = index + 1
            });
            for (var i = 0; i < this.conditionsConfig.conditionNodes.length; i++) {
                this.conditionsConfig.conditionNodes[i].error = this.conditionStr(this.conditionsConfig.conditionNodes[i], i) == "请设置条件" && i != this.conditionsConfig.conditionNodes.length - 1
            }
            this.$emit("update:nodeconfig", this.conditionsConfig);
        },
        getDebounceData(event, type = 1) {
            this.$func.debounce(function () {
                if (event.target.value) {
                    if (type == 1) {
                        this.departments.childDepartments = [];
                        this.$axios.get(`/employees.json?searchName=${event.target.value}&pageNum=1&pageSize=30`).then(res => {
                            this.departments.employees = res.data.list
                        })
                    } else {
                        this.$axios.get(`/roles.json?searchName=${event.target.value}&pageNum=1&pageSize=30`).then(res => {
                            this.roles = res.data.list
                        })
                    }
                } else {
                    type == 1 ? this.getDepartmentList() : this.getRoleList();
                }
            }.bind(this))()
        },
        handleClick() {
            this.copyerSearchName = "";
            this.conditionRoleSearchName = "";
            if (this.activeName == 1) {
                this.getDepartmentList();
            } else {
                this.getRoleList();
            }
        },
        addCopyer() {
            this.copyerSearchName = "";
            this.copyerVisible = true;
            this.activeName = "1";
            this.getDepartmentList();
            this.copyerEmployessList = [];
            this.copyerRoleList = [];
            for (var i = 0; i < this.copyerConfig.nodeUserList.length; i++) {
                var { type, name, targetId } = this.copyerConfig.nodeUserList[i];
                if (type == 1) {
                    this.copyerEmployessList.push({
                        employeeName: name,
                        id: targetId
                    });
                } else if (type == 2) {
                    this.copyerRoleList.push({
                        roleName: name,
                        roleId: targetId
                    });
                }
            }
        },
        sureCopyer() {
            this.copyerConfig.nodeUserList = [];
            this.copyerEmployessList.map(item => {
                this.copyerConfig.nodeUserList.push({
                    type: 1,
                    targetId: item.id,
                    name: item.employeeName
                })
            });
            this.copyerRoleList.map(item => {
                this.copyerConfig.nodeUserList.push({
                    type: 2,
                    targetId: item.roleId,
                    name: item.roleName
                })
            });
            this.copyerVisible = false;
        },
        saveCopyer() {
            this.copyerConfig.ccSelfSelectFlag = this.ccSelfSelectFlag.length == 0 ? 0 : 1;
            this.copyerConfig.error = !this.copyerStr(this.copyerConfig);
            this.$emit("update:nodeconfig", this.copyerConfig);
            this.copyerDrawer = false;
        },
        copyerStr(nodeconfig) {
            if (nodeconfig.nodeUserList.length != 0) {
                return this.arrToStr(nodeconfig.nodeUserList)
            } else {
                if (nodeconfig.ccSelfSelectFlag == 1) {
                    return "发起人自选"
                }
            }
        },
        changeRange(val) {
            this.approverConfig.nodeUserList = [];
        },
        changeType(val) {
            this.approverConfig.nodeUserList = [];
            this.approverConfig.examineMode = 1;
            this.approverConfig.noHanderAction = 2;
            if (val == 2) {
                this.approverConfig.directorLevel = 1;
            } else if (val == 4) {
                this.approverConfig.selectMode = 1;
                this.approverConfig.selectRange = 1;
            } else if (val == 7) {
                this.approverConfig.examineEndDirectorLevel = 1
            }
        },
        addApprover() {
            this.approverVisible = true;
            this.approverSearchName = "";
            this.getDepartmentList();
            this.approverEmplyessList = [];
            for (var i = 0; i < this.approverConfig.nodeUserList.length; i++) {
                var { name, targetId } = this.approverConfig.nodeUserList[i];
                this.approverEmplyessList.push({
                    employeeName: name,
                    id: targetId
                });
            }
        },
        addRoleApprover() {
            this.approverRoleVisible = true;
            this.approverRoleSearchName = "";
            this.getRoleList();
            this.roleList = [];
            for (var i = 0; i < this.approverConfig.nodeUserList.length; i++) {
                var { name, targetId } = this.approverConfig.nodeUserList[i];
                this.roleList.push({
                    roleName: name,
                    roleId: targetId
                });
            }
        },
        sureApprover() {
            this.approverConfig.nodeUserList = [];
            if (this.approverConfig.settype == 1 || (this.approverConfig.settype == 4 && this.approverConfig.selectRange == 2)) {
                this.approverEmplyessList.map(item => {
                    this.approverConfig.nodeUserList.push({
                        type: 1,
                        targetId: item.id,
                        name: item.employeeName
                    })
                });
                this.approverVisible = false;
            } else if (this.approverConfig.settype == 4 && this.approverConfig.selectRange == 3) {
                this.roleList.map(item => {
                    this.approverConfig.nodeUserList.push({
                        type: 2,
                        targetId: item.roleId,
                        name: item.roleName
                    })
                });
                this.approverRoleVisible = false;
            }
        },
        setApproverStr(nodeconfig) {
            if (nodeconfig.settype == 1) {
                if (nodeconfig.nodeUserList.length == 1) {
                    return nodeconfig.nodeUserList[0].name
                } else if (nodeconfig.nodeUserList.length > 1) {
                    if (nodeconfig.examineMode == 1) {
                        return this.arrToStr(nodeconfig.nodeUserList)
                    } else if (nodeconfig.examineMode == 2) {
                        return nodeconfig.nodeUserList.length + "人会签"
                    }
                }
            } else if (nodeconfig.settype == 2) {
                let level = nodeconfig.directorLevel == 1 ? '直接主管' : '第' + nodeconfig.directorLevel + '级主管'
                if (nodeconfig.examineMode == 1) {
                    return level
                } else if (nodeconfig.examineMode == 2) {
                    return level + "会签"
                }
            } else if (nodeconfig.settype == 4) {
                if (nodeconfig.selectRange == 1) {
                    return "发起人自选"
                } else {
                    if (nodeconfig.nodeUserList.length > 0) {
                        if (nodeconfig.selectRange == 2) {
                            return "发起人自选"
                        } else {
                            return '发起人从' + nodeconfig.nodeUserList[0].name + '中自选'
                        }
                    } else {
                        return "";
                    }
                }
            } else if (nodeconfig.settype == 5) {
                return "发起人自己"
            } else if (nodeconfig.settype == 7) {
                return '从直接主管到通讯录中级别最高的第' + nodeconfig.examineEndDirectorLevel + '个层级主管'
            }
        },
        saveApprover() {
            this.approverConfig.error = !this.setApproverStr(this.approverConfig)
            this.$emit("update:nodeconfig", this.approverConfig);
            this.approverDrawer = false;
        },
        addPromoter() {
            this.promoterVisible = true;
            this.getDepartmentList();
            this.promoterSearchName = "";
            this.checkedEmployessList = [];
            this.checkedDepartmentList = [];
            for (var i = 0; i < this.flowPermission1.length; i++) {
                var { type, name, targetId } = this.flowPermission1[i];
                if (type == 1) {
                    this.checkedEmployessList.push({
                        employeeName: name,
                        id: targetId
                    });
                } else if (type == 3) {
                    this.checkedDepartmentList.push({
                        departmentName: name,
                        id: targetId
                    });
                }
            }
        },
        surePromoter() {
            this.flowPermission1 = [];
            this.checkedDepartmentList.map(item => {
                this.flowPermission1.push({
                    type: 3,
                    targetId: item.id,
                    name: item.departmentName
                })
            });
            this.checkedEmployessList.map(item => {
                this.flowPermission1.push({
                    type: 1,
                    targetId: item.id,
                    name: item.employeeName
                })
            });
            this.promoterVisible = false;
        },
        savePromoter() {
            this.$emit("update:flowpermission", this.flowPermission1);
            this.promoterDrawer = false;
        },
        arrToStr(arr) {
            if (arr) {
                return arr.map(item => { return item.name }).toString()
            }
        },
        toggleStrClass(item, key) {
            let a = item.zdy1 ? item.zdy1.split(",") : []
            return a.some(item => { return item == key });
        },
        toStrChecked(item, key) {
            let a = item.zdy1 ? item.zdy1.split(",") : []
            var isIncludes = this.toggleStrClass(item, key);
            if (!isIncludes) {
                a.push(key)
                item.zdy1 = a.toString()
            } else {
                this.removeStrEle(item, key);
            }
        },
        removeStrEle(item, key) {
            let a = item.zdy1 ? item.zdy1.split(",") : []
            var includesIndex;
            a.map((item, index) => {
                if (item == key) {
                    includesIndex = index
                }
            });
            a.splice(includesIndex, 1);
            item.zdy1 = a.toString()
        },
        toggleClass(arr, elem, key = 'id') {
            return arr.some(item => { return item[key] == elem[key] });
        },
        toChecked(arr, elem, key = 'id') {
            var isIncludes = this.toggleClass(arr, elem, key);
            !isIncludes ? arr.push(elem) : this.removeEle(arr, elem, key);
        },
        removeEle(arr, elem, key = 'id') {
            var includesIndex;
            arr.map((item, index) => {
                if (item[key] == elem[key]) {
                    includesIndex = index
                }
            });
            arr.splice(includesIndex, 1);
        },
        getRoleList() {
            this.$axios.get("/roles.json").then(res => {
                this.roles = res.data.list;
            })
        },
        getDepartmentList(parentId = 0) {
            this.$axios.get("/departments.json?parentId=" + parentId).then(res => {
                this.departments = res.data;
            })
        },
        delNode() {
            this.$emit("update:nodeconfig", this.nodeconfig.childNode);
        },
        addTerm() {
            let len = this.nodeconfig.conditionNodes.length + 1
            this.nodeconfig.conditionNodes.push({
                "nodeName": "条件" + len,
                "type": 3,
                "priorityLevel": len,
                "conditionList": [],
                "nodeUserList": [],
                "childNode": null
            });
            for (var i = 0; i < this.nodeconfig.conditionNodes.length; i++) {
                this.nodeconfig.conditionNodes[i].error = this.conditionStr(this.nodeconfig.conditionNodes[i], i) == "请设置条件" && i != this.nodeconfig.conditionNodes.length - 1
            }
            this.$emit("update:nodeconfig", this.nodeconfig);
        },
        delTerm(index) {
            this.nodeconfig.conditionNodes.splice(index, 1)
            for (var i = 0; i < this.nodeconfig.conditionNodes.length; i++) {
                this.nodeconfig.conditionNodes[i].error = this.conditionStr(this.nodeconfig.conditionNodes[i], i) == "请设置条件" && i != this.nodeconfig.conditionNodes.length - 1
            }
            this.$emit("update:nodeconfig", this.nodeconfig);
            if (this.nodeconfig.conditionNodes.length == 1) {
                if (this.nodeconfig.childNode) {
                    if (this.nodeconfig.conditionNodes[0].childNode) {
                        this.reData(this.nodeconfig.conditionNodes[0].childNode, this.nodeconfig.childNode)
                    } else {
                        this.nodeconfig.conditionNodes[0].childNode = this.nodeconfig.childNode
                    }
                }
                this.$emit("update:nodeconfig", this.nodeconfig.conditionNodes[0].childNode);
            }
        },
        reData(data, addData) {
            if (!data.childNode) {
                data.childNode = addData
            } else {
                this.reData(data.childNode, addData)
            }
        },
        setPerson(priorityLevel) {
            var { type } = this.nodeconfig;
            if (type == 0) {
                this.promoterDrawer = true;
                this.flowPermission1 = this.flowpermission;
            } else if (type == 1) {
                this.approverDrawer = true;
                this.approverConfig = JSON.parse(JSON.stringify(this.nodeconfig))
                this.approverConfig.settype = this.approverConfig.settype ? this.approverConfig.settype : 1
            } else if (type == 2) {
                this.copyerDrawer = true;
                this.copyerConfig = JSON.parse(JSON.stringify(this.nodeconfig))
                this.ccSelfSelectFlag = this.copyerConfig.ccSelfSelectFlag == 0 ? [] : [this.copyerConfig.ccSelfSelectFlag]
            } else {
                this.conditionDrawer = true
                this.bPriorityLevel = priorityLevel;
                this.conditionsConfig = JSON.parse(JSON.stringify(this.nodeconfig))
                this.conditionConfig = this.conditionsConfig.conditionNodes[priorityLevel - 1]
            }
        },
        arrTransfer(index, type = 1) {//向左-1,向右1
            this.nodeconfig.conditionNodes[index] = this.nodeconfig.conditionNodes.splice(index + type, 1, this.nodeconfig.conditionNodes[index])[0];
            this.nodeconfig.conditionNodes.map((item, index) => {
                item.priorityLevel = index + 1
            })
            for (var i = 0; i < this.nodeconfig.conditionNodes.length; i++) {
                this.nodeconfig.conditionNodes[i].error = this.conditionStr(this.nodeconfig.conditionNodes[i], i) == "请设置条件" && i != this.nodeconfig.conditionNodes.length - 1
            }
            this.$emit("update:nodeconfig", this.nodeconfig);
        }
    },
})





Vue.component('addNode', {
    props: ['childNodeP',],
    template: ` 
    <div class="add-node-btn-box">
        <div class="add-node-btn">
            <el-popover placement="right-start" v-model="visible">
                <div class="add-node-popover-body">
                    <a class="add-node-popover-item approver" @click="addType(1)">
                        <div class="item-wrapper">
                            <span class="iconfont"></span>
                        </div>
                        <p>审批人</p>
                    </a>
                    <a class="add-node-popover-item notifier" @click="addType(2)">
                        <div class="item-wrapper">
                            <span class="iconfont"></span>
                        </div>
                        <p>抄送人</p>
                    </a>
                    <a class="add-node-popover-item condition" @click="addType(4)">
                        <div class="item-wrapper">
                            <span class="iconfont"></span>
                        </div>
                        <p>条件分支</p>
                    </a>
                </div>
                <button class="btn" type="button" slot="reference">
                    <span class="iconfont"></span>
                </button>
            </el-popover>
        </div>
    </div>
              `,
    data: function () {
        return {
            visible: false
        }
    },
    methods: {
        addType(type) {
            this.visible = false;
            if (type != 4) {
                var data;
                if (type == 1) {
                    data = {
                        "nodeName": "审核人",
                        "error": true,
                        "type": 1,
                        "settype": 1,
                        "selectMode": 0,
                        "selectRange": 0,
                        "directorLevel": 1,
                        "replaceByUp": 0,
                        "examineMode": 1,
                        "noHanderAction": 1,
                        "examineEndDirectorLevel": 0,
                        "childNode": this.childNodeP,
                        "nodeUserList": []
                    }
                } else if (type == 2) {
                    data = {
                        "nodeName": "抄送人",
                        "type": 2,
                        "ccSelfSelectFlag": 1,
                        "childNode": this.childNodeP,
                        "nodeUserList": []
                    }
                }
                this.$emit("update:childNodeP", data)
            } else {
                this.$emit("update:childNodeP", {
                    "nodeName": "路由",
                    "type": 4,
                    "childNode": null,
                    "conditionNodes": [{
                        "nodeName": "条件1",
                        "error": true,
                        "type": 3,
                        "priorityLevel": 1,
                        "conditionList": [],
                        "nodeUserList": [],
                        "childNode": this.childNodeP,
                    }, {
                        "nodeName": "条件2",
                        "type": 3,
                        "priorityLevel": 2,
                        "conditionList": [],
                        "nodeUserList": [],
                        "childNode": null
                    }]
                })
            }
        }
    },
})


/**
 * 

      <el-drawer title="发起人" :visible.sync="promoterDrawer" direction="rtl" class="set_promoter" size="550px" :before-close="savePromoter">
            <div class="demo-drawer__content">
                <div class="promoter_content drawer_content">
                    <p>{{arrToStr(flowPermission1)?arrToStr(flowPermission1):'所有人'}}</p>
                    <el-button type="primary" v-on:click="addPromoter">添加/修改发起人</el-button>
                </div>
                <div class="demo-drawer__footer clear">
                    <el-button type="primary" v-on:click="savePromoter">确 定</el-button>
                    <el-button v-on:click="promoterDrawer = false">取 消</el-button>
                </div>
                <el-dialog title="选择成员" :visible.sync="promoterVisible" width="600px" append-to-body class="promoter_person">
                    <div class="person_body clear">
                        <div class="person_tree l">
                            <input type="text" placeholder="搜索成员" v-model="promoterSearchName" v-on:input="getDebounceData($event)">
                            <p class="ellipsis tree_nav" v-if="!promoterSearchName">
                                <span v-on:click="getDepartmentList(0)" class="ellipsis">天下</span>
                                <span v-for="(item,index) in departments.titleDepartments" class="ellipsis"
                                :key="index+'a'" v-on:click="getDepartmentList(item.id)">{{item.departmentName}}</span>
                            </p>
                            <ul>
                                <li v-for="(item,index) in departments.childDepartments" :key="index+'b'" class="check_box">
                                    <a :class="toggleClass(checkedDepartmentList,item)&&'active'" v-on:click="toChecked(checkedDepartmentList,item)">
                                        <img src="v-on:/assets/images/icon_file.png">{{item.departmentName}}</a>
                                    <i v-on:click="getDepartmentList(item.id)">下级</i>
                                </li>
                                <li v-for="(item,index) in departments.employees" :key="index+'c'" class="check_box">
                                    <a :class="toggleClass(checkedEmployessList,item)&&'active'" v-on:click="toChecked(checkedEmployessList,item)" :title="item.departmentNames">
                                        <img src="v-on:/assets/images/icon_people.png">{{item.employeeName}}</a>
                                </li>
                            </ul>
                        </div>
                        <div class="has_selected l">
                            <p class="clear">已选（{{checkedDepartmentList.length+checkedEmployessList.length}}）
                                <a v-on:click="checkedDepartmentList=[];checkedEmployessList=[]">清空</a>
                            </p>
                            <ul>
                                <li v-for="(item,index) in checkedDepartmentList" :key="index+'d'">
                                    <img src="v-on:/assets/images/icon_file.png">
                                    <span>{{item.departmentName}}</span>
                                    <img src="v-on:/assets/images/cancel.png" v-on:click="removeEle(checkedDepartmentList,item)">
                                </li>
                                <li v-for="(item,index) in checkedEmployessList" :key="index+'e'">
                                    <img src="v-on:/assets/images/icon_people.png">
                                    <span>{{item.employeeName}}</span>
                                    <img src="v-on:/assets/images/cancel.png" v-on:click="removeEle(checkedEmployessList,item)">
                                </li>
                            </ul>
                        </div>
                    </div>
                    <span slot="footer" class="dialog-footer">
                        <el-button v-on:click="promoterVisible = false">取 消</el-button>
                        <el-button type="primary" v-on:click="surePromoter">确 定</el-button>
                    </span>
                </el-dialog>
            </div>
        </el-drawer>



        <el-drawer title="审批人设置" :visible.sync="approverDrawer" direction="rtl" class="set_promoter" size="550px" :before-close="saveApprover">
            <div class="demo-drawer__content">
                <div class="drawer_content">
                    <div class="approver_content">
                        <el-radio-group v-model="approverConfig.settype" class="clear" v-on:change="changeType">
                            <el-radio :label="1">指定成员</el-radio>
                            <el-radio :label="2">主管</el-radio>
                            <el-radio :label="4">发起人自选</el-radio>
                            <el-radio :label="5">发起人自己</el-radio>
                            <el-radio :label="7">连续多级主管</el-radio>
                        </el-radio-group>
                        <el-button type="primary" v-on:click="addApprover" v-if="approverConfig.settype==1">添加/修改成员</el-button>
                        <p class="selected_list" v-if="approverConfig.settype==1">
                            <span v-for="(item,index) in approverConfig.nodeUserList" :key="index">{{item.name}}
                                <img src="v-on:/assets/images/add-close1.png" v-on:click="removeEle(approverConfig.nodeUserList,item,'targetId')">
                            </span>
                            <a v-if="approverConfig.nodeUserList.length!=0" v-on:click="approverConfig.nodeUserList=[]">清除</a>
                        </p>
                    </div>
                    <div class="approver_manager" v-if="approverConfig.settype==2">
                        <p>
                            <span>发起人的：</span>
                            <select v-model="approverConfig.directorLevel">
                                <option v-for="item in directormaxlevel" :value="item" :key="item">{{item==1?'直接':'第'+item+'级'}}主管</option>
                            </select>
                        </p>
                        <p class="tip">找不到主管时，由上级主管代审批</p>
                    </div>
                    <div class="approver_self" v-if="approverConfig.settype==5">
                        <p>该审批节点设置“发起人自己”后，审批人默认为发起人</p>
                    </div>
                    <div class="approver_self_select" v-show="approverConfig.settype==4">
                        <el-radio-group v-model="approverConfig.selectMode" style="width: 100%;">
                            <el-radio :label="1">选一个人</el-radio>
                            <el-radio :label="2">选多个人</el-radio>
                        </el-radio-group>
                        <h3>选择范围</h3>
                        <el-radio-group v-model="approverConfig.selectRange" style="width: 100%;" v-on:change="changeRange">
                            <el-radio :label="1">全公司</el-radio>
                            <el-radio :label="2">指定成员</el-radio>
                            <el-radio :label="3">指定角色</el-radio>
                        </el-radio-group>
                        <el-button type="primary" v-on:click="addApprover" v-if="approverConfig.selectRange==2">添加/修改成员</el-button>
                        <el-button type="primary" v-on:click="addRoleApprover" v-if="approverConfig.selectRange==3">添加/修改角色</el-button>
                        <p class="selected_list" v-if="approverConfig.selectRange==2||approverConfig.selectRange==3">
                            <span v-for="(item,index) in approverConfig.nodeUserList" :key="index">{{item.name}}
                                <img src="v-on:/assets/images/add-close1.png" v-on:click="removeEle(approverConfig.nodeUserList,item,'targetId')">
                            </span>
                            <a v-if="approverConfig.nodeUserList.length!=0&&approverConfig.selectRange!=1" v-on:click="approverConfig.nodeUserList=[]">清除</a>
                        </p>
                    </div>
                    <div class="approver_manager" v-if="approverConfig.settype==7">
                        <p>审批终点</p>
                        <p style="padding-bottom:20px">
                            <span>发起人的：</span>
                            <select v-model="approverConfig.examineEndDirectorLevel">
                                <option v-for="item in directormaxlevel" :value="item" :key="item">{{item==1?'最高':'第'+item}}层级主管</option>
                            </select>
                        </p>
                    </div>
                    <div class="approver_some" v-if="(approverConfig.settype==1&&approverConfig.nodeUserList.length>1)||approverConfig.settype==2||(approverConfig.settype==4&&approverConfig.selectMode==2)">
                        <p>多人审批时采用的审批方式</p>
                        <el-radio-group v-model="approverConfig.examineMode" class="clear">
                            <el-radio :label="1">依次审批</el-radio>
                            <br/>
                            <el-radio :label="2" v-if="approverConfig.settype!=2">会签(须所有审批人同意)</el-radio>
                        </el-radio-group>
                    </div>
                    <div class="approver_some" v-if="approverConfig.settype==2||approverConfig.settype==7">
                        <p>审批人为空时</p>
                        <el-radio-group v-model="approverConfig.noHanderAction" class="clear">
                            <el-radio :label="1">自动审批通过/不允许发起</el-radio>
                            <br/>
                            <el-radio :label="2">转交给审核管理员</el-radio>
                        </el-radio-group>
                    </div>
                </div>
                <div class="demo-drawer__footer clear">
                    <el-button type="primary" v-on:click="saveApprover">确 定</el-button>
                    <el-button v-on:click="approverDrawer = false">取 消</el-button>
                </div>
                <el-dialog title="选择成员" :visible.sync="approverVisible" width="600px" append-to-body class="promoter_person">
                    <div class="person_body clear">
                        <div class="person_tree l">
                            <input type="text" placeholder="搜索成员" v-model="approverSearchName" v-on:input="getDebounceData($event)">
                            <p class="ellipsis tree_nav" v-if="!approverSearchName">
                                <span v-on:click="getDepartmentList(0)" class="ellipsis">天下</span>
                                <span v-for="(item,index) in departments.titleDepartments" class="ellipsis"
                                :key="index+'a'" v-on:click="getDepartmentList(item.id)">{{item.departmentName}}</span>
                            </p>
                            <ul>
                                <li v-for="(item,index) in departments.childDepartments" :key="index+'b'" class="check_box not">
                                    <a><img src="v-on:/assets/images/icon_file.png">{{item.departmentName}}</a>
                                    <i v-on:click="getDepartmentList(item.id)">下级</i>
                                </li>
                                <li v-for="(item,index) in departments.employees" :key="index+'c'" class="check_box">
                                    <a :class="toggleClass(approverEmplyessList,item)&&'active'" v-on:click="toChecked(approverEmplyessList,item)" :title="item.departmentNames">
                                        <img src="v-on:/assets/images/icon_people.png">{{item.employeeName}}</a>
                                </li>
                            </ul>
                        </div>
                        <div class="has_selected l">
                            <p class="clear">已选（{{approverEmplyessList.length}}）
                                <a v-on:click="approverEmplyessList=[]">清空</a>
                            </p>
                            <ul>
                                <li v-for="(item,index) in approverEmplyessList" :key="index+'e'">
                                    <img src="v-on:/assets/images/icon_people.png">
                                    <span>{{item.employeeName}}</span>
                                    <img src="v-on:/assets/images/cancel.png" v-on:click="removeEle(approverEmplyessList,item)">
                                </li>
                            </ul>
                        </div>
                    </div>
                    <span slot="footer" class="dialog-footer">
                        <el-button v-on:click="approverVisible = false">取 消</el-button>
                        <el-button type="primary" v-on:click="sureApprover">确 定</el-button>
                    </span>
                </el-dialog>
                 <el-dialog title="选择角色" :visible.sync="approverRoleVisible" width="600px" append-to-body class="promoter_person">
                    <div class="person_body clear">
                        <div class="person_tree l">
                            <input type="text" placeholder="搜索角色" v-model="approverRoleSearchName" v-on:input="getDebounceData($event,2)">
                            <ul>
                                <li v-for="(item,index) in roles" :key="index+'b'" class="check_box not"
                                    :class="toggleClass(roleList,item,'roleId')&&'active'" v-on:click="roleList=[item]">
                                    <a :title="item.description"><img src="v-on:/assets/images/icon_role.png">{{item.roleName}}</a>
                                </li>
                            </ul>
                        </div>
                        <div class="has_selected l">
                            <p class="clear">已选（{{roleList.length}}）
                                <a v-on:click="roleList=[]">清空</a>
                            </p>
                            <ul>
                                <li v-for="(item,index) in roleList" :key="index+'e'">
                                    <img src="v-on:/assets/images/icon_role.png">
                                    <span>{{item.roleName}}</span>
                                    <img src="v-on:/assets/images/cancel.png" v-on:click="removeEle(roleList,item,'roleId')">
                                </li>
                            </ul>
                        </div>
                    </div>
                    <span slot="footer" class="dialog-footer">
                        <el-button v-on:click="approverRoleVisible = false">取 消</el-button>
                        <el-button type="primary" v-on:click="sureApprover">确 定</el-button>
                    </span>
                </el-dialog>
            </div>
        </el-drawer>


        <el-drawer title="抄送人设置" :visible.sync="copyerDrawer" direction="rtl" class="set_copyer" size="550px" :before-close="saveCopyer">
            <div class="demo-drawer__content">
                <div class="copyer_content drawer_content">
                    <el-button type="primary" v-on:click="addCopyer">添加成员</el-button>
                    <p class="selected_list">
                        <span v-for="(item,index) in copyerConfig.nodeUserList" :key="index">{{item.name}}
                            <img src="v-on:/assets/images/add-close1.png" v-on:click="removeEle(copyerConfig.nodeUserList,item,'targetId')">
                        </span>
                        <a v-if="copyerConfig.nodeUserList&&copyerConfig.nodeUserList.length!=0" v-on:click="copyerConfig.nodeUserList=[]">清除</a>
                    </p>
                    <el-checkbox-group v-model="ccSelfSelectFlag" class="clear">
                        <el-checkbox :label="1">允许发起人自选抄送人</el-checkbox>
                    </el-checkbox-group>
                </div>
                <div class="demo-drawer__footer clear">
                    <el-button type="primary" v-on:click="saveCopyer">确 定</el-button>
                    <el-button v-on:click="copyerDrawer = false">取 消</el-button>
                </div>
                <el-dialog title="选择成员" :visible.sync="copyerVisible" width="600px" append-to-body class="promoter_person">
                    <div class="person_body clear">
                        <div class="person_tree l">
                            <input type="text" placeholder="搜索成员" v-model="copyerSearchName" v-on:input="getDebounceData($event,activeName)">
                            <el-tabs v-model="activeName" v-on:tab-click="handleClick">
                                <el-tab-pane label="组织架构" name="1"></el-tab-pane>
                                <el-tab-pane label="角色列表" name="2"></el-tab-pane>
                            </el-tabs>
                            <p class="ellipsis tree_nav" v-if="activeName==1&&!copyerSearchName">
                                <span v-on:click="getDepartmentList(0)" class="ellipsis">天下</span>
                                <span v-for="(item,index) in departments.titleDepartments" class="ellipsis"
                                :key="index+'a'" v-on:click="getDepartmentList(item.id)">{{item.departmentName}}</span>
                            </p>
                            <ul style="height: 360px;" v-if="activeName==1">
                                <li v-for="(item,index) in departments.childDepartments" :key="index+'b'" class="check_box not">
                                    <a><img src="v-on:/assets/images/icon_file.png">{{item.departmentName}}</a>
                                    <i v-on:click="getDepartmentList(item.id)">下级</i>
                                </li>
                                <li v-for="(item,index) in departments.employees" :key="index+'c'" class="check_box">
                                    <a :class="toggleClass(copyerEmployessList,item)&&'active'" v-on:click="toChecked(copyerEmployessList,item)" :title="item.departmentNames">
                                        <img src="v-on:/assets/images/icon_people.png">{{item.employeeName}}</a>
                                </li>
                            </ul>
                            <ul style="height: 360px;" v-if="activeName==2">
                                <li v-for="(item,index) in roles" :key="index+'c'" class="check_box">
                                    <a :class="toggleClass(copyerRoleList,item,'roleId')&&'active'" v-on:click="toChecked(copyerRoleList,item,'roleId')" :title="item.description">
                                        <img src="v-on:/assets/images/icon_role.png">{{item.roleName}}</a>
                                </li>
                            </ul>
                        </div>
                        <div class="has_selected l">
                            <p class="clear">已选（{{copyerEmployessList.length+copyerRoleList.length}}）
                                <a v-on:click="copyerEmployessList=[];copyerRoleList=[]">清空</a>
                            </p>
                            <ul>
                                <li v-for="(item,index) in copyerRoleList" :key="index+'e'">
                                    <img src="v-on:/assets/images/icon_role.png">
                                    <span>{{item.roleName}}</span>
                                    <img src="v-on:/assets/images/cancel.png" v-on:click="removeEle(copyerRoleList,item,'roleId')">
                                </li>
                                <li v-for="(item,index) in copyerEmployessList" :key="index+'e1'">
                                    <img src="v-on:/assets/images/icon_people.png">
                                    <span>{{item.employeeName}}</span>
                                    <img src="v-on:/assets/images/cancel.png" v-on:click="removeEle(copyerEmployessList,item)">
                                </li>
                            </ul>
                        </div>
                    </div>
                    <span slot="footer" class="dialog-footer">
                        <el-button v-on:click="copyerVisible = false">取 消</el-button>
                        <el-button type="primary" v-on:click="sureCopyer">确 定</el-button>
                    </span>
                </el-dialog>
            </div>
        </el-drawer>



        <el-drawer title="条件设置" :visible.sync="conditionDrawer" direction="rtl" class="condition_copyer" size="550px" :before-close="saveCondition">
            <select v-model="conditionConfig.priorityLevel">
                <option v-for="item in conditionsConfig.conditionNodes.length" :value="item" :key="item">优先级{{item}}</option>
            </select>
            <div class="demo-drawer__content">
                <div class="condition_content drawer_content">
                    <p class="tip">当审批单同时满足以下条件时进入此流程</p>
                    <ul>
                        <li v-for="(item,index) in conditionConfig.conditionList" :key="index">
                            <span class="ellipsis">{{item.type==1?'发起人':item.showName}}：</span>
                            <div v-if="item.type==1">
                                <p :class="conditionConfig.nodeUserList.length > 0?'selected_list':''" v-on:click.self="addConditionRole" style="cursor:text">
                                    <span v-for="(item1,index1) in conditionConfig.nodeUserList" :key="index1">
                                        {{item1.name}}<img src="v-on:/assets/images/add-close1.png" v-on:click="removeEle(conditionConfig.nodeUserList,item1,'targetId')">
                                    </span>
                                    <input type="text" placeholder="请选择具体人员/角色/部门" v-if="conditionConfig.nodeUserList.length == 0" v-on:click="addConditionRole">
                                </p>
                            </div>
                            <div v-else-if="item.columnType == 'String' && item.showType == 3">
                                <p class="check_box">
                                    <a :class="toggleStrClass(item,item1.key)&&'active'" v-on:click="toStrChecked(item,item1.key)"
                                    v-for="(item1,index1) in JSON.parse(item.fixedDownBoxValue)" :key="index1">{{item1.value}}</a>
                                </p>
                            </div>
                            <div v-else>
                                <p>
                                    <select v-model="item.optType" :style="'width:'+(item.optType==6?370:100)+'px'" v-on:change="changeOptType(item)">
                                        <option value="1">小于</option>
                                        <option value="2">大于</option>
                                        <option value="3">小于等于</option>
                                        <option value="4">等于</option>
                                        <option value="5">大于等于</option>
                                        <option value="6">介于两个数之间</option>
                                    </select>
                                    <input v-if="item.optType!=6" type="text" :placeholder="'请输入'+item.showName" v-enter-number="2" v-model="item.zdy1">
                                </p>
                                <p v-if="item.optType==6">
                                    <input type="text" style="width:75px;" class="mr_10" v-enter-number="2" v-model="item.zdy1">
                                    <select style="width:60px;" v-model="item.opt1">
                                        <option value="<">&lt;</option>
                                        <option value="≤">≤</option>
                                    </select>
                                    <span class="ellipsis" style="display:inline-block;width:60px;vertical-align: text-bottom;">{{item.showName}}</span>
                                    <select style="width:60px;" class="ml_10" v-model="item.opt2">
                                        <option value="<">&lt;</option>
                                        <option value="≤">≤</option>
                                    </select>
                                    <input type="text" style="width:75px;" v-enter-number="2" v-model="item.zdy2">
                                </p>
                            </div>
                            <a v-if="item.type==1" v-on:click="conditionConfig.nodeUserList= [];removeEle(conditionConfig.conditionList,item,'columnId')">删除</a>
                            <a v-if="item.type==2" v-on:click="removeEle(conditionConfig.conditionList,item,'columnId')">删除</a>
                        </li>
                    </ul>
                    <el-button type="primary" v-on:click="addCondition">添加条件</el-button>
                    <el-dialog title="选择条件" :visible.sync="conditionVisible" width="480px" append-to-body class="condition_list">
                        <p>请选择用来区分审批流程的条件字段</p>
                        <p class="check_box">
                            <a :class="toggleClass(conditionList,{columnId:0},'columnId')&&'active'" v-on:click="toChecked(conditionList,{columnId:0},'columnId')">发起人</a>
                            <a v-for="(item,index) in conditions" :key="index" :class="toggleClass(conditionList,item,'columnId')&&'active'"
                            v-on:click="toChecked(conditionList,item,'columnId')">{{item.showName}}</a>
                        </p>
                        <span slot="footer" class="dialog-footer">
                            <el-button v-on:click="conditionVisible = false">取 消</el-button>
                            <el-button type="primary" v-on:click="sureCondition">确 定</el-button>
                        </span>
                    </el-dialog>
                </div>
                <el-dialog title="选择成员" :visible.sync="conditionRoleVisible" width="600px" append-to-body class="promoter_person">
                    <div class="person_body clear">
                        <div class="person_tree l">
                            <input type="text" placeholder="搜索成员" v-model="conditionRoleSearchName" v-on:input="getDebounceData($event,activeName)">
                            <el-tabs v-model="activeName" v-on:tab-click="handleClick">
                                <el-tab-pane label="组织架构" name="1"></el-tab-pane>
                                <el-tab-pane label="角色列表" name="2"></el-tab-pane>
                            </el-tabs>
                            <p class="ellipsis tree_nav" v-if="activeName==1&&!conditionRoleSearchName">
                                <span v-on:click="getDepartmentList(0)" class="ellipsis">天下</span>
                                <span v-for="(item,index) in departments.titleDepartments" class="ellipsis"
                                :key="index+'a'" v-on:click="getDepartmentList(item.id)">{{item.departmentName}}</span>
                            </p>
                            <ul style="height: 360px;" v-if="activeName==1">
                                <li v-for="(item,index) in departments.childDepartments" :key="index+'b'" class="check_box">
                                    <a :class="toggleClass(conditionDepartmentList,item)&&'active'" v-on:click="toChecked(conditionDepartmentList,item)">
                                        <img src="v-on:/assets/images/icon_file.png">{{item.departmentName}}</a>
                                    <i v-on:click="getDepartmentList(item.id)">下级</i>
                                </li>
                                <li v-for="(item,index) in departments.employees" :key="index+'c'" class="check_box">
                                    <a :class="toggleClass(conditionEmployessList,item)&&'active'" v-on:click="toChecked(conditionEmployessList,item)" :title="item.departmentNames">
                                        <img src="v-on:/assets/images/icon_people.png">{{item.employeeName}}</a>
                                </li>
                            </ul>
                            <ul style="height: 360px;" v-if="activeName==2">
                                <li v-for="(item,index) in roles" :key="index+'c'" class="check_box">
                                    <a :class="toggleClass(conditionRoleList,item,'roleId')&&'active'" v-on:click="toChecked(conditionRoleList,item,'roleId')" :title="item.description">
                                        <img src="v-on:/assets/images/icon_role.png">{{item.roleName}}</a>
                                </li>
                            </ul>
                        </div>
                        <div class="has_selected l">
                            <p class="clear">已选（{{conditionDepartmentList.length+conditionEmployessList.length+conditionRoleList.length}}）
                                <a v-on:click="conditionDepartmentList=[];conditionEmployessList=[];conditionRoleList=[]">清空</a>
                            </p>
                            <ul>
                                <li v-for="(item,index) in conditionRoleList" :key="index+'e'">
                                    <img src="v-on:/assets/images/icon_role.png">
                                    <span>{{item.roleName}}</span>
                                    <img src="v-on:/assets/images/cancel.png" v-on:click="removeEle(conditionRoleList,item,'roleId')">
                                </li>
                                <li v-for="(item,index) in conditionDepartmentList" :key="index+'e1'">
                                    <img src="v-on:/assets/images/icon_file.png">
                                    <span>{{item.departmentName}}</span>
                                    <img src="v-on:/assets/images/cancel.png" v-on:click="removeEle(conditionDepartmentList,item)">
                                </li>
                                <li v-for="(item,index) in conditionEmployessList" :key="index+'e2'">
                                    <img src="v-on:/assets/images/icon_people.png">
                                    <span>{{item.employeeName}}</span>
                                    <img src="v-on:/assets/images/cancel.png" v-on:click="removeEle(conditionEmployessList,item)">
                                </li>
                            </ul>
                        </div>
                    </div>
                    <span slot="footer" class="dialog-footer">
                        <el-button v-on:click="conditionRoleVisible = false">取 消</el-button>
                        <el-button type="primary" v-on:click="sureConditionRole">确 定</el-button>
                    </span>
                </el-dialog>
                <div class="demo-drawer__footer clear">
                    <el-button type="primary" v-on:click="saveCondition">确 定</el-button>
                    <el-button v-on:click="conditionDrawer = false">取 消</el-button>
                </div>
            </div>
        </el-drawer>

 **/