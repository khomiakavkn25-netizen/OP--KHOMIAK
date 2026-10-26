using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OpamLab04;

internal static class Program
{
    private const int N = 27;
    private const int K = 1;

    private const int AddCount = 100_000;
    private const int ContainsListSize = 100_000;
    private const int ContainsSearches = 1_000;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        List<int> main = BuildMainList();
        PrintList("Основний список", main);
        Console.WriteLine();

        RunTask1(main);
        Console.WriteLine();
        RunTask2(main);
        Console.WriteLine();
        RunTask3();
        Console.WriteLine();
        RunTask4(main);
        Console.WriteLine();
        RunTask5();
        Console.WriteLine();
        RunEdgeCases();
    }

    private static List<int> BuildMainList()
    {
        var rnd = new Random(N);
        var list = new List<int>(20);
        for (int i = 0; i < 20; i++)
            list.Add(rnd.Next(-9, 10));
        return list;
    }

    private static void PrintList(string title, List<int> list)
    {
        Console.WriteLine($"{title}: [{string.Join(", ", list)}] (Count = {list.Count})");
    }

    private static void RunTask1(List<int> source)
    {
        Console.WriteLine("=== Завдання 1: побудова за критерієм (> K % 7) ===");
        List<int> result = BuildGreaterThan(source, K % 7);
        PrintList("Результат", result);
    }

    private static List<int> BuildGreaterThan(List<int> source, int threshold)
    {
        var result = new List<int>();
        for (int i = 0; i < source.Count; i++)
        {
            if (source[i] > threshold)
                result.Add(source[i]);
        }
        result.TrimExcess();
        return result;
    }

    private static void RunTask2(List<int> source)
    {
        Console.WriteLine("=== Завдання 2: вилучення від'ємних ===");

        var wrong = new List<int>(source);
        RemoveNegativesForwardInPlace(wrong);
        PrintList("1) Хибний (цикл з початку)", wrong);
        List<int> missed = CollectNegatives(wrong);
        PrintList("   Пропущені від'ємні", missed);

        var backward = new List<int>(source);
        RemoveNegativesBackwardInPlace(backward);
        PrintList("2) Цикл з кінця", backward);

        List<int> built = BuildWithoutNegatives(source);
        PrintList("3) Новий список", built);

        Console.WriteLine($"Варіанти 2 і 3 збігаються: {AreEqual(backward, built)}");
    }

    private static void RemoveNegativesForwardInPlace(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] < 0)
                list.RemoveAt(i);
        }
    }

    private static void RemoveNegativesBackwardInPlace(List<int> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] < 0)
                list.RemoveAt(i);
        }
    }

    private static List<int> BuildWithoutNegatives(List<int> source)
    {
        var result = new List<int>();
        for (int i = 0; i < source.Count; i++)
        {
            if (source[i] >= 0)
                result.Add(source[i]);
        }
        return result;
    }

    private static List<int> CollectNegatives(List<int> source)
    {
        var result = new List<int>();
        for (int i = 0; i < source.Count; i++)
        {
            if (source[i] < 0)
                result.Add(source[i]);
        }
        return result;
    }

    private static bool AreEqual(List<int> x, List<int> y)
    {
        if (x.Count != y.Count)
            return false;
        for (int i = 0; i < x.Count; i++)
        {
            if (x[i] != y[i])
                return false;
        }
        return true;
    }

    private static void RunTask3()
    {
        Console.WriteLine("=== Завдання 3: злиття ===");
        List<int> first = BuildSorted(10 + K % 5, N);
        List<int> second = BuildSorted(6 + K % 4, N + 100);
        PrintList("Перший", first);
        PrintList("Другий", second);

        List<int> merged = Merge(first, second);
        PrintList("Злитий", merged);
        Console.WriteLine($"Впорядкований: {IsSorted(merged)}");
    }

    private static List<int> BuildSorted(int length, int seed)
    {
        var rnd = new Random(seed);
        var list = new List<int>(length);
        if (length == 0)
            return list;

        int value = rnd.Next(1, 10);
        list.Add(value);
        for (int i = 1; i < length; i++)
        {
            value += rnd.Next(1, 10);
            list.Add(value);
        }
        return list;
    }

    private static List<int> Merge(List<int> x, List<int> y)
    {
        var result = new List<int>(x.Count + y.Count);
        int i = 0;
        int j = 0;

        while (i < x.Count && j < y.Count)
        {
            if (x[i] <= y[j])
            {
                result.Add(x[i]);
                i++;
            }
            else
            {
                result.Add(y[j]);
                j++;
            }
        }

        while (i < x.Count)
        {
            result.Add(x[i]);
            i++;
        }
        while (j < y.Count)
        {
            result.Add(y[j]);
            j++;
        }

        return result;
    }

    private static bool IsSorted(List<int> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] < list[i - 1])
                return false;
        }
        return true;
    }

    private static void RunTask4(List<int> source)
    {
        Console.WriteLine("=== Завдання 4: розділення за парністю індексу ===");
        var (even, odd) = SplitByIndexParity(source);
        PrintList("На парних індексах", even);
        PrintList("На непарних індексах", odd);
        Console.WriteLine($"Сума Count збігається з вихідним: {even.Count + odd.Count == source.Count}");
    }

    private static (List<int> Even, List<int> Odd) SplitByIndexParity(List<int> source)
    {
        var even = new List<int>((source.Count + 1) / 2);
        var odd = new List<int>(source.Count / 2);
        for (int i = 0; i < source.Count; i++)
        {
            if (i % 2 == 0)
                even.Add(source[i]);
            else
                odd.Add(source[i]);
        }
        return (even, odd);
    }

    private static void RunTask5()
    {
        Console.WriteLine("=== Завдання 5: замір часу ===");
#if DEBUG
        Console.WriteLine("УВАГА: запущено Debug. Для коректних результатів: dotnet run -c Release");
#endif

        MeasureAdd(1000, false);
        MeasureAdd(1000, true);
        MeasureInsertFront(1000);
        MeasureContains(1000, 1000);

        double add = MeasureAdd(AddCount, false);
        double addCap = MeasureAdd(AddCount, true);
        double insert = MeasureInsertFront(AddCount);
        double contains = MeasureContains(ContainsListSize, ContainsSearches);

        Console.WriteLine($"{AddCount} x Add(x), без місткості:        {add:F2} мс");
        Console.WriteLine($"{AddCount} x Add(x), з місткістю:          {addCap:F2} мс");
        Console.WriteLine($"{AddCount} x Insert(0, x):                 {insert:F2} мс");
        Console.WriteLine($"{ContainsSearches} x Contains у списку з {ContainsListSize}: {contains:F2} мс");
        if (add > 0)
            Console.WriteLine($"Insert(0, x) / Add(x) = {insert / add:F1} разів");
    }

    private static double MeasureAdd(int count, bool preallocate)
    {
        List<int> list = preallocate ? new List<int>(count) : new List<int>();
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < count; i++)
            list.Add(i);
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds;
    }

    private static double MeasureInsertFront(int count)
    {
        var list = new List<int>();
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < count; i++)
            list.Insert(0, i);
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds;
    }

    private static double MeasureContains(int listSize, int searches)
    {
        var list = new List<int>(listSize);
        for (int i = 0; i < listSize; i++)
            list.Add(i);

        bool found = false;
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < searches; i++)
            found |= list.Contains(-1);
        sw.Stop();

        if (found)
            Console.WriteLine("Неможливо: -1 знайдено.");
        return sw.Elapsed.TotalMilliseconds;
    }

    private static void RunEdgeCases()
    {
        Console.WriteLine("=== Крайові випадки ===");

        CheckCase("порожній", new List<int>());
        CheckCase("один елемент {5}", new List<int> { 5 });
        CheckCase("нічого не підходить {1, 0, 1}", new List<int> { 1, 0, 1 });
        CheckCase("усе підходить під завдання 1 {2, 3, 4}", new List<int> { 2, 3, 4 });
        CheckCase("усе підходить під завдання 2 {-1, -2, -3}", new List<int> { -1, -2, -3 });

        Console.WriteLine("-- злиття з порожнім списком --");
        List<int> merged = Merge(new List<int>(), new List<int> { 1, 2, 3 });
        PrintList("Merge({}, {1, 2, 3})", merged);
        Console.WriteLine($"Впорядкований: {IsSorted(merged)}");
    }

    private static void CheckCase(string name, List<int> list)
    {
        Console.WriteLine($"-- {name} --");
        PrintList("Завд. 1", BuildGreaterThan(list, K % 7));

        var forward = new List<int>(list);
        RemoveNegativesForwardInPlace(forward);
        PrintList("Завд. 2 (хибний)", forward);

        var backward = new List<int>(list);
        RemoveNegativesBackwardInPlace(backward);
        PrintList("Завд. 2 (з кінця)", backward);

        PrintList("Завд. 2 (новий)", BuildWithoutNegatives(list));

        var (even, odd) = SplitByIndexParity(list);
        PrintList("Завд. 4 (парні)", even);
        PrintList("Завд. 4 (непарні)", odd);
        Console.WriteLine($"Сума Count: {even.Count + odd.Count == list.Count}");
    }
}