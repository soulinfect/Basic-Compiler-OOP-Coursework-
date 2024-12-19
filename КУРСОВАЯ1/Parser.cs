using System;
using System.Collections.Generic;
using КУРСОВАЯ1.SimpleCompilerWinForms;

namespace КУРСОВАЯ1 {
    namespace SimpleCompilerWinForms
    {
        class Parser
        {
            private Lexer _lexer;
            private Token _currentToken;
            private Dictionary<string, double> _variables = new Dictionary<string, double>(); // хранение переменных

            public double? LastExpressionValue { get; private set; }

            public Parser(Lexer lexer)
            {
                _lexer = lexer;
                _currentToken = _lexer.GetNextToken();
            }

            private void Eat(TokenType type)
            {
                if (_currentToken.Type == type)
                {
                    _currentToken = _lexer.GetNextToken();
                }
                else
                {
                    throw new Exception($"Unexpected token: {_currentToken}. Expected: {type}");
                }
            }
            public void Parse()
            {
                while (_currentToken.Type != TokenType.EndOfInput)
                {
                    ParseStatement();
                }
            }
            private void ParseStatement()
            {
                if (_currentToken.Type == TokenType.If)
                {
                    ParseIfElse();
                }
                else if (_currentToken.Type == TokenType.Int ||
                         _currentToken.Type == TokenType.Float ||
                         _currentToken.Type == TokenType.Double)
                {
                    ParseDeclaration();
                }
                else if (_currentToken.Type == TokenType.Identifier) // Если это идентификатор (переменная)
                {
                    ParseExpression(); // Обрабатываем выражение
                    if (_currentToken.Type != TokenType.Semicolon)
                    {
                        throw new Exception($"Expected semicolon after expression but found: {_currentToken}");
                    }
                    Eat(TokenType.Semicolon); // Завершаем выражение точкой с запятой
                }
                else if (_currentToken.Type == TokenType.Literal)
                {
                    ParseExpression(); // Обрабатываем литерал
                    if (_currentToken.Type != TokenType.Semicolon)
                    {
                        throw new Exception($"Expected semicolon after expression but found: {_currentToken}");
                    }
                    Eat(TokenType.Semicolon); // Завершаем выражение точкой с запятой
                }
                else
                {
                    throw new Exception($"Unexpected statement: {_currentToken}");
                }
            }
            private void ParseExpression()
            {
                // Заполнение стека значениями и операторами для вычисления
                Stack<double> values = new Stack<double>();
                Stack<TokenType> operators = new Stack<TokenType>();

                // Обрабатываем все операнды и операторы в выражении
                ParseTerm(values, operators);

                // Обрабатываем операторы сложения и вычитания
                while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
                {
                    operators.Push(_currentToken.Type);
                    Eat(_currentToken.Type);
                    ParseTerm(values, operators);
                }

                // Применяем операторы и вычисляем результат
                while (operators.Count > 0)
                {
                    ApplyOperator(values, operators);
                }

                // Результат выражения будет на верхушке стека
                LastExpressionValue = values.Pop();
            }
            private void ApplyOperator(Stack<double> values, Stack<TokenType> operators)
            {
                if (values.Count < 2 || operators.Count == 0)
                    throw new Exception("Invalid expression");

                double b = values.Pop();
                double a = values.Pop();
                TokenType op = operators.Pop();

                double result;
                switch (op)
                {
                    case TokenType.Plus:
                        result = a + b;
                        break;
                    case TokenType.Minus:
                        result = a - b;
                        break;
                    case TokenType.Multiply:
                        result = a * b;
                        break;
                    case TokenType.Divide:
                        result = a / b;
                        break;
                    default:
                        throw new Exception($"Unexpected operator: {op}");
                }

                values.Push(result);
            }
            private void ParseIfElse()
            {
                Eat(TokenType.If);
                Eat(TokenType.OpenParen);
                Eat(TokenType.CloseParen);
                Eat(TokenType.OpenBrace);
                Console.WriteLine("Parsed 'if' block.");
                Eat(TokenType.CloseBrace);

                if (_currentToken.Type == TokenType.Else)
                {
                    Eat(TokenType.Else);
                    Eat(TokenType.OpenBrace);
                    Console.WriteLine("Parsed 'else' block.");
                    Eat(TokenType.CloseBrace);
                }
            }
            private void ParseDeclaration()
            {
                Console.WriteLine($"Parsed declaration type: {_currentToken.Value}");
                TokenType declarationType = _currentToken.Type;
                Eat(_currentToken.Type); // int, float, or double

                if (_currentToken.Type == TokenType.Identifier)
                {
                    string variableName = _currentToken.Value;
                    Eat(TokenType.Identifier); // Variable name

                    if (_currentToken.Type == TokenType.Equals)
                    {
                        Eat(TokenType.Equals); // =
                        double value = ParseExpressionValue(); // Вычислить значение выражения
                        _variables[variableName] = value; // Сохранить переменную
                        Console.WriteLine($"Variable '{variableName}' assigned value {value}");
                    }

                    if (_currentToken.Type == TokenType.Semicolon)
                    {
                        Eat(TokenType.Semicolon); // ;
                    }
                    else
                    {
                        throw new Exception($"Expected ';' after declaration or assignment, found {_currentToken}");
                    }
                }
                else
                {
                    throw new Exception($"Expected variable name, found {_currentToken}");
                }
            }
            private void ParseTerm(Stack<double> values, Stack<TokenType> operators)
            {
                ParseFactor(values);
                while (_currentToken.Type == TokenType.Multiply || _currentToken.Type == TokenType.Divide)
                {
                    operators.Push(_currentToken.Type);
                    Eat(_currentToken.Type);
                    ParseFactor(values);
                }
            }
            private void ParseFactor(Stack<double> values)
            {
                if (_currentToken.Type == TokenType.Identifier)
                {
                    // Ищем значение переменной в словаре
                    if (!_variables.TryGetValue(_currentToken.Value, out double value))
                    {
                        throw new Exception($"Undefined variable: {_currentToken.Value}");
                    }

                    // Добавляем значение переменной в стек
                    values.Push(value);
                    Eat(TokenType.Identifier); // Переходим к следующему токену
                }
                else if (_currentToken.Type == TokenType.Literal)
                {
                    // Литерал, добавляем в стек
                    values.Push(double.Parse(_currentToken.Value));
                    Eat(TokenType.Literal);
                }
                else if (_currentToken.Type == TokenType.OpenParen)
                {
                    Eat(TokenType.OpenParen);
                    values.Push(ParseExpressionValue()); // Рекурсивно вызываем для вложенных выражений
                    Eat(TokenType.CloseParen);
                }
                else
                {
                    throw new Exception($"Unexpected token in factor: {_currentToken}");
                }
            }
            private double ParseExpressionValue()
            {
                Stack<double> values = new Stack<double>();
                Stack<TokenType> operators = new Stack<TokenType>();

                void ApplyOperator()
                {
                    if (values.Count < 2 || operators.Count == 0)
                        throw new Exception("Invalid expression");

                    double b = values.Pop();
                    double a = values.Pop();
                    TokenType op = operators.Pop();

                    double result;
                    switch (op)
                    {
                        case TokenType.Plus:
                            result = a + b;
                            break;
                        case TokenType.Minus:
                            result = a - b;
                            break;
                        case TokenType.Multiply:
                            result = a * b;
                            break;
                        case TokenType.Divide:
                            result = a / b;
                            break;
                        default:
                            throw new Exception($"Unexpected operator: {op}");
                    }

                    values.Push(result);
                }

                ParseTerm(values, operators); // Обрабатываем первый операнд
                while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
                {
                    operators.Push(_currentToken.Type);
                    Eat(_currentToken.Type);
                    ParseTerm(values, operators); // Обрабатываем следующий операнд
                }

                while (operators.Count > 0)
                {
                    ApplyOperator();
                }

                return values.Pop(); // Возвращаем окончательный результат выражения
            }
        }
    }

}
