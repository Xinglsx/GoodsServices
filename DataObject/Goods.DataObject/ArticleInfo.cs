using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Goods.DataObject
{
    /// <summary>
    /// 文章信息类
    /// </summary>
    [DataContract]
    public class ArticleInfo
    {
        /// <summary>
        /// 文章主键
        /// </summary>
        [DataMember]
        public string ID { get; set; }
        /// <summary>
        /// 文章村里
        /// </summary>
        [DataMember]
        public string Title { get; set; }
        /// <summary>
        /// 文章主题
        /// </summary>
        [DataMember]
        public string Subject { get; set; }
        /// <summary>
        /// 文章发布时间
        /// </summary>
        [DataMember]
        public DateTime? PublishTime { get; set; }
        /// <summary>
        /// 阅读数量
        /// </summary>
        [DataMember]
        public int ReadedCount { get; set; }
        /// <summary>
        /// 评论数量
        /// </summary>
        [DataMember]
        public int CommentCount { get; set; }
        /// <summary>
        /// 文章状态
        /// </summary>
        [DataMember]
        public short State { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }
    }
}
