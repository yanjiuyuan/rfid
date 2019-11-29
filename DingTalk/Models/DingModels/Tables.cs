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
        public int FlowId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否支持新增(字段)
        /// </summary>
        public bool IsAdd { get; set; }

        /// <summary>
        /// 是否支持删除(字段)
        /// </summary>
        public bool IsDel { get; set; }

        /// <summary>
        /// 是否支持修改(字段)
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateMan { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public string CreateManId { get; set; }


        /// <summary>
        /// 当前操作类型(1 新增 2 删除 3 修改(列属性不支持修改)) 调用修改接口时传
        /// </summary>
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
        Add = 1,
        Delete = 2,
        Modify = 3 
    }
}
