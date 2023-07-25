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
        decimal firstNumber;
        decimal secondNumber;

        bool firstNumberDefined = false;

        enum Operation
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Equal
        }

        Operation selectedOperation;

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
            else if ((e.Key == Key.OemPeriod) || (e.Key == Key.Decimal))
            {
                InputDot();
            }
            else if (e.Key == Key.Delete)
            {
                ClearCurrentNumber();
            }
            else if (e.Key == Key.Back)
            {
                DoBackspace();
            }
            else if ((e.Key == Key.Add) || (e.Key == Key.OemPlus))
            {
                Add();
            }
            else if ((e.Key == Key.OemMinus) || (e.Key == Key.Subtract))
            {
                if ((TextBlockMain.Text.Length == 0) && (!firstNumberDefined))
                    Negate();
                else
                    Subtract();
            }
            else if (e.Key == Key.Multiply)
            {
                Multiply();
            }
            else if (e.Key == Key.Divide)
            {
                Divide();
            }
            else if (e.Key == Key.Enter)
            {
                Equal();
            }
        }

        private void ButtonDigits_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string buttonContent = (sender as Button).Content.ToString();
                int digit = int.Parse(buttonContent);
                InputDigit(digit);
            }
            catch { }
        }

        private void InputDigit(int digit)
        {
            if ((digit < 0) || (digit > 9)) return;

            if (TextBlockMain.Text == "0")
                TextBlockMain.Text = digit.ToString();
            else if (TextBlockMain.Text == "-0")
                TextBlockMain.Text = '-' + digit.ToString();
            else
                TextBlockMain.Text += digit.ToString();
        }

        private void ClearCurrentNumber()
        {
            TextBlockMain.Text = "";
        }

        private void ButtonDot_Click(object sender, RoutedEventArgs e)
        {
            InputDot();
        }

        private void InputDot()
        {
            if (TextBlockMain.Text.Contains('.')) return;

            TextBlockMain.Text += '.';
        }

        private void ButtonPlusMinus_Click(object sender, RoutedEventArgs e)
        {
            Negate();
        }

        private void Negate()
        {
            try
            {
                if (TextBlockMain.Text.StartsWith('-'))
                    TextBlockMain.Text = TextBlockMain.Text.Substring(1, TextBlockMain.Text.Length - 1);
                else
                    TextBlockMain.Text = '-' + TextBlockMain.Text;
            }
            catch { }
        }

        private void ButtonBackspace_Click(object sender, RoutedEventArgs e)
        {
            DoBackspace();
        }

        private void DoBackspace()
        {
            if (TextBlockMain.Text.Length <= 1)
                TextBlockMain.Text = "";
            else
                TextBlockMain.Text = TextBlockMain.Text.Substring(0, TextBlockMain.Text.Length - 1);
        }

        private void ProcessOperation(Operation operation)
        {
            if (!firstNumberDefined)
            {
                if (operation != Operation.Equal) AcceptFirstNumber(operation);
            }
            else if (decimal.TryParse(TextBlockMain.Text, out secondNumber))
            {
                AcceptSecondNumberAndCalculateResult(operation);
            }
            else
            {
                if (operation != Operation.Equal) ChangeOperation(operation);
            }
        }

        private void AcceptFirstNumber(Operation operation)
        {
            try
            {
                firstNumber = decimal.Parse(TextBlockMain.Text);
                selectedOperation = operation;

                TextBlockHistory.Text = TextBlockMain.Text + GetOperator(operation);

                TextBlockMain.Text = "";

                firstNumberDefined = true;
            }
            catch { }
        }

        private void AcceptSecondNumberAndCalculateResult(Operation newOperation)
        {
            try
            {
                decimal result;

                switch (selectedOperation)
                {
                    case Operation.Addition: result = firstNumber + secondNumber; break;
                    case Operation.Subtraction: result = firstNumber - secondNumber; break;
                    case Operation.Multiplication: result = firstNumber * secondNumber; break;
                    case Operation.Division: result = firstNumber / secondNumber; break;
                    default: throw new Exception();
                }

                TextBlockMain.Text = result.ToString();

                if (newOperation == Operation.Equal)
                {
                    TextBlockHistory.Text = firstNumber.ToString() + GetOperator(selectedOperation) + GetSecondNumberFormatted() + '=';
                    firstNumberDefined = false;
                }
                else
                {
                    firstNumber = result;
                    selectedOperation = newOperation;
                    TextBlockMain.Text = "";
                    TextBlockHistory.Text = result.ToString() + GetOperator(selectedOperation);
                }
            }
            catch { }
        }

        private void ChangeOperation(Operation newOperation)
        {
            selectedOperation = newOperation;
            TextBlockHistory.Text = firstNumber.ToString() + GetOperator(selectedOperation);
        }

        private string GetSecondNumberFormatted()
        {
            if (secondNumber < 0)
                return '(' + secondNumber.ToString() + ')';
            else
                return secondNumber.ToString();
        }

        private char GetOperator(Operation operation)
        {
            switch (operation)
            {
                case Operation.Addition: return '+';
                case Operation.Subtraction: return '-';
                case Operation.Multiplication: return '×';
                case Operation.Division: return '÷';
            }
            throw new Exception();
        }

        private void Add()
        {
            ProcessOperation(Operation.Addition);
        }

        private void Subtract()
        {
            ProcessOperation(Operation.Subtraction);
        }

        private void Multiply()
        {
            ProcessOperation(Operation.Multiplication);
        }

        private void Divide()
        {
            ProcessOperation(Operation.Division);
        }

        private void Equal()
        {
            ProcessOperation(Operation.Equal);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void ButtonSubtract_Click(object sender, RoutedEventArgs e)
        {
            Subtract();
        }

        private void ButtonMultiply_Click(object sender, RoutedEventArgs e)
        {
            Multiply();
        }

        private void ButtonDivide_Click(object sender, RoutedEventArgs e)
        {
            Divide();
        }

        private void ButtonEqual_Click(object sender, RoutedEventArgs e)
        {
            Equal();
        }

        private void ButtonClearEntry_Click(object sender, RoutedEventArgs e)
        {
            ClearCurrentNumber();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearCurrentNumber();
            TextBlockHistory.Text = "";
            firstNumberDefined = false;
        }
    }
}
