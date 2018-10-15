using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewReversedPolish
{
    static class Program
    {
        static void Main(string[] args)
        {
            ConsoleKey key = ConsoleKey.C;
            while (key == ConsoleKey.C)
            {
                Console.WriteLine("Enter equation:");
                string equation = Console.ReadLine();
                string rp = ReversedPolish(equation);
                Console.WriteLine(rp);
                int answer = GetAnswer(rp);
                Console.WriteLine(answer);
                Console.WriteLine("Press C to do another one or press any other key to close.");
                key = Console.ReadKey().Key;
                Console.WriteLine();
            }
        }
        public enum Operator
        {
            OPEN_BRACKET,
            CLOSE_BRACKET,
            POWER,
            DIVIDE,
            MULTIPLY,
            ADD,
            SUBTRACT
        }
        public static Dictionary<char, Operator> Char_To_Operator = new Dictionary<char, Operator>()
        {
            {'(',Operator.OPEN_BRACKET},
            {')',Operator.CLOSE_BRACKET},
            {'^',Operator.POWER},
            {'/',Operator.DIVIDE},
            {'*',Operator.MULTIPLY},
            {'+',Operator.ADD},
            {'-',Operator.SUBTRACT}
        };
        public static Dictionary<Operator, int> Quantities = new Dictionary<Operator, int>()
        {
            {Operator.POWER,2},
            {Operator.DIVIDE,2},
            {Operator.MULTIPLY,2},
            {Operator.ADD,2},
            {Operator.SUBTRACT,2}
        };
        public static string ReversedPolish(string equation)
        {
            if (equation.CharCount('(') != equation.CharCount(')'))
            {
                Console.WriteLine("Quantity of opening and closing brackets inconsistent.");
                return "";
            }
            Stack<char> digits = new Stack<char>();
            Stack<char> operators = new Stack<char>();
            Stack<int> numbers = new Stack<int>();
            StringBuilder sb = new StringBuilder();
            Func<char, bool> isOperator = (c) =>
            {
                return Char_To_Operator.ContainsKey(c);
            };
            Func<string> popDigits = () =>
            {
                string number = "";
                while (digits.Count > 0)
                {
                    number = digits.Pop() + number;
                }
                return number;
            };
            Func<Func<char, bool>, string> popOperators = (condition) =>
            {
                StringBuilder addition = new StringBuilder();
                //string addition = "";
                char op = ')';
                List<char> ops = new List<char>();
                while (condition(op))
                {
                    op = operators.Pop();
                    if (op != '(' && op != ')')
                    {
                        ops.Add(op);
                    }
                }
                ops.Reverse();
                //Comparison<char> comp = (a, b) =>
                //{
                //    int op1 = (int)Char_To_Operator[a];
                //    int op2 = (int)Char_To_Operator[b];
                //    return op1 > op2 ? 1 : -1;
                //};
                //ops.Sort(comp);
                foreach (char o in ops)
                {
                    addition.Append(o + " ");
                    //addition += o + " ";
                }
                return addition.ToString();
            };
            Action<Func<char,bool>> appendNumbersAndOperators = (condition) =>
            {
                char opp2 = 'C';
                while (condition(opp2))
                {
                    opp2 = operators.Pop();
                    int quantity = Quantities[Char_To_Operator[opp2]];
                    string addition = "";
                    for (int j = 0; j < quantity; j++)
                    {
                        if (numbers.Count > 0)
                        {
                            addition = numbers.Pop() + " " + addition;
                            //sb.Append(numbers.Pop() + " ");
                        }
                    }

                    if (opp2 != '(' && opp2 != ')')
                    {
                        addition = opp2 + " " + addition;
                        //sb.Append(opp + " ");
                    }
                    sb.Append(addition);
                }
                sb.Append(" ");
            };
            for (int i = 0; i < equation.Length; i++)
            {
                if (Char.IsDigit(equation[i]))
                {
                    digits.Push(equation[i]);
                }
                else
                {
                    int num;
                    if (int.TryParse(popDigits(), out num))
                    {
                        numbers.Push(num);
                        //sb.Append(popDigits() + " ");
                    }
                }
                if (isOperator(equation[i]))
                {
                    operators.Push(equation[i]);
                    if (equation[i] == ')')
                    {
                        //sb.Append(popOperators((c) => { return c != '(' && operators.Count > 0; }) + " ");
                        appendNumbersAndOperators((c) => { return c != '(' && operators.Count > 0; });
                    }
                }
            }
            sb.Append(popDigits() + " ");
            appendNumbersAndOperators((c) => { return operators.Count > 0; });
            //sb.Append(popOperators((c) => { return operators.Count > 0; }));
            return sb.ToString();
        }
        public static int GetAnswer(string reversedPolish)
        {
            List<string> segments = reversedPolish.Split(' ').ToList();
            for (int i = 0; i < segments.Count; i++)
            {
                if (String.IsNullOrEmpty(segments[i]) || String.IsNullOrWhiteSpace(segments[i]))
                {
                    segments.RemoveAt(i);
                    i--;
                }
            }

            Stack<int> numbers = new Stack<int>();
            Func<int> getNum = () =>
            {
                if (numbers.Count > 0)
                {
                    return numbers.Pop();
                }
                return 0;
            };
            Func<Operator, int> compute = (op) =>
            {
                int a;
                int b;
                switch (op)
                {
                    case Operator.POWER:
                        int p = getNum();
                        int n = getNum();
                        return (int)Math.Pow(n, p);
                    case Operator.DIVIDE:
                        b = getNum();
                        a = getNum();
                        return b != 0 ? a / b : 0;
                    case Operator.MULTIPLY:
                        b = getNum();
                        a = getNum();
                        return a * b;
                    case Operator.ADD:
                        b = getNum();
                        a = getNum();
                        return a + b;
                    case Operator.SUBTRACT:
                        b = getNum();
                        a = getNum();
                        return a - b;
                }
                return 0;
            };
            foreach (string segment in segments)
            {
                int num;
                if (int.TryParse(segment,out num))
                {
                    numbers.Push(num);
                }
                else if (Char_To_Operator.ContainsKey(segment[0]))
                {
                    int result = compute(Char_To_Operator[segment[0]]);
                    numbers.Push(result);
                }
            }
            if (numbers.Count > 0)
            {
                return numbers.Pop();
            }
            return 0;
        }
        public static int CharCount(this string s, char c)
        {
            int count = 0;
            foreach (char cha in s)
            {
                if (cha == c)
                {
                    count++;
                }
            }
            return count;
        }
    }
}