using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Editor.GameProject
{
    /// <summary>
    /// ProjectBrowserDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        public ProjectBrowserDialog()
        {
            InitializeComponent();
            Loaded += OnProjectBrowserDialogLoaded;
        }

        private void OnProjectBrowserDialogLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnProjectBrowserDialogLoaded;
            if (!OpenProject.Projects.Any())
            {
                OpenProjectButton.IsEnabled = false;
                OpenProjectView.Visibility = Visibility.Hidden;
                OnToggleButtonClick(CreateProjectButton, new RoutedEventArgs());
            }
        }

        private void OnToggleButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender == OpenProjectButton)
            {
                if (CreateProjectButton.IsChecked == true)
                {
                    CreateProjectButton.IsChecked = false;
                    BrowserContent.Margin = new Thickness(0);
                }
                OpenProjectButton.IsChecked = true;
            }
            else
            {
                if (OpenProjectButton.IsChecked == true)
                {
                    OpenProjectButton.IsChecked = false;
                    BrowserContent.Margin = new Thickness(-800, 0, 0, 0);
                }
                CreateProjectButton.IsChecked = true;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {

        }
    }
}
