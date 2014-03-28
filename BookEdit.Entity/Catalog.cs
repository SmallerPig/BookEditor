using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*==========================================================
*作者：SmallerPig
*时间：2014/2/13 13:47:54
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
using System.Xml.Linq;

namespace BookEdit.Entity
{
    public class Catalog
    {
        public Catalog()
        {

        }

        public IList<Chapter> Chapters { get; set; }


        public delegate void OnDataChange(object sender);

        public event OnDataChange DataChange;

        public IList<BookEdit.Entity.Chapter> CreateMenu()
        {
            IList<BookEdit.Entity.Chapter> chapters = new List<Chapter>();

            XElement rootNode = XElement.Load(@"source\catalog.xml");

            IEnumerable<XElement> targetNodes = from target in rootNode.Descendants("chapter")
                                                select target;
            foreach (var Chapters in targetNodes)
            {
                BookEdit.Entity.Chapter chapter = new Chapter() { Nodes = new List<Node>() };
                var xAttribute = Chapters.Attribute("id");
                if (xAttribute != null) chapter.Id = xAttribute.Value;
                var attribute = Chapters.Attribute("title");
                if (attribute != null) chapter.Title = attribute.Value;

                IEnumerable<XElement> nodes = from a in Chapters.Descendants("node")
                                              select a;
                foreach (var node in nodes)
                {
                    BookEdit.Entity.Node n = new Node();
                    n.Id = node.Attribute("id").Value;
                    n.Src = node.Attribute("src").Value;
                    chapter.Nodes.Add(n);
                }
                chapters.Add(chapter);
            }

            return chapters;




            //return new List<Chapter>()
            //{
            //    new Chapter()
            //    {
            //        Id = 1,Title = "章节一",Nodes = new List<Node>()
            //        {
            //            new Node(){Id = 11,Src = "src111"},
            //            new Node(){Id = 12,Src = "src122"},
            //            new Node(){Id = 13,Src = "src132"},
            //        }
            //    },new Chapter()
            //    {
            //        Id = 2,Title = "章节二",Nodes = new List<Node>()
            //        {
            //            new Node(){Id = 21,Src = "src211"},
            //            new Node(){Id = 22,Src = "src222"},
            //            new Node(){Id = 23,Src = "src232"},
            //        }
            //    },new Chapter()
            //    {
            //        Id = 3,Title = "章节三",Nodes = new List<Node>()
            //        {
            //            new Node(){Id = 31,Src = "src311"},
            //            new Node(){Id = 32,Src = "src322"},
            //            new Node(){Id = 33,Src = "src332"},
            //        }
            //    },
            //};
        }
    }
}
