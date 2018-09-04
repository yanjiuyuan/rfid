namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Receiving")]
    public partial class Receiving
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(300)]
        public string TaskId { get; set; }
        /// <summary>
        /// 来问单位
        /// </summary>
        [StringLength(200)]
        public string ReceivingUnit { get; set; }
        /// <summary>
        /// 文件编号
        /// </summary>
        [StringLength(100)]
        public string ReceivingNo { get; set; }
        /// <summary>
        /// 文件文号
        /// </summary>
        [StringLength(100)]
        public string FileNo { get; set; }
        /// <summary>
        /// 收文时间
        /// </summary>
        [StringLength(100)]
        public string ReceivingTime { get; set; }
        /// <summary>
        /// 主要内容
        /// </summary>
        public string MainIdea { get; set; }
        /// <summary>
        /// 拟办意见
        /// </summary>
        public string Suggestion { get; set; }
        /// <summary>
        /// 领导阅示
        /// </summary>
        public string Leadership { get; set; }
        /// <summary>
        /// 承办部门阅办情况
        /// </summary>
        public string Review { get; set; }
        /// <summary>
        /// 办理落实情况
        /// </summary>
        public string HandleImplementation { get; set; }
    }
}
