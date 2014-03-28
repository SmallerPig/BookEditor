using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace RY.Common
{
    /// <summary>
    /// Cookie操作类
    /// add by zhouzhilong
    /// add time:2011-10-13
    /// version：1.3
    /// </summary>
    public static class CookiesHelp
    {
        #region cookies加密解密方法组合
        /// <summary>
        /// cookies加密组合方式
        /// 暂定base64二次加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string JiaCoo(string str)
        {
            Encrypt enc = new Encrypt();
            return Encrypt.Base64Encode("gb2312", Encrypt.Base64Encode("gb2312", str));
        }
        /// <summary>
        /// cookies解密组合方式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string JieCoo(string str)
        {
            Encrypt enc = new Encrypt();
            return Encrypt.Base64Decode("gb2312", Encrypt.Base64Decode("gb2312", str));
        }
        #endregion

        #region 写入到同一个Cookies对象
        /// <summary>
        /// 写入到同一个Cookies对象
        /// </summary>
        /// <param name="cookieObjName">cookie对象名</param>
        /// <param name="cookieKeyName">cookie键名</param>
        /// <param name="cookieKeyValue">cookie键值</param>
        /// <param name="iExpires">cookie保存时间</param>
        public static void SetCookieTotal(string cookieObjName, string cookieKeyName, string cookieKeyValue, string iExpires)
        {
            if (HttpContext.Current.Request.Cookies["UserInfo"] == null)
            {
                HttpCookie cookie = new HttpCookie("UserInfo");
                DateTime dt = DateTime.Now;
                cookie.Expires = dt.Add(new TimeSpan(0, 6, 0, 0, 0));
                cookie.Values.Add("id", JiaCoo("1"));
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        #endregion

        #region 创建一个键是key的值为value的cookies对象
        /// <summary>
        /// 创建cookies对象，并设置value值，修改cookies的value值也用这个方法，因为对cookies修改必须重新设Expires
        /// </summary>
        /// <param name="cookieKeyName">cookies对象名</param>
        /// <param name="cooKeyValue">cookie对象Value值</param>
        /// <param name="iExpires">cookie对象有效时间(此处是小时)</param>
        /// <param name="strDomains">cookie作用域</param>
        /// <param name="strPath">cookie传输的虚拟路径</param>
        public static void SetCookie(string cookieKeyName, string cooKeyValue, int iExpires, string strDomains, string strPath)
        {
            if (!string.IsNullOrEmpty(cookieKeyName) && !string.IsNullOrEmpty(cooKeyValue))
            {
                HttpCookie objCookie = new HttpCookie(cookieKeyName.Trim());
                objCookie.Value = JiaCoo(cooKeyValue.Trim());
                if (iExpires > 0)
                {
                    DateTime dt = DateTime.Now;
                    objCookie.Expires = dt.Add(new TimeSpan(0, iExpires, 0, 0, 0));
                }
                string _strDomain = SelectDomain(strDomains);
                if (_strDomain.Length > 0)
                {
                    objCookie.Domain = _strDomain;
                }
                objCookie.Path = strPath.Trim();
                HttpContext.Current.Response.Cookies.Add(objCookie);
            }
        }
        public static void SetCookie(string cookieKeyName, string strKeyValue, int iExpires, string strDomains)
        {
            SetCookie(cookieKeyName, strKeyValue, iExpires, strDomains, "/");
        }
        public static void SetCookie(string cookieKeyName, string strKeyValue, int iExpires)
        {
            SetCookie(cookieKeyName, strKeyValue, iExpires, "", "/");
        }
        /// <summary>
        /// 默认为一个小时(iExpires参数不加时)
        /// </summary>
        /// <param name="cookieKeyName"></param>
        /// <param name="strKeyValue"></param>
        public static void SetCookie(string cookieKeyName, string strKeyValue)
        {
            SetCookie(cookieKeyName, strKeyValue, 1, "", "/");//默认为一小时
        }
        #endregion

        #region 创建cookies对象并赋多个KEY键值

        /// <summary>
        /// 创建COOKIE对象并赋多个KEY键值
        /// 设键/值如下：
        /// NameValueCollection myCol = new NameValueCollection();
        /// myCol.Add("red", "rojo");
        /// myCol.Add("green", "verde");
        /// myCol.Add("blue", "azul");
        /// myCol.Add("red", "rouge");   结果“red:rojo,rouge；green:verde；blue:azul”
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">COOKIE对象有效时间（小时）</param>
        /// <param name="KeyValue">键/值对集合</param>
        /// <param name="strDomains">作用域,多个域名用;隔开</param>
        /// <param name="strPath">作用路径</param>
        public static void SetCookieObj(string strCookieName, int iExpires, NameValueCollection KeyValue, string strDomains, string strPath)
        {
            HttpCookie objCookie = new HttpCookie(strCookieName.Trim());
            foreach (String key in KeyValue.AllKeys)
            {
                objCookie[key] = JiaCoo(KeyValue[key].Trim());
            }
            if (iExpires > 0)
            {
                DateTime dt = DateTime.Now;
                objCookie.Expires = dt.Add(new TimeSpan(0, iExpires, 0, 0, 0));
            }
            string _strDomain = SelectDomain(strDomains);
            if (_strDomain.Length > 0)
            {
                objCookie.Domain = _strDomain;
            }
            objCookie.Path = strPath.Trim();
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }
        public static void SetCookieObj(string strCookieName, int iExpires, NameValueCollection KeyValue, string strDomains)
        {
            SetCookieObj(strCookieName, iExpires, KeyValue, strDomains, "/");
        }
        public static void SetCookieObj(string strCookieName, int iExpires, NameValueCollection KeyValue)
        {
            SetCookieObj(strCookieName, iExpires, KeyValue, "", "/");
        }
        #endregion

        #region 修改某个cookies对象的key键的值

        /// <summary>
        /// 修改某个COOKIE对象某个Key键的键值 或 给某个COOKIE对象添加Key键 都调用本方法，
        /// 操作成功返回字符串"success"，如果对象本就不存在，则返回字符串null。
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <param name="KeyValue">Key键值</param>
        /// <param name="iExpires">COOKIE对象有效时间(小时);注意：虽是修改功能，实则重建覆盖，所以时间也要重设，因为没办法获得旧的有效期</param>
        /// <param name="strDomains">作用域,多个域名用;隔开</param>
        /// <param name="strPath">作用路径</param>
        /// <returns>如果对象本就不存在，则返回字符串null，如果操作成功返回字符串"success"。</returns>
        public static string EditCookie(string strCookieName, string strKeyName, string KeyValue, int iExpires, string strDomains, string strPath)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return null;
            }
            else
            {
                HttpCookie objCookie = HttpContext.Current.Request.Cookies[strCookieName];
                objCookie[strKeyName] = JiaCoo(KeyValue.Trim());
                if (iExpires > 0)
                {
                    DateTime dt = DateTime.Now;
                    objCookie.Expires = dt.Add(new TimeSpan(0, iExpires, 0, 0, 0));
                }
                string _strDomain = SelectDomain(strDomains);
                if (_strDomain.Length > 0)
                {
                    objCookie.Domain = _strDomain;
                }
                objCookie.Path = strPath.Trim();
                HttpContext.Current.Response.Cookies.Add(objCookie);
                return "success";
            }
        }
        public static string EditCookie(string strCookieName, string strKeyName, string KeyValue, int iExpires, string strPath)
        {
            return EditCookie(strCookieName, strKeyName, KeyValue, iExpires, "", strPath);
        }
        public static string EditCookie(string strCookieName, string strKeyName, string KeyValue, int iExpires)
        {
            return EditCookie(strCookieName, strKeyName, KeyValue, iExpires, "", "/");
        }
        #endregion

        #region 删除某个cookies对象或某个cookies键的值

        /// <summary>
        /// 删除COOKIE对象【注意是一个集合】
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strDomains">作用域,多个域名用;隔开</param>
        /// <param name="strPath">作用路径</param>
        public static void DeleteCookiesObj(string strCookieName, string strDomains, string strPath)
        {
            HttpCookie objCookie = new HttpCookie(strCookieName.Trim());
            string _strDomain = SelectDomain(strDomains);
            if (_strDomain.Length > 0)
            {
                objCookie.Domain = _strDomain;
            }
            objCookie.Path = strPath.Trim();
            objCookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }
        public static void DeleteCookiesObj(string strCookieName, string strDomains)
        {
            DeleteCookiesObj(strCookieName, strDomains, "/");
        }
        public static void DeleteCookiesObj(string strCookieName)
        {
            DeleteCookiesObj(strCookieName, "", "/");
        }


        /// <summary>
        /// 删除某个COOKIE对象或某个Key子键，操作成功返回字符串"success"，如果对象本就不存在，则返回字符串null
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名(为空表示删除整个cookie对象)</param>
        /// <param name="iExpires">COOKIE对象有效时间注意：虽是修改功能，实则重建覆盖，所以时间也要重设，因为没办法获得旧的有效期</param>
        /// <returns>如果对象本就不存在，则返回字符串null，如果操作成功返回字符串"success"。</returns>
        public static string RemoveCookie(string strCookieName, string strKeyName)
        {
            HttpResponse response = HttpContext.Current.Response;
            if (response != null)
            {
                HttpCookie cookie = response.Cookies[strCookieName];
                if (cookie != null)
                {
                    if (!string.IsNullOrEmpty(strKeyName) && cookie.HasKeys)
                    {
                        TimeSpan ts = new TimeSpan(-1, 0, 0, 0);
                        cookie.Expires = DateTime.Now.Add(ts);
                        response.AppendCookie(cookie);
                        //cookie.Values.Remove(strKeyName);
                    }
                    else
                    {
                        TimeSpan ts = new TimeSpan(-1, 0, 0, 0);
                        cookie.Expires = DateTime.Now.Add(ts);
                        response.AppendCookie(cookie);
                        //response.Cookies.Remove(strCookieName);
                    }
                    return "success";
                }
                else
                {
                    return "false";
                }

            }
            else
            {
                return null;
            }
        }
        public static string RemoveCookie(string strCookieName)
        {
            return RemoveCookie(strCookieName, null);
        }
        #endregion

        #region 读取某个cookies对象或者某个键对应的值

        /// <summary>
        /// 读取Cookie某个对象的Value值(注意，这个对象是个集合字符串)，
        /// example：id=1&username=Jon&last_login_time=2010-10-14 16:21:24 
        /// 返回Value值，如果对象不存在，则返回字符串null
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <returns>Value值，如果对象本就不存在，则返回字符串null</returns>
        public static string GetCookieValue(string strCookieName)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return null;
            }
            else
            {
                string _value = HttpContext.Current.Request.Cookies[strCookieName].Value;
                return JieCoo(_value);
                //example：id=1&username=Jon&last_login_time=2010-10-14 16:21:24 
            }
        }

        /// <summary>
        /// 读取Cookie某个对象的某个Key键的键值，返回Key键值，如果对象不存在，则返回字符串null，

        /// 如果Key键不存在，则返回字符串"KeyNonexistence"
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <returns>Key键值，如果对象不存在，则返回字符串null，如果Key键不存在，则返回字符串"KeyNonexistence"</returns>
        public static string GetCookieValue(string strCookieName, string strKeyName)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return null;
            }
            else
            {
                HttpCookie coo = HttpContext.Current.Request.Cookies[strCookieName];
                if (string.IsNullOrEmpty(coo[strKeyName]))
                {
                    return null;
                }
                else
                {
                    string _value = HttpContext.Current.Request.Cookies[strCookieName][strKeyName];
                    return JieCoo(_value);
                }
            }
        }
        #endregion

        #region 定位Cookies作用域

        /*
         * 相同的所有Cookie 在客户端都存在一个文件中;
         * Cookie之间以”*”分割,每个Cookie的第一行是 Cookie 的名称，第二行是值,
         * 第三行是Domain属性＋Path属性组成的一个字符串，指示此Cookie的作用域;
         * 
         * Request.Cookies 属性中包含了客户端发送到服务器的所有Cookie的集合，
         * 只有在请求URL的作用范围内的Cookie才会被浏览器连同Http请求一起发送到服务器,
         */
        /// <summary>
        /// 将Cookie定位到正确的域

        /// </summary>
        /// <param name="strDomains">cookie作用域</param>
        /// <returns>作用域</returns>
        private static string SelectDomain(string strDomains)
        {
            bool _isLocalServer = false;
            if (strDomains.Trim().Length == 0)
            {
                return "";
            }
            string _thisDomain = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();
            //if (_thisDomain.IndexOf(".") < 0)//没有找到返回-1，说明是计算机名，找到>0是域名

            if (!_thisDomain.Contains("."))
            {
                _isLocalServer = true;
            }
            string _strDomain = "school51.come";//这个域名是暂时编的，具体定位到当前域名

            string[] _strDomains = strDomains.Split(';');
            for (int i = 0; i < _strDomains.Length; i++)
            {
                if (!_thisDomain.Contains(_strDomains[i].Trim()))
                {
                    continue;
                }
                else
                {
                    //区分真实域名(或IP)与计算机名
                    if (_isLocalServer)
                        _strDomain = "";//作用域留空，否则Cookie不能写入
                    else
                        _strDomain = _strDomains[i].Trim();
                    break;
                }
            }
            return _strDomain;
        }
        #endregion
    }
}