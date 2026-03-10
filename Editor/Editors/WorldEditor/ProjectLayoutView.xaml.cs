using Editor.Components;
using Editor.GameProject;
using Editor.Utilities;
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
            var listBox = sender as ListBox;
            var newSelection = listBox?.SelectedItems.Cast<GameObject>().ToList();
            var previousSelection = newSelection!.Except(e.AddedItems.Cast<GameObject>()).Concat(e.RemovedItems.Cast<GameObject>()).ToList();

            Project.UndoRedo.Add(new UndoRedoAction(
                () => // Undo: 元の選択状態に戻す
                {
                    listBox!.UnselectAll();
                    previousSelection.ForEach(x => ((ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(x)).IsSelected = true);
                },
                () => // Redo: 新しい選択状態にする
                {
                    listBox!.UnselectAll();
                    newSelection!.ForEach(x => ((ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(x)).IsSelected = true);
                },
                "Selection changed"
            ));

            MSGameObject msObject = null!;
            if (newSelection.Count != 0)
            {
                msObject = new MSGameObject(newSelection);
            }
            GameObjectView.Instance.DataContext = msObject;
        }
    }
}
