using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Goods.DataObject
{
    [DataContract]
    public class VersionInfo
    {
        /// <summary>
        /// 版本号
        /// </summary>
        [DataMember]
        public int versionNumber { get; set; }
        /// <summary>
        /// 版本编号
        /// </summary>
        [DataMember]
        public string version { get; set; }
        /// <summary>
        /// 下载地址
        /// </summary>
        [DataMember]
        public string downloadAddress { get; set; }
        /// <summary>
        /// 更新内容
        /// </summary>
        [DataMember]
        public string updateContent { get; set; }
    }
}
