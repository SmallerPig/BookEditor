using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
/*==========================================================
*作者：SmallerPig
*时间：2014/2/14 14:21:11
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
namespace BookEdit.BLL
{
    public class RtfToXML
    {
        private string Rtf;

        public RtfToXML(string rtf)
        {
            this.Rtf = rtf;
        }

        public static string Conver(string rtf)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                    {
                        Indent = true,
                        Encoding = new UTF8Encoding(false),
                        NewLineChars = Environment.NewLine
                    };
                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    xmlWriter.WriteStartDocument(true);
                    xmlWriter.WriteStartElement("Catalog");
                }
                string xml = Encoding.UTF8.GetString(memoryStream.ToArray());
                return xml;

            }
        }



    }
}
