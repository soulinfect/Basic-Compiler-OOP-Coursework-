using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using КУРСОВАЯ1.SimpleCompilerWinForms;

namespace КУРСОВАЯ1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void CompileButton_Click_1(object sender, EventArgs e)
        {
            string input = inputTextBox.Text;
            outputTextBox.Clear();
            resultTextBox.Clear();

            try
            {
                Lexer lexer = new Lexer(input);
                Parser parser = new Parser(lexer);

                // Перехватываем поток вывода для консольных сообщений
                var outputBuilder = new System.Text.StringBuilder();
                using (var writer = new System.IO.StringWriter(outputBuilder))
                {
                    Console.SetOut(writer);

                    // Запускаем парсер
                    parser.Parse();

                    // Отображаем результат парсинга в outputTextBox
                    outputTextBox.Text = "Compilation successful.\n" + outputBuilder.ToString();

                    // Теперь парсер вернет значение выражения, если оно есть
                    if (parser.LastExpressionValue.HasValue)
                    {
                        resultTextBox.Text = parser.LastExpressionValue.ToString();
                    }
                    else
                    {
                        resultTextBox.Text = "Unable to evaluate.";
                    }
                }
            }
            catch (Exception ex)
            {
                outputTextBox.Text = $"Compilation error: {ex.Message}";
            }
        }
        private double EvaluateExpression(Stack<double> values, Stack<TokenType> operators)
        {
            while (operators.Count > 0)
            {
                double b = values.Pop();
                double a = values.Pop();
                TokenType op = operators.Pop();

                switch (op)
                {
                    case TokenType.Plus:
                        values.Push(a + b);
                        break;
                    case TokenType.Minus:
                        values.Push(a - b);
                        break;
                    case TokenType.Multiply:
                        values.Push(a * b);
                        break;
                    case TokenType.Divide:
                        values.Push(a / b);
                        break;
                    case TokenType.LessThan:
                        values.Push(a < b ? 1 : 0);
                        break;
                    case TokenType.GreaterThan:
                        values.Push(a > b ? 1 : 0);
                        break;
                    case TokenType.Equals:
                        values.Push(Math.Abs(a - b) < 1e-9 ? 1 : 0); // Проверка равенства с учетом точности
                        break;
                    case TokenType.NotEquals:
                        values.Push(Math.Abs(a - b) >= 1e-9 ? 1 : 0);
                        break;
                    case TokenType.LessOrEqual:
                        values.Push(a <= b ? 1 : 0);
                        break;
                    case TokenType.GreaterOrEqual:
                        values.Push(a >= b ? 1 : 0);
                        break;
                    default:
                        throw new Exception($"Unexpected operator: {op}");
                }
            }

            return values.Pop();
        }



    }
}
