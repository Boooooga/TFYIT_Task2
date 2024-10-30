using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TFYIT_Task2
{
    internal class LexAnalyzer
    {
        string path = "";
        string[] reservedWords = { "if", "then", "else", "end", "or", "and", "not" };
        string[] specialSigns = { ">", "<", "==", "<>", ">=", "<=", "=", "+", "-", "*", "/" };
        List<string> messages = new List<string>();
        private string[] delimiters = { ".", ";", ",", "(", ")" };

        Dictionary<string, List<string>> lexemes = new Dictionary<string, List<string>>();


        // ПАРАМЕТРЫ АВТОМАТА
        private enum States { S, ID, NUM, OPER, ASGN, CMP, LMEQUAL, EQ, F, DLM, ERR } // состояния автомата
        // S - начальное, ID - идентификатор, NUM - константа, OPER - арифметическая операция,
        // ASGN - присваивание, CMP - сравнение (логическое), LMEQUAL - знаки ">=" и "<=",
        // EQ - знак "==", DLM - разделитель
        private States currentState;

        public List<string> Analyze(string path)
        {
            int i = 0;
            int start = 0;
            string text;
            List<string> allLexemes = new List<string>();
            // читаем анализируемый текст из файла
            using (StreamReader file = new StreamReader(path))
            {
                text = file.ReadToEnd();
            }

            Console.Write($"Исходный текст: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.ResetColor();
            Console.WriteLine("\n");

            text = text.ToLower() + " ";
            while (currentState != States.F && currentState != States.ERR)
            {
                States prevState = currentState;
                bool add = true;

                switch (currentState)
                {
                    case States.S:
                        if (text[i] == ' ')
                        {
                        }
                        // если считанный символ - буква
                        else if (char.IsLetter(text[i]))
                            currentState = States.ID;
                        // если считанный символ - цифра
                        else if (char.IsDigit(text[i]))
                            currentState = States.NUM;
                        // если считанный символ - знак сравнения
                        else if (text[i] == '>' || text[i] == '<')
                            currentState = States.CMP;
                        // если считанный символ - знак равенства
                        else if (text[i] == '=')
                            currentState = States.ASGN;
                        // если считанный символ - арифметическая операция
                        else if (text[i] == '+' || text[i] == '-' || text[i] == '*' || text[i] == '/')
                            currentState = States.OPER;
                        else if (text[i] == ';')
                            currentState = States.DLM;
                        else currentState = States.ERR;
                        break;

                    case States.ID:
                        if (text[i] == ' ')
                            currentState = States.S;
                        else if (char.IsLetterOrDigit(text[i]))
                            add = false;
                        else if (text[i] == '>' || text[i] == '<')
                            currentState = States.CMP;
                        else if (text[i] == '=')
                            currentState = States.ASGN;
                        else if (text[i] == '+' || text[i] == '-' || text[i] == '*' || text[i] == '/')
                            currentState = States.OPER;
                        else if (text[i] == ';')
                            currentState = States.DLM;
                        else
                        {
                            currentState = States.ERR;
                            add = false;
                        }
                        break;

                    case States.NUM:
                        if (text[i] == ' ')
                            currentState = States.S;
                        else if (char.IsDigit(text[i]))
                            add = false;
                        else if (text[i] == '>' || text[i] == '<')
                            currentState = States.CMP;
                        else if (text[i] == '=')
                            currentState = States.ASGN;
                        else if (text[i] == '+' || text[i] == '-' || text[i] == '*' || text[i] == '/')
                            currentState = States.OPER;
                        else if (text[i] == ';')
                            currentState = States.DLM;
                        else
                        {
                            currentState = States.ERR;
                            add = false;
                        }
                        break;

                    case States.CMP:
                        if (text[i] == ' ')
                            currentState = States.S;
                        else if (char.IsLetter(text[i]))
                            currentState = States.ID;
                        else if (char.IsDigit(text[i]))
                            currentState = States.NUM;
                        else if (text[i] == '=')
                        {
                            currentState = States.LMEQUAL;
                            add = false; // если получился символ >= или <=
                        }
                        else if (text[i] == ';')
                            currentState = States.DLM;
                        //else if (text[i - 1] == '<' && text[i] == '>')
                        //    currentState = States.S;
                        else
                        {
                            currentState = States.ERR;
                            add = false;
                        }
                        break;

                    case States.ASGN:
                        if (text[i] == ' ')
                            currentState = States.S;
                        else if (char.IsLetter(text[i]))
                            currentState = States.ID;
                        else if (char.IsDigit(text[i]))
                            currentState = States.NUM;
                        else if (text[i] == '=')
                        {
                            currentState = States.EQ;
                            add = false;
                        }
                        else if (text[i] == ';')
                            currentState = States.DLM;
                        else
                        {
                            currentState = States.ERR;
                            add = false;
                        }
                        break;

                    case States.OPER:
                        if (text[i] == ' ')
                            currentState = States.S;
                        else if (char.IsLetter(text[i]))
                            currentState = States.ID;
                        else if (char.IsDigit(text[i]))
                            currentState = States.NUM;
                        else if (text[i] == ';')
                            currentState = States.DLM;
                        else
                        {
                            currentState = States.ERR;
                            add = false;
                        }
                        break;

                    case States.LMEQUAL:
                    case States.EQ:
                        if (text[i] == ' ')
                            currentState = States.S;
                        else if (char.IsLetter(text[i]))
                            currentState = States.ID;
                        else if (char.IsDigit(text[i]))
                            currentState = States.NUM;
                        else if (text[i] == ';')
                            currentState = States.DLM;
                        else
                        {
                            currentState = States.ERR;
                            add = false;
                        }
                        break;

                    case States.DLM:
                        if (text[i] == ' ')
                            currentState = States.S;
                        else if (char.IsLetter(text[i]))
                            currentState = States.ID;
                        else if (char.IsDigit(text[i]))
                            currentState = States.NUM;
                        else
                        {
                            currentState = States.ERR;
                            add = false;
                        }
                        break;
                }

                string message = "";
                if (add)
                {
                    for (int j = start; j < i; j++)
                    {
                        message += text[j];
                    }
                    if (messages.Count > 0 && message.Replace(" ", "").Length != 0)
                    {
                        if (messages.Last() != message.Replace(" ", ""))
                        {
                            messages.Add(message.Replace(" ", ""));
                        }
                    }
                    else if (message.Replace(" ", "").Length != 0)
                        messages.Add(message.Replace(" ", ""));
                }

                if ((currentState != prevState) && (currentState == States.ID ||
                    currentState == States.NUM || currentState == States.CMP ||
                    currentState == States.ASGN || currentState == States.OPER
                    || currentState == States.DLM))
                {
                    start = i;
                }

                if (currentState != States.F)
                    i++;

                if (message.Contains("end"))
                {
                    break;
                }

            }

            foreach (string item in messages)
            {
                if (reservedWords.Contains(item))
                {
                    if (!lexemes.Keys.Contains("Зарезервированные слова"))
                    {
                        lexemes.Add("Зарезервированные слова", new List<string> { item });
                        allLexemes.Add(item);
                        Console.WriteLine($"{item} \t| зарезервированное слово");
                    }
                    else
                    {
                        lexemes["Зарезервированные слова"].Add(item);
                        allLexemes.Add(item);
                        Console.WriteLine($"{item} \t| зарезервированное слово");
                    }
                }
                // если добавляем спецсимвол
                else if (specialSigns.Contains(item))
                {
                    if (!lexemes.Keys.Contains("Спецсимволы"))
                    {
                        lexemes.Add("Спецсимволы", new List<string> { item });
                        allLexemes.Add(item);
                        Console.WriteLine($"{item} \t| спецсимвол");
                    }
                    else
                    {
                        lexemes["Спецсимволы"].Add(item);
                        allLexemes.Add(item);
                        Console.WriteLine($"{item} \t| спецсимвол");
                    }
                }
                // если добавляем разделитель
                else if (delimiters.Contains(item))
                {
                    if (!lexemes.Keys.Contains("Разделители"))
                    {
                        lexemes.Add("Разделители", new List<string> { item });
                        allLexemes.Add(item);
                        Console.WriteLine($"{item} \t| разделитель");
                    }
                    else
                    {
                        lexemes["Разделители"].Add(item);
                        allLexemes.Add(item);
                        Console.WriteLine($"{item} \t| разделитель");
                    }
                }
                // если добавляем константу
                else if (item.All(char.IsDigit))
                {
                    if (!lexemes.Keys.Contains("Константы"))
                    {
                        lexemes.Add("Константы", new List<string> { item });
                        allLexemes.Add(item);
                        Console.WriteLine($"{item} \t| константа");
                    }
                    else
                    {
                        lexemes["Константы"].Add(item);
                        allLexemes.Add(item);
                        Console.WriteLine($"{item} \t| константа");
                    }
                }
                else
                {
                    if (!lexemes.Keys.Contains("Идентификаторы"))
                    {
                        lexemes.Add("Идентификаторы", new List<string> { item });
                        allLexemes.Add(item);
                        Console.WriteLine($"{item} \t| идентификатор");
                    }
                    else
                    {
                        lexemes["Идентификаторы"].Add(item);
                        allLexemes.Add(item);
                        Console.WriteLine($"{item} \t| идентификатор");
                    }
                }
            }


            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (string key in lexemes.Keys)
            {
                Console.Write($"{key}: ");
                foreach (string value in lexemes[key].Distinct())
                {
                    Console.Write($"{value}, ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();

            return allLexemes;
        }
    }
}
