/*==========================================================
*作者：SmallerPig
*时间：2013/8/26 13:34:08
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace RY.Common
{
    public class AppcleValicate
    {

        static string url = "https://sandbox.itunes.apple.com/verifyReceipt";//苹果服务器验证地址

        public static int ValicateCer(string cer)
        {
            string par = cer;
            string postdata = "{\"receipt-data\":\"" + par + "\"}";
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,//URL     必需项    
                Method = "post",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = "",//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = postdata,//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "text/html",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
            };
            HttpResult result = http.GetHtml(item);
            string html = result.Html;

            AppleValicateResult result1 = JSONToObject<AppleValicateResult>(html);
            return result1.status;
        }


        public static T JSONToObject<T>(string jsonText)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<T>(jsonText);
        }

        class AppleValicateResult
        {
            public int status { get; set; }
        }
    }
}
