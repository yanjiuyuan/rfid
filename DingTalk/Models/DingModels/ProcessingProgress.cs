﻿namespace DingTalk.Models.DingModels
        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 大类别
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 小类别
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 备注(蔡靓弥)
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 项目编码
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 项目名称
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 流水号
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// bom
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 走账
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 设计员
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 设计员Id
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// BOM时间
        /// </summary>
        [StringLength(200)]

        /// <summary>
        /// 2D
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 3D
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 需求时间
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 需求数量
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 预计开工时间
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 预计完成时间
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 开始时间
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 自制数（件）
        /// </summary>
        [StringLength(200)]
        public string HomemadeNumberZZ { get; set; }
        /// <summary>
        /// 加工时间（天）自制
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 进度（件）自制
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 外协数（件）外协
        /// </summary>
        [StringLength(200)]

        /// <summary>
        /// 加工时间（天）外协
        /// </summary>
        [StringLength(200)]


        /// <summary>
        /// 进度(已完成、报废等)
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 实际加工完成时间
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 组装（%）
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 调试（%）
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 进度说明
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 备注(胡锡涛)
        /// </summary>
        [StringLength(200)]
        /// <summary>
        /// 记录人
        /// </summary>
        [StringLength(200)]

        /// <summary>
        /// 记录人Id
        /// </summary>
        [StringLength(200)]

        /// <summary>
        /// 备注(设计人员)
        /// </summary>
        [StringLength(200)]
    
        /// <summary>
        /// 是否已读
        /// </summary>

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

        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(200)]
        public string CreateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [StringLength(200)]
        public string FinishTime { get; set; }

        /// <summary>
        /// 对应权限
        /// </summary>
        [NotMapped]
        public List<int> Power { get; set; }
    }