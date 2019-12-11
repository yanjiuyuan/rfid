namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Pks")]
    public partial class Pks
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 当前ColumnId
        /// </summary>
        [Required]
        public int ColumnId { get; set; }

        /// <summary>
        /// 关联外键ColumnId
        /// </summary>
        [Required]
        public int OutColumnId { get; set; }
        
        /// <summary>
        /// 关联的表名(不用传)
        /// </summary>
        [NotMapped]
        public string TableName { get; set; }

        /// <summary>
        /// 关联的字段名(不用传)
        /// </summary>
        [NotMapped]
        public string ColumName { get; set; }
    }
}