namespace Goods.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Userloginrecord")]
    public partial class Userloginrecord
    {
        [StringLength(36)]
        public string id { get; set; }

        [StringLength(36)]
        public string userid { get; set; }

        public DateTime? logintime { get; set; }

        [StringLength(16)]
        public string loginaddress { get; set; }
    }
}
