namespace Goods.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Questions
    {
        [StringLength(36)]
        public string id { get; set; }

        [StringLength(36)]
        public string feedbackuserid { get; set; }

        [StringLength(36)]
        public string feedbackusernickname { get; set; }

        public DateTime? feedbacktime { get; set; }

        public string contact { get; set; }

        [Column(TypeName = "text")]
        public string content { get; set; }
    }
}
