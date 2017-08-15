using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Goods.DataObject
{
    /// <summary>
    /// 用户信息类
    /// </summary>
    [DataContract]
    public class UserInfo
    {
        /// <summary>
        /// 用户代码
        /// </summary>
        [DataMember]
        public string userCode { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        [DataMember]
        public string userName { get; set; }
        /// <summary>
        ///用户密码 
        /// </summary>
        [DataMember]
        public string passWord { get; set; }
    }
}
