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

namespace Goods.Service
{
    public class BaseInfoService : IBaseInfoService
    {
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
                                 select user).ToList();
                    if (query == null || query.Count == 0)
                    {
                        result.code = -102;
                        result.message = "您输入的用户名不存在！";
                        return result;
                    }
                    //匹配密码
                    query = (from user in GoodsDb.Users
                             where user.userid == strCode && user.password == password
                             select user).ToList();
                    if (query == null || query.Count == 0)
                    {
                        result.code = -103;
                        result.message = "您输入的密码错误！";
                        return result;
                    }
                    result.code = 1;
                    result.data = query.FirstOrDefault();
                    //result.data.passWord = "";//不需要返回密码
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
        /// <param name="password">用户密码(暂未加密)</param>
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
                    //新增用户
                    Users tempUser = new Users
                    {
                        id = Guid.NewGuid().ToString(),
                        userid = strCode,
                        nickname = strCode,
                        password = password,
                        registertime = DateTime.Now,
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
        #endregion

    }
}