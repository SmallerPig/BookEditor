using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*==========================================================
*作者：SmallerPig
*时间：2014/3/4 15:09:07
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
using System.Windows.Controls;
using System.Windows.Documents;
using BookEdit.BLL;

namespace BookEdit.Editor
{
    internal class MyBlockUIContainer : BlockUIContainer, IUIClickable
    {
        private Image img;

        public MyBlockUIContainer(int bookId, string paragId,string nodeId, Image img)
            : base(img)
        {
            this.img = img;
            BookId = bookId;
            this.ParagId = paragId;
            NodeId = nodeId;
        }

        public string ParagId { get; set; }




        public int BookId { get; set; }


        public string NodeId
        {
            get;
            set;
        }
    }
}
