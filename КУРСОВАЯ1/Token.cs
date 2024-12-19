using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace КУРСОВАЯ1
{
    namespace SimpleCompilerWinForms
    {
        // Перечисление типов токенов
        enum TokenType
        {
            If,             // Ключевое слово "if"
            Else,           // Ключевое слово "else"
            OpenParen,      // Открывающая скобка "("
            CloseParen,     // Закрывающая скобка ")"
            OpenBrace,      // Открывающая фигурная скобка "{"
            CloseBrace,     // Закрывающая фигурная скобка "}"
            Identifier,     // Идентификаторы (переменные, функции)
            Literal,        // Литералы (числа)
            Plus,           // Оператор "+"
            Minus,          // Оператор "-"
            Multiply,       // Оператор "*"
            Divide,         // Оператор "/"
            Int,            // Ключевое слово "int"
            Float,          // Ключевое слово "float"
            Double,         // Ключевое слово "double"
            EndOfInput,     // Конец входных данных
            Semicolon,      // Конец строки ;
            LessThan,       // <
            GreaterThan,    // >
            Equals,         // ==
            NotEquals,      // !=
            LessOrEqual,    // <=
            GreaterOrEqual, // >=
            Unknown,        // Неизвестный токен
            Equal           // для символа '='
        }

        // Класс токена
        class Token
        {
            public TokenType Type { get; set; } // Тип токена
            public string Value { get; set; }  // Значение токена

            public Token(TokenType type, string value)
            {
                Type = type;
                Value = value;
            }

            // Переопределение метода ToString для удобного отображения токенов
            public override string ToString() => $"{Type}: {Value}";
        }
    }

}
