namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KisOffice")]
    public partial class KisOffice
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public string FItemID { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string FNumber { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string FName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string FModel { get; set; }
        /// <summary>
        /// 预计价格
        /// </summary>
        public string FNote { get; set; }
    }
}
