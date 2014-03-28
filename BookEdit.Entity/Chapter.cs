using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookEdit.Entity.Interface;

/*==========================================================
*作者：SmallerPig
*时间：2014/2/13 13:47:39
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
namespace BookEdit.Entity
{
    public class Chapter : IDeleteable<Chapter>, IUIDble
    {
        public Chapter()
        {
            Chapters = new List<Chapter>();
        }

        public string Id
        {
            get;
            set;
        }

        public string TitlePrifix { get; set; }

        public string Title { get; set; }

        public IEnumerable<IUIDble> ChildrenList { get; set; }

        public IList<Node> Nodes { get; set; }

        public IList<Chapter> Chapters { get; set; }

        public void Delete(Chapter chapter)
        {
            throw new NotImplementedException();
        }
    }
}
