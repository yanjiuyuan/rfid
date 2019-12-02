namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SqlExe")]
    public partial class SqlExe
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 执行Sql
        /// </summary>
        public string Sql { get; set; }

        public string TableName { get; set; }

        public string ApplyMan { get; set; }

        public string ApplyManId { get; set; }

        public DateTime DateTime { get; set; }
    }
}
