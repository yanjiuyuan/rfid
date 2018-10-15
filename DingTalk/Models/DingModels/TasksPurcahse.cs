namespace DingTalk.Models.DingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class TasksPurcahse
    {
        public decimal Id { get; set; }

        public int? TaskId { get; set; }

        [StringLength(50)]
        public string ApplyMan { get; set; }

        [StringLength(500)]
        public string ApplyManId { get; set; }

        [StringLength(200)]
        public string Dept { get; set; }

        [StringLength(30)]
        public string ApplyTime { get; set; }

        public int? IsEnable { get; set; }

        public int? FlowId { get; set; }

        public int? NodeId { get; set; }

        public string Remark { get; set; }

        public bool? IsSend { get; set; }

        public int? State { get; set; }

        public string ImageUrl { get; set; }

        public string FileUrl { get; set; }

        public string Title { get; set; }

        [StringLength(500)]
        public string ProjectId { get; set; }

        public bool? IsPost { get; set; }

        public string OldImageUrl { get; set; }

        public string OldFileUrl { get; set; }

        public bool? IsBacked { get; set; }

        public string MediaId { get; set; }

        public string FilePDFUrl { get; set; }

        public string OldFilePDFUrl { get; set; }

        public string MediaIdPDF { get; set; }

        [StringLength(500)]
        public string PdfState { get; set; }

        [StringLength(200)]
        public string ProjectName { get; set; }

        public List<Purchase> PurchaseList { get; set; }

    }
}
