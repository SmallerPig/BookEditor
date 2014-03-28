using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookEdit.Entity.Interface;

/*==========================================================
*作者：SmallerPig
*时间：2014/2/13 13:46:33
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
namespace BookEdit.Entity
{
    public class Node : IDeleteable<Node>, IUIDble
    {
        public Node()
        {
            int a = 1;
        }

        public string Id { get; set; }

        public Chapter Parent { set; get; }

        public int Index { get; set; }

        public string TitlePrifix
        {
            get { return Parent.TitlePrifix+"." + (Index+1); }
        }

        public string Src { get; set; }

        public IList<Paragraph> PList { get; set; }

        public void Delete(Node t)
        {
            throw new NotImplementedException();
        }
    }
}
