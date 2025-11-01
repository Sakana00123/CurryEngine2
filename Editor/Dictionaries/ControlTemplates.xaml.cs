using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Editor.Dictionaries
{
    public partial class ControlTemplates : ResourceDictionary
    {
        private void OnTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO: ここ通ってないので動作してないのであとで確認する。ResourceDictionaryが機能してない。
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
