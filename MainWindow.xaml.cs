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
        bool secondNumberDefined = false;

        enum SecondNumberModifier
        {
            None,
            Percentage,
            Reverse,
            Sqr,
            Sqrt
        }

        SecondNumberModifier secondNumberModifier = SecondNumberModifier.None;

        bool textBlockMainShouldBeCleared = false;
        bool textBlockHistoryShouldBeCleared = false;

        enum Operation
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Equal
        }

        Operation selectedOperation;

        enum Error
        {
            DivisionByZero,
            RootExtraction
        }

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
                ClearTextBlockMain();
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
            else if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.D5))
            {
                Percent();
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

        private void ClearTextBlockMainIfNeeded()
        {
            if (textBlockMainShouldBeCleared)
            {
                ClearTextBlockMain();
                textBlockMainShouldBeCleared = false;
            }
        }

        private void ClearTextBlockHistoryIfNeeded()
        {
            if (textBlockHistoryShouldBeCleared)
            {
                ClearTextBlockHistory();
                firstNumberDefined = false;
                secondNumberDefined = false;
                textBlockHistoryShouldBeCleared = false;
            }
        }

        private void InputDigit(int digit)
        {
            if ((digit < 0) || (digit > 9)) return;

            ClearTextBlockMainIfNeeded();
            ClearTextBlockHistoryIfNeeded();
            CheckCurrentNumber();

            if (TextBlockMain.Text == "0")
                TextBlockMain.Text = digit.ToString();
            else
                TextBlockMain.Text += digit.ToString();
        }

        private void ClearTextBlockMain()
        {
            TextBlockMain.Text = "0";
            TextBlockMain.FontSize = 45;
        }

        private void ClearTextBlockHistory()
        {
            TextBlockHistory.Text = "";
            TextBlockHistory.FontSize = 16;
        }

        private void ButtonDot_Click(object sender, RoutedEventArgs e)
        {
            InputDot();
        }

        private void InputDot()
        {
            if (TextBlockMain.Text.Contains('.')) return;

            ClearTextBlockMainIfNeeded();
            ClearTextBlockHistoryIfNeeded();
            CheckCurrentNumber();

            TextBlockMain.Text += '.';
        }

        private void ButtonPlusMinus_Click(object sender, RoutedEventArgs e)
        {
            Negate();
        }

        private void CheckCurrentNumber()
        {
            if (!decimal.TryParse(TextBlockMain.Text, out decimal currentNumber))
            {
                ClearTextBlockMain();
                ClearTextBlockHistory();
                firstNumberDefined = false;
                secondNumberDefined = false;
                textBlockHistoryShouldBeCleared = false;
                textBlockMainShouldBeCleared = false;
            }
        }

        private void Negate()
        {
            try
            {
                ClearTextBlockHistoryIfNeeded();
                CheckCurrentNumber();

                if (TextBlockMain.Text.StartsWith('-'))
                    TextBlockMain.Text = TextBlockMain.Text.Substring(1, TextBlockMain.Text.Length - 1);
                else if (TextBlockMain.Text != "0")
                    TextBlockMain.Text = '-' + TextBlockMain.Text;

                textBlockMainShouldBeCleared = false;
            }
            catch { }
        }

        private void ButtonBackspace_Click(object sender, RoutedEventArgs e)
        {
            DoBackspace();
        }

        private void DoBackspace()
        {
            ClearTextBlockHistoryIfNeeded();

            if (TextBlockMain.Text.Length <= 1)
                ClearTextBlockMain();
            else
                TextBlockMain.Text = TextBlockMain.Text.Substring(0, TextBlockMain.Text.Length - 1);

            CheckCurrentNumber();
            textBlockMainShouldBeCleared = false;
        }

        private void ResetSecondNumberModifier()
        {
            secondNumberModifier = SecondNumberModifier.None;
        }

        private void ProcessOperation(Operation operation)
        {
            CheckCurrentNumber();

            if (operation != Operation.Equal)
            {
                ClearTextBlockHistoryIfNeeded();
            }

            if (!firstNumberDefined)
            {
                if (operation == Operation.Equal)
                {
                    // Unary operations

                    if ((secondNumberModifier != SecondNumberModifier.None) && (secondNumberModifier != SecondNumberModifier.Percentage))
                    {
                        DoUnaryOperation();
                    }
                }
                else
                {
                    // First number and operation are input

                    AcceptFirstNumber(operation);
                }

                ResetSecondNumberModifier();
            }
            else if ((textBlockMainShouldBeCleared) && (secondNumberDefined) && (operation == Operation.Equal))
            {
                if ((secondNumberModifier != SecondNumberModifier.None) && (secondNumberModifier != SecondNumberModifier.Percentage))
                {
                    // Unary operation after equal button
                    DoUnaryOperation();
                }
                else
                {
                    // The case when the equal button is pushed again
                    CalculateResult(operation);
                }
            }
            else if ((!textBlockMainShouldBeCleared) && (decimal.TryParse(TextBlockMain.Text, out secondNumber)))
            {
                // Standard calculation

                CalculateResult(operation);
            }
            else
            {
                // Change operation

                if (operation != Operation.Equal) ChangeOperation(operation);
            }
        }

        private void DoUnaryOperation()
        {
            try
            {
                firstNumberDefined = false;

                secondNumber = decimal.Parse(TextBlockMain.Text);

                TextBlockMain.Text = ApplyModifier().ToStringDecimal();
                TextBlockHistory.Text = GetSecondNumberFormatted();

                textBlockMainShouldBeCleared = true;
                textBlockHistoryShouldBeCleared = true;
            }
            catch { }
        }

        private void AcceptFirstNumber(Operation operation)
        {
            try
            {
                firstNumber = decimal.Parse(TextBlockMain.Text);
                selectedOperation = operation;

                TextBlockHistory.Text = TextBlockMain.Text + GetOperator(operation);

                firstNumberDefined = true;
                textBlockMainShouldBeCleared = true;
            }
            catch { }
        }

        private void RaiseError(Error error)
        {
            if (error == Error.DivisionByZero)
                TextBlockMain.Text = "Cannot divide by zero";
            else if (error == Error.RootExtraction)
                TextBlockMain.Text = "Cannot extract root";
            else
                ClearTextBlockMain();

            if (firstNumberDefined)
                TextBlockHistory.Text = firstNumber.ToStringDecimal() + GetOperator(selectedOperation) + GetSecondNumberFormatted() + '=';
            else
                TextBlockHistory.Text = GetSecondNumberFormatted();

            firstNumberDefined = false;
            secondNumberDefined = false;
            textBlockHistoryShouldBeCleared = true;
            textBlockMainShouldBeCleared = true;

            throw new Exception();
        }

        private decimal ApplyModifier()
        {
            if (secondNumberModifier == SecondNumberModifier.Reverse)
            {
                if (secondNumber == 0) RaiseError(Error.DivisionByZero);
                return 1 / secondNumber;
            }
            else if (secondNumberModifier == SecondNumberModifier.Sqr)
            {
                return secondNumber * secondNumber;
            }
            else if (secondNumberModifier == SecondNumberModifier.Sqrt)
            {
                if (secondNumber < 0) RaiseError(Error.RootExtraction);
                return (decimal)Math.Sqrt((double)secondNumber);
            }
            else
            {
                return secondNumber;
            }
        }

        private void CalculateResult(Operation newOperation)
        {
            try
            {
                decimal tempSecondNumber;
                decimal result;

                if (secondNumberModifier == SecondNumberModifier.Percentage)
                {
                    tempSecondNumber = firstNumber * secondNumber / 100M;
                }
                else
                {
                    tempSecondNumber = ApplyModifier();
                }

                switch (selectedOperation)
                {
                    case Operation.Addition: result = firstNumber + tempSecondNumber; break;
                    case Operation.Subtraction: result = firstNumber - tempSecondNumber; break;
                    case Operation.Multiplication: result = firstNumber * tempSecondNumber; break;
                    case Operation.Division:
                        if (tempSecondNumber == 0) RaiseError(Error.DivisionByZero);
                        result = firstNumber / tempSecondNumber;
                        break;
                    default: throw new Exception();
                }

                secondNumberDefined = true;

                TextBlockMain.Text = result.ToStringDecimal();

                if (newOperation == Operation.Equal)
                {
                    TextBlockHistory.Text = firstNumber.ToStringDecimal() + GetOperator(selectedOperation) + GetSecondNumberFormatted() + '=';
                    firstNumber = result;
                    textBlockHistoryShouldBeCleared = true;
                }
                else
                {
                    TextBlockHistory.Text = result.ToStringDecimal() + GetOperator(newOperation);
                    firstNumber = result;
                    selectedOperation = newOperation;
                }

                textBlockMainShouldBeCleared = true;
            }
            catch { }
        }

        private void ChangeOperation(Operation newOperation)
        {
            selectedOperation = newOperation;
            TextBlockHistory.Text = TextBlockHistory.Text.Substring(0, TextBlockHistory.Text.Length - 1) + GetOperator(selectedOperation);
        }

        private string GetSecondNumberFormatted()
        {
            switch (secondNumberModifier)
            {
                case SecondNumberModifier.Percentage:
                    if (secondNumber < 0)
                        return "(" + secondNumber.ToStringDecimal() + "%)";
                    else
                        return secondNumber.ToStringDecimal() + "%";
                case SecondNumberModifier.Reverse:
                    if (secondNumber < 0)
                        return "(1/(" + secondNumber.ToStringDecimal() + "))";
                    else
                        return "(1/" + secondNumber.ToStringDecimal() + ")";
                case SecondNumberModifier.Sqr:
                    return "sqr(" + secondNumber.ToStringDecimal() + ")";
                case SecondNumberModifier.Sqrt:
                    return "sqrt(" + secondNumber.ToStringDecimal() + ")";
                default:
                    if (secondNumber < 0)
                        return "(" + secondNumber.ToStringDecimal() + ")";
                    else
                        return secondNumber.ToStringDecimal();
            }
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
            ClearTextBlockMain();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBlockMain();
            ClearTextBlockHistory();
            firstNumberDefined = false;
            secondNumberDefined = false;
            textBlockMainShouldBeCleared = true;
        }

        private void ButtonPercent_Click(object sender, RoutedEventArgs e)
        {
            Percent();
        }

        private void Percent()
        {
            secondNumberModifier = SecondNumberModifier.Percentage;
            Equal();
        }

        private void ButtonReverse_Click(object sender, RoutedEventArgs e)
        {
            secondNumberModifier = SecondNumberModifier.Reverse;
            Equal();
        }

        private void ButtonSqr_Click(object sender, RoutedEventArgs e)
        {
            secondNumberModifier = SecondNumberModifier.Sqr;
            Equal();
        }

        private void ButtonSqrt_Click(object sender, RoutedEventArgs e)
        {
            secondNumberModifier = SecondNumberModifier.Sqrt;
            Equal();
        }

        private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null) AdjustFontSize(textBlock);
        }

        private void AdjustFontSize(TextBlock textBlock)
        {
            if (textBlock.ActualWidth > (MainGrid.ActualWidth - 20))
            {
                textBlock.FontSize -= 2;
            }
        }
    }

    public static class DecimalExtensions
    {
        public static string ToStringDecimal(this decimal number)
        {
            return number.ToString("0.############################");
        }
    }
}
