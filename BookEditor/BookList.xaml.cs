using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BookEdit.Entity;
using ICSharpCode.SharpZipLib.Zip;

namespace BookEdit.Editor
{
    /// <summary>
    /// BookList.xaml 的交互逻辑
    /// </summary>
    public partial class BookList : Window
    {
        private Entity.User user;
        private static string host = Properties.Settings.Default.HostAddress;

        private string getBookList = Properties.Settings.Default.GetBookList;
        private BackgroundWorker backgroundWorker;
        private IList<Entity.Book> booklist = new List<Book>();


        public BookList(Entity.User u)
        {
            user = u;
            InitializeComponent();
            backgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
            getBookList = host + getBookList + u.Id;
            backgroundWorker.RunWorkerAsync(new BookListData() { URL = getBookList });

        }

        public override void EndInit()
        {
            this.txtb_UserName.Text = user.UserName;
            base.EndInit();
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            Entity.Book book = bookList.SelectedItem as Entity.Book;
            if (book == null)
            {
                MessageBox.Show("还未选择任何书籍！");
                return;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(@"zips/" + book.Id);
            if (directoryInfo.Exists)
            {
                MainEditor me = new MainEditor(book,user);
                me.ShowDialog();
            }
            else
            {
                if (MessageBox.Show("Are you sure to download the book right now?",
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) !=
                    MessageBoxResult.Yes) return;
                this.block_Tips.Text = "准备下载.....请稍候！";
                btn_Edit.IsEnabled = false;
                directoryInfo.Create();
                string filsString = book.PackName;
                string fileName = filsString.Substring(filsString.LastIndexOf('/'), filsString.Length - filsString.LastIndexOf('/'));

                DownLoadData dld = new DownLoadData()
                {
                    HostAddressURL = host + "zips/" + book.PackName,
                    FileName = @"zips/" + book.Id + "/" + fileName,
                    BookId = book.Id
                };
                backgroundWorker.RunWorkerAsync(dld);

                //MessageBox.Show(DownloadFile(host + "zips/" + book.PackName, @"zips/" + book.Id + "/" + fileName)
                //    ? "OK!"
                //    : "network error!");
                return;
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BookListData url = e.Argument as BookListData;
            if (url != null)
            {
                string bookListString = BLL.User.GetBookList(url.URL);
                booklist = BLL.Book.XMLToBooks(bookListString);
                url.Result = "OK";
                e.Result = url;
                return;
            }
            DownLoadData dld = e.Argument as DownLoadData;
            if (dld != null && DownloadFile(dld.HostAddressURL, dld.FileName))
            {
                using (ZipFile zipFile = new ZipFile(dld.FileName))
                {
                    foreach (ZipEntry zipEntry in zipFile)
                    {
                        UnZip(zipEntry.Name, zipFile, dld);
                    }
                }
                using (ZipFile rbkif = new ZipFile(dld.FileName.Substring(0, dld.FileName.LastIndexOf('/')) + "/book.rbkif"))
                {
                    foreach (ZipEntry zipEntry in rbkif)
                    {
                        UnZip(zipEntry.Name, rbkif, dld);
                    }
                }


                dld.Result = "OK";
                e.Result = dld;
            }

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BookListData bld = e.Result as BookListData;
            if (bld != null)
            {
                this.bookList.SetBinding(FrameworkElement.DataContextProperty, new Binding(".") { Source = booklist });
                this.btn_Edit.IsEnabled = true;
                this.block_Tips.Text = "左边列出了您的书籍";
                return;
            }
            DownLoadData downLoadData = e.Result as DownLoadData;
            if (downLoadData != null)
            {
                this.block_Tips.Text = "下载完成！请点击按钮进行编辑操作";
                btn_Edit.IsEnabled = true;
                MessageBox.Show("OK!");
            }
        }



        private bool DownloadFile(string URL, string filename)
        {
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                System.IO.Stream st = myrp.GetResponseStream();
                long stll = myrp.ContentLength;
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    long sol = so.Length;
                    long stl = stll;
                    this.block_Tips.Dispatcher.Invoke(new SetTipsValue_dg(SetTipsValue), sol, stl);
                    so.Write(by, 0, osize);
                    osize = st.Read(by, 0, (int)by.Length);
                }
                so.Close();
                st.Close();
                myrp.Close();
                Myrq.Abort();
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        private delegate void SetTipsValue_dg(long solength, long stlength);
        private void SetTipsValue(long solength, long stlength)
        {
            string progress = ((float)solength/stlength).ToString("P");
            block_Tips.Text = "下载中...." + progress;
        }

        private bool UnZip(string entryname, ZipFile zf, DownLoadData downLoadData)
        {
            var zipEntry = zf.GetEntry(entryname);
            if (zipEntry == null)
            {
                return false;
            }
            string filepath = System.AppDomain.CurrentDomain.BaseDirectory + @"Zips\" + downLoadData.BookId + @"\";
            //string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + RY.Common.DirectoryAndFile.GetFileExt(entryname);

            string name = filepath + entryname;
            if (zipEntry.IsDirectory)
            {
                Directory.CreateDirectory(name);
                return false;
            }
            DirectoryInfo directoryInfo = new FileInfo(name).Directory;
            if (directoryInfo != null && !directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            using (FileStream writer = System.IO.File.Create(name)) //解压后的文件
            {
                var stream = zf.GetInputStream(zipEntry);
                int bufferSize = 1024 * 2; //缓冲区大小
                int readCount = 0; //读入缓冲区的实际字节
                byte[] buffer = new byte[bufferSize];
                readCount = stream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    writer.Write(buffer, 0, readCount);
                    readCount = stream.Read(buffer, 0, bufferSize);
                }
                writer.Close();
            }
            return true;
        }


    }

    class BookListData
    {
        public string URL { get; set; }

        public string Result { get; set; }
    }

    class DownLoadData
    {

        public int BookId { get; set; }

        public string HostAddressURL { get; set; }

        public string FileName { get; set; }

        public string Result { get; set; }

    }

}
