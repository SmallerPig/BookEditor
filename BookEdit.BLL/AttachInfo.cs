using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using RY.Common;


/*==========================================================
*作者：SmallerPig
*时间：2014/3/12 14:53:13
*版权所有:无锡睿阅数字科技有限公司
============================================================*/

namespace BookEdit.BLL
{
    public class AttachInfo
    {


        public static string PostData(Entity.AttachInfo attachInfo,int bookId,string nodeId,string paragId, string address)
        {
            string postData ="id="+ attachInfo.Id + "&title=" + attachInfo.Title + "&content=" + attachInfo.Centent + "&bookId=" + bookId + "&nodeId=" + nodeId
                 + "&paragId=" + paragId;   

            HttpHelper httpHelper = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = address,//URL     必需项    
                //PostDataType = PostDataType.String,
                //Encoding = Encoding.UTF8,
                Method = "post",//URL     可选项 默认为Get   
                Postdata = postData,//Post数据     可选项GET时不需要写   
                ContentType = "application/x-www-form-urlencoded",
            };


            HttpResult result = httpHelper.GetHtml(item);
            string html = result.Html;

            return html;
        }


        public static string Delete(Entity.AttachInfo attachInfo, int bookId, string nodeId, string paragId, string address )
        {
            string postData = "id=" + attachInfo.Id+ "&bookId=" + bookId + "&nodeId=" + nodeId + "&paragId=" + paragId;

            HttpHelper httpHelper = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = address,//URL     必需项    
                //PostDataType = PostDataType.String,
                //Encoding = Encoding.UTF8,
                Method = "post",//URL     可选项 默认为Get   
                Postdata = postData,//Post数据     可选项GET时不需要写   
                ContentType = "application/x-www-form-urlencoded",
            };


            HttpResult result = httpHelper.GetHtml(item);
            string html = result.Html;
            return html;
        }






        public static IList<Entity.AttachInfo> GetListByPar(IUIClickable ui,string address)
        {

            IList<Entity.AttachInfo> result = new List<Entity.AttachInfo>();
            string url = address + "?bookId=" + ui.BookId + "&nodeId=" + ui.NodeId + "&paragId=" + ui.ParagId;
            HttpHelper httpHelper = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,//URL     必需项    
                //PostDataType = PostDataType.String,
                //Encoding = Encoding.UTF8,
                //Method = "get",//URL     可选项 默认为Get   
                //Postdata = postData,//Post数据     可选项GET时不需要写   
                //ContentType = "application/x-www-form-urlencoded",
            };
            HttpResult httpresult = httpHelper.GetHtml(item);
            if (httpresult.StatusCode == HttpStatusCode.OK)
            {
                string html = httpresult.Html;
                XElement xElement = XElement.Parse(html,LoadOptions.None);
                IEnumerable<XElement> xElements = RY.Common.XmlHelper.GetXElements(xElement, "attachedInfo");
                
                foreach (XElement element in xElements)
                {
                    string XMLCentent = RY.Common.XmlHelper.GetXElements(element, "Article").FirstOrDefault().Value;
                    string centent = ParseXMLToString(XMLCentent);
                    Entity.AttachInfo attachInfo =new Entity.AttachInfo();
                    attachInfo.Id =int.Parse(RY.Common.XmlHelper.GetXElements(element, "Id").FirstOrDefault().Value);
                    attachInfo.Title = RY.Common.XmlHelper.GetXElements(element, "Title").FirstOrDefault().Value;
                    attachInfo.Centent = centent;
                    result.Add(attachInfo);
                }
                return result;
            }
            return null;

        }

        private static string ParseXMLToString(string XMLCentent)
        {
            string result = "";
            XMLCentent = "<root>" + XMLCentent + "</root>";
            XElement xElement = XElement.Parse(XMLCentent, LoadOptions.None);
            IEnumerable<XElement> xElements = RY.Common.XmlHelper.GetXElements(xElement, "p");
            foreach (var element in xElements)
            {
                AttachInforParag attachInforParag = new AttachInforParag();
                string type = RY.Common.XmlHelper.GetNodeAttributeValue(element, "type");
                MediaType MyStatus = ParseEnum<MediaType>(type);
                attachInforParag.Type = MyStatus;
                switch (MyStatus)
                {
                    case MediaType.image:
                        attachInforParag.Src = RY.Common.XmlHelper.GetNodeAttributeValue(element, "src");
                        break;
                    case MediaType.video:
                    case MediaType.audio:
                        attachInforParag.Brief = RY.Common.XmlHelper.GetNodeAttributeValue(element, "brief");
                        attachInforParag.Src = RY.Common.XmlHelper.GetNodeAttributeValue(element, "src");
                        break;
                    case MediaType.link:
                        attachInforParag.Brief = RY.Common.XmlHelper.GetNodeAttributeValue(element, "brief");
                        attachInforParag.Link = RY.Common.XmlHelper.GetNodeAttributeValue(element, "link");
                        break;
                    case MediaType.text:
                        attachInforParag.Content = element.Value;
                        break;
                }
                string tempResult = attachInforParag.ToString();
                result += tempResult+"\r\n";
            }


            return result;
        }

        class AttachInforParag
        {
            public string Src { get; set; }
            public string Brief { get; set; }

            public MediaType Type { get; set; }

            public string Link { get; set; }

            public string Content { get; set; }

            public override string ToString()
            {
                string temp = "";
                switch (Type)
                {
                    case MediaType.image:
                        temp = string.Format("[{0} src=\"{1}\"]", Type, Src);
                        break;
                    case MediaType.video:
                    case MediaType.audio:
                        temp = string.Format("[{0} src=\"{1}\" brief=\"{2}\"]",Type, Src,Brief);
                        break;
                    case MediaType.link:
                        temp = string.Format("[{0} href=\"{1}\" brief=\"{2}\"]", Type, Link, Brief);
                        break;
                    case MediaType.text:
                        temp = Content;
                        break;
                    default:
                        break;
                }
                return temp;
            }
        }


        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        enum MediaType
        {
            audio,
            video,
            image,
            link,
            text
        }
    }
}
