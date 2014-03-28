using System;
using System.Text.RegularExpressions;

namespace RY.Common
{
    /// <summary>
    /// 字符串操作类
    /// add by: zhouzhilong
    /// vs:1.2
    /// </summary>
    public static class StringHelp
    {


        /// <summary>
        /// 生成随机校验码
        /// </summary>
        /// <param name="num">生成校验码位数</param>
        /// <returns></returns>
        public static string GenCode(int num)
        {
            string[] source ={"0","1","2","3","4","5","6","7","8","9",
                         "A","B","C","D","E","F","G","H","I","J","K","L","M","N",
                       "O","P","Q","R","S","T","U","V","W","X","Y","Z",
                             "a","b","c","d","e","f","g","h","i","j","k","l","m","n",
                       "o","p","q","r","s","t","u","v","w","x","y","z"};
            string code = "";
            Random rd = new Random();
            for (int i = 0; i < num; i++)
            {
                code += source[rd.Next(0, source.Length)];
            }
            return code;
        }




        #region 字符串 翻转、截取、替换
        /// <summary>
        /// 三元判断字符串
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static string Formate2Str(string str1, string str2)
        {
            return (!string.IsNullOrEmpty(str1) && str1.Trim().Length > 0) ? str1 : ((!string.IsNullOrEmpty(str2) && str2.Trim().Length > 0) ? str2 : "");
        }

        /// <summary>
        /// 翻转一个字符串
        /// </summary>
        /// <param name="inputString">目标字符串</param>
        /// <returns>翻转后的字符串</returns>
        public static string ReverseStr(string inputString)
        {
            char[] c = inputString.ToCharArray();
            System.Array.Reverse(c);
            return new string(c);
        }

        /// <summary>
        /// 截取字符串(一般截取，无汉字英文判断)
        /// </summary>
        /// <param name="originalStr">待截取的字符串</param>
        /// <param name="len">截取的长度</param>
        /// <param name="type">保留形式(0无省略号，1有省略号,省略号长度不计算在内)</param>
        /// <returns>截取后的字符串</returns>
        public static string CutStirngNormal(string originalStr, int len, int type)
        {
            if (len >= originalStr.Trim().Length)
            {
                return originalStr.Trim();
            }
            else
            {
                return originalStr.Trim().Substring(0, len) + (type == 1 ? "......" : "");
            }
        }

        /// <summary>
        /// 截取指定长度字符串,汉字为2个字符
        /// </summary>
        /// <param name="inputString">要截取的目标字符串</param>
        /// <param name="len">截取长度</param>
        /// <returns>截取后的字符串</returns>
        public static string CutString(string inputString, int len)
        {
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }
                if (tempLen > len)
                    break;
            }
            return tempString;
        }

        /// <summary>
        /// 去掉多余空格
        /// </summary>
        /// <param name="original">目标字符串</param>
        /// <returns>去除空格后的字符串</returns>
        public static string RemoveSpaceStr(string originalStr)
        {
            return System.Text.RegularExpressions.Regex.Replace(originalStr, "\\s{2,}", " ");
        }

        /// <summary>
        /// 不区分大小写的进行字符串替换
        /// </summary>
        /// <param name="original">原字符串</param>
        /// <param name="pattern">需替换字符</param>
        /// <param name="replacement">被替换内容</param>
        /// <returns>替换后的字符串</returns>
        public static string ReplaceEx(string original, string pattern, string replacement)
        {
            int count;
            int position0;
            int position1;

            count = 0;
            position0 = 0;
            position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
            {
                for (int i = position0; i < position1; ++i) chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i) chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i) chars[count++] = original[i];
            return new string(chars, 0, count);
        }
        #endregion

        #region 一般特殊字符过滤

        /// <summary>
        /// 过滤一些特殊符号，如单引号、双引号、回车符和分号等;
        /// 可扩展该方法.
        /// </summary>
        /// <param name="theString">待过滤的字符</param>
        /// <returns>过滤后的字符</returns>
        public static string SafetyStr(string theString)
        {
            string[] arryReg = { "'", ";", "\"", "\r", "\n" };
            for (int i = 0; i < arryReg.Length; i++)
            {
                theString = theString.Replace(arryReg[i], string.Empty);
            }
            return theString;
        }
        /// <summary>
        /// 过滤所有特殊特号(&、_除外)
        /// </summary>
        /// <param name="theString">目标字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string FilterSymbolStr(string theString)
        {
            //string[] aryReg = { "'", "\"", "\r", "\n", "<", ">", "%", "?", ",", ".", "=", "-", "_", ";", "|", "[", "]", "&", "/" };
            //考虑到url参数含有&、_问题，对字符串数组进行修改，另外/ %在url中进行HttpUtility.UrlEncode方法进行编码时
            //string[] aryReg = { "'", "\"", "\r", "\n", "<", ">", "?", ",", ".", "=", "-", ";", "|", "[", "]", "/" };
            //会出先，对此情况应先进行HttpUtility.UrlDecode解码，进行转换。【last edit time 2011-10-23,zhouzhilong】

            string[] aryReg = { "'", "\"", "\r", "\n", "<", ">", "%", "?", ",", ".", "=", "-", "_", ";", "|", "[", "]", "&", "/" };
            for (int i = 0; i < aryReg.Length; i++)
            {
                theString = theString.Replace(aryReg[i], string.Empty);
            }
            return theString;
        }
        #endregion

        #region HTML特殊字符处理
        ///<summary>   
        ///去除HTML标记(html代码提取纯文本内容)
        ///</summary>   
        ///<param name="NoHTML">包括HTML的源码</param>   
        ///<returns>已经去除后的文字</returns>   
        public static string NoHtmlTag(string Htmlstring)
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&ldquo;", "“", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&rdquo;", "”", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring = Htmlstring.Replace("<", "&lt;");
            Htmlstring = Htmlstring.Replace(">", "&gt;");
            return Htmlstring;
        }

        /// <summary>
        /// 去掉html里面的js
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string NoJsInHtml(string Htmlstring)
        {
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"([\r\n])[\s]+", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring.Replace("\r\n", " ");
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return Htmlstring;
        }

        /// <summary>
        /// 替换html中的特殊字符
        /// </summary>
        /// <param name="theString">需要进行替换的文本。</param>
        /// <returns>替换完的文本。</returns>
        public static string HtmlEncode(string theString)
        {
            theString = theString.Replace(">", "&gt;");
            theString = theString.Replace("<", "&lt;");
            theString = theString.Replace("  ", " &nbsp;");
            theString = theString.Replace("\"", "&quot;");
            theString = theString.Replace("'", "&#39;");
            theString = theString.Replace("\r\n", "<br/> ");
            return theString;
        }
        /// <summary>
        /// 恢复html中的特殊字符
        /// </summary>
        /// <param name="theString">需要恢复的文本。</param>
        /// <returns>恢复好的文本。</returns>
        public static string HtmlDecode(string theString)
        {
            theString = theString.Replace("&gt;", ">");
            theString = theString.Replace("&lt;", "<");
            theString = theString.Replace(" &nbsp;", "  ");
            theString = theString.Replace("&quot;", "\"");
            theString = theString.Replace("&#39;", "'");
            theString = theString.Replace("<br/> ", "\r\n");
            return theString;
        }

        /// <summary>
        /// 输出单行简介(theString一般是比较简短的含有html标记符号的内容，进行处理，单行输出)
        /// </summary>
        /// <param name="theString">目标字符串</param>
        /// <returns>处理后的字符串</returns>
        public static string SimpleLineSummary(string theString)
        {
            theString = theString.Replace("&gt;", "");
            theString = theString.Replace("&lt;", "");
            theString = theString.Replace(" &nbsp;", "  ");
            theString = theString.Replace("&quot;", "\"");
            theString = theString.Replace("&#39;", "'");
            theString = theString.Replace("<br/> ", "\r\n");
            theString = theString.Replace("\"", "");
            theString = theString.Replace("\t", " ");
            theString = theString.Replace("\r", " ");
            theString = theString.Replace("\n", " ");
            theString = Regex.Replace(theString, "\\s{2,}", " ");
            return theString;
        }

        #endregion

        #region HTML转js、js字符串方法

        /// <summary>
        /// 将html转成js代码,不完全和原始数据一致
        /// </summary>
        /// <param name="source">hmtl内容</param>
        /// <returns>js代码</returns>
        public static string Html2Js(string source)
        {
            return String.Format("document.write(\"{0}\");",
                String.Join("\");\r\ndocument.write(\"", source.Replace("\\", "\\\\")
                                        .Replace("/", "\\/")
                                        .Replace("'", "\\'")
                                        .Replace("\"", "\\\"")
                                        .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            ));
        }
        /// <summary>
        /// 将html转成可输出的js字符串,不完全和原始数据一致
        /// </summary>
        /// <param name="source">html内容</param>
        /// <returns>js字符串</returns>
        public static string Html2JsStr(string source)
        {
            return String.Format("{0}",
                String.Join(" ", source.Replace("\\", "\\\\")
                                        .Replace("/", "\\/")
                                        .Replace("'", "\\'")
                                        .Replace("\"", "\\\"")
                                        .Replace("\t", "")
                                        .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            ));
        }
        #endregion

        #region 格式化时间自定义处理
        /// <summary>        
        /// 格式化显示时间为几个月,几天前,几小时前,几分钟前,或几秒前；
        /// 此方法多用在发帖，回帖，最新评论，活动等有传达时效性的模块上
        /// </summary>
        /// <param name="dt">要格式化显示的时间</param>
        /// <returns>几个月,几天前,几小时前,几分钟前,或几秒前</returns>        
        public static string DateStringFromNow(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.TotalDays > 60) { return dt.ToShortDateString(); }
            else if (span.TotalDays > 30) { return "1个月前"; }
            else if (span.TotalDays > 14) { return "2周前"; }
            else if (span.TotalDays > 7) { return "1周前"; }
            else if (span.TotalDays > 1) { return string.Format("{0}天前", (int)Math.Floor(span.TotalDays)); }
            else if (span.TotalHours > 1) { return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours)); }
            else if (span.TotalMinutes > 1) { return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes)); }
            else if (span.TotalSeconds >= 1) { return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds)); }
            else { return "1秒前"; }
        }

        public static readonly string[] weekDay = { "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日" };
        #endregion

        #region UBB代码处理函数
        /// <summary> 
        /// UBB代码处理函数 
        /// UBB代码是HTML的一个变种，它常用在论坛中，作为替代HTML代码的安全代码。
        /// 详情参考百度百科：http://baike.baidu.com/view/168792.htm
        /// </summary> 
        /// <param name="content">输入字符串</param> 
        /// <returns>输出字符串</returns> 
        public static string UBB2HTML(string content)  //ubb转html
        {
            content = Regex.Replace(content, @"\[b\](.+?)\[/b\]", "<b>$1</b>", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\[i\](.+?)\[/i\]", "<i>$1</i>", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\[u\](.+?)\[/u\]", "<u>$1</u>", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\[p\](.+?)\[/p\]", "<p>$1</p>", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\[align=left\](.+?)\[/align\]", "<align='left'>$1</align>", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\[align=center\](.+?)\[/align\]", "<align='center'>$1</align>", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\[align=right\](.+?)\[/align\]", "<align='right'>$1</align>", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\[url=(?<url>.+?)]\[/url]", "<a href='${url}' target=_blank>${url}</a>", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\[url=(?<url>.+?)](?<name>.+?)\[/url]", "<a href='${url}' target=_blank>${name}</a>", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\[quote](?<text>.+?)\[/quote]", "<div class=\"quote\">${text}</div>", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\[img](?<img>.+?)\[/img]", "<a href='${img}' target=_blank><img src='${img}' alt=''/></a>", RegexOptions.IgnoreCase);
            return content;
        }

        public static string Comment_UBB(string content, string urlHead)
        {
            content = Regex.Replace(content, @"\[face(\d\d?|100)\]", "<img border=\"0\" style=\"width:19px; height:19px;\" src=\"" + urlHead + "face/$1.gif\" align=\"absmiddle\" />", RegexOptions.IgnoreCase);
            return content;
        }
        #endregion

        #region 由Object取值

        /// <summary>
        /// 取得Int值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetInt(object obj)
        {
            if (obj != null)
            {
                int i_ret;
                if (int.TryParse(obj.ToString(), out i_ret))
                {
                    return i_ret;
                }
                else
                {
                    return 0;
                }
            }
            else
                return 0;
        }

        /// <summary>
        /// 获得Long值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long GetLong(object obj)
        {
            if (obj != null)
            {
                long i_ret;
                if (long.TryParse(obj.ToString(), out i_ret))
                {
                    return i_ret;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 取得Decimal值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal GetDecimal(object obj)
        {
            if (obj != null)
            {
                decimal i_ret;
                if (decimal.TryParse(obj.ToString(), out i_ret))
                {
                    return i_ret;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 取得Guid值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid GetGuid(object obj)
        {
            if (obj != null)
            {
                return new Guid(obj.ToString());
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 取得DateTime值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(object obj)
        {
            if (obj != null)
            {
                DateTime i_ret;
                if (DateTime.TryParse(obj.ToString(), out i_ret))
                {
                    return i_ret;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 取得bool值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetBool(object obj)
        {
            if (obj != null && (obj.ToString() == "1" || obj.ToString().ToLower() == "true"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  取得byte[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Byte[] GetByte(object obj)
        {
            if (obj != null || obj.ToString() != "")
            {
                return (Byte[])obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取得string值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetString(object obj)
        {
            if (obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion
    }
}
