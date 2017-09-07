using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Goods.Model;

namespace Goods.DataObject
{
    /// <summary>
    /// 广告信息
    /// </summary>
    public class AdInfo
    {
        //广告信息
        public Advertisements adInfo { get; set; }
        //商品类广告对应的商品信息
        public Model.Goods goodsInfo { get; set; }
    }
}
