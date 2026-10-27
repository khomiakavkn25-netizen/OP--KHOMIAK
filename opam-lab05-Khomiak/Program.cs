using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OpamLab05;

internal static class Program
{
    private const int N = 27;
    private const int K = 1;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        RunTask1();
        Console.WriteLine();
        RunTask2();
        Console.WriteLine();
        RunTask3();
        Console.WriteLine();
        RunTask4();
        Console.WriteLine();
        RunEdgeCases();
    }

    private static string[] BracketTests()
    {
        return new[]
        {
            "(a[b]{c})",
            "([)]",
            ")(",
            "((",
            "",
            new string('(', K) + new string(')', K - 1)
        };
    }

    private static bool IsOpen(char c)
    {
        return c == '(' || c == '[' || c == '{';
    }

    private static bool IsClose(char c)
    {
        return c == ')' || c == ']' || c == '}';
    }

    private static bool Matches(char open, char close)
    {
        return (open == '(' && close == ')')
            || (open == '[' && close == ']')
            || (open == '{' && close == '}');
    }

    private static void RunTask1()
    {
        Console.WriteLine("=== Завдання 1: дужки ===");
        foreach (string text in BracketTests())
        {
            var (balanced, errorIndex) = CheckBrackets(text);
            Console.WriteLine($"\"{text}\" -> збалансований: {balanced}, індекс помилки: {errorIndex}");
        }
    }

    private static (bool Balanced, int ErrorIndex) CheckBrackets(string text)
    {
        var chars = new Stack<char>();
        var indexes = new Stack<int>();

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (IsOpen(c))
            {
                chars.Push(c);
                indexes.Push(i);
            }
            else if (IsClose(c))
            {
                if (chars.Count == 0)
                    return (false, i);

                char open = chars.Pop();
                indexes.Pop();
                if (!Matches(open, c))
                    return (false, i);
            }
        }

        if (chars.Count > 0)
        {
            int first = -1;
            while (indexes.Count > 0)
                first = indexes.Pop();
            return (false, first);
        }

        return (true, -1);
    }

    private static void RunTask2()
    {
        Console.WriteLine("=== Завдання 2: зворотний польський запис ===");
        string[] expressions =
        {
            "3 4 + 2 *",
            "5 3 -",
            "8 2 /",
            "2 3 4 * +",
            "5 0 /",
            "8 6 - 2 / 5 min 1 max",
            "1 x +",
            "1 2",
            "1 +"
        };

        foreach (string expression in expressions)
            Console.WriteLine($"\"{expression}\" -> {FormatRpn(EvaluateRpn(expression))}");
    }

    private static string FormatRpn((bool Ok, double Value, string Message) result)
    {
        if (!result.Ok)
            return "помилка: " + result.Message;
        return result.Value.ToString(CultureInfo.InvariantCulture);
    }

    private static bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/"
            || token == "min" || token == "max";
    }

    private static double Apply(string op, double left, double right)
    {
        switch (op)
        {
            case "+": return left + right;
            case "-": return left - right;
            case "*": return left * right;
            case "/": return left / right;
            case "min": return left < right ? left : right;
            default: return left > right ? left : right;
        }
    }

    private static (bool Ok, double Value, string Message) EvaluateRpn(string expression)
    {
        string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0)
            return (false, 0, "Порожній вираз.");

        var stack = new Stack<double>();
        foreach (string token in tokens)
        {
            if (IsOperator(token))
            {
                if (stack.Count < 2)
                    return (false, 0, $"Не вистачає чисел для операції '{token}'.");

                double right = stack.Pop();
                double left = stack.Pop();
                if (token == "/" && right == 0)
                    return (false, 0, "Ділення на нуль.");

                stack.Push(Apply(token, left, right));
            }
            else if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
            {
                stack.Push(number);
            }
            else
            {
                return (false, 0, $"Невідомий токен '{token}'.");
            }
        }

        if (stack.Count != 1)
            return (false, 0, "Забагато чисел у виразі.");

        return (true, stack.Pop(), "");
    }

    private static void RunTask3()
    {
        int count = 8 + K % 5;
        Console.WriteLine($"=== Завдання 3: дві черги, заявок: {count} ===");
        int[] weights = BuildWeights(count);
        Console.WriteLine("Ваги: " + string.Join(", ", weights));

        var (records, maxLength) = Simulate(weights);
        foreach (var r in records)
            Console.WriteLine($"Заявка {r.Number}: вага {r.Weight}, початок {r.Start}, завершення {r.Finish}");
        Console.WriteLine($"Максимальна довжина черги: {maxLength}");
    }

    private static int[] BuildWeights(int count)
    {
        var rnd = new Random(N);
        int[] weights = new int[count];
        for (int i = 0; i < count; i++)
            weights[i] = rnd.Next(1, 10);
        return weights;
    }

    private static (List<(int Number, int Weight, int Start, int Finish)> Records, int MaxLength) Simulate(int[] weights)
    {
        var normal = new Queue<int>();
        var priority = new Queue<int>();
        for (int number = 1; number <= weights.Length; number++)
        {
            if (number % 3 == 0)
                priority.Enqueue(number);
            else
                normal.Enqueue(number);
        }

        var records = new List<(int Number, int Weight, int Start, int Finish)>(weights.Length);
        int maxLength = normal.Count + priority.Count;
        int time = 0;

        while (normal.Count > 0 || priority.Count > 0)
        {
            int total = normal.Count + priority.Count;
            if (total > maxLength)
                maxLength = total;

            int number;
            if (priority.Count > 0)
                number = priority.Dequeue();
            else
                number = normal.Dequeue();

            int weight = weights[number - 1];
            records.Add((number, weight, time, time + weight));
            time += weight;
        }

        return (records, maxLength);
    }

    private static void RunTask4()
    {
        Console.WriteLine("=== Завдання 4: власний стек ===");
        var stack = new IntStack(K % 4 + 2);
        Console.WriteLine($"Початково: Count = {stack.Count}, Capacity = {stack.Capacity}");

        int pushes = K % 4 + 3;
        for (int i = 1; i <= pushes; i++)
        {
            stack.Push(i);
            Console.WriteLine($"Push({i}): Count = {stack.Count}, Capacity = {stack.Capacity}");
        }

        Console.WriteLine($"Peek: {stack.Peek()}, Count після Peek: {stack.Count}");

        Console.WriteLine("-- завдання 1 на власному стеку --");
        bool allSame = true;
        foreach (string text in BracketTests())
        {
            var standard = CheckBrackets(text);
            var own = CheckBracketsOwnStack(text);
            bool same = standard == own;
            allSame = allSame && same;
            Console.WriteLine($"\"{text}\" -> {own.Balanced}, {own.ErrorIndex}, збігається зі Stack<T>: {same}");
        }
        Console.WriteLine($"Усі результати збігаються: {allSame}");
    }

    private static (bool Balanced, int ErrorIndex) CheckBracketsOwnStack(string text)
    {
        var chars = new IntStack(4);
        var indexes = new IntStack(4);

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (IsOpen(c))
            {
                chars.Push(c);
                indexes.Push(i);
            }
            else if (IsClose(c))
            {
                if (chars.Count == 0)
                    return (false, i);

                char open = (char)chars.Pop();
                indexes.Pop();
                if (!Matches(open, c))
                    return (false, i);
            }
        }

        if (chars.Count > 0)
        {
            int first = -1;
            while (indexes.Count > 0)
                first = indexes.Pop();
            return (false, first);
        }

        return (true, -1);
    }

    private static void RunEdgeCases()
    {
        Console.WriteLine("=== Крайові випадки ===");

        var emptyBrackets = CheckBrackets("");
        Console.WriteLine($"Порожній рядок, завд. 1: {emptyBrackets.Balanced}, {emptyBrackets.ErrorIndex}");
        Console.WriteLine($"Порожній рядок, завд. 2: {FormatRpn(EvaluateRpn(""))}");

        var noBrackets = CheckBrackets("abc");
        Console.WriteLine($"Рядок без дужок \"abc\": {noBrackets.Balanced}, {noBrackets.ErrorIndex}");

        Console.WriteLine($"Одне число \"42\": {FormatRpn(EvaluateRpn("42"))}");
        Console.WriteLine($"Ділення на нуль \"5 0 /\": {FormatRpn(EvaluateRpn("5 0 /"))}");

        var (records, maxLength) = Simulate(new int[0]);
        Console.WriteLine($"Жодної заявки: записів {records.Count}, максимальна довжина черги {maxLength}");

        try
        {
            new IntStack(2).Pop();
            Console.WriteLine("Pop на порожньому стеку: виняток не виник (помилка).");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Pop на порожньому стеку: {ex.Message}");
        }
    }
}