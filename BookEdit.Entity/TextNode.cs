using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*==========================================================
*作者：SmallerPig
*时间：2014/2/13 14:07:09
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
namespace BookEdit.Entity
{
    public class TextNode
    {
        public TextNode()
        {

        }

        public string FontFamily { get; set; }

        public double FontSize { get; set; }

        public string FontColor { get; set; }


        public string Content { get; set; }

    }
}
