using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace КУРСОВАЯ1
{
    namespace SimpleCompilerWinForms
    {
        class Lexer
        {
            private string _input;
            private int _position;

            public Lexer(string input)
            {
                _input = input;
                _position = 0;
            }

            public Token GetNextToken()
            {
                SkipWhitespace();

                if (_position >= _input.Length)
                    return new Token(TokenType.EndOfInput, "");

                char current = _input[_position];

                if (char.IsLetter(current))
                {
                    string identifier = ReadWhile(char.IsLetter);
                    switch (identifier)
                    {
                        case "if":
                            return new Token(TokenType.If, identifier);
                        case "else":
                            return new Token(TokenType.Else, identifier);
                        case "int":
                            return new Token(TokenType.Int, identifier);
                        case "float":
                            return new Token(TokenType.Float, identifier);
                        case "double":
                            return new Token(TokenType.Double, identifier);
                        default:
                            return new Token(TokenType.Identifier, identifier);
                    }
                }


                if (char.IsDigit(current))
                {
                    string literal = ReadWhile(c => char.IsDigit(c) || c == '.');
                    return new Token(TokenType.Literal, literal);
                }

                switch (current)
                {
                    case '(':
                        _position++;
                        return new Token(TokenType.OpenParen, "(");
                    case ')':
                        _position++;
                        return new Token(TokenType.CloseParen, ")");
                    case '{':
                        _position++;
                        return new Token(TokenType.OpenBrace, "{");
                    case '}':
                        _position++;
                        return new Token(TokenType.CloseBrace, "}");
                    case '+':
                        _position++;
                        return new Token(TokenType.Plus, "+");
                    case '-':
                        _position++;
                        return new Token(TokenType.Minus, "-");
                    case '*':
                        _position++;
                        return new Token(TokenType.Multiply, "*");
                    case '/':
                        _position++;
                        return new Token(TokenType.Divide, "/");
                    case ';':
                        _position++;
                        return new Token(TokenType.Semicolon, ";");
                    case '<':
                        _position++;
                        if (_position < _input.Length && _input[_position] == '=')
                        {
                            _position++;
                            return new Token(TokenType.LessOrEqual, "<=");
                        }
                        return new Token(TokenType.LessThan, "<");
                    case '>':
                        _position++;
                        if (_position < _input.Length && _input[_position] == '=')
                        {
                            _position++;
                            return new Token(TokenType.GreaterOrEqual, ">=");
                        }
                        return new Token(TokenType.GreaterThan, ">");
                    case '=':
                        _position++;
                        if (_position < _input.Length && _input[_position] == '=')
                        {
                            _position++;
                            return new Token(TokenType.Equals, "==");
                        }
                        return new Token(TokenType.Equals, "="); // Одиночный `=` используется в других контекстах (например, присваивание)
                    case '!':
                        _position++;
                        if (_position < _input.Length && _input[_position] == '=')
                        {
                            _position++;
                            return new Token(TokenType.NotEquals, "!=");
                        }
                        return new Token(TokenType.Unknown, "!");

                    default:
                        _position++;
                        return new Token(TokenType.Unknown, current.ToString());
                }
            }

            private void SkipWhitespace()
            {
                while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
                {
                    _position++;
                }
            }

            private string ReadWhile(Func<char, bool> condition)
            {
                int start = _position;
                while (_position < _input.Length && condition(_input[_position]))
                {
                    _position++;
                }
                return _input.Substring(start, _position - start);
            }
        }
    }
}
