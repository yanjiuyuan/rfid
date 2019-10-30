namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Role")]
    public partial class Role
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [StringLength(200)]
        public string RoleName { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [StringLength(200)]
        public string CreateMan { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        [StringLength(200)]
        public string CreateManId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(200)]
        public string CreateTime { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? IsEnable { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [StringLength(200)]
        public string Remark { get; set; }


        [NotMapped]
        public List<Roles> roles { get; set; }
    }
}
