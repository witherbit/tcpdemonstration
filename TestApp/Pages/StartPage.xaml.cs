using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using withersdk.Net.Methods.Wrapper;
using withersdk.Ui;
using withersdk.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace TestApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
            this.OpacityAnimation(0, 1, 0.2);
            SetMinheight(400);
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var tb = sender as TextBlock;
            tb.ForegroundFadeTo("#7721ff".ConvertToBrush(), 0.2);
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            var tb = sender as TextBlock;
            tb.ForegroundFadeTo("#000000".ConvertToBrush(), 0.2);
        }


        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            button.BackgroundFadeTo("#7721ff".ConvertToBrush(), 0.2);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            button.BackgroundFadeTo("#000000".ConvertToBrush(), 0.2);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if( !uiLogin.Text.IsNullOrEmpty() && 
                uiLogin.Text.Length > 6 && 
                !uiPassword.Password.IsNullOrEmpty() && 
                uiPassword.Password.Length > 8)
            {
                var method = new RequestMethod.SignIn
                {
                    Login = uiLogin.Text,
                    Password = uiPassword.Password,
                };
                uiButton.IsEnabled = false;
                uiExist.IsEnabled = false;
                Client.Connector.SendMethod(method);
            }
            else
                uiErrorMessage.Text = "Неправильно заполнены поля ввода";
        }

        private async void TextBlock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            this.OpacityAnimation(1, 0, 0.2);
            await Task.Run(() =>
            {
                Task.Delay(200).Wait();
                this.Invoke(() => MainWindow.SetPage(new SignUpPage()));
            });
        }

        private void SetMinheight(double value)
        {
            if(MainWindow.Instance.ActualHeight <= 450)
            {
                MainWindow.Instance.HeightAnimation(MainWindow.Instance.ActualHeight, value, 0.2);
            }
            MainWindow.Instance.MinHeight = value;
        }

        private void uiLogin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex _regex = new Regex("[^a-zA-Z0-9]+$");
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void uiPassword_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex _regex = new Regex("[^a-zA-Z0-9!@#$%^&*()_+=-]+$");
            e.Handled = _regex.IsMatch(e.Text);
        }

        public void Cancel(string text)
        {
            this.Invoke(() =>
            {
                uiErrorMessage.Text = text;
                uiButton.IsEnabled = true;
                uiExist.IsEnabled = true;
            });
        }
    }
}
