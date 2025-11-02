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
    /// GameObjectView.xaml の相互作用ロジック
    /// </summary>
    public partial class GameEntityView : UserControl
    {
        private Action _undoAction;
        private string _propertyName;
        public static GameEntityView? Instance { get; private set; }
        public GameEntityView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;

            // DataContext の変更を監視して、プロパティ変更をキャッチする
            DataContextChanged += (_, __) =>
            {
                if (DataContext != null)
                {
                    (DataContext as MSEntity).PropertyChanged += (s, e) => _propertyName = e.PropertyName;
                }
            };
        }

        /// <summary>
        /// IsActive プロパティ変更用の UndoRedo アクションを取得します。
        /// </summary>
        /// <returns> UndoRedo 用のアクション </returns>
        private Action GetRenameAction()
        {
            var vm = DataContext as MSEntity;
            var selection = vm?.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
            return new Action(() =>
            {
                selection?.ForEach(item => item.entity.Name = item.Name);
                (DataContext as MSEntity)?.Refresh();
            });
        }

        /// <summary>
        /// IsActive プロパティ変更用の UndoRedo アクションを取得します。
        /// </summary>
        /// <returns> UndoRedo 用のアクション </returns>
        private Action GetIsEnabledAction()
        {
            var vm = DataContext as MSEntity;
            var selection = vm?.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
            return new Action(() =>
            {
                selection?.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
                (DataContext as MSEntity)?.Refresh();
            });
        }

        /// <summary>
        /// 名前変更のフォーカスが当たったときに UndoRedo 用のアクションを準備します。
        /// </summary>
        private void OnName_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _undoAction = GetRenameAction();
        }

        /// <summary>
        /// 名前変更のフォーカスが外れたときに UndoRedo に登録します。
        /// </summary>
        private void OnName_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // 名前が変更された場合のみ UndoRedo に登録
            if (_propertyName == nameof(MSEntity.Name) && _undoAction != null)
            {
                var redoAction = GetRenameAction();
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename GameEntity"));
                _propertyName = null;
            }
            _undoAction = null;
        }

        /// <summary>
        /// IsActive チェックボックスがクリックされたときの処理
        /// </summary>
        private void OnIsEnabled_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var undoAction = GetIsEnabledAction();
            var vm = DataContext as MSEntity;
            if (vm == null) return;
            vm.IsEnabled = ((CheckBox)sender).IsChecked == true;
            var redoAction = GetIsEnabledAction();
            Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction, 
                vm.IsEnabled == true ? "IsEnabled Game Entity" : "Disabled Game Entity"));
        }

        /// <summary>
        /// TextBox でキーが押されたときの処理(参考動画ではここじゃないけどいくら試しても動かなかったから動かすためにここに書いてる)
        /// </summary>
        private void OnTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            Debug.Assert(textBox != null);
            var expression = textBox.GetBindingExpression(TextBox.TextProperty);
            if (expression == null) return;

            if (e.Key == Key.Enter) // Enterキーが押された場合
            {
                // コマンドが設定されていれば実行する
                if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
                {
                    command.Execute(textBox.Text);
                }
                else // コマンドが設定されていなければ変更を確定させる
                {
                    // 変更を確定させる
                    expression.UpdateSource();
                }
                // フォーカスを外して変更を確定させる
                Keyboard.ClearFocus();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape) // Escapeキーが押された場合
            {
                // 変更をキャンセルする
                expression.UpdateTarget();
                // フォーカスを外す
                var scope = FocusManager.GetFocusScope(textBox);
                FocusManager.SetFocusedElement(scope, null as IInputElement);
                Keyboard.ClearFocus();
            }
        }
    }
}
