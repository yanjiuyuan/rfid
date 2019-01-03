namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CarTable")]
    public partial class CarTable
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        [StringLength(30)]
        public string TaskId { get; set; }
        /// <summary>
        /// 同行人数
        /// </summary>
        [StringLength(100)]
        public string PeerNumber { get; set; }
        /// <summary>
        /// 用车开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 用车结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 用车事由
        /// </summary>
        public string MainContent { get; set; }
        /// <summary>
        /// 计划行车路线
        /// </summary>
        public string PlantTravelWay { get; set; }
        /// <summary>
        /// 实际行车路线
        /// </summary>
        public string FactTravelWay { get; set; }
        /// <summary>
        /// 开始公里数
        /// </summary>
        [StringLength(200)]
        public string StartKilometres { get; set; }
        /// <summary>
        /// 结束公里数
        /// </summary>
        [StringLength(200)]
        public string EndKilometres { get; set; }
        /// <summary>
        /// 使用公里数
        /// </summary>
        [StringLength(200)]
        public string UseKilometres { get; set; }
        /// <summary>
        /// 是否公车
        /// </summary>
        public bool? IsPublicCar { get; set; }
        /// <summary>
        /// 是否选择被占用的车
        /// </summary>
        public bool? IsChooseOccupyCar { get; set; }
        /// <summary>
        /// 车辆唯一Id
        /// </summary>
        [StringLength(100)]
        public string CarId { get; set; }
        /// <summary>
        /// 驾驶人
        /// </summary>
        [StringLength(200)]
        public string DrivingMan { get; set; }
        /// <summary>
        /// 最近一次占用车辆Id
        /// </summary>
        [StringLength(200)]
        public string OccupyCarId { get; set; }
        /// <summary>
        /// 实际公里数
        /// </summary>
        [StringLength(200)]
        public string FactKilometre { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [StringLength(200)]
        public string CarNumber { get; set; }

    }
}
