using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// ProperyChangeTest.xaml 的交互逻辑
    /// </summary>
    public partial class ProperyChangeTest : Window
    {

        private Binding binding;
private ObservableCollection<Entity.AttachInfo> viewModel = new ObservableCollection<Entity.AttachInfo>();

public ProperyChangeTest()
{
    InitializeComponent();

    viewModel.Add(new Entity.AttachInfo()
    {
        Centent = "测试内容内容1",
        Title = "标题1"
    });
    viewModel.Add(new Entity.AttachInfo()
    {
        Centent = "测试内容内容2",
        Title = "标题2"
    });
    viewModel.Add(new Entity.AttachInfo()
    {
        Centent = "测试内容内容3",
        Title = "标题3"
    });

    this.TabControlMain.SetBinding(TabControl.ItemsSourceProperty,
        new Binding(".")
        {
            Source = viewModel,
            UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
            Mode = BindingMode.TwoWay
        });
}

private void Button_Click(object sender, RoutedEventArgs e)
{
    viewModel.First().Centent = "修改后！";
}
    }
}
