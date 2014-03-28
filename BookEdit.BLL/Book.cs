using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*==========================================================
*作者：SmallerPig
*时间：2014/2/27 15:58:36
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
using System.Xml.Linq;

namespace BookEdit.BLL
{
    public class Book
    {
        public static IList<Entity.Book> XMLToBooks(string xml)
        {
            IList<Entity.Book> result = new List<Entity.Book>();
            XElement rootNode = XElement.Parse(xml);
            IEnumerable<XElement> targetNodes = from target in rootNode.Descendants("Book")
                                                select target;
            foreach (var b in targetNodes)
            {
                Entity.Book book = new Entity.Book();
                book.Id =int.Parse(b.Attribute("id").Value);
                book.Title = b.Element("Title").Value;
                book.Author = b.Element("Author").Value;
                book.Category = b.Element("Category").Value;
                book.Tag = b.Element("Tag").Value;
                book.Cover = b.Element("Cover").Value;
                book.Publisher = b.Element("Publisher").Value;
                book.Summary = b.Element("Summary").Value;
                book.Language = b.Element("Language").Value;
                book.PackName = b.Element("Pack").Value;
                result.Add(book);
            }
            return result;
        }
    }
}
