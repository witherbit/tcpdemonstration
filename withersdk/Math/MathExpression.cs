using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace withersdk.Math
{
    public static class MathExpression
    {
        private static double Evaluator(Queue<object> str)  //с помощью операций .Pop() вычисляет выражение по словам
        {
            Stack<double> Stack = new Stack<double>();

            for (int TokenCounter = 0; TokenCounter < str.Count; TokenCounter++)
            {
                #region numbers
                if (str.ElementAt(TokenCounter) is double)
                {
                    Stack.Push((double)str.ElementAt(TokenCounter));
                }
                #endregion

                #region operators
                else
                {
                    object current = str.ElementAt(TokenCounter);
                    double temp;
                    if (Stack.Count < 2) throw new Exception("Maldeformed expression");

                    temp = (double)Stack.Pop();
                    if (current.Equals(Symbols.ADD)) temp += Stack.Pop();
                    else if (current.Equals(Symbols.SUB)) temp = Stack.Pop() - temp;
                    else if (current.Equals(Symbols.MUL)) temp *= Stack.Pop();
                    else if (current.Equals(Symbols.DIV)) { if (temp == 0.0) throw new DivideByZeroException("Division by zero in expression"); else temp = Stack.Pop() / temp; }
                    else if (current.Equals(Symbols.MOD)) { if (temp == 0.0) throw new DivideByZeroException("Division by zero in expression"); else temp = Stack.Pop() % temp; }
                    else temp = System.Math.Pow(Stack.Pop(), temp);

                    Stack.Push(temp);
                }
                #endregion
            }

            if (Stack.Count > 1) throw new Exception("Maldeformed expression");

            return (double)Stack.Pop();
        }

        private static Queue<object> ToWords(Queue<object> str) //Преобразует элементы выражения в "слова"
        {
            Stack<object> Stack = new Stack<object>();
            Queue<object> Queue = new Queue<object>();

            for (int TokenCounter = 0; TokenCounter < str.Count; TokenCounter++)
            {
                #region Numbers
                if (str.ElementAt(TokenCounter) is double) Queue.Enqueue(str.ElementAt(TokenCounter));
                #endregion

                #region Left Paren
                else if (str.ElementAt(TokenCounter).Equals(Symbols.LEFT_PAREN))
                {
                    if (TokenCounter >= 1 && (str.ElementAt(TokenCounter - 1) is double || str.ElementAt(TokenCounter - 1).Equals(Symbols.RIGHT_PAREN)))
                    {
                        List<object> list = new List<object>(str);
                        list.Insert(TokenCounter, Symbols.MUL);
                        str = new Queue<object>(list);
                        TokenCounter--;
                    }
                    else Stack.Push(Symbols.LEFT_PAREN);
                }
                #endregion
                #region Right Paren
                else if (str.ElementAt(TokenCounter).Equals(Symbols.RIGHT_PAREN))
                {
                    bool IsParenMatched = false;

                    while (Stack.Count > 0 && !IsParenMatched)
                    {
                        if (Stack.Peek().Equals(Symbols.LEFT_PAREN))
                        {
                            Stack.Pop();
                            IsParenMatched = true;
                        }
                        else Queue.Enqueue(Stack.Pop());
                    }

                    if (!IsParenMatched) throw new Exception("Parentheses mismatch");
                }
                #endregion

                #region characters
                else
                {
                    object current = str.ElementAt(TokenCounter);

                    if (current.Equals(Symbols.MOD) || current.Equals(Symbols.MUL) || current.Equals(Symbols.DIV))
                    {
                        while (Stack.Count > 0 &&
                            !Stack.Peek().Equals(Symbols.LEFT_PAREN) &&
                            !(Stack.Peek().Equals(Symbols.ADD) || Stack.Peek().Equals(Symbols.SUB)))
                        {
                            Queue.Enqueue(Stack.Pop());
                        }

                    }

                    if (current.Equals(Symbols.ADD) || current.Equals(Symbols.SUB))
                    {
                        while (Stack.Count > 0 &&
                            !Stack.Peek().Equals(Symbols.LEFT_PAREN))
                        {
                            Queue.Enqueue(Stack.Pop());
                        }
                    }

                    Stack.Push(current);
                }
                #endregion
            }
            while (Stack.Count > 0)
            {
                if (Stack.Peek().Equals(Symbols.LEFT_PAREN)) throw new Exception("Parentheses mismatch");
                Queue.Enqueue(Stack.Pop());
            }


            return Queue;
        }

        private static Queue<object> Parser(string str) //парсит входящее выражение
        {
            if (string.IsNullOrWhiteSpace(str)) throw new ArgumentException("Empty string");

            Queue<object> Queue = new Queue<object>();
            StringBuilder Stringbuilder = new StringBuilder(System.Text.RegularExpressions.Regex.Replace(str, @"\s\s+", " "));

            for (int TokenCounter = 0; TokenCounter < Stringbuilder.Length; TokenCounter++)
            {
                #region numbers
                if (Stringbuilder[TokenCounter] >= '0' && Stringbuilder[TokenCounter] <= '9')
                {
                    string ParseDigits = Stringbuilder[TokenCounter].ToString();
                    bool HasDecimal = false;
                    if (TokenCounter + 1 < Stringbuilder.Length)
                    {
                        for (; Stringbuilder[TokenCounter + 1] == '.' || Stringbuilder[TokenCounter + 1] >= '0' && Stringbuilder[TokenCounter + 1] <= '9';)
                        {
                            TokenCounter++;
                            if (Stringbuilder[TokenCounter] == '.' && !HasDecimal)
                            {
                                ParseDigits += '.';
                                HasDecimal = true;
                            }
                            else if (Stringbuilder[TokenCounter] == '.' && HasDecimal) throw new Exception("Unmatched decimal point at position '" + TokenCounter + "'");
                            else ParseDigits += Stringbuilder[TokenCounter];

                            if (TokenCounter + 1 >= Stringbuilder.Length) break;
                        }
                    }
                    Queue.Enqueue(double.Parse(ParseDigits, CultureInfo.InvariantCulture));
                }
                #endregion
                //parse symbols
                #region symbols
                else
                {
                    switch (Stringbuilder[TokenCounter])
                    {
                        case Symbols.LEFT_PAREN:
                            Queue.Enqueue(Symbols.LEFT_PAREN);
                            continue;
                        case Symbols.RIGHT_PAREN:
                            Queue.Enqueue(Symbols.RIGHT_PAREN);
                            continue;
                        case Symbols.MUL:
                            Queue.Enqueue(Symbols.MUL);
                            continue;
                        case Symbols.EXP:
                            Queue.Enqueue(Symbols.EXP);
                            continue;
                        case Symbols.DIV:
                            Queue.Enqueue(Symbols.DIV);
                            continue;
                        case Symbols.MOD:
                            Queue.Enqueue(Symbols.MOD);
                            continue;
                        case Symbols.ADD:
                            Queue.Enqueue(Symbols.ADD);
                            continue;
                        case Symbols.SUB:
                            Queue.Enqueue(Symbols.SUB);
                            continue;
                    }
                    if (char.IsWhiteSpace(Stringbuilder[TokenCounter])) continue;
                    throw new Exception("Unknown symbol: '" + Stringbuilder[TokenCounter] + "'");
                }
                #endregion
            }


            //fix negatives, if any
            for (int i = 1; i < Queue.Count; i++)
            {
                //two tokens in a row and the second token being '-' means it a negative number
                bool negate = false;
                //if we're at the beginning
                if (i == 1)
                {
                    if (Queue.ElementAt(i - 1).Equals(Symbols.SUB)) negate = true;
                }
                //otherwise
                else
                {
                    if (Queue.ElementAt(i - 1).Equals(Symbols.SUB) && !(Queue.ElementAt(i - 2) is double)) negate = true;
                }


                if (negate)
                {
                    if (Queue.ElementAt(i) is double)
                    {
                        //negate the double
                        List<object> list = new List<object>(Queue);
                        list[i] = -(double)list[i];
                        list.RemoveAt(i - 1);
                        Queue = new Queue<object>(list);
                    }
                    else
                    {
                        //place parentheses around expression and multiply by -1
                        int iterator;
                        int paren_count = 0;
                        for (iterator = i; iterator < Queue.Count && paren_count >= 0; iterator++)
                        {
                            if (Queue.ElementAt(i).Equals(Symbols.LEFT_PAREN)) paren_count++;
                            else if (Queue.ElementAt(i).Equals(Symbols.RIGHT_PAREN)) paren_count--;
                        }

                        List<object> list = new List<object>(Queue);

                        //insert (-1 * ( ... ) ) to avoid any ambiguity
                        list.RemoveAt(i - 1); //remove '-'
                        list.Insert(i - 1, Symbols.LEFT_PAREN);
                        list.Insert(i, -1.0);
                        list.Insert(i + 1, Symbols.MUL);
                        list.Insert(i + 2, Symbols.LEFT_PAREN);
                        list.Insert(iterator + 3, Symbols.RIGHT_PAREN);
                        list.Insert(iterator + 4, Symbols.RIGHT_PAREN);

                        Queue = new Queue<object>(list);
                    }
                }
            }

            //handle implicit multiplication 
            //eg, 2sin(2pi)
            List<object> List = new List<object>(Queue);
            for (int i = 1; i < List.Count; i++)
            {
                if (List.ElementAt(i) is string &&

                    List.ElementAt(i - 1) is double)
                {
                    List.Insert(i, Symbols.MUL);
                }
            }

            //convert constants to their actual values

            return new Queue<object>(List);
        }

        public static double Calculate(this string str) //считает выражение
        {
            try
            {
                return Evaluator(ToWords(Parser(str)));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    internal static class Symbols
    {
        public const char LEFT_PAREN = '(';
        public const char RIGHT_PAREN = ')';
        public const char EXP = '^';
        public const char MUL = '*';
        public const char DIV = '/';
        public const char MOD = '%';
        public const char ADD = '+';
        public const char SUB = '-';
    }
}
