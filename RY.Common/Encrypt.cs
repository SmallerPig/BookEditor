using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Configuration;


namespace RY.Common
{
    /// <summary>
    /// ͨ�ü���/�����ࡣ
    /// </summary>
    public class Encrypt
    {
        #region ====�ԳƼ���(����)=====
        //SymmetricAlgorithm ���жԳ��㷨��ʵ�ֶ�������м̳еĳ������
        private SymmetricAlgorithm mobjCryptoService;
        private string key;
        public Encrypt()
        {
            mobjCryptoService = new RijndaelManaged();
            key = "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";//�Զ�����ܴ�
        }
        /// <summary>
        /// �����Կ
        /// </summary>
        /// <returns>��Կ</returns>
        private byte[] GetLegalKey()
        {
            string sTemp = key;
            mobjCryptoService.GenerateKey();// ��������������дʱ���������ڸ��㷨�������Կ
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>
        /// ��ó�ʼ����IV
        /// </summary>
        /// <returns>��������IV</returns>
        private byte[] GetLegalIV()
        {
            string sTemp = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;// ��ȡ�����öԳ��㷨�ĳ�ʼ������
            int IVLength = bytTemp.Length;//���һ�� 32 λ��������ʾ System.Array ������ά����Ԫ�ص�����
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>
        /// ���ܷ���(����ʵ����Encrypt��)
        /// </summary>
        /// <param name="Source">�����ܵĴ�</param>
        /// <returns>�������ܵĴ�</returns>
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
        /// ���ܷ���(����ʵ����Encrypt��)
        /// </summary>
        /// <param name="Source">�����ܵĴ�</param>
        /// <returns>�������ܵĴ�</returns>
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

        #region ========DES����========

        /// <summary>
        /// DES����
        /// </summary>
        /// <param name="Text">�����ܵ��ַ���</param>
        /// <returns>���ܺ���ַ���</returns>
        public static string DESEncrypt(string Text)
        {
            return DESEncrypt(Text, "loveyajuan");
        }

        /// <summary> 
        /// DES�������� 
        /// </summary> 
        /// <param name="Text">�����ܵ��ַ���</param> 
        /// <param name="sKey">������Կ</param> 
        /// <returns>���ܺ���ַ���</returns> 
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

        #region ========DES����========

        /// <summary>
        /// DES����
        /// </summary>
        /// <param name="Text">�����ܵ��ַ���</param>
        /// <returns>���ܺ������</returns>
        public static string DESDecrypt(string Text)
        {
            return DESDecrypt(Text, "loveyajuan");
        }

        /// <summary> 
        /// DES�������� 
        /// </summary> 
        /// <param name="Text">�����ܵ��ַ���</param> 
        /// <param name="sKey">������Կ</param> 
        /// <returns>���ܺ������</returns> 
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

        #region ========MD5����========
        /// <summary>
        /// MD5����
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

        #region ====�����1���ܽ���====
        /// <summary>
        /// �����1����
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static string EncryptOrderStr(string rs) //�����1���� 
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
        /// ˳���1���� 
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static string DecryptOrderStr(string rs) //˳���1���� 
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

        #region =====Escape���ܽ���====
        /*statement������ȫ���ַ������������*/

        /// <summary>
        /// Escape����
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
        /// UnEscape����
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

        #region ======Base64�����=====

        /// <summary>
        /// Base64����
        /// </summary>
        /// <param name="code_type">��������</param>
        /// <param name="code">��������ַ���</param>
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
        /// Base64����
        /// </summary>
        /// <param name="code_type">��������</param>
        /// <param name="code">��������ַ���</param>
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