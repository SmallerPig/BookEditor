using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BookEdit.Entity;

namespace BookEdit.Editor
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private Entity.User user;
        private static string host = Properties.Settings.Default.HostAddress;
        private static string loginAddress = host + "API/Login";

        private BackgroundWorker backgroundWorker;

        private int loginresult = 0;




        public Login()
        {
            InitializeComponent();
            backgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
            user = new User();
        }

        private void btn_Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            user.UserName = txt_UserId.Text;
            user.Password = txt_Password.Password;
            if (!string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Password))
            {
                btn_Login.IsEnabled = false;
                this.txtb_Tips.Visibility = Visibility;
                backgroundWorker.RunWorkerAsync(user);
            }
            else
            {
                MessageBox.Show("some error!");
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Entity.User u = e.Argument as Entity.User;
            loginresult = BLL.User.Login(user.UserName, user.Password, loginAddress);
        }

        //private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{

        //}

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (loginresult != 0)
            {
                user.Id = loginresult;
                BookList bl = new BookList(user);
                bl.Show();
                this.Close();
            }
            else
            {
                btn_Login.IsEnabled = true;
                this.txtb_Tips.Text = "用户名或密码错误";
                this.txtb_Tips.Visibility = Visibility;
            }

        }
    }
}
