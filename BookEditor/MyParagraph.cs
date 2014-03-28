using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*==========================================================
*作者：SmallerPig
*时间：2014/3/4 15:08:16
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
using System.Windows.Documents;
using BookEdit.BLL;

namespace BookEdit.Editor
{
    internal class MyParagraph : Paragraph, IUIClickable
    {
        public int BookId { get; set; }

        public string ParagId { get; set; }


        public MyParagraph(int bookId,string paragId,string nodeId)
        {
            BookId = bookId;
            ParagId = paragId;
            NodeId = nodeId;
        }



        public string NodeId
        {
            get; set;
        }
    }
}
