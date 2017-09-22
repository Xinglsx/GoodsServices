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
        #region 版本信息
        /// <summary>
        /// 获取当前版本信息
        /// </summary>
        /// <param name="ipAddress">访问地址</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "GetVersionInfo", Method = "GET",
                   ResponseFormat = WebMessageFormat.Json)]
        [Description("获取服务器APP版本信息。")]
        ReturnResult<VersionInfo> GetVersionInfo();
        #endregion

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
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="type">用户类型 0-普通用户、游客 1-VIP用户 2-SVIP用户 
        /// 3-特约用户（可发布商品） 4-管理员 5-超级管理员 9-全部</param>
        /// <param name="filter">查询条件</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "UserInfo/GetUserInfos?curPage={curPage}&pageSize={pageSize}&type={type}&filter={filter}",
            Method = "GET",RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("获取用户信息。")]
        ReturnResult<List<Users>> GetUserInfos(int curPage, int pageSize, int type,string filter);
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userInfo">待更新用户信息</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "UserInfo/SaveUserInfo", Method = "POST",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("更新用户信息。")]
        ReturnResult<bool> SaveUserInfo(Users userInfo);
        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="id">用户主键</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "UserInfo/ChangePassword", Method = "POST",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("用户修改密码。")]
        ReturnResult<bool> ChangePassword(string id, string oldPassword, string newPassword);
        /// <summary>
        /// 保存用户反馈信息
        /// </summary>
        /// <param name="userInfo">用户反馈信息</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "UserInfo/SaveQuestion", Method = "POST",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("保存用户反馈信息。")]
        ReturnResult<bool> SaveQuestion(Questions questionInfo);
        #endregion

        #region 商品相关

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="curPage">获取第几页 默认从0开始</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="type">获取类型</param>
        /// <param name="filter">查询条件</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Goods/GetGoodsList?curPage={curPage}&pageSize={pageSize}&type={type}&filter={filter}", 
            Method = "GET", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("获取商品列表。")]
        ReturnResult<List<Model.Goods>> GetGoodsList(int curPage,int pageSize, int type,string filter);

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
        ReturnResult<string> UpdatePictrue(string strBase64);

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

        #region 广告相关

        /// <summary>
        /// 获取广告信息
        /// </summary>
        /// <param name="key">广告关键字</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Advertisement/GetAdvertisement?key={key}",
            Method = "GET", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("获取广告信息")]
        ReturnResult<AdInfo> GetAdvertisement(string key);
        #endregion

        #region 淘宝接口
        /// <summary>
        /// 获取淘宝客粉丝优惠券列表
        /// </summary>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="q">查询条件</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Tbk/GetCouponList?pageNo={pageNo}&pageSize={pageSize}&q={q}",
            Method = "GET", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("获取淘宝客粉丝优惠券列表。")]
        ReturnResult<List<CouponInfo>> GetCouponList(long pageNo, long pageSize, string q);
        /// <summary>
        /// 创建淘口令
        /// </summary>
        /// <param name="text">界面显示文字</param>
        /// <param name="url">需要转换的URL</param>
        /// <param name="logo">界面显示的图片</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "Tbk/CreateTpwd?text={text}&url={url}&logo={logo}",
            Method = "GET", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("创建淘口令。")]
        ReturnResult<string> CreateTpwd(string text, string url, string logo);
        #endregion
    }
}
