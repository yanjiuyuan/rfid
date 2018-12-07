namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserInfo")]
    public partial class UserInfo
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string nickName { get; set; }

        [StringLength(300)]
        public string id { get; set; }

        [StringLength(300)]
        public string corpId { get; set; }

        [StringLength(300)]
        public string emplId { get; set; }

        public bool? isAuth { get; set; }

        public bool? isManager { get; set; }

        [StringLength(200)]
        public string rightLevel { get; set; }

        [StringLength(200)]
        public string avatar { get; set; }

        [StringLength(100)]
        public string FinnalLoginTime { get; set; }
    }
}
