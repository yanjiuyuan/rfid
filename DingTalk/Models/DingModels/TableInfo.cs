namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TableInfo")]
    public partial class TableInfo
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 外键
        /// </summary>
        public int TableID { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 字段属性( 0 : string  1 : int  2 : bool)
        /// </summary>
        public int ColumnProperty { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public string ColumnLength { get; set; }

        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool IsNull { get; set; }

        /// <summary>
        /// 是否支持查询
        /// </summary>
        public bool IsSupportQuery { get; set; }

        /// <summary>
        /// 是否支持删除
        /// </summary>
        public bool IsSupportDelete { get; set; }

        /// <summary>
        /// 是否支持修改
        /// </summary>
        public bool IsSupportModify { get; set; }

        /// <summary>
        /// 当前操作类型(1 新增 2 删除 3 修改) 调用修改接口时传
        /// </summary>
        [NotMapped]
        public OperateType operateType { get; set; }

    }
}
