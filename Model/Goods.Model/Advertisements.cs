namespace Goods.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Advertisements
    {
        [StringLength(36)]
        public string id { get; set; }

        [StringLength(36)]
        public string key { get; set; }

        public short? type { get; set; }

        [StringLength(256)]
        public string img { get; set; }

        [Column(TypeName = "text")]
        public string content { get; set; }

        [StringLength(36)]
        public string goodskey { get; set; }

        [Column(TypeName = "date")]
        public DateTime? begintime { get; set; }

        [Column(TypeName = "date")]
        public DateTime? endtime { get; set; }

        public int? state { get; set; }
    }
}
