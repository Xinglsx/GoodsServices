using Goods.ServiceConstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Goods.DataObject;
using Goods.Model;
using Goods.Core;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;

namespace Goods.Service
{
    public class BaseInfoService : IBaseInfoService
    {
        #region 私有变量
        private string timeStyle = "yyyyMMddHHmmss";
        #endregion

        #region 构造函数
        public BaseInfoService()
        {
            FileInfo file = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log4Net.config"));
            log4net.Config.XmlConfigurator.Configure(file);
        }
        #endregion

        #region 版本相关
        /// <summary>
        /// 获取app版本信息
        /// </summary>
        /// <returns></returns>
        public ReturnResult<VersionInfo> GetVersionInfo()
        {
            ReturnResult<VersionInfo> result = new ReturnResult<VersionInfo>();
            VersionInfo version = new VersionInfo();
            version.versionNumber = 0;
            try
            {
                //获取txt文件中的版本号和更新内容
                string[] lines = File.ReadAllLines(GetVersionFilePath());
                bool isVersionNumber = false;
                bool isVersion = false;
                bool isUpdateContent = false;
                foreach (string line in lines)
                {
                    if (isVersionNumber)
                    {
                        try
                        {
                            version.versionNumber = Convert.ToInt32(line);
                        }
                        catch { }//转错不处理
                        
                        isVersionNumber = false;
                    }
                    else if (isVersion)
                    {
                        version.version = line;
                        isVersion = false;
                    }
                    else if (isUpdateContent)
                    {
                        version.updateContent += line + "\r\n";
                    }

                    if (line != string.Empty && line.Contains("VersionNumber"))
                    {
                        isVersionNumber = true;
                    }
                    if (line != string.Empty && line.Contains("Version"))
                    {
                        isVersion = true;
                    }
                    if (line != string.Empty && line.Contains("UpdateContent"))
                    {
                        isUpdateContent = true;
                    }
                }
                if(string.IsNullOrEmpty(version.version))
                {
                    version.version = "1.0.0";
                }
                if (string.IsNullOrEmpty(version.updateContent))
                {
                    version.updateContent = "优化了主要流程！";
                }
                version.downloadAddress = ConfigurationManager.AppSettings["Localhost"] 
                    + @"/Download/com.mingshu.goods.apk";

                result.data = version;
                result.code = 1;
            }
            catch (Exception ex)
            {
                result.code = -1;
                LogWriter.WebError(ex);
                result.message = ex.Message;
                return result;
            }
            return result;
        }
        #endregion

        #region 用户相关
        /// <summary>
        /// 验证用户登录信息，登录名支持用户名、邮箱、手机号。
        /// </summary>
        /// <param name="strCode">用户名、邮箱或手机号</param>
        /// <param name="password">用户密码(加密)</param>
        /// <returns></returns>
        public ReturnResult<Users> ValidateUserInfo(string strCode, string password)
        {
            ReturnResult<Users> result = new ReturnResult<Users>();

            try
            {
                using (GoodsEntities GoodsDb = new GoodsEntities())
                {
                    //验证用户
                    var query = (from user in GoodsDb.Users
                                 where user.userid == strCode
                                 select user).FirstOrDefault();
                    if (query == null)
                    {
                        result.code = -102;
                        result.message = "您输入的用户名不存在！";
                        return result;
                    }
                    else
                    {
                        if(EncryptDES(password, (query.registertime??DateTime.Now)
                            .ToString(timeStyle)) != query.password)
                        {
                            result.code = -103;
                            result.message = "您输入的密码错误！";
                            return result;
                        }
                        else
                        {
                            result.code = 1;
                            result.data = query;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                result.code = -1;
                result.message = "服务已断开，请稍后重试！";
                //记录日志
                LogWriter.WebError(exp);
            }

            return result;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="strCode">用户名</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        public ReturnResult<Users> RegisterUserInfo(string strCode, string password)
        {
            ReturnResult<Users> result = new ReturnResult<Users>();

            try
            {
                using (GoodsEntities GoodsDb = new GoodsEntities())
                {
                    //查找用户
                    var query = (from user in GoodsDb.Users
                                 where user.userid == strCode
                                 select user).ToList();
                    if (query != null && query.Count != 0)
                    {
                        result.code = -104;
                        result.message = "无法注册，用户名已存在！";
                        return result;
                    }
                    DateTime now = DateTime.Now;
                    //新增用户
                    Users tempUser = new Users
                    {
                        id = Guid.NewGuid().ToString(),
                        userid = strCode,
                        nickname = strCode,
                        password = EncryptDES(password, now.ToString(timeStyle)),
                        usertype = 0,
                        registertime = now,
                    };
                    GoodsDb.Users.Add(tempUser);
                    if (GoodsDb.SaveChanges() <= 0)
                    {
                        result.code = -1;
                        result.message = "用户信息注册失败！";
                        return result;
                    }
                    result.code = 1;
                    result.data = tempUser;
                }
            }
            catch (Exception exp)
            {
                result.code = -1;
                result.message = "服务已断开，请稍后重试！";
                //记录日志
                LogWriter.WebError(exp);
            }

            return result;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="type">用户类型 0-普通用户、游客 1-VIP用户 2-SVIP用户 
        /// 3-特约用户（可发布商品） 4-管理员 5-超级管理员 9-全部</param>
        /// <param name="filter">查询条件</param>
        /// <returns></returns>
        public ReturnResult<List<Users>> GetUserInfos(int curPage, int pageSize, int type, string filter)
        {
            ReturnResult<List<Users>> result = new ReturnResult<List<Users>>();
            using (GoodsEntities GoodsDb = new GoodsEntities())
            {
                try
                {
                    //查寻用户
                    result.data = (from user in GoodsDb.Users
                                   where (filter == "" ||user.userid.Contains(filter)) 
                                   && (user.usertype == type || (type == 9 && user.usertype != 5))
                                   orderby user.registertime descending
                                   select user).Skip(curPage * pageSize).Take(pageSize).ToList();
                }
                catch (Exception exp)
                {
                    LogWriter.WebError(exp);
                    result.code = -1;
                    result.message = "服务已断开，请稍后重试！";
                    return result;
                }
                result.code = 1;
            }
            return result;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userInfo">待更新用户信息</param>
        /// <returns></returns>
        public ReturnResult<bool> SaveUserInfo(Users userInfo)
        {
            ReturnResult<bool> result = new ReturnResult<bool>();

            try
            {
                using (GoodsEntities GoodsDb = new GoodsEntities())
                {
                    //验证用户
                    Users updateUser = (from user in GoodsDb.Users
                                 where user.id == userInfo.id
                                 select user).FirstOrDefault();
                    updateUser.userlevel = userInfo.userlevel;
                    updateUser.nickname = userInfo.nickname;
                    updateUser.phonenumber = userInfo.phonenumber;
                    updateUser.qq = userInfo.qq;
                    updateUser.realname = userInfo.realname;
                    updateUser.sina = userInfo.sina;
                    updateUser.taobao = userInfo.taobao;
                    updateUser.usertype = userInfo.usertype;
                    updateUser.wechat = userInfo.wechat;
                    updateUser.birth = userInfo.birth;
                    updateUser.idcard = userInfo.idcard;

                    if(GoodsDb.SaveChanges()<= 0)
                    {
                        result.code = -1;
                        result.message = "没有数据更新！";
                    }
                    else
                    {
                        result.code = 1;
                        result.data = true; ;
                    }
                }
            }
            catch (Exception exp)
            {
                result.code = -1;
                result.message = "服务已断开，请稍后重试！";
                //记录日志
                LogWriter.WebError(exp);
            }

            return result;
        }
        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="id">用户主键</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        public ReturnResult<bool> ChangePassword(string id, string oldPassword, string newPassword)
        {
            ReturnResult<bool> result = new ReturnResult<bool>();

            try
            {
                using (GoodsEntities goodsDb = new GoodsEntities())
                {
                    var user = (from u in goodsDb.Users
                                 where u.id == id
                                 select u).FirstOrDefault();
                    oldPassword = EncryptDES(oldPassword, (user.registertime ?? DateTime.Now)
                        .ToString(timeStyle));
                    LogWriter.WebLog("oldPassword:" + oldPassword);
                    if(user.password == oldPassword)
                    {
                        newPassword = EncryptDES(newPassword, (user.registertime ?? DateTime.Now)
                            .ToString(timeStyle));
                        LogWriter.WebLog("newPassword:" + newPassword);
                        user.password = newPassword;
                        if(goodsDb.SaveChanges() <= 0)
                        {
                            result.code = -110;
                            result.message = "用户密码更新失败！";
                           
                        }
                        result.code = 1;
                        result.data = true;
                    }
                    else
                    {
                        result.code = -109;
                        result.message = "用户原密码错误！";
                    }
                };
            }
            catch (Exception exp)
            {
                //记录日志
                LogWriter.WebError(exp);
                result.code = -1;
                result.message = "服务已断开，请稍后重试！";
            }

            return result;
        }

        #endregion

        #region 商品相关
        /// <summary>
        /// 获取商品-
        /// </summary>
        /// <param name="curPage">获取第几页 默认从0开始</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="type">0-保存草稿 1-待审核 2-已审核 9-全部 10-保存草稿+待审核</param>
        /// <returns></returns>
        public ReturnResult<List<Model.Goods>> GetGoodsList(int curPage, int pageSize,int type)
        {
            List<int> states = new List<int>();
            if (type == 10)
            {
                states.Add(0);
                states.Add(1);
            }
            else if (type == 9)
            {
                states.Add(0);
                states.Add(1);
                states.Add(2);
                states.Add(3);
            }
            else
            {
                states.Add(type);
            }

            if (pageSize <= 0) pageSize = 10;//默认取10条
            ReturnResult<List<Model.Goods>> result = new ReturnResult<List<Model.Goods>>();
            try
            {
                using (GoodsEntities GoodsDb = new GoodsEntities())
                {
                    var query = (from goods in GoodsDb.Goods
                                 where states.Contains(goods.state ?? 0)
                                 orderby goods.recommendtime descending
                                 select goods).Skip(curPage * pageSize).Take(pageSize);
                    List<Goods.Model.Goods> tempResult = query.ToList();
                    if (tempResult == null || tempResult.Count == 0)
                    {
                        result.code = -105;
                        result.message = "已加载完。";
                        return result;
                    }
                    result.code = 1;
                    result.data = tempResult;
                }
            }
            catch (Exception exp)
            {
                result.code = -1;
                result.message = "服务已断开，请稍后重试！";
                //记录日志
                LogWriter.WebError(exp);
            }
            return result;
        }

        /// <summary>
        /// 保存商品信息
        /// </summary>
        /// <param name="goodsInfo">商品信息</param>
        /// <returns></returns>
        public ReturnResult<bool> SaveGoodsInfo(Model.Goods goodsInfo)
        {
            ReturnResult<bool> result = new ReturnResult<bool>();
            if(!string.IsNullOrEmpty(goodsInfo.id))//需要修改的数据
            {
                if(goodsInfo.state >= 2)
                {
                    goodsInfo.audittime = DateTime.Now;
                }
                else
                {
                    goodsInfo.recommendtime = DateTime.Now;
                }
            }
            
            try
            { 
                using (GoodsEntities GoodsDb = new GoodsEntities())
                {
                    if (string.IsNullOrEmpty(goodsInfo.id))
                    {
                        goodsInfo.id = Guid.NewGuid().ToString();
                        goodsInfo.image = ImageUtil.StrToUri(goodsInfo.image, goodsInfo.id + ".jpg");
                        goodsInfo.buyimage = ImageUtil.StrToUri(goodsInfo.buyimage, goodsInfo.id + "_buy.jpg");
                        GoodsDb.Goods.Add(goodsInfo);
                    }
                    else
                    {
                        //修改的商品，可能已经是地址模式化
                        if (goodsInfo.image != null && !goodsInfo.image.Contains(".jpg"))
                        {
                            goodsInfo.image = ImageUtil.StrToUri(goodsInfo.image, goodsInfo.id + ".jpg");
                        }
                        if (goodsInfo.image != null && !goodsInfo.image.Contains("http:"))
                        {
                            goodsInfo.buyimage = ImageUtil.StrToUri(goodsInfo.buyimage, goodsInfo.id + "_buy.jpg");
                        }
                        Model.Goods temp = GoodsDb.Goods.First(p => p.id == goodsInfo.id);
                        temp.state = goodsInfo.state;
                        temp.image = goodsInfo.image;
                        temp.buyimage = goodsInfo.buyimage;
                        temp.recommender = goodsInfo.recommender;
                        temp.recommendname = goodsInfo.recommendname;
                        temp.auditname = goodsInfo.auditname;
                        temp.audituser = goodsInfo.audituser;
                        temp.audittime = goodsInfo.audittime;
                    }
                    GoodsDb.SaveChanges();
                    result.code = 1;
                    result.data = true;
                }
            }
            catch (Exception exp)
            {
                LogWriter.WebError(exp);
                result.code = -1;
                result.message = "服务已断开，请稍后重试！";
                result.data = false;
            }
            return result;
        }
        /// <summary>
        /// 将上传的图片流转成本地文件并返回文件的地址
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public ReturnResult<string> UpdatePictrue(String strBase64)
        {
            ReturnResult<string> result = new ReturnResult<string>();

            byte[] bt = Convert.FromBase64String(strBase64);

            string fileName = Guid.NewGuid().ToString() + ".jpg";
            string savePath = @"D:\GoodsService\";
            string relativePath = @"images\goods\";
            string uploadFolder = savePath + relativePath;

            string filePath = Path.Combine(uploadFolder, fileName);
            File.WriteAllBytes(filePath, bt);

            //byte[] buff = System.Text.Encoding.ASCII.GetBytes(strBase64);
            //MemoryStream ms = new MemoryStream(buff);
            //Image image = Image.FromStream(ms);
            //image.Save(filePath);
            //ms.Close();

            result.code = 1;
            result.data = @"http://192.168.10.61:8890/" + relativePath + fileName;
            return result;
        }
        /// <summary>
        /// 商品点击量自增
        /// </summary>
        /// <param name="articleId">商品唯一标识</param>
        /// <returns>是否新增成功</returns>
        public ReturnResult<bool> ClickCounIncrement(string goodsId)
        {
            ReturnResult<bool> result = new ReturnResult<bool>();

            try
            {
                using (GoodsEntities GoodsDb = new GoodsEntities())
                {
                    Model.Goods query = GoodsDb.Goods.FirstOrDefault(goods =>
                        goods.id == goodsId && goods.state == 2);
                    if (query == null)
                    {
                        result.code = -106;
                        result.message = "找不到对应商品。";
                        return result;
                    }
                    query.clickcount = (query.clickcount ?? 0) + 1;
                    if (GoodsDb.SaveChanges() <= 0)
                    {
                        result.code = -107;
                        result.message = "商品点击数未更新成功。";
                        return result;
                    }
                    result.code = 1;
                    result.data = true;
                }
            }
            catch (Exception exp)
            {
                result.code = -1;
                result.message = "服务已断开，请稍后重试！";
                //记录日志
                LogWriter.WebError(exp);
            }

            return result;
        }
        #endregion

        #region 私有方法
        private string GetVersionFilePath()
        {
            try
            {
                string versionFilePath = ConfigurationManager.AppSettings["AppVersionFilePath"];
                return versionFilePath;
            }
            catch (Exception ex)
            {
                return @"C:\AppVersion.txt";
            }
        }

        #region 加密相关
        //默认密钥向量
        private byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        private string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        private string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }

        #endregion
        #endregion

    }
}