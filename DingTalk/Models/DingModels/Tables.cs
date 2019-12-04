namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tables")]
    public partial class Tables
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 外键(必传)
        /// </summary>
        [Required]
        public int FlowId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [Required]
        public string TableName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否支持新增(预留字段)
        /// </summary>
        public bool IsAdd { get; set; }

        /// <summary>
        /// 是否支持删除(预留字段)
        /// </summary>
        public bool IsDel { get; set; }

        /// <summary>
        /// 是否支持修改(预留字段)
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 最终操作人
        /// </summary>
        [Required]
        public string CreateMan { get; set; }

        /// <summary>
        /// 最终操作人Id
        /// </summary>
        [Required]
        public string CreateManId { get; set; }


        /// <summary>
        /// 当前操作类型(0 不变  1 新增  2 删除 3 修改 ) 调用修改接口时传
        /// </summary>
        [Required]
        [NotMapped]
        public OperateType operateType { get; set; }

        [NotMapped]
        public List<TableInfo> tableInfos { get; set; }
    }

    /// <summary>
    /// 当前操作类型
    /// </summary>
    public enum OperateType
    {
        None = 0,
        Add = 1,
        Delete = 2,
        Modify = 3
    }
}
