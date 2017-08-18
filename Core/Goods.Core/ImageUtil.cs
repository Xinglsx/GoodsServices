using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;

namespace Goods.Core
{
    public class ImageUtil
    {
        /// <summary>
        /// 将上传的Base64图片转成文件的地址
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string StrToUri(String strBase64,string fileName)
        {
            string imagePath = ConfigurationManager.AppSettings["ImagePath"];
            string imageUri = ConfigurationManager.AppSettings["ImageUri"];

            string filePath = Path.Combine(imagePath, fileName);
            string result = string.Empty;
            byte[] bt = Convert.FromBase64String(strBase64);
            File.WriteAllBytes(filePath, bt);
            result = imageUri + fileName;

            return result;
        }

    }
}
