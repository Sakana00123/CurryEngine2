using Editor.Components;
using Editor.GameProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Editor.Editors
{
    /// <summary>
    /// ProjectLayoutView.xaml の相互作用ロジック
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGameObject_Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var vm = button?.DataContext as Scene;
            vm?.AddGameObjectCommand.Execute(new GameObject(vm) { Name = "GameObject" });
        }

        private void OnGameObjects_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var entity = (sender as ListBox)?.SelectedItems[0];
            GameObjectView.Instance!.DataContext = entity;
        }
    }
}
