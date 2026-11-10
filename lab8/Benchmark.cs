using System;
using System.Diagnostics;
using System.Globalization;

namespace OpamLab08;

static class Benchmark
{
    static int sink;

    public static void Run()
    {
        Table1();
        Table2();
    }

    static void Table1()
    {
        Console.WriteLine("=== Завдання 5, таблиця 1: середня кількість порівнянь (100 запитів) ===");
        Console.WriteLine("| n | Лінійний | Бінарний | Відношення | Теорія (n/2 і log2 n) |");
        Console.WriteLine("|---|---|---|---|---|");

        int[] sizes = { 1_000, 10_000, 100_000, 1_000_000 };
        foreach (int n in sizes)
        {
            int[] a = Program.MakeSorted(n, Program.N);
            var rnd = new Random(Program.N + 1);
            long linear = 0;
            long binary = 0;
            const int queries = 100;
            for (int i = 0; i < queries; i++)
            {
                int target = rnd.Next(0, 20 * Program.K + 1);
                Search.LinearSearch(a, target, Program.Cmp, out int c1);
                Search.BinarySearch(a, target, Program.Cmp, out int c2);
                linear += c1;
                binary += c2;
            }
            double avgLinear = (double)linear / queries;
            double avgBinary = (double)binary / queries;
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "| {0} | {1:F1} | {2:F1} | {3:F1} | {4:F0} і {5:F1} |",
                n, avgLinear, avgBinary, avgLinear / avgBinary, n / 2.0, Math.Log2(n)));
        }
        Console.WriteLine();
    }

    static double MinOfThree(Action action)
    {
        double best = double.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            double ms = sw.Elapsed.TotalMilliseconds;
            if (ms < best) best = ms;
        }
        return best;
    }

    static void RunLinear(int[] raw, int[] queries, int q)
    {
        for (int i = 0; i < q; i++)
        {
            sink += Search.LinearSearch(raw, queries[i], Program.Cmp, out _);
        }
    }

    static void RunSortThenBinary(int[] raw, int[] queries, int q)
    {
        int[] copy = (int[])raw.Clone();
        Array.Sort(copy, (x, y) => Program.Cmp(x, y));
        for (int i = 0; i < q; i++)
        {
            sink += Search.BinarySearch(copy, queries[i], Program.Cmp, out _);
        }
    }

    static void Table2()
    {
        Console.WriteLine("=== Завдання 5, таблиця 2: точка окупності (n = 100 000) ===");
        int[] raw = Program.MakeData(Program.Size, Program.N);
        var rnd = new Random(Program.N + 7);
        int[] queries = new int[100];
        for (int i = 0; i < queries.Length; i++) queries[i] = rnd.Next(0, 2 * 20 * Program.K + 1);

        // прогрівальний запуск
        RunLinear(raw, queries, 5);
        RunSortThenBinary(raw, queries, 5);

        Console.WriteLine("| q | Лінійно, мс | Сортування + бінарно, мс | Хто швидший |");
        Console.WriteLine("|---|---|---|---|");
        int[] counts = { 1, 5, 10, 20, 30, 50, 100 };
        foreach (int q in counts)
        {
            double linear = MinOfThree(() => RunLinear(raw, queries, q));
            double sorted = MinOfThree(() => RunSortThenBinary(raw, queries, q));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "| {0} | {1:F3} | {2:F3} | {3} |", q, linear, sorted,
                linear < sorted ? "лінійний" : "сортування + бінарний"));
        }
        Console.WriteLine($"(контрольна сума, щоб результати не відкинув компілятор: {sink})");
        Console.WriteLine();
    }
}