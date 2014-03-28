using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using BookEdit.BLL;
using Microsoft.Win32;
using RY.Common;

namespace BookEdit.Editor
{
    /// <summary>
    /// AttachInfo.xaml 的交互逻辑
    /// </summary>
    public partial class AttachInfo : Window
    {
        private IUIClickable paragInfor;
        private MediaType mediaType;
        private string audioFileFilter = "音频文件(mp3 mav)|*.mp3;*.wav";
        private string videoFileFilter = "视频文件(mp4)|*.mp4";
        private string imgFileFilter = "图片文件(jpg bmp png)|*.jpg;*.bmp;*.png";
        private static string host = Properties.Settings.Default.HostAddress;


        private readonly Regex paragReg = new Regex(@"^(<p).*(</p>)$");
        private readonly Regex mediaRegex = new Regex(@"\[{1}.*?\]{1}");

        private Entity.AttachInfo selectAttachInfo;
        private IList<Entity.AttachInfo> itemsList;
        private ObservableCollection<Entity.AttachInfo> viewModel = new ObservableCollection<Entity.AttachInfo>();

        
        public AttachInfo()
        {
            InitializeComponent();
        }

        public AttachInfo(IUIClickable iClickable)
        {
            paragInfor = iClickable;
            InitializeComponent();
            itemsList = BLL.AttachInfo.GetListByPar(iClickable, host + "API/GetAttachInfoListByParag");
            if (itemsList == null)
            {
                itemsList = new List<Entity.AttachInfo>();
            }
            //Entity.AttachInfo attachInfo = new Entity.AttachInfo()
            //{
            //    Title = "",
            //    Centent = ""
            //};
            //itemsList.Add(attachInfo);
            foreach (var item in itemsList)
            {
                viewModel.Add(item);
            }
            //TabControlMain.ItemsSource = itemsList;
            this.TabControlMain.SetBinding(TabControl.ItemsSourceProperty,
                new Binding(".")
                {
                    Source = viewModel,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                });
        }

        private void btn_AddAudio_Click(object sender, RoutedEventArgs e)
        {
            mediaType = MediaType.Audio;
            OpenFileDialog openDialog = SetOpenFileDialog(audioFileFilter);
            string fileName = OpenSelect(openDialog);
            if (fileName == null)
                return;
            string url = UploadFile(fileName);
            string result = string.Format("[audio src=\"{0}\" brief=\"简介\"]", url);
            InsertContentToRichTextBox(result);
        }


        



        private void btn_AddVideo_Click(object sender, RoutedEventArgs e)
        {
            mediaType = MediaType.Video;
            OpenFileDialog openDialog = SetOpenFileDialog(videoFileFilter);
            string fileName = OpenSelect(openDialog);

            if (fileName == null)
                return;
            string url = UploadFile(fileName);
            string result = string.Format("[video src=\"{0}\" brief=\"简介\"]", url);
            InsertContentToRichTextBox(result);
        }

        private string UploadFile(string fileName)
        {
            string url="";
            switch (mediaType)
            {
                case MediaType.Audio:
                    url = host+"/Upload/AttachUploadAudio";
                    break;
                case MediaType.Img:
                    url = host+"/Upload/AttachUploadImg";
                    break;
                case MediaType.Video:
                    url = host+"/Upload/AttachUploadVideo";
                    break;
                default:
                    break;
            }
            System.Net.WebClient webClient = new WebClient();
            this.IsEnabled = false;
            byte[] b = webClient.UploadFile(url, "POST", fileName);
            string result = System.Text.Encoding.UTF8.GetString(b);
            this.IsEnabled = true;
            return result;
        }

        private string OpenSelect(OpenFileDialog openDialog)
        {
            var showDialog = openDialog.ShowDialog();
            if (showDialog != null && showDialog.Value)
            {
                return openDialog.FileName;
            }
            return null;
        }

        private void btn_AddImg_Click(object sender, RoutedEventArgs e)
        {
            mediaType = MediaType.Img;
            OpenFileDialog openDialog = SetOpenFileDialog(imgFileFilter);
            string fileName = OpenSelect(openDialog);
            if (fileName == null)
                return;
            string url = UploadFile(fileName);
            string result = string.Format("[image src=\"{0}\"]", url);
            InsertContentToRichTextBox(result);
        }

        private void InsertContentToRichTextBox(string result)
        {
            //if (!rtb_Content.CaretPosition.IsAtLineStartPosition && rtb_Content.CaretPosition != rtb_Content.CaretPosition.DocumentStart )
            //{
            //    rtb_Content.CaretPosition.InsertParagraphBreak();
            //}
            //rtb_Content.CaretPosition.InsertTextInRun(result);
            selectAttachInfo.Centent += result;
            //rtb_Content.CaretPosition.InsertParagraphBreak();
        }

        private void btn_AddLink_Click(object sender, RoutedEventArgs e)
        {
            mediaType = MediaType.Link;
            AddLink addLink = new AddLink();
            addLink.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (addLink.ShowDialog() == true)
            {
                string t = addLink.LinkText;
                string a = addLink.LinkAddress;
                string result = string.Format("[link href=\"{0}\" brief=\"{1}\"]", a, t);
                InsertContentToRichTextBox(result);
            }

        }

        OpenFileDialog SetOpenFileDialog(string filter)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = filter,
                RestoreDirectory = true
            };
            return openDialog;
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {


            selectAttachInfo = TabControlMain.SelectedContent as Entity.AttachInfo;
            if (string.IsNullOrEmpty(selectAttachInfo.Title) || string.IsNullOrEmpty(selectAttachInfo.Centent))
            {
                MessageBox.Show("请填写完整批注信息");
                return;
                
            }
                string content = selectAttachInfo.Centent;
                //mediaRegex.ma
                MatchCollection matchs = mediaRegex.Matches(content);
                string[] splitStrings = new string[matchs.Count];
                int index = 0;
                foreach (Match match in matchs)
                {
                    string tempString = match.Value.Replace('[', '<').Replace("]", "/>");
                    XElement xElement;
                    try
                    {
                        xElement = XElement.Parse(tempString);
                        splitStrings[index] = ParseMediaString(xElement);
                        content = content.Replace(match.Value, "\r" + splitStrings[index] + "\n");//替换成<p type=..></p>格式
                    }
                    catch (Exception)
                    {
                    }
                    index++;

                }
                char[] charArray = new[] { '\r', '\n' };
                string[] contentArray = content.Split(charArray, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var block in contentArray)
                {
                    stringBuilder.Append(!paragReg.IsMatch(block) ? string.Format("<p type=\"text\">{0}</p>", block) : block);
                }
            Entity.AttachInfo attachInfo;

            if (selectAttachInfo.Id==0)
            {
                attachInfo = new Entity.AttachInfo
                {
                    Id = selectAttachInfo.Id,
                    Title = HttpUtility.UrlPathEncode(selectAttachInfo.Title),
                    Centent = HttpUtility.UrlPathEncode(stringBuilder.ToString())
                };                
            }
            else
            {
                attachInfo = selectAttachInfo;
                attachInfo.Title = HttpUtility.UrlPathEncode(attachInfo.Title);
                attachInfo.Centent = HttpUtility.UrlPathEncode(stringBuilder.ToString());
            }
            string result = BLL.AttachInfo.PostData(attachInfo, paragInfor.BookId, paragInfor.NodeId, paragInfor.ParagId,
                 host + "API/AddAttachInfo");

            MessageBox.Show(result == "success" ? "Ok" : "Error");
            this.Close();
            //MessageBox.Show(TextBox_Content.Content);
        }

        private string ParseMediaString(XElement xElement)
        {
            string result = "";
            string src = "";
            switch (xElement.Name.LocalName)
            {
                case "image":
                    src = XmlHelper.GetNodeAttributeValue(xElement,"src");
                    result = string.Format("<p type=\"image\" src=\"{0}\" ></p>", src);
                    break;
                case "video":
                    src = XmlHelper.GetNodeAttributeValue(xElement,"src");
                    result = string.Format("<p type=\"video\" src=\"{0}\" brief=\"简介\"></p>", src);
                    break;
                case "audio":
                    src = XmlHelper.GetNodeAttributeValue(xElement, "src");
                    result = string.Format("<p type=\"audio\" src=\"{0}\" brief=\"简介\"></p>", src);
                    break;
                case "link":
                    string href = XmlHelper.GetNodeAttributeValue(xElement, "href");
                    string brief = XmlHelper.GetNodeAttributeValue(xElement, "brief");
                    result = string.Format("<p type=\"link\" href=\"{0}\" brief=\"{1}\" ></p>", href, brief);
                    break;
                default:
                    break;
            }
            return result;
        }

        private void btn_Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectAttachInfo = itemsList[TabControlMain.SelectedIndex];
        }





        class ViewMode : INotifyPropertyChanged
        {
            private IList<Entity.AttachInfo> itemsList;

            public IList<Entity.AttachInfo> ItemsList
            {
                get { return itemsList; }
                set
                {
                    if (value == itemsList) return;
                    NotifyPropertyChanged("Centent");
                    itemsList = value;
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

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            selectAttachInfo = TabControlMain.SelectedContent as Entity.AttachInfo;
            if (selectAttachInfo.Id==0)
                return;
            string result = BLL.AttachInfo.Delete(selectAttachInfo, paragInfor.BookId, paragInfor.NodeId, paragInfor.ParagId, host + "API/DeleteAttachInfo");
            MessageBox.Show(result == "success" ? "Ok" : "Error");
            this.Close();


        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            Entity.AttachInfo attachInfo = new Entity.AttachInfo()
            {
                Title = "",
                Centent = ""
            };
            viewModel.Add(attachInfo);
            itemsList.Add(attachInfo);
        }

    }
}
