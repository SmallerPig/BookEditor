using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookEdit.Editor
{
    /// <summary>
    /// AddLink.xaml 的交互逻辑
    /// </summary>
    public partial class AddLink : Window
    {
        public string LinkAddress { get; set; }
        public string LinkText { get; set; }


        public AddLink()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LinkAddress = txt_LinkAddress.Text;
            LinkText = txt_LinkText.Text;
            DialogResult = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
