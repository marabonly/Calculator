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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // ---------------------------------------
        //               C H R O M E
        // ---------------------------------------

        void ChromeButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void ChromeButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            return;

            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        void ChromeButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        
        void Window_StateChanged(object sender, EventArgs e)
        {
            /*
            if (this.WindowState == WindowState.Maximized)
            {
                RootElement.Margin = new Thickness(8);
                SetImage(ImageButtonMaximize, "chrome_maximize_backward.png");
            }
            else
            {
                RootElement.Margin = new Thickness(0);
                SetImage(ImageButtonMaximize, "chrome_maximize_forward.png");
            }
            */
        }
        
        public static void SetImage(Image image, string imageFilename)
        {
            if (image == null) return;

            try
            {
                image.Source = new BitmapImage(new Uri("pack://application:,,,/Calculator;component/img/" + imageFilename));
            }
            catch { }
        }

        // ---------------------------------------
        //       E N D   O F   C H R O M E
        // ---------------------------------------

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                int digit = 0;

                if (e.Key >= Key.D0 && e.Key <= Key.D9) digit = (int)e.Key - (int)Key.D0;
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) digit = (int)e.Key - (int)Key.NumPad0;

                InputDigit(digit);
            }
            else if (e.Key == Key.Delete)
            {
                ClearCurrentNumber();
            }
        }

        private void ButtonDigits_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    string buttonContent = (sender as Button).Content.ToString();
                    int digit = int.Parse(buttonContent);
                    InputDigit(digit);
                }
            }
            catch { }
        }

        private void InputDigit(int digit)
        {
            if ((digit < 0) || (digit > 9)) return;

            if (TextBlockMain.Text == "0")
                TextBlockMain.Text = digit.ToString();
            else
                TextBlockMain.Text += digit.ToString();
        }

        private void ClearCurrentNumber()
        {
            TextBlockMain.Text = "0";
        }
    }
}
