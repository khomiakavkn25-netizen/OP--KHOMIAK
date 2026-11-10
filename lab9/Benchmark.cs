using System;
using System.Diagnostics;
using System.Globalization;

namespace OpamLab09;

static class Benchmark
{
    static string Fmt(double ms)
    {
        return ms.ToString("F2", CultureInfo.InvariantCulture);
    }

    static double Time(int[] source, Action<int[]> sort)
    {
        double best = double.MaxValue;
        for (int run = 0; run < 3; run++)
        {
            int[] copy = (int[])source.Clone();
            var sw = Stopwatch.StartNew();
            sort(copy);
            sw.Stop();
            if (!Checks.IsSorted(copy, Program.Cmp)) Console.WriteLine("  ПОМИЛКА: результат не відсортований");
            double ms = sw.Elapsed.TotalMilliseconds;
            if (ms < best) best = ms;
        }
        return best;
    }

    public static int HybridThresholds(int[] data)
    {
        Console.WriteLine($"=== Завдання 3: гібрид, n = {data.Length}, випадковий вхід ===");
        Console.WriteLine("| Поріг | Час, мс | Порівнянь | Переставлянь |");
        Console.WriteLine("|---|---|---|---|");

        // прогрів
        int[] warm = Program.MakeRandom(5000);
        foreach (int t in Program.Thresholds)
        {
            Sorts.HybridSort((int[])warm.Clone(), Program.Cmp, t, out _, out _);
        }

        int bestThreshold = Program.Thresholds[0];
        double bestTime = double.MaxValue;
        foreach (int t in Program.Thresholds)
        {
            double ms = Time(data, a => Sorts.HybridSort(a, Program.Cmp, t, out _, out _));
            int[] copy = (int[])data.Clone();
            Sorts.HybridSort(copy, Program.Cmp, t, out int c, out int m);
            Console.WriteLine($"| {t} | {Fmt(ms)} | {c} | {m} |");
            if (ms < bestTime)
            {
                bestTime = ms;
                bestThreshold = t;
            }
        }
        Console.WriteLine($"Найкращий поріг: {bestThreshold} ({Fmt(bestTime)} мс)");
        Console.WriteLine();
        return bestThreshold;
    }

    public static void TimeTable()
    {
        Console.WriteLine("=== Завдання 5: час, випадковий вхід ===");
        Console.WriteLine($"| n | Простий (вибором), мс | Швидкий, мс | Додатковий (гібрид, поріг {Program.BestThreshold}), мс | Array.Sort, мс |");
        Console.WriteLine("|---|---|---|---|---|");

        int[] warm = Program.MakeRandom(3000);
        Sorts.SelectionSort((int[])warm.Clone(), Program.Cmp, out _, out _);
        Sorts.QuickSort((int[])warm.Clone(), Program.Cmp, out _, out _);
        Sorts.HybridSort((int[])warm.Clone(), Program.Cmp, Program.BestThreshold, out _, out _);
        Array.Sort((int[])warm.Clone());

        int[] sizes = { 1_000, 10_000, 100_000, 1_000_000 };
        foreach (int n in sizes)
        {
            int[] data = Program.MakeRandom(n);
            string simple = "—";
            if (n <= 100_000)
            {
                simple = Fmt(Time(data, a => Sorts.SelectionSort(a, Program.Cmp, out _, out _)));
            }
            string quick = Fmt(Time(data, a => Sorts.QuickSort(a, Program.Cmp, out _, out _)));
            string hybrid = Fmt(Time(data, a => Sorts.HybridSort(a, Program.Cmp, Program.BestThreshold, out _, out _)));
            string std = Fmt(Time(data, a => Array.Sort(a)));
            Console.WriteLine($"| {n} | {simple} | {quick} | {hybrid} | {std} |");
        }
        Console.WriteLine();
    }

    static double ProbeSimple(int n)
    {
        int[] data = Program.MakeRandom(n);
        var sw = Stopwatch.StartNew();
        Sorts.SelectionSort(data, Program.Cmp, out _, out _);
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds;
    }

    public static void SimpleLimit()
    {
        Console.WriteLine("=== Завдання 5: межа прийнятності простого сортування (1 секунда) ===");
        int lo = 1_000;
        int hi = lo;
        double timeAtHi = ProbeSimple(hi);
        while (timeAtHi <= 1000)
        {
            lo = hi;
            hi *= 2;
            timeAtHi = ProbeSimple(hi);
        }
        for (int step = 0; step < 6; step++)
        {
            int mid = (lo + hi) / 2;
            double t = ProbeSimple(mid);
            if (t <= 1000) lo = mid;
            else
            {
                hi = mid;
                timeAtHi = t;
            }
        }
        Console.WriteLine($"  ще вкладається в 1 с: n ≈ {lo}; перестає: n ≈ {hi} ({Fmt(timeAtHi)} мс)");
        Console.WriteLine();
    }
}