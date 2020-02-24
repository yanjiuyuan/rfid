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
        /// 权限（0十楼、1十一楼、2十二楼、3十三楼、4基地办公楼、5基地宿舍四楼、6消毒管理员、7北峰宿舍楼、8基地宿舍小楼）
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