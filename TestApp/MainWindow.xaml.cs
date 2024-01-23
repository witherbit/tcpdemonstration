using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TestApp.Pages;
using withersdk.Ui;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            Client.Initialize(new withersdk.Net.Client.ConnectArgs
            {
                Address = System.IO.File.ReadAllText("IP.txt"),
                Port = 5553
            });
            Closing += (sender, e) => Client.Connector.Disconnect();
            Instance = this;
            uiFrame.Content = new ConnectionPage();
        }

        internal static void SetPage(Page page)
        {
            Instance.uiFrame.Content = page;
        }

        internal static Page GetPage
        {
            get =>
            (Page)Instance.uiFrame.Content;
        }

        internal static void InvokeAct(Action act)
        {
            Instance.Invoke(act);
        }
    }
}
