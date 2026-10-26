using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpamLab03;

internal static class Program
{
    private const int N = 27;
    private const int K = 1;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        RunTask1();
        Console.WriteLine();

        var rnd = new Random(N);
        int[] array = new int[16];
        for (int i = 0; i < array.Length; i++)
            array[i] = rnd.Next(-20, 21); // [-20; 20]

        RunTask2And5(array);
        Console.WriteLine();

        RunTask3();
        Console.WriteLine();

        RunTask4();
        Console.WriteLine();

        RunEdgeCases();
    }


    private static void RunTask1()
    {
        Console.WriteLine("=== Завдання 1: обхід каталогів ===");
        Console.Write("Введіть шлях до папки: ");
        string? path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            Console.WriteLine("Папку не знайдено.");
            return;
        }

        var (count, size, skipped) = CountCsFiles(path);
        Console.WriteLine($"Файлів .cs: {count}");
        Console.WriteLine($"Сумарний розмір: {size} байт");
        Console.WriteLine($"Пропущено недоступних папок/файлів: {skipped}");
    }

    private static (int Count, long Size, int Skipped) CountCsFiles(string path)
    {
        string[] files;
        string[] dirs;
        try
        {
            files = Directory.GetFiles(path);
            dirs = Directory.GetDirectories(path);
        }
        catch (UnauthorizedAccessException)
        {
            return (0, 0, 1);
        }
        catch (IOException)
        {
            return (0, 0, 1);
        }

        int count = 0;
        long size = 0;
        int skipped = 0;

        foreach (string file in files)
        {
            if (!string.Equals(Path.GetExtension(file), ".cs", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                size += new FileInfo(file).Length;
                count++;
            }
            catch (UnauthorizedAccessException)
            {
                skipped++;
            }
            catch (IOException)
            {
                skipped++;
            }
        }

        foreach (string dir in dirs)
        {
            try
            {
                if ((File.GetAttributes(dir) & FileAttributes.ReparsePoint) != 0)
                    continue;
            }
            catch (UnauthorizedAccessException)
            {
                skipped++;
                continue;
            }
            catch (IOException)
            {
                skipped++;
                continue;
            }

            var sub = CountCsFiles(dir);
            count += sub.Count;
            size += sub.Size;
            skipped += sub.Skipped;
        }

        return (count, size, skipped);
    }


    private static void RunTask2And5(int[] array)
    {
        Console.WriteLine("=== Завдання 2: максимум поділом навпіл ===");
        Console.WriteLine("Масив: " + string.Join(", ", array));

        int maxDepth = 0;
        int recursive = Solve(array, 0, array.Length - 1, 0, ref maxDepth);
        Console.WriteLine($"Максимум (рекурсія): {recursive}");
        Console.WriteLine($"Максимальна глибина рекурсії: {maxDepth} (log2(16) = 4)");

        Console.WriteLine();
        Console.WriteLine("=== Завдання 5: ітеративна версія ===");
        int iterative = MaxIterative(array);
        Console.WriteLine($"Максимум (цикл): {iterative}");
        Console.WriteLine(recursive == iterative ? "Результати збігаються." : "РЕЗУЛЬТАТИ РІЗНІ!");
    }

    private static int Solve(int[] a, int lo, int hi)
    {
        int depth = 0;
        return Solve(a, lo, hi, 0, ref depth);
    }

    private static int Solve(int[] a, int lo, int hi, int depth, ref int maxDepth)
    {
        if (depth > maxDepth)
            maxDepth = depth;

        if (lo == hi)
            return a[lo];

        int mid = lo + (hi - lo) / 2;
        int left = Solve(a, lo, mid, depth + 1, ref maxDepth);
        int right = Solve(a, mid + 1, hi, depth + 1, ref maxDepth);
        return left > right ? left : right;
    }

    private static int MaxIterative(int[] a)
    {
        int best = a[0];
        for (int i = 1; i < a.Length; i++)
        {
            if (a[i] > best)
                best = a[i];
        }
        return best;
    }

    private static void RunTask3()
    {
        int m = K % 8 + 5;
        Console.WriteLine($"=== Завдання 3: сходи, M = {m} ===");
        int total = Climb(m, new List<int>());
        Console.WriteLine($"Кількість способів: {total}");
    }

    private static int Climb(int left, List<int> path)
    {
        if (left == 0)
        {
            Console.WriteLine(string.Join("+", path));
            return 1;
        }

        int total = 0;
        for (int step = 1; step <= 2; step++)
        {
            if (step > left)
                break;

            path.Add(step);
            total += Climb(left - step, path);
            path.RemoveAt(path.Count - 1);
        }
        return total;
    }


    private static void RunTask4()
    {
        Console.WriteLine("=== Завдання 4: трасування ===");
        var rnd = new Random(N);
        int[] a = new int[4];
        for (int i = 0; i < a.Length; i++)
            a[i] = rnd.Next(-20, 21);

        Console.WriteLine("Масив: " + string.Join(", ", a));
        MaxTrace(a, 0, a.Length - 1, 0);
    }

    private static int MaxTrace(int[] a, int lo, int hi, int depth)
    {
        string indent = new string(' ', depth * 2);

        if (lo == hi)
        {
            Console.WriteLine($"{indent}enter Max({lo}..{hi}) -> {a[lo]}");
            return a[lo];
        }

        Console.WriteLine($"{indent}enter Max({lo}..{hi})");
        int mid = lo + (hi - lo) / 2;
        int left = MaxTrace(a, lo, mid, depth + 1);
        int right = MaxTrace(a, mid + 1, hi, depth + 1);
        int result = left > right ? left : right;
        Console.WriteLine($"{indent}exit  Max({lo}..{hi}) -> {result}");
        return result;
    }

    private static void RunEdgeCases()
    {
        Console.WriteLine("=== Крайові випадки ===");

        string empty = Path.Combine(Path.GetTempPath(), "opam-lab03-empty-" + Guid.NewGuid());
        Directory.CreateDirectory(empty);
        try
        {
            var r = CountCsFiles(empty);
            Console.WriteLine($"Порожня папка: файлів {r.Count}, розмір {r.Size}, пропущено {r.Skipped}");
        }
        finally
        {
            Directory.Delete(empty);
        }

        Console.WriteLine("Один елемент {5}: " + Solve(new[] { 5 }, 0, 0));

        Console.WriteLine("Усі однакові {3,3,3,3}: " + Solve(new[] { 3, 3, 3, 3 }, 0, 3));

        Console.WriteLine("Сходи M = -1, способів: " + Climb(-1, new List<int>()));

        Console.WriteLine("Папка без доступу перевіряється вручну, див. README.");
    }
}