/*==========================================================
*作者：SmallerPig
*时间：2014/2/13 13:48:54
*版权所有:无锡睿阅数字科技有限公司
============================================================*/


using System;
using System.Collections.Generic;
using BookEdit.Entity.Interface;

namespace BookEdit.Entity
{
    public class Paragraph:IDeleteable<Paragraph>
    {
        public string Id { get; set; }

        public ParagraphType Type { get; set; }

        public double MarginTop { get; set; }

        public double MarginBottom { get; set; }

        public IList<TextNode> Texts { get; set; }


        public string Thumbnail { get; set; }

        public BaseMedia Media { get; set; }


        public void Delete(Paragraph t)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Gallery:BaseMedia
    {
        public string Src { get; set; }
        public override void SetType()
        {
            type = ParagraphType.Gallery;
        }

    }

    public class Video:BaseMedia
    {
        public string Src { get; set; }

        public override void SetType()
        {
            type = ParagraphType.Video;
        }
    }

    public class Audio:BaseMedia
    {
        public string Src { get; set; }

        public override void SetType()
        {
            type = ParagraphType.Audio;
        }

    }

    public class Link :BaseMedia
    {
        public string Href { get; set; }
        public override void SetType()
        {
            type = ParagraphType.Link;
        }

    }

    public class Html : BaseMedia
    {
        

        public string Src { get; set; }

        public override void SetType()
        {
            type = ParagraphType.Html;
        }
    }
    
    public abstract class BaseMedia
    {
        public string Thumbnail { get; set; }

        public string Brief { get; set; }

        protected ParagraphType type { get; set; }

        public abstract void SetType();
    }

    public enum ParagraphType
    {
        Text,
        Gallery,
        Video,
        Audio,
        Link,
        Html,
    }

}