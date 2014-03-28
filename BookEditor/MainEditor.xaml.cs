using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using BookEdit.BLL;
using BookEdit.Entity;
using BookEdit.Entity.Interface;
using ICSharpCode.SharpZipLib.Zip;
using Book = BookEdit.Entity.Book;
using Chapter = BookEdit.Entity.Chapter;
using Node = BookEdit.Entity.Node;
using Paragraph = System.Windows.Documents.Paragraph;
using User = BookEdit.Entity.User;

namespace BookEdit.Editor
{

    public partial class MainEditor : Window
    {

        //private string connectionString = Properties.Setting
        public FlowDocument flowDocument;
        private IList<BookEdit.Entity.Interface.IUIDble> chapters;
        private string catalogFileName;
        private Book book;
        private User user;


        public MainEditor()
        {
            var b = new Entity.Book() {Id = 15};
            var u = new User() {Id = 1};
            MainEditor m = new MainEditor(b,u);
            m.Show();
            this.Hide();
        }

        public MainEditor(Entity.Book b,Entity.User u)
        {
            book = b;
            user = u;
            InitializeComponent();
            catalogFileName = @"Zips/" + book.Id + @"/catalog.xml";

            chapters = new List<IUIDble>();
            try
            {
                XElement rootNode = XElement.Load(catalogFileName);

                IEnumerable<XElement> targetNodes = RY.Common.XmlHelper.GetXElements(rootNode, "chapter");
                foreach (var chaptersNodes in targetNodes)
                {
                    XElement parentElement = chaptersNodes.Parent;
                    if (parentElement.Name=="chapter")
                    {
                        continue;
                    }
                    Chapter chapter = new Chapter
                    {
                        Nodes = new List<Node>(),
                        ChildrenList = new List<IUIDble>(),
                        Id = RY.Common.XmlHelper.GetNodeAttributeValue(chaptersNodes, "uid"),
                        TitlePrifix = RY.Common.XmlHelper.GetNodeAttributeValue(chaptersNodes, "titlePrifix"),
                        Title = RY.Common.XmlHelper.GetNodeAttributeValue(chaptersNodes, "title")
                    };
                    if (parentElement.Name == "catalog")
                    {
                        chapter.ChildrenList = (ParseXMLToEntity(chaptersNodes,chapter));

                        #region 下面为垃圾代码，用上面方法看起来更牛逼呢。嘿嘿~ 

                        //IEnumerable<XElement> nodes = RY.Common.XmlHelper.GetXElements(chaptersNodes, "node");
                        //IEnumerable<XElement> childrenChapters = RY.Common.XmlHelper.GetXElements(chaptersNodes, "chapter");
                        //foreach (var node in nodes)
                        //{
                        //    XElement nodeParentElement = node.Parent;
                        //    string parentId = RY.Common.XmlHelper.GetNodeAttributeValue(nodeParentElement, "uid");
                        //    if (parentId == chapter.Id)
                        //    {
                        //        Entity.Node n = new Node
                        //        {
                        //            Id = RY.Common.XmlHelper.GetNodeAttributeValue(node, "uid"),
                        //            Src = RY.Common.XmlHelper.GetNodeAttributeValue(node, "src")
                        //        };
                        //        chapter.ChildrenList.Add(n);
                        //    }
                        //}
                        //foreach (var childrenChapter in childrenChapters)
                        //{
                        //    Chapter c = new Chapter()
                        //    {
                        //        Nodes = new List<Node>(),
                        //        ChildrenList = new List<IUIDble>(),
                        //        Id = RY.Common.XmlHelper.GetNodeAttributeValue(childrenChapter, "uid"),
                        //        TitlePrifix = RY.Common.XmlHelper.GetNodeAttributeValue(childrenChapter, "titlePrifix"),
                        //        Title = RY.Common.XmlHelper.GetNodeAttributeValue(childrenChapter, "title")
                        //    };
                        //    IEnumerable<XElement> ns = RY.Common.XmlHelper.GetXElements(childrenChapter, "node");
                        //    foreach (Node n in ns.Select(node => new Node
                        //    {
                        //        Id = RY.Common.XmlHelper.GetNodeAttributeValue(node, "uid"),
                        //        Src = RY.Common.XmlHelper.GetNodeAttributeValue(node, "src")
                        //    }))
                        //    {

                        //        c.ChildrenList.Add(n);
                        //    }
                        //    chapter.ChildrenList.Add(c);
                        //}
                        #endregion
                    }
                    chapters.Add(chapter);
                }
                this.MenuTreeView.SetBinding(FrameworkElement.DataContextProperty, new Binding(".") {Source = chapters});
            }
            catch
            {
                MessageBox.Show("数据损坏!请重新下载该书籍");
                DirectoryInfo di = new DirectoryInfo(@"Zips/" + book.Id);
                di.Delete(true);
                //DialogResult = false;
            }
        }

        private Chapter ParserXEmelent(XElement chaptersNodes)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<IUIDble> ParseXMLToEntity(XElement xElement, Chapter parent)
        {
            foreach (var element in xElement.Descendants())
            {
                if (element.Name == "chapter" && element.Parent == xElement)
                {
                    yield return ParseXMLToChapter(element);
                }
                else if (element.Name == "node" && element.Parent==xElement)
                {
                    yield return ParseXMLToNode(element,ref parent);
                }
            }
        }

        private IUIDble ParseXMLToChapter(XElement xElement)
        {
            Chapter chapter = new Chapter()
            {
                ChildrenList = new List<IUIDble>(),
                Id = RY.Common.XmlHelper.GetNodeAttributeValue(xElement, "uid"),
                Nodes = new List<Node>(),
                TitlePrifix = RY.Common.XmlHelper.GetNodeAttributeValue(xElement, "titlePrifix"),
                Title = RY.Common.XmlHelper.GetNodeAttributeValue(xElement, "title")
            };
            chapter.ChildrenList = (ParseXMLToEntity(xElement,chapter));
            return chapter;
        }

        private IUIDble ParseXMLToNode(XElement xElement,ref Chapter parent)
        {
            Node node = new Node()
            {
                Id = RY.Common.XmlHelper.GetNodeAttributeValue(xElement, "uid"),
                Src = RY.Common.XmlHelper.GetNodeAttributeValue(xElement, "src"),
                Parent = parent,
                Index = parent.Nodes.Count
            };
            parent.Nodes.Add(node);
            return node;
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Node node = new Node() {PList = new List<Entity.Paragraph>()};
            BookEdit.Entity.Paragraph p = null;
            TextNode tn = null;
            BaseMedia bm = null;


            XElement rootNode = XElement.Load(catalogFileName);
            IEnumerable<XElement> plist = from target in rootNode.Descendants("p")
                                                select target;
            foreach (var par in plist)
            {
                p = new Entity.Paragraph(){ Texts = new List<TextNode>()};
                string type = par.Attribute("type").Value;
                switch (type)
                {
                    case "text":
                        p.Type = ParagraphType.Text;
                        IEnumerable<XElement> textlist = from n in par.Descendants("text")
                            select n;
                        foreach (var textNode in textlist)
                        {
                            tn = new TextNode();
                            tn.Content =System.Web.HttpUtility.UrlDecode(textNode.Value);
                            p.Texts.Add(tn);
                        }
                        break;
                    case "gallery":
                        p.Type = ParagraphType.Gallery;
                        p.Thumbnail = RY.Common.XmlHelper.GetNodeAttributeValue(par, "thumbnail");// par.Attribute("thumbnail").Value;
                        break;
                }
                node.PList.Add(p);
            }
            SetRichBoxContent(node);
        }

        private void SetRichBoxContent(Node node)
        {

            FlowDocument fd = new FlowDocument();

            foreach (var par in node.PList)
            {
                MyParagraph gar = new MyParagraph(book.Id,par.Id,node.Id);

                gar.MouseEnter += ContentElement_OnMouseEnter;
                gar.MouseLeave += ContentElement_OnMouseLeave;
                gar.MouseLeftButtonDown += Paragraph_MouseLeftButtonDown;
                Image img;
                BitmapImage bImg;
                MyBlockUIContainer blockUiContainer;
                switch (par.Type)
                {
                    case ParagraphType.Text:
                        gar.TextIndent = 2;
                        foreach (var aar in par.Texts)
                        {
                            Run aaar = new Run { Text = aar.Content };
                            gar.Inlines.Add(aaar);
                        }
                        fd.Blocks.Add(gar);
                        break;
                    default:
                        img = new Image() { StretchDirection = StretchDirection.DownOnly };
                        bImg = new BitmapImage();
                        img.IsEnabled = true;
                        bImg.BeginInit();
                        bImg.UriSource = new Uri("Zips/" + book.Id + "/" + par.Thumbnail, UriKind.Relative);
                        bImg.EndInit();
                        img.Source = bImg;
                        blockUiContainer = new MyBlockUIContainer(book.Id,par.Id,node.Id,img);
                        blockUiContainer.MouseEnter += ContentElement_OnMouseEnter;
                        blockUiContainer.MouseLeave += ContentElement_OnMouseLeave;
                        blockUiContainer.MouseLeftButtonDown += Paragraph_MouseLeftButtonDown;
                        fd.Blocks.Add(blockUiContainer);
                        break;
                }
            }
            this.RichTextBox.Document = fd;

        }


        private void ContentElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Paragraph p = sender as Paragraph;
            if (p != null)
            {
                //p.Padding = new Thickness(2);
                p.Background = System.Windows.Media.Brushes.Aqua;
            }
        }

        private void ContentElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Paragraph p = sender as Paragraph;
            if (p != null)
            {
                //p.Padding = new Thickness(0);
                p.Background = null;

            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = MenuTreeView.SelectedItem as Entity.Node;
            if (item!=null)
            {
                LodingNode(item);
            }
        }

        private void LodingNode(Node parent)
        {
            Node node = parent;
            node.PList = new List<Entity.Paragraph>();
            BaseMedia bm = null;


            XElement rootNode = XElement.Load(@"zips/"+book.Id+"/"+node.Src);
            IEnumerable<XElement> plist = from target in rootNode.Descendants("p")
                                          select target;
            foreach (var par in plist)
            {
                BookEdit.Entity.Paragraph p = new Entity.Paragraph() { Texts = new List<TextNode>() };
                p.Id = RY.Common.XmlHelper.GetNodeAttributeValue(par, "uid");
                var xAttribute = par.Attribute("type");
                if (xAttribute != null)
                {
                    string type = xAttribute.Value;
                    switch (type)
                    {
                        case "text":
                            p.Type = ParagraphType.Text;
                            IEnumerable<XElement> textlist = from n in par.Descendants("text")
                                select n;
                            foreach (var textNode in textlist)
                            {
                                TextNode tn = new TextNode();
                                tn.Content = System.Web.HttpUtility.UrlDecode(textNode.Value);
                                p.Texts.Add(tn);
                            }
                            break;
                        //case "gallery":
                        //    p.Type = ParagraphType.Gallery;
                        //    p.Thumbnail = RY.Common.XmlHelper.GetNodeAttributeValue(par, "thumbnail");
                        //    break;
                        default:
                            p.Type = ParagraphType.Gallery;
                            p.Thumbnail = RY.Common.XmlHelper.GetNodeAttributeValue(par, "thumbnail");
                            break;

                    }
                }
                else
                {
                    p.Type = ParagraphType.Text;
                    IEnumerable<XElement> textlist = from n in par.Descendants("text")
                                                     select n;
                    foreach (var textNode in textlist)
                    {
                        TextNode tn = new TextNode();
                        tn.Content = System.Web.HttpUtility.UrlDecode(textNode.Value);
                        p.Texts.Add(tn);
                    }
                }
                node.PList.Add(p);
            }
            SetRichBoxContent(node);
        }

        private void Paragraph_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IUIClickable p = sender as IUIClickable;
            if (p!=null)
            {
                AttachInfo attachInfo = new AttachInfo(p);
                attachInfo.ShowDialog();
            }
        }

 
    }
}