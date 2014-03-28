using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Configuration;


namespace RY.Common
{
    /// <summary>
    /// 通用加密/解密类。
    /// </summary>
    public class Encrypt
    {
        #region ====对称加密(向量)=====
        //SymmetricAlgorithm 所有对称算法的实现都必须从中继承的抽象基类
        private SymmetricAlgorithm mobjCryptoService;
        private string key;
        public Encrypt()
        {
            mobjCryptoService = new RijndaelManaged();
            key = "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";//自定义的密串
        }
        /// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey()
        {
            string sTemp = key;
            mobjCryptoService.GenerateKey();// 当在派生类中重写时，生成用于该算法的随机密钥
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private byte[] GetLegalIV()
        {
            string sTemp = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;// 获取或设置对称算法的初始化向量
            int IVLength = bytTemp.Length;//获得一个 32 位整数，表示 System.Array 的所有维数中元素的总数
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>
        /// 加密方法(请先实例化Encrypt类)
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        public string EncrypStrByIV(string Source)
        {
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
            MemoryStream ms = new MemoryStream();
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }
        /// <summary>
        /// 解密方法(请先实例化Encrypt类)
        /// </summary>
        /// <param name="Source">待解密的串</param>
        /// <returns>经过解密的串</returns>
        public string DecrypStrByIV(string Source)
        {
            byte[] bytIn = Convert.FromBase64String(Source);
            MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
        #endregion

        #region ========DES加密========

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="Text">待加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string DESEncrypt(string Text)
        {
            return DESEncrypt(Text, "loveyajuan");
        }

        /// <summary> 
        /// DES加密数据 
        /// </summary> 
        /// <param name="Text">待加密的字符串</param> 
        /// <param name="sKey">加密密钥</param> 
        /// <returns>加密后的字符串</returns> 
        public static string DESEncrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ========DES解密========

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="Text">待解密的字符串</param>
        /// <returns>解密后的明文</returns>
        public static string DESDecrypt(string Text)
        {
            return DESDecrypt(Text, "loveyajuan");
        }

        /// <summary> 
        /// DES解密数据 
        /// </summary> 
        /// <param name="Text">待解密的字符串</param> 
        /// <param name="sKey">解密密钥</param> 
        /// <returns>解密后的明文</returns> 
        public static string DESDecrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion

        #region ========MD5加密========
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="paramstr"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string paramstr)
        {
            string privateKey = "loveyajuan";
            string tempStr = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(privateKey, "MD5");

            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(paramstr + tempStr, "MD5").ToLower();
        }
        #endregion

        #region ====倒序加1加密解密====
        /// <summary>
        /// 倒序加1加密
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static string EncryptOrderStr(string rs) //倒序加1加密 
        {
            byte[] by = new byte[rs.Length];
            for (int i = 0; i <= rs.Length - 1; i++)
            {
                by[i] = (byte)((byte)rs[i] + 1);
            }
            rs = "";
            for (int i = by.Length - 1; i >= 0; i--)
            {
                rs += ((char)by[i]).ToString();
            }
            return rs;
        }
        /// <summary>
        /// 顺序减1解码 
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static string DecryptOrderStr(string rs) //顺序减1解码 
        {
            byte[] by = new byte[rs.Length];
            for (int i = 0; i <= rs.Length - 1; i++)
            {
                by[i] = (byte)((byte)rs[i] - 1);
            }
            rs = "";
            for (int i = by.Length - 1; i >= 0; i--)
            {
                rs += ((char)by[i]).ToString();
            }
            return rs;
        }
        #endregion

        #region =====Escape加密解密====
        /*statement：处理全角字符还是有问题的*/

        /// <summary>
        /// Escape加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Escape(string str)
        {
            if (str == null)
            {
                return String.Empty;
            }
            StringBuilder sb = new StringBuilder();
            int len = str.Length;
            for (int i = 0; i < len; i++)
            {
                char c = str[i];
                if (Char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '/' || c == '\\' || c == '.')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(Uri.HexEscape(c));
                }
            }

            return sb.ToString();
        }
        /// <summary>
        /// UnEscape解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnEscape(string str)
        {
            if (str == null)
            {
                return String.Empty;
            }
            StringBuilder sb = new StringBuilder();
            int len = str.Length;
            int i = 0;
            while (i != len)
            {
                if (Uri.IsHexEncoding(str, i))
                {
                    sb.Append(Uri.HexUnescape(str, ref i));
                }
                else
                {
                    sb.Append(str[i++]);
                }
            }
            return sb.ToString();
        }
        #endregion

        #region ======Base64编解码=====

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="code_type">编码类型</param>
        /// <param name="code">待编码的字符串</param>
        /// <returns></returns>
        public static string Base64Encode(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="code_type">编码类型</param>
        /// <param name="code">带解码的字符串</param>
        /// <returns></returns>
        public static string Base64Decode(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
        #endregion

    }
}