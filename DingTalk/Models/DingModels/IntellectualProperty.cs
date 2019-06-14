namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IntellectualProperty")]
    public partial class IntellectualProperty
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(30)]
        public string TaskId { get; set; }
      
        /// <summary>
        /// 申请名称
        /// </summary>
        [StringLength(300)]
        public string Name { get; set; }

        /// <summary>
        /// 申请类别
        /// </summary>
        [StringLength(300)]
        public string Type { get; set; }

        /// <summary>
        /// 申报单位
        /// </summary>
        [StringLength(300)]
        public string Company { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [StringLength(300)]
        public string Project { get; set; }
        /// <summary>
        /// 项目Id
        /// </summary>
        [StringLength(300)]
        public string ProjectNo { get; set; }

        /// <summary>
        /// 申请发明人
        /// </summary>
        [StringLength(300)]
        public string Inventor { get; set; }

        /// <summary>
        /// 申请发明人Id
        /// </summary>
        [StringLength(300)]
        public string InventorId { get; set; }

        /// <summary>
        /// 申报名称
        /// </summary>
        [StringLength(300)]
        public string ActualName { get; set; }
        /// <summary>
        /// 申报类别
        /// </summary>
        [StringLength(300)]
        public string ActualType { get; set; }
        /// <summary>
        /// 申报发明人或设计人
        /// </summary>
        [StringLength(300)]
        public string ActualInventor { get; set; }
        /// <summary>
        /// 申报发明人或设计人Id
        /// </summary>
        [StringLength(300)]
        public string ActualInventorId { get; set; }
    }
}