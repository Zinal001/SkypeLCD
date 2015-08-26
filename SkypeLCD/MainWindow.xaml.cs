using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Timers;
using Zinal.SkypeLibrary;

namespace Zinal.SkypeLCD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool Exit = false;

        private SkypeListener _Listener = new SkypeListener(@"C:\Users\IT\AppData\Roaming\Skype\laniz.001\main.db", "laniz.001");

        private LCDHandler _LCD = new LCDHandler();

        public MainWindow()
        {
            InitializeComponent();

            _LCD.ConfigurationWindow = this;

            _Listener.MessageSeen += new EventHandler<MessageEventArgs>(_Listener_MessageSeen);

            _Listener.Start();

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !Exit;
            if (!Exit)
                this.Hide();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtDBPath.Text = Properties.Settings.Default.path;
            txtSkypeUsername.Text = Properties.Settings.Default.username;

            if (String.IsNullOrEmpty(Properties.Settings.Default.username) || String.IsNullOrEmpty(Properties.Settings.Default.path) || !File.Exists(Properties.Settings.Default.path))
            {

            }
            else
                this.Hide();
        }

        void _Listener_MessageSeen(object sender, MessageEventArgs e)
        {
            Console.WriteLine("[" + e.Msg.timestamp.ToShortTimeString() + "] " + e.Msg.from_dispname + ": " + e.Msg.ParsedMessage);
            _LCD.LastMessage = e.Msg;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtSkypeUsername.Text.Length < 3)
            {
                MessageBox.Show("You have to provide your username", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(txtDBPath.Text))
            {
                MessageBox.Show("Invalid main.db path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Properties.Settings.Default.username = txtSkypeUsername.Text;
            Properties.Settings.Default.path = txtDBPath.Text;
            Properties.Settings.Default.Save();

            _Listener.Username = txtSkypeUsername.Text;
            _Listener.ChangeDBPath(txtDBPath.Text);
            this.Hide();
        }
    }
}
