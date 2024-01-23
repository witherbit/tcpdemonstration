using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
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

namespace TestApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
            this.OpacityAnimation(0, 1, 0.2);
            SetMinheight(450);
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
            if (!uiLogin.Text.IsNullOrEmpty() &&
                uiLogin.Text.Length > 6 &&
                !uiPassword.Password.IsNullOrEmpty() &&
                uiPassword.Password.Length > 8 &&
                !uiName.Text.IsNullOrEmpty() &&
                uiName.Text.Length > 1)
            {
                var method = new RequestMethod.SignUp
                {
                    Login = uiLogin.Text,
                    Password = uiPassword.Password,
                    Name = uiName.Text,
                    Email = uiEmail.Text.Length >= 7 ? uiEmail.Text : null,
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
                this.Invoke(() => MainWindow.SetPage(new StartPage()));
            });
        }

        private async void SetMinheight(double value)
        {
            if (MainWindow.Instance.ActualHeight <= 450)
            {
                MainWindow.Instance.HeightAnimation(MainWindow.Instance.ActualHeight, value, 0.2);
            }
            await Task.Run(() =>
            {
                Task.Delay(200).Wait();
                this.Invoke(() => MainWindow.Instance.MinHeight = value);
            });
        }

        private void uiLogin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex _regex = new Regex("[^a-zA-Z0-9]+$");
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void uiName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex _regex = new Regex("[^а-яА-Яa-zA-Z]+$");
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void uiPassword_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex _regex = new Regex("[^a-zA-Z0-9!@#$%^&*()_+=-]+$");
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void uiEmail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex _regex = new Regex("[^a-z0-9@.]+$");
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
