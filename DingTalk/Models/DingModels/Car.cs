namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Car")]
    public partial class Car
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(30)]
        public string Name { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        [StringLength(200)]
        public string Type { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [StringLength(200)]
        public string CarNumber { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        [StringLength(200)]
        public string Color { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [StringLength(100)]
        public string CreateMan { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(100)]
        public string CreateTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool? State { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 最近一次用车开始时间
        /// </summary>
        public DateTime? FinnalStartTime { get; set; }
        /// <summary>
        /// 最近一次用车结束时间
        /// </summary>
        public DateTime? FinnalEndTime { get; set; }
        /// <summary>
        /// 车辆是否被占用
        /// </summary>
        public bool? IsOccupyCar { get; set; }
        /// <summary>
        /// 最近一次使用人
        /// </summary>
        [StringLength(200)]
        public string UseMan { get; set; }
        /// <summary>
        /// 最近一次占用车辆Id
        /// </summary>
        [StringLength(200)]
        public string OccupyCarId { get; set; }
        /// <summary>
        /// 每公里单价
        /// </summary>
        public double? UnitPricePerKilometre { get; set; }
        /// <summary>
        /// 车辆时间段
        /// </summary>
        public string UseTimes { get; set; }
        /// <summary>
        /// 总公里数
        /// </summary>
        [NotMapped]
        public string TotalKilometres { get; set; }
        /// <summary>
        /// 接口调用人Id
        /// </summary>
        [NotMapped]
        public string ApplyManId { get; set; }

        [NotMapped]
        public List<CarTable> carTables { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        [NotMapped]
        public string TaskId { get; set; }


    }
}
