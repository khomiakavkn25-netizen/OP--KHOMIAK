using System;
using System.Text;

namespace OpamLab09;

delegate void SortAlgo(int[] a, Func<int, int, int> cmp, out int comparisons, out int moves);

sealed class Algo
{
    public string Name;
    public SortAlgo Run;

    public Algo(string name, SortAlgo run)
    {
        Name = name;
        Run = run;
    }
}

static class Program
{
    internal const int N = 27;
    internal const int K = 1;

    internal static readonly int[] Thresholds = { 1, 4, 16, 2 * K + 3 };

    internal static int BestThreshold = 16;

    internal static readonly Func<int, int, int> Cmp = (x, y) => x.CompareTo(y);

    internal static int[] MakeRandom(int n)
    {
        var rnd = new Random(N);
        int[] a = new int[n];
        for (int i = 0; i < n; i++) a[i] = rnd.Next(0, 20 * K + 1);
        return a;
    }

    internal static int[] MakeSorted(int n)
    {
        int[] a = MakeRandom(n);
        Sorts.QuickSort(a, Cmp, out _, out _);
        return a;
    }

    internal static int[] MakeReversed(int n)
    {
        int[] a = MakeSorted(n);
        for (int i = 0, j = n - 1; i < j; i++, j--)
        {
            int tmp = a[i];
            a[i] = a[j];
            a[j] = tmp;
        }
        return a;
    }

    internal static int[] MakeAlmostSorted(int n)
    {
        int[] a = MakeSorted(n);
        var rnd = new Random(N);
        for (int k = 0; k < K; k++)
        {
            int i = rnd.Next(n);
            int j = rnd.Next(n);
            int tmp = a[i];
            a[i] = a[j];
            a[j] = tmp;
        }
        return a;
    }

    static readonly string[] InputNames = { "Випадковий", "Відсортований", "Навпаки", "Майже" };

    static int[][] MakeInputs(int n)
    {
        return new[] { MakeRandom(n), MakeSorted(n), MakeReversed(n), MakeAlmostSorted(n) };
    }


    static SortAlgo HybridWith(int threshold)
    {
        return (int[] a, Func<int, int, int> cmp, out int c, out int m) =>
            Sorts.HybridSort(a, cmp, threshold, out c, out m);
    }

    static Algo[] AllAlgos()
    {
        var list = new System.Collections.Generic.List<Algo>
        {
            new Algo("вибором", Sorts.SelectionSort<int>),
            new Algo("швидке (Гоар, медіана трьох)", Sorts.QuickSort<int>),
            new Algo("швидке (Ломуто, демо)", Sorts.QuickSortLomuto<int>),
        };
        foreach (int t in Thresholds) list.Add(new Algo($"гібрид, поріг {t}", HybridWith(t)));
        return list.ToArray();
    }

    static bool RunAndCheck(Algo algo, int[] input, out int comparisons, out int moves)
    {
        int[] copy = (int[])input.Clone();
        algo.Run(copy, Cmp, out comparisons, out moves);
        return Checks.IsSorted(copy, Cmp) && Checks.IsPermutationOf(copy, input, Cmp);
    }

    static string Show(int[] a)
    {
        return "[" + string.Join(", ", a) + "]";
    }


    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
#if DEBUG
        Console.WriteLine("УВАГА: збірка Debug. Для вимірювань запускайте: dotnet run -c Release");
        Console.WriteLine();
#endif
        Console.WriteLine($"N = {N}, K = {K}, a = {N % 3}, b = {N % 4}, c = {N % 5}");
        Console.WriteLine();

        Task1Trace();
        Correctness();
        AllEqualDemo();
        BestThreshold = Benchmark.HybridThresholds(MakeRandom(1_000_000));
        Task4Tables();
        Benchmark.TimeTable();
        Benchmark.SimpleLimit();
        Stability.Run();
        EdgeCases();
    }


    static void Task1Trace()
    {
        Console.WriteLine("=== Завдання 1: трасування сортування вибором ===");
        int count = K % 5 + 6;
        int[] a = new int[count];
        Array.Copy(MakeRandom(count), a, count);
        Console.WriteLine($"Масив для трасування (перші {count} елементів): {Show(a)}");

        int comparisons = 0;
        int moves = 0;
        for (int i = 0; i < a.Length - 1; i++)
        {
            int min = i;
            for (int j = i + 1; j < a.Length; j++)
            {
                comparisons++;
                if (a[j] < a[min]) min = j;
            }
            bool swapped = min != i;
            if (swapped)
            {
                int tmp = a[i];
                a[i] = a[min];
                a[min] = tmp;
                moves += 2;
            }
            Console.WriteLine($"  прохід {i + 1}: мінімум на позиції {min}, {(swapped ? "обмін" : "без обміну")}: {Show(a)}");
        }
        Console.WriteLine($"  порівнянь {comparisons}, переставлянь {moves}");
        Console.WriteLine();
    }


    static void Correctness()
    {
        Console.WriteLine("=== Перевірка IsSorted + IsPermutationOf (n = 1000, чотири входи) ===");
        int[][] inputs = MakeInputs(1000);
        foreach (Algo algo in AllAlgos())
        {
            string line = "";
            for (int i = 0; i < inputs.Length; i++)
            {
                bool ok = RunAndCheck(algo, inputs[i], out _, out _);
                line += $" {InputNames[i]}: {(ok ? "OK" : "ПОМИЛКА")};";
            }
            Console.WriteLine($"  {algo.Name}:{line}");
        }
        Console.WriteLine();
    }


    static void AllEqualDemo()
    {
        Console.WriteLine("=== Усі елементи однакові: порівняння ===");
        Console.WriteLine("| n | Ломуто («<» і «>=») | Гоар (зупинка на рівних) | n(n-1)/2 | n·log2 n |");
        Console.WriteLine("|---|---|---|---|---|");
        int[] sizes = { 1_000, 10_000 };
        foreach (int n in sizes)
        {
            int[] equal = new int[n];
            for (int i = 0; i < n; i++) equal[i] = 7;

            int[] x = (int[])equal.Clone();
            Sorts.QuickSortLomuto(x, Cmp, out int cLomuto, out _);
            int[] y = (int[])equal.Clone();
            Sorts.QuickSort(y, Cmp, out int cHoare, out _);

            long quadratic = (long)n * (n - 1) / 2;
            Console.WriteLine($"| {n} | {cLomuto} | {cHoare} | {quadratic} | {n * Math.Log2(n):F0} |");
        }
        Console.WriteLine();
    }


    static void Task4Tables()
    {
        Console.WriteLine("=== Завдання 4: порівняння / переставляння ===");
        Algo[] algos =
        {
            new Algo("вибором (простий)", Sorts.SelectionSort<int>),
            new Algo("швидке (Гоар, медіана трьох)", Sorts.QuickSort<int>),
            new Algo($"гібрид, поріг {BestThreshold}", HybridWith(BestThreshold)),
        };
        int[] sizes = { 100, 1_000, 10_000 };

        foreach (Algo algo in algos)
        {
            Console.WriteLine($"Алгоритм: {algo.Name}");
            Console.WriteLine("| n | Випадковий | Відсортований | Навпаки | Майже |");
            Console.WriteLine("|---|---|---|---|---|");
            foreach (int n in sizes)
            {
                int[][] inputs = MakeInputs(n);
                string row = $"| {n} |";
                for (int i = 0; i < inputs.Length; i++)
                {
                    bool ok = RunAndCheck(algo, inputs[i], out int c, out int m);
                    row += ok ? $" {c} / {m} |" : " ПОМИЛКА |";
                }
                Console.WriteLine(row);
            }
            Console.WriteLine();
        }
    }


    static int[] TwoValues()
    {
        int[] a = { 3, 3, 3, 3, 3, 8, 8, 8, 8, 8 };
        var rnd = new Random(N);
        for (int i = a.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            int tmp = a[i];
            a[i] = a[j];
            a[j] = tmp;
        }
        return a;
    }

    static void EdgeCases()
    {
        Console.WriteLine("=== Крайові випадки ===");
        string[] names =
        {
            "порожній масив", "один елемент", "два елементи в порядку",
            "два елементи навпаки", "усі елементи однакові", "два різних значення, по пів масиву",
        };
        int[][] cases =
        {
            new int[0], new[] { 5 }, new[] { 1, 2 }, new[] { 2, 1 },
            new[] { 7, 7, 7, 7, 7, 7 }, TwoValues(),
        };
        Algo[] algos = AllAlgos();

        for (int i = 0; i < cases.Length; i++)
        {
            bool allOk = true;
            string failed = "";
            foreach (Algo algo in algos)
            {
                if (!RunAndCheck(algo, cases[i], out _, out _))
                {
                    allOk = false;
                    failed += " " + algo.Name + ";";
                }
            }
            int[] sorted = (int[])cases[i].Clone();
            Sorts.QuickSort(sorted, Cmp, out _, out _);
            Console.WriteLine($"  {names[i]}: {Show(cases[i])} -> {Show(sorted)}; " +
                              $"IsSorted і IsPermutationOf у всіх алгоритмів: {(allOk ? "пройдено" : "ПОМИЛКА:" + failed)}");
        }
    }
}