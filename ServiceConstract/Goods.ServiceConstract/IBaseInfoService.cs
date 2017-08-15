using Goods.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.ComponentModel;
using Goods.Model;
using System.IO;

namespace Goods.ServiceConstract
{
    [ServiceContract]
    public interface IBaseInfoService
    {
        #region 用户相关

        /// <summary>
        /// 验证用户登录信息，登录名支持用户名、邮箱、手机号。
        /// </summary>
        /// <param name="strCode">用户名、邮箱或手机号</param>
        /// <param name="password">用户密码(暂未加密)</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "UserInfo/ValidateUserInfo",Method = "POST",
            RequestFormat = WebMessageFormat.Json,ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("验证用户登录信息，登录名支持用户名、邮箱、手机号。")]
        ReturnResult<Users> ValidateUserInfo(string strCode,string password);

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="strCode">用户名</param>
        /// <param name="password">用户密码(暂未加密)</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "UserInfo/RegisterUserInfo", Method = "POST", 
            RequestFormat = WebMessageFormat.Json,ResponseFormat = WebMessageFormat.Json, 
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("注册用户。")]
        ReturnResult<Users> RegisterUserInfo(string strCode, string password);

        #endregion

        #region 商品相关

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="curPage">获取第几页 默认从0开始</param>
        /// <param name="pageSize">每页个数</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Goods/GetGoodsList?curPage={curPage}&pageSize={pageSize}", 
            Method = "GET", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("获取商品列表。")]
        ReturnResult<List<Model.Goods>> GetGoodsList(int curPage,int pageSize);

        /// <summary>
        /// 保存商品信息
        /// </summary>
        /// <param name="goodsInfo">商品信息</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Goods/SaveGoodsInfo",Method = "POST", 
            RequestFormat = WebMessageFormat.Json,ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("保存商品信息。")]
        ReturnResult<bool> SaveGoodsInfo(Model.Goods goodsInfo);
        /// <summary>
        /// 保存商品信息
        /// </summary>
        /// <param name="stream">图片流</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Goods/UpdatePictrue", Method = "POST",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("上传图片后获取图片地址。")]
        ReturnResult<string> UpdatePictrue(Stream stream);

        /// <summary>
        /// 商品点击量自增
        /// </summary>
        /// <param name="articleId">商品唯一标识</param>
        /// <returns>是否新增成功</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Goods/ClickCounIncrement",
            Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("文章阅读数量自增。")]
        ReturnResult<bool> ClickCounIncrement(string goodsId);
        #endregion

    }
}
