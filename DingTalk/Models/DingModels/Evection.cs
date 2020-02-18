namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Evection")]
    public partial class Evection
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(30)]
        public string TaskId { get; set; }

        /// <summary>
        /// 时长
        /// </summary>
        [StringLength(300)]
        public string Duration { get; set; }

        /// <summary>
        /// 地点
        /// </summary>
        [StringLength(300)]
        public string Place { get; set; }

        /// <summary>
        /// 定位地点
        /// </summary>
        [StringLength(200)]
        public string LocationPlace { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [StringLength(100)]
        public string BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [StringLength(300)]
        public string EndTime { get; set; }

        /// <summary>
        /// 事由
        /// </summary>
        [StringLength(300)]
        public string Content { get; set; }

        /// <summary>
        /// 外出人员
        /// </summary>
        [StringLength(500)]
        public string EvectionMan { get; set; }

        /// <summary>
        /// 外出人员Id
        /// </summary>
        [StringLength(500)]
        public string EvectionManId { get; set; }

        /// <summary>
        /// 接触人员
        /// </summary>
        [StringLength(200)]
        public string ContactPeople { get; set; }


    }
}