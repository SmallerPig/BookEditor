using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookEdit.Entity
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string PackName { get; set; }


        string category;

        string tag;

        string cover;
        string publisher;
        string translator;
        string language;

        string summary;

        public string Category
        {
            get { return category; }
            set { category = value; }
        }
        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public string Cover
        {
            get { return cover; }
            set { cover = value; }
        }

        public string Publisher
        {
            get { return publisher; }
            set { publisher = value; }
        }
        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }









    }
}
