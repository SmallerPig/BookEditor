using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Xml;
/*==========================================================
*作者：SmallerPig
*时间：2013/9/22 17:44:56
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
namespace RY.Common
{
    public class ZipEdit
    {
        public static void EditZip(string filename, int bookid)
        {
            string content;
            string editfilename = @"setting.xml";
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
            using (ZipFile zf = new ZipFile(fs))
            {
                var ze = zf.GetEntry(editfilename);
                if (ze == null)
                {
                }
                else
                {
                    string OutString="";
                    using (Stream s = zf.GetInputStream(ze))
                    {
                        StreamReader inputReader = new StreamReader(s);
                        content = inputReader.ReadToEnd();
                        try
                        {
                            XmlDocument mDoc = new XmlDocument();
                            mDoc.LoadXml(content);
                            XmlNode metaNode = mDoc.SelectSingleNode("//meta");
                            XmlNode idNode = metaNode.SelectSingleNode("id");
                            idNode.InnerText = bookid.ToString();
                            OutString = mDoc.InnerXml;
                        }
                        catch
                        { }
                    }
                    zf.BeginUpdate();
                    zf.Add(new StateDataSource(OutString), editfilename, CompressionMethod.Stored,true);
                    zf.CommitUpdate();
                }
            }
        }



        class StateDataSource : IStaticDataSource
        {
            string source;
            Stream stream;

            public StateDataSource(string source)
            {
                this.source = source;
            }

            public StateDataSource(Stream stream)
            {
                this.stream = stream;
            }

            public Stream GetSource()
            {
                if (source != null && string.Empty != source)
                {
                    byte[] array = Encoding.UTF8.GetBytes(source);
                    MemoryStream returnstream = new MemoryStream(array);
                    return returnstream;
                }
                else
                    return stream;
            }

        }
    }

}
