namespace DingTalk.Models.DingModels{    using System;    using System.Collections.Generic;    using System.ComponentModel.DataAnnotations;    using System.ComponentModel.DataAnnotations.Schema;    using System.Data.Entity.Spatial;    [Table("ProcessingProgress")]    public partial class ProcessingProgress    {        [Column(TypeName = "numeric")]        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        public decimal Id { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(200)]        public string CompanyName { get; set; }
        /// <summary>
        /// 大类别
        /// </summary>
        [StringLength(200)]        public string ProjectType { get; set; }
        /// <summary>
        /// 小类别
        /// </summary>
        [StringLength(200)]        public string ProjectSmallType { get; set; }
        /// <summary>
        /// 备注(蔡靓弥)
        /// </summary>
        [StringLength(200)]        public string Remark1 { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        [StringLength(200)]        public string ProjectId { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        [StringLength(200)]        public string ProjectName { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        [StringLength(200)]        public string TaskId { get; set; }
        /// <summary>
        /// bom
        /// </summary>
        [StringLength(200)]        public string Bom { get; set; }
        /// <summary>
        /// 走账
        /// </summary>
        [StringLength(200)]        public string AccountKeeping { get; set; }
        /// <summary>
        /// 设计员
        /// </summary>
        [StringLength(200)]        public string Designer { get; set; }
        /// <summary>
        /// 设计员Id
        /// </summary>
        [StringLength(200)]        public string DesignerId { get; set; }
        /// <summary>
        /// BOM时间
        /// </summary>
        [StringLength(200)]        public string BomTime { get; set; }

        /// <summary>
        /// 2D
        /// </summary>
        [StringLength(200)]        public string TwoD { get; set; }
        /// <summary>
        /// 3D
        /// </summary>
        [StringLength(200)]        public string ThreeD { get; set; }
        /// <summary>
        /// 需求时间
        /// </summary>
        [StringLength(200)]        public string NeedTime { get; set; }
        /// <summary>
        /// 需求数量
        /// </summary>
        [StringLength(200)]        public string NeedCount { get; set; }
        /// <summary>
        /// 预计开工时间
        /// </summary>
        [StringLength(200)]        public string ScheduledDate { get; set; }
        /// <summary>
        /// 预计完成时间
        /// </summary>
        [StringLength(200)]        public string CompletionTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [StringLength(200)]        public string BeginTime { get; set; }
        /// <summary>
        /// 自制数（件）
        /// </summary>
        [StringLength(200)]
        public string HomemadeNumber { get; set; }
        /// <summary>
        /// 加工时间（天）自制
        /// </summary>
        [StringLength(200)]        public string ProcessingTimeZZ { get; set; }
        /// <summary>
        /// 进度（件）自制
        /// </summary>
        [StringLength(200)]        public string ProgressNumberZZ { get; set; }
        /// <summary>
        /// 外协数（件）自制
        /// </summary>
        [StringLength(200)]        public string ExternalNumberZZ { get; set; }

        /// <summary>
        /// 加工时间（天）外协
        /// </summary>
        [StringLength(200)]        public string ProcessingTimeYX { get; set; }
        /// <summary>
        /// 进度（件）外协
        /// </summary>
        [StringLength(200)]        public string ProgressNumberYX { get; set; }
        /// <summary>
        /// 外协数（件）外协
        /// </summary>
        [StringLength(200)]        public string ExternalNumberYX { get; set; }


        /// <summary>
        /// 进度(已完成、报废等)
        /// </summary>
        [StringLength(200)]        public string SpeedOfProgress { get; set; }
        /// <summary>
        /// 实际加工完成时间
        /// </summary>
        [StringLength(200)]        public string ActualCompletionTime { get; set; }
        /// <summary>
        /// 组装（%）
        /// </summary>
        [StringLength(200)]        public string Assemble { get; set; }
        /// <summary>
        /// 调试（%）
        /// </summary>
        [StringLength(200)]        public string Debugging { get; set; }
        /// <summary>
        /// 进度说明
        /// </summary>
        [StringLength(200)]        public string ProgressStatement { get; set; }
        /// <summary>
        /// 备注(胡锡涛)
        /// </summary>
        [StringLength(200)]        public string Remark2 { get; set; }
        /// <summary>
        /// 记录人
        /// </summary>
        [StringLength(200)]        public string NoteTaker { get; set; }

        /// <summary>
        /// 记录人Id
        /// </summary>
        [StringLength(200)]        public string NoteTakerId { get; set; }

        /// <summary>
        /// 备注(设计人员)
        /// </summary>
        [StringLength(200)]        public string Remark3 { get; set; }
    
        /// <summary>
        /// 是否已读
        /// </summary>        public bool? IsAlreadyRead { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        [StringLength(200)]
        public string HeadOfDepartments { get; set; }

        /// <summary>
        /// 部门负责人Id
        /// </summary>
        [StringLength(200)]
        public string HeadOfDepartmentsId { get; set; }

        /// <summary>
        /// 制表人(蔡靓弥)
        /// </summary>
        [StringLength(200)]
        public string Tabulator { get; set; }

        /// <summary>
        /// 制表人Id
        /// </summary>
        [StringLength(200)]
        public string TabulatorId { get; set; }
    }}