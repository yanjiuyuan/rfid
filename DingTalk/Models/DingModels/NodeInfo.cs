namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeInfo")]
    public partial class NodeInfo
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public int? NodeId { get; set; }

        /// <summary>
        /// 流程Id
        /// </summary>
        [StringLength(100)]
        public string FlowId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [StringLength(200)]
        public string NodeName { get; set; }

        /// <summary>
        /// 节点配置好的审批人
        /// </summary>
        [StringLength(500)]
        public string NodePeople { get; set; }

        /// <summary>
        /// 节点配置好的审批人Id
        /// </summary>
        [StringLength(500)]
        public string PeopleId { get; set; }

        /// <summary>
        /// 下一个处理节点的Id
        /// </summary>
        [StringLength(200)]
        public string PreNodeId { get; set; }

        /// <summary>
        /// 多人出现时是否需要同时处理
        /// </summary>
        public bool? IsAllAllow { get; set; }

        [StringLength(500)]
        public string Condition { get; set; }

        /// <summary>
        /// 是否可以退回
        /// </summary>
        public bool? IsBack { get; set; }

        /// <summary>
        /// 是否需要选人
        /// </summary>
        public bool? IsNeedChose { get; set; }

        /// <summary>
        /// 是否是抄送
        /// </summary>
        public bool? IsSend { get; set; }

        /// <summary>
        /// 退回节点的NodeId
        /// </summary>
        [StringLength(100)]
        public string BackNodeId { get; set; }

        /// <summary>
        /// 选择节点的NodeId
        /// </summary>
        [StringLength(100)]
        public string ChoseNodeId { get; set; }

        /// <summary>
        /// 选人控件单选或者多选(单选 0 多选 1  格式  0,1,0,1 )
        /// </summary>
        [StringLength(200)]
        public string IsSelectMore { get; set; }

        /// <summary>
        /// 是否必选 (0 不必选 1 必选  格式  0,1,0,1)
        /// </summary>
        [StringLength(200)]
        public string IsMandatory { get; set; }

        /// <summary>
        /// 选人控件类型(0 钉钉选人控件 1 角色选人控件)
        /// </summary>
        [StringLength(50)]
        public string ChoseType { get; set; }

        /// <summary>
        /// 角色选人配置的角色信息(超级管理员,图书管理员)
        /// </summary>
        [StringLength(500)]
        public string RoleNames { get; set; }

        /// <summary>
        /// 选人角色列表
        /// </summary>
        [NotMapped]
        public Dictionary<string, List<Roles>> RolesList { get; set; }
        //public List<List<Roles>> Roles { get; set; }


    }
}
