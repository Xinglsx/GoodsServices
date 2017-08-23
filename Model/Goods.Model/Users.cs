namespace Goods.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Users
    {
        [StringLength(36)]
        public string id { get; set; }

        [StringLength(20)]
        public string userid { get; set; }

        [StringLength(20)]
        public string password { get; set; }

        [StringLength(20)]
        public string nickname { get; set; }

        [StringLength(18)]
        public string idcard { get; set; }

        [StringLength(32)]
        public string realname { get; set; }

        public DateTime? birth { get; set; }

        [StringLength(11)]
        public string phonenumber { get; set; }

        [StringLength(800)]
        public string avatar { get; set; }

        public short? usertype { get; set; }

        public short? userlevel { get; set; }

        [StringLength(256)]
        public string usersignature { get; set; }

        [StringLength(36)]
        public string wechat { get; set; }

        [StringLength(16)]
        public string qq { get; set; }

        [StringLength(32)]
        public string sina { get; set; }

        [StringLength(64)]
        public string taobao { get; set; }

        public DateTime? registertime { get; set; }
    }
}
