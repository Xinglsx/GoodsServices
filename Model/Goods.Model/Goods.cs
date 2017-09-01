namespace Goods.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Goods
    {
        [StringLength(36)]
        public string id { get; set; }

        [StringLength(500)]
        public string description { get; set; }

        [StringLength(500)]
        public string reason { get; set; }

        [StringLength(256)]
        public string link { get; set; }

        [StringLength(36)]
        public string command { get; set; }

        [StringLength(36)]
        public string price { get; set; }

        public DateTime? expirydate { get; set; }

        public bool? isbuy { get; set; }

        public short? state { get; set; }

        public bool? recommendflag { get; set; }

        [StringLength(36)]
        public string recommender { get; set; }

        [StringLength(36)]
        public string recommendname { get; set; }

        public DateTime? recommendtime { get; set; }

        [StringLength(500)]
        public string auditopinion { get; set; }

        public DateTime? audittime { get; set; }

        [StringLength(36)]
        public string audituser { get; set; }

        [StringLength(36)]
        public string auditname { get; set; }

        public long? dealcount { get; set; }

        public long? clickcount { get; set; }

        [Column(TypeName = "text")]
        public string image { get; set; }

        [Column(TypeName = "text")]
        public string buyimage { get; set; }
    }
}
