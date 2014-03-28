using RY.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookEdit.BLL
{
    public class User
    {
        public static int Login(string userName, string password,string address )
        {
            string postData = "username=" + userName + "&password=" + password;

            HttpHelper httpHelper = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = address,//URL     必需项    
                //PostDataType = PostDataType.String,
                Method = "post",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写可选项默认转小写   
                Postdata = postData,//Post数据     可选项GET时不需要写   
                ContentType = "application/x-www-form-urlencoded",
            };


            HttpResult result = httpHelper.GetHtml(item);
            string html = result.Html;
            int returnresult = 0;

            return int.TryParse(html, out returnresult) ? returnresult : 0;

        }

        public static string GetBookList(string address)
        {
            HttpHelper httpHelper = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = address,//URL     必需项    
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
            };


            HttpResult result = httpHelper.GetHtml(item);
            string html = result.Html;
            return html;

        }



    }
}
