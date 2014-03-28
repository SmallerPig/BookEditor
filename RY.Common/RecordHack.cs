using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RY.Common
{
    public static class RecordHack
    {

        /// <summary>
        /// 服务器地址
        /// </summary>
        /// <returns></returns>
        public static string ServerUrl()
        {
            if (HttpContext.Current.Request.ServerVariables["Server_Port"].ToString() == "80")
                return "http://" + HttpContext.Current.Request.Url.Host;
            else
                return "http://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.ServerVariables["Server_Port"].ToString();
        }

        /// <summary>
        /// 当前地址
        /// </summary>
        public static string GetCurrentUrl
        {
            get
            {
                string strUrl;
                strUrl = HttpContext.Current.Request.ServerVariables["Url"];
                if (HttpContext.Current.Request.QueryString.Count == 0) //如果无参数


                    return strUrl;
                else
                    return strUrl + "?" + HttpContext.Current.Request.ServerVariables["Query_String"];
            }
        }

        /// <summary>
        /// 获得用户IP
        /// </summary>
        public static string GetUserIp
        {
            get
            {
                string ip;
                string[] temp;
                bool isErr = false;
                if (HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"] == null)
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                else
                    ip = HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"].ToString();
                if (ip.Length > 15)
                    isErr = true;
                else
                {
                    temp = ip.Split('.');
                    if (temp.Length == 4)
                    {
                        for (int i = 0; i < temp.Length; i++)
                        {
                            if (temp[i].Length > 3) isErr = true;
                        }
                    }
                    else
                        isErr = true;
                }

                if (isErr)
                    return "1.1.1.1";
                else
                    return ip;
            }
        }

        /// <summary>
        /// 当前访客
        /// </summary>
        public static string ThisUser()
        {
            if (System.Web.HttpContext.Current.User.Identity.Name != null)
                return System.Web.HttpContext.Current.User.Identity.Name;
            else
                return "游客";
        }

        /// <summary>
        /// 保存访问日志
        /// </summary>
        /// <param name="_type">1代表访问者,2代表非法</param>
        /// <param name="_second">脚本秒数</param>
        public static void SaveVisitLog(int _type, int _second)
        {
            SaveVisitLog(_type, _second, "");
        }

        /// <summary>
        /// 保存访问日志
        /// </summary>
        /// <param name="_type">1代表访问者,2代表非法</param>
        /// <param name="_second">脚本秒数</param>
        /// <param name="_logfilename">自定义log保存路径</param>
        public static void SaveVisitLog(int _type, int _second, string _logfilename)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(System.Web.HttpContext.Current.Server.MapPath("~/data_log/"));
            if (!dir.Exists)
                dir.Create();

            if (_type == 1)
            {
                string _savefile = _logfilename == "" ? "~/data_log/vister_" + DateTime.Now.ToString("yyyyMMdd") + ".log" : _logfilename;
                Single s = (Single)DateTime.Now.Subtract(HttpContext.Current.Timestamp).TotalSeconds;
                if (s > _second)
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(HttpContext.Current.Server.MapPath(_savefile), true, System.Text.Encoding.UTF8);
                    sw.WriteLine(System.DateTime.Now.ToString());
                    sw.WriteLine("\tIP 地 址：" + GetUserIp);
                    sw.WriteLine("\t访 问 者：" + ThisUser());
                    sw.WriteLine("\t浏 览 器：" + HttpContext.Current.Request.Browser.Browser + HttpContext.Current.Request.Browser.Version);
                    sw.WriteLine("\t耗    时：" + ((Single)DateTime.Now.Subtract(HttpContext.Current.Timestamp).TotalSeconds).ToString("0.000") + "秒");
                    sw.WriteLine("\t地    址：" + ServerUrl() + GetCurrentUrl);
                    sw.WriteLine("---------------------------------------------------------------------------------------------------");
                    sw.Close();
                    sw.Dispose();
                }
            }
            else
            {
                string _savefile = _logfilename == "" ? "~/data_log/hacker_" + DateTime.Now.ToString("yyyyMMdd") + ".log" : _logfilename;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(HttpContext.Current.Server.MapPath(_savefile), true, System.Text.Encoding.UTF8);
                sw.WriteLine(System.DateTime.Now.ToString());
                sw.WriteLine("\tIP 地 址：" + GetUserIp);
                sw.WriteLine("\t访 问 者：" + ThisUser());
                sw.WriteLine("\t浏 览 器：" + HttpContext.Current.Request.Browser.Browser + HttpContext.Current.Request.Browser.Version);
                sw.WriteLine("\t来    源：" + ServerUrl() + GetCurrentUrl);
                sw.WriteLine("\t地    址：" + ServerUrl() + GetCurrentUrl);
                sw.WriteLine("---------------------------------------------------------------------------------------------------");
                sw.Close();
                sw.Dispose();
            }
        }
    }
}