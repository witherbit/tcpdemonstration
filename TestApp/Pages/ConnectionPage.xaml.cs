using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using withersdk.Net.Client;
using withersdk.Ui;
using withersdk.Utils;

namespace TestApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ConnectionPage.xaml
    /// </summary>
    public partial class ConnectionPage : Page
    {
        bool hasAnimated = false;
        Thickness RandomThinknessUp => new Thickness(1, 0, 1, 50);
        Thickness RandomThinknessDown => new Thickness(0, 50, 0, 0);
        public ConnectionPage()
        {
            InitializeComponent();
            this.OpacityAnimation(0, 1, 0.2);
            SetMinheight(400);
            MainWindow.Instance.Closing += (sender, e) =>
            {
                hasAnimated = false;
            };
            Client.Connector.TunnelStateUpdate += StateUpdate;
            Client.Connector.StateUpdate += StateChanged;
            Client.Connector.Connect();
        }

        private async void StateChanged(object sender, bool e)
        {
            await Task.Run(() =>
            {
                if (!e && hasAnimated)
                {
                    this.Invoke(() =>
                    {
                        uiState.Text = "Ошибка подключения";
                    });
                    hasAnimated = false;
                    Task.Delay(1000).Wait();
                    this.Invoke(() =>
                    {
                        SetDefault(uiCircle0);
                        SetDefault(uiCircle1);
                        SetDefault(uiCircle2);
                        SetDefault(uiCircle3);
                        SetDefault(uiCircle4);
                        SetDefault(uiCircle5);
                        SetDefault(uiCircle6);
                        SetDefault(uiCircle7);
                    });
                    Task.Delay(2000).Wait();
                    this.Invoke(() =>
                    {
                        uiState.Text = "Подключение (0/16)";
                    });
                    Client.Connector.Connect();
                }
                else if (e && !hasAnimated)
                {
                    StartAnimate();
                }
            });
        }

        public async void StartAnimate()
        {
            hasAnimated = true;
            double wait = 0.07;
            double max = 25;
            await Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    uiCircle0.HeightAnimation(20, max, wait);
                    uiCircle0.WidthAnimation(20, max, wait);
                    uiCircle1.MarginAnimation(RandomThinknessUp, wait);
                }));
                Task.Delay((int)(wait * 1000)).Wait();
                this.Invoke(new Action(() =>
                {
                    uiCircle1.HeightAnimation(20, max, wait);
                    uiCircle1.WidthAnimation(20, max, wait);
                    uiCircle2.MarginAnimation(RandomThinknessUp, wait);
                }));
                Task.Delay((int)(wait * 1000)).Wait();
                this.Invoke(new Action(() =>
                {
                    uiCircle2.HeightAnimation(20, max, wait);
                    uiCircle2.WidthAnimation(20, max, wait);
                    uiCircle3.MarginAnimation(RandomThinknessUp, wait);
                }));
                Task.Delay((int)(wait * 1000)).Wait();
                while (!Client.Connector.CancellationToken.IsCancellationRequested && hasAnimated)
                {
                    this.Invoke(new Action(() =>
                    {
                        uiCircle3.HeightAnimation(20, max, wait);
                        uiCircle3.WidthAnimation(20, max, wait);
                        uiCircle4.MarginAnimation(RandomThinknessUp, wait);
                        uiCircle0.HeightAnimation(max, 20, wait);
                        uiCircle0.WidthAnimation(max, 20, wait);
                        uiCircle1.MarginAnimation(RandomThinknessDown, wait);
                    }));
                    Task.Delay((int)(wait * 1000)).Wait();
                    this.Invoke(new Action(() =>
                    {
                        uiCircle4.HeightAnimation(20, max, wait);
                        uiCircle4.WidthAnimation(20, max, wait);
                        uiCircle5.MarginAnimation(RandomThinknessUp, wait);
                        uiCircle1.HeightAnimation(max, 20, wait);
                        uiCircle1.WidthAnimation(max, 20, wait);
                        uiCircle2.MarginAnimation(RandomThinknessDown, wait);
                    }));
                    Task.Delay((int)(wait * 1000)).Wait();
                    this.Invoke(new Action(() =>
                    {
                        uiCircle5.HeightAnimation(20, max, wait);
                        uiCircle5.WidthAnimation(20, max, wait);
                        uiCircle6.MarginAnimation(RandomThinknessUp, wait);
                        uiCircle2.HeightAnimation(max, 20, wait);
                        uiCircle2.WidthAnimation(max, 20, wait);
                        uiCircle3.MarginAnimation(RandomThinknessDown, wait);
                    }));
                    Task.Delay((int)(wait * 1000)).Wait();
                    this.Invoke(new Action(() =>
                    {
                        uiCircle6.HeightAnimation(20, max, wait);
                        uiCircle6.WidthAnimation(20, max, wait);
                        uiCircle7.MarginAnimation(RandomThinknessUp, wait);
                        uiCircle3.HeightAnimation(max, 20, wait);
                        uiCircle3.WidthAnimation(max, 20, wait);
                        uiCircle4.MarginAnimation(RandomThinknessDown, wait);
                    }));
                    Task.Delay((int)(wait * 1000)).Wait();
                    this.Invoke(new Action(() =>
                    {
                        uiCircle7.HeightAnimation(20, max, wait);
                        uiCircle7.WidthAnimation(20, max, wait);
                        uiCircle0.MarginAnimation(RandomThinknessUp, wait);
                        uiCircle4.HeightAnimation(max, 20, wait);
                        uiCircle4.WidthAnimation(max, 20, wait);
                        uiCircle5.MarginAnimation(RandomThinknessDown, wait);
                    }));
                    Task.Delay((int)(wait * 1000)).Wait();
                    this.Invoke(new Action(() =>
                    {
                        uiCircle0.HeightAnimation(20, max, wait);
                        uiCircle0.WidthAnimation(20, max, wait);
                        uiCircle1.MarginAnimation(RandomThinknessUp, wait);
                        uiCircle5.HeightAnimation(max, 20, wait);
                        uiCircle5.WidthAnimation(max, 20, wait);
                        uiCircle6.MarginAnimation(RandomThinknessDown, wait);
                    }));
                    Task.Delay((int)(wait * 1000)).Wait();
                    this.Invoke(new Action(() =>
                    {
                        uiCircle1.HeightAnimation(20, max, wait);
                        uiCircle1.WidthAnimation(20, max, wait);
                        uiCircle2.MarginAnimation(RandomThinknessUp, wait);
                        uiCircle6.HeightAnimation(max, 20, wait);
                        uiCircle6.WidthAnimation(max, 20, wait);
                        uiCircle7.MarginAnimation(RandomThinknessDown, wait);
                    }));
                    Task.Delay((int)(wait * 1000)).Wait();
                    this.Invoke(new Action(() =>
                    {
                        uiCircle2.HeightAnimation(20, max, wait);
                        uiCircle2.WidthAnimation(20, max, wait);
                        uiCircle3.MarginAnimation(RandomThinknessUp, wait);
                        uiCircle7.HeightAnimation(max, 20, wait);
                        uiCircle7.WidthAnimation(max, 20, wait);
                        uiCircle0.MarginAnimation(RandomThinknessDown, wait);
                    }));
                    Task.Delay((int)(wait * 1000)).Wait();
                }
                if (Client.Connector.State)
                {
                    Client.Ping();
                    wait = 2;
                    this.Invoke(() =>
                    {
                        SetOK(uiCircle0, wait);
                        SetOK(uiCircle1, wait);
                        SetOK(uiCircle2, wait);
                        SetOK(uiCircle3, wait);
                        SetOK(uiCircle4, wait);
                        SetOK(uiCircle5, wait);
                        SetOK(uiCircle6, wait);
                        SetOK(uiCircle7, wait);
                    });
                    Task.Delay((int)(wait * 1000)).Wait();
                    this.Invoke(() => this.OpacityAnimation(1, 0, 0.2));
                    Task.Delay(200).Wait();
                    this.Invoke(() => MainWindow.SetPage(new StartPage()));
                }
            });
        }

        private void StateUpdate(object sender, TunnelState e)
        {
            this.Invoke(new Action(() =>
            {
                switch (e)
                {
                    case TunnelState.Hello_Prepare:
                        uiCircle0.BorderBrushFadeTo("ff4d4d".ConvertToBrush(), 0.5);
                        uiState.Text = "Подпись данных (1/16)";
                        break;
                    case TunnelState.Hello_Send:
                        uiCircle0.BackgroundFadeTo("ff4d4d".ConvertToBrush(), 0.5);
                        uiState.Text = "Отправка данных на сервер (2/16)";
                        break;
                    case TunnelState.Pack_Get_Peer:
                        uiCircle1.BorderBrushFadeTo("ff4dbd".ConvertToBrush(), 0.5);
                        uiState.Text = "Проверка подписи (3/16)";
                        break;
                    case TunnelState.Pack_Dec_Peer:
                        uiCircle1.BackgroundFadeTo("ff4dbd".ConvertToBrush(), 0.5);
                        uiState.Text = "Создание общего ключа шифрования (4/16)";
                        break;
                    case TunnelState.Pack_Peer_Prepare:
                        uiCircle2.BorderBrushFadeTo("ff4dfb".ConvertToBrush(), 0.5);
                        uiState.Text = "Подпись публичного ключа (5/16)";
                        break;
                    case TunnelState.Pack_Peer_Send:
                        uiCircle2.BackgroundFadeTo("ff4dfb".ConvertToBrush(), 0.5);
                        uiState.Text = "Отправка публичного ключа на сервер (6/16)";
                        break;
                    case TunnelState.Pack_Get_MAC:
                        uiCircle3.BorderBrushFadeTo("be4dff".ConvertToBrush(), 0.5);
                        uiState.Text = "Проверка подписи (7/16)";
                        break;
                    case TunnelState.Pack_Dec_MAC:
                        uiCircle3.BackgroundFadeTo("be4dff".ConvertToBrush(), 0.5);
                        uiState.Text = "Создание общего MAC ключа (8/16)";
                        break;
                    case TunnelState.Pack_MAC_Prepare:
                        uiCircle4.BorderBrushFadeTo("834dff".ConvertToBrush(), 0.5);
                        uiState.Text = "Подпись публичного ключа (9/16)";
                        break;
                    case TunnelState.Pack_MAC_Send:
                        uiCircle4.BackgroundFadeTo("834dff".ConvertToBrush(), 0.5);
                        uiState.Text = "Отправка публичного ключа на сервер (10/16)";
                        break;
                    case TunnelState.Pack_Get_Check:
                        uiCircle5.BorderBrushFadeTo("4dacff".ConvertToBrush(), 0.5);
                        uiState.Text = "Проверка подписи (11/16)";
                        break;
                    case TunnelState.Pack_Dec_Check:
                        uiCircle5.BackgroundFadeTo("4dacff".ConvertToBrush(), 0.5);
                        uiState.Text = "Проверка MAC (12/16)";
                        break;
                    case TunnelState.Pack_Check_Prepare:
                        uiCircle6.BorderBrushFadeTo("4dffcc".ConvertToBrush(), 0.5);
                        uiState.Text = "Подпись проверочных данных (13/16)";
                        break;
                    case TunnelState.Pack_Check_Send:
                        uiCircle6.BackgroundFadeTo("4dffcc".ConvertToBrush(), 0.5);
                        uiState.Text = "Отправка проверочных данных на сервер (14/16)";
                        break;
                    case TunnelState.Establish:
                        uiCircle7.BorderBrushFadeTo("abff62".ConvertToBrush(), 0.5);
                        uiState.Text = "Проверка подписи (15/16)";
                        break;
                    case TunnelState.Connected:
                        uiCircle7.BackgroundFadeTo("abff62".ConvertToBrush(), 0.5);
                        uiState.Text = "Зашифрованное соединение создано (16/16)";
                        break;
                }
            }));
            if(e == TunnelState.Connected)
            {
                Task.Delay(100).Wait();
                hasAnimated = false;
            }
        }

        private void SetOK(Border border, double wait)
        {
            border.BorderBrushFadeTo("abff62".ConvertToBrush(), wait);
            border.BackgroundFadeTo("abff62".ConvertToBrush(), wait);
            border.HeightAnimation(border.ActualHeight, 20, wait);
            border.WidthAnimation(border.ActualWidth, 20, wait);
            border.MarginAnimation(new Thickness(0, 0, 0, 0), wait);
            if (border == uiCircle3)
                uiOK_O.OpacityAnimation(0, 1, wait * 2);
            else if (border == uiCircle4)
                uiOK_K.OpacityAnimation(0, 1, wait * 2);
            else
                border.OpacityAnimation(1, 0, wait);
        }

        private void SetDefault(Border border)
        {
            border.BackgroundFadeTo("ffffff".ConvertToBrush(), 1);
            border.BorderBrushFadeTo("3b3b3b".ConvertToBrush(), 1);
            border.HeightAnimation(border.ActualHeight, 20, 1);
            border.WidthAnimation(border.ActualWidth, 20, 1);
            border.MarginAnimation(new Thickness(0, 0, 0, 0), 1);
            uiOK_O.OpacityAnimation(uiOK_O.Opacity, 0, 1 * 2);
            uiOK_K.OpacityAnimation(uiOK_K.Opacity, 0, 1 * 2);
        }

        private void SetMinheight(double value)
        {
            if (MainWindow.Instance.ActualHeight <= 450)
            {
                MainWindow.Instance.HeightAnimation(MainWindow.Instance.ActualHeight, value, 0.2);
            }
            MainWindow.Instance.MinHeight = value;
        }
    }
}
