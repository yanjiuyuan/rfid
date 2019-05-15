namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cooperate")]
    public partial class Cooperate
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(30)]
        public string TaskId { get; set; }

        /// <summary>
        /// 协作部门
        /// </summary>
        [StringLength(200)]
        public string CooperateDept { get; set; }
        /// <summary>
        /// 协作人
        /// </summary>
        [StringLength(500)]
        public string CooperateMan { get; set; }
        /// <summary>
        /// 协作人Id
        /// </summary>
        [StringLength(500)]
        public string CooperateManId { get; set; }
        /// <summary>
        /// 协作内容
        /// </summary>
        [StringLength(1000)]
        public string CooperateContent { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        [StringLength(200)]
        public string PlanBeginTime { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        [StringLength(200)]
        public string PlanEndTime { get; set; }
        /// <summary>
        /// 计划天数
        /// </summary>
        [StringLength(200)]
        public string PlanDays { get; set; }
        /// <summary>
        /// 实际开始时间
        /// </summary>
        [StringLength(200)]
        public string FactBeginTime { get; set; }
        /// <summary>
        /// 实际结束时间
        /// </summary>
        [StringLength(200)]
        public string FactEndTime { get; set; }
        /// <summary>
        /// 实际天数
        /// </summary>
        [StringLength(200)]
        public string FactDays { get; set; }
        /// <summary>
        /// 实际协作人
        /// </summary>
        [StringLength(500)]
        public string FactCooperateMan { get; set; }
        /// <summary>
        /// 实际协作人Id
        /// </summary>
        [StringLength(500)]
        public string FactCooperateManId { get; set; }
        /// <summary>
        /// 实际协作内容
        /// </summary>
        [StringLength(1000)]
        public string FactCooperateContent { get; set; }

    }
}