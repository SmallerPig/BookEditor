using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

/*==========================================================
*作者：SmallerPig
*时间：2014/1/2 14:34:38
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
namespace RY.Common
{
    public class EntityToXML
    {

        public static string ToXML<T>(IList<T> TList, string ingor) where T : Listable
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.Encoding = new UTF8Encoding(false);
                xmlWriterSettings.NewLineChars = Environment.NewLine;
                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    xmlWriter.WriteStartDocument(true);
                    xmlWriter.WriteStartElement("Roots");
                    if (TList != null)
                    {
                        foreach (T t in TList)
                        {
                            Type type = typeof(T);
                            xmlWriter.WriteStartElement(type.Name);
                            xmlWriter.WriteStartAttribute("id");
                            xmlWriter.WriteString(t.Id.ToString());
                            xmlWriter.WriteEndAttribute();
                            foreach (PropertyInfo propertyInfo in type.GetProperties())
                            {
                                if (propertyInfo.CanRead && propertyInfo.Name.ToLower() != ingor.ToLower())
                                {
                                    if (propertyInfo.PropertyType == typeof(DateTime))
                                    {
                                        xmlWriter.WriteStartElement(propertyInfo.Name);
                                        DateTime dt = Convert.ToDateTime(propertyInfo.GetValue(t, null));
                                        xmlWriter.WriteCData(dt.ToString("yyyy-MM-dd hh:mm:ss"));
                                        xmlWriter.WriteEndElement();
                                    }
                                    if (propertyInfo.PropertyType == typeof(String) || propertyInfo.PropertyType == typeof(int))
                                    {
                                        object ob = propertyInfo.GetValue(t, null);
                                        if (null != ob)
                                        {
                                            xmlWriter.WriteStartElement(propertyInfo.Name);
                                            xmlWriter.WriteCData(ob.ToString());
                                            xmlWriter.WriteEndElement();
                                        }
                                    }
                                }
                            }
                            xmlWriter.WriteEndElement();
                        }
                    }
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                }
                string xml = Encoding.UTF8.GetString(memoryStream.ToArray());
                return xml;
            }
        }

        
    }

    public abstract class Listable
    {
        public int Id { get; set; }
    }



}
