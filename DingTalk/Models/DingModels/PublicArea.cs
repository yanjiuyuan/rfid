namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PublicArea")]
    public partial class PublicArea
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        /// <summary>
        /// 权限（0,1,2,3,4,5,6 分别表示十楼、十一楼、十二楼、十三楼、基地办公楼、基地宿舍、消毒管理员）
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        //  效果判定
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 保洁员
        /// </summary>
        public string ClearPeople { get; set; }

        /// <summary>
        /// 监督人
        /// </summary>
        public string ControlPeople { get; set; }

        /// <summary>
        /// 监督人Id
        /// </summary>
        [StringLength(200)]
        public string ControlPeopleId { get; set; }

        /// <summary>
        /// 日期(格式 2020-02-19)
        /// </summary>
        public DateTime Date { get; set; }

    }
}