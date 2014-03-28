using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

/*==========================================================
*作者：SmallerPig
*时间：2014/3/12 14:55:47
*版权所有:无锡睿阅数字科技有限公司
============================================================*/

namespace BookEdit.Entity
{
    public class AttachInfo : INotifyPropertyChanged
    {

        public int Id { get; set; }

        public string PreTitle {
            get
            {
                return string.IsNullOrEmpty(Title) ? "+" : Title;
            }
        }


        public string Title { get; set; }

        private string centent;

        public string Centent {
            get { return centent; }
            set
            {
                if (value != centent) ;
                centent = value;
                NotifyPropertyChanged("Centent");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }
}
