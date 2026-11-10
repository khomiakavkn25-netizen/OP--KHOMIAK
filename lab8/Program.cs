using System;
using System.Text;

namespace OpamLab08;

static class Program
{
    internal const int N = 27;
    internal const int K = 1;
    internal const int Size = 100_000;

    const int X = 7;
    const int Y = 14;

    const int Absent = 20 * K + 1;

    internal static readonly Func<int, int, int> Cmp = (x, y) => x.CompareTo(y);

    internal static int[] MakeData(int n, int seed)
    {
        var rnd = new Random(seed);
        int[] a = new int[n];
        for (int i = 0; i < n; i++) a[i] = rnd.Next(0, 20 * K + 1);
        return a;
    }

    internal static int[] MakeSorted(int n, int seed)
    {
        int[] a = MakeData(n, seed);
        Array.Sort(a, (x, y) => Cmp(x, y));
        return a;
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
#if DEBUG
        Console.WriteLine("УВАГА: збірка Debug. Для вимірювань запускайте: dotnet run -c Release");
        Console.WriteLine();
#endif
        Console.WriteLine($"N = {N}, K = {K}, a = {N % 3}, b = {N % 4}, c = {N % 5}");
        Console.WriteLine($"X = {X}, Y = {Y}, відсутнє значення = {Absent}");
        Console.WriteLine();

        int[] a = MakeSorted(Size, N);

        Task1(a);
        Task2(a);
        Task3(a);
        Task4(a);
        EdgeCases();
        Benchmark.Run();
    }


    static void Task1(int[] a)
    {
        Console.WriteLine("=== Завдання 1: лінійний пошук ===");
        int[] targets = { X, a[0], Absent };
        string[] names = { "значення X", "перший елемент", "значення, якого немає" };
        for (int i = 0; i < targets.Length; i++)
        {
            int idx = Search.LinearSearch(a, targets[i], Cmp, out int c);
            Console.WriteLine($"  {names[i]} ({targets[i]}): індекс {idx}, порівнянь {c}");
        }
        Console.WriteLine();
    }


    static void Task2(int[] a)
    {
        Console.WriteLine("=== Завдання 2: бінарний пошук ===");
        int[] targets = { X, a[0], Absent };
        string[] names = { "значення X", "перший елемент", "значення, якого немає" };
        for (int i = 0; i < targets.Length; i++)
        {
            int idx = Search.BinarySearch(a, targets[i], Cmp, out int c);
            Console.WriteLine($"  {names[i]} ({targets[i]}): індекс {idx}, порівнянь {c}");
        }
        Console.WriteLine();

        Broken1(a);
        Broken2(a);
        Broken3(a);
        Broken4();
    }

    static void Broken1(int[] a)
    {
        Console.WriteLine("--- Поломка 1: while (lo < hi) ---");
        int missed = 0;
        for (int v = 0; v <= 20 * K; v++)
        {
            int good = Search.BinarySearch(a, v, Cmp, out _);
            int bad = Broken.BinaryLoLtHi(a, v, Cmp, out _);
            if (good >= 0 && bad < 0) missed++;
        }
        Console.WriteLine($"  на основному масиві значень, що є, але не знайдені: {missed}");

        int[] small = { 1, 3, 5, 7, 9 };
        int g = Search.BinarySearch(small, 9, Cmp, out _);
        int b = Broken.BinaryLoLtHi(small, 9, Cmp, out _);
        Console.WriteLine($"  масив {{1,3,5,7,9}}, шукаємо 9: правильний = {g}, зламаний = {b}");
        Console.WriteLine();
    }

    static void Broken2(int[] a)
    {
        Console.WriteLine("--- Поломка 2: lo = mid ---");
        const int limit = 1000;
        int idx = Broken.BinaryLoMid(a, Absent, Cmp, limit, out int comparisons, out int lo, out int hi, out int idle);
        Console.WriteLine($"  шукаємо {Absent}: результат {idx}, порівнянь {comparisons} (обмеження {limit} кроків)");
        Console.WriteLine($"  межі в кінці: lo = {lo}, hi = {hi}; кроків без руху меж: {idle}");
        Console.WriteLine();
    }

    static void Broken3(int[] a)
    {
        Console.WriteLine("--- Поломка 3: mid = (lo + hi) / 2 ---");
        int diff = 0;
        for (int v = 0; v <= Absent; v++)
        {
            int good = Search.BinarySearch(a, v, Cmp, out int c1);
            int bad = Broken.BinaryOverflowMid(a, v, Cmp, out int c2);
            if (good != bad || c1 != c2) diff++;
        }
        Console.WriteLine($"  на {Size} елементах розбіжностей з правильною версією: {diff}");

        int lo = 1_500_000_000;
        int hi = 2_000_000_000;
        int broken = Broken.MidSum(lo, hi);
        int correct = lo + (hi - lo) / 2;
        Console.WriteLine($"  lo = {lo}, hi = {hi}: (lo + hi) / 2 = {broken}, а правильно = {correct}");
        Console.WriteLine($"  межа: lo + hi > {int.MaxValue}, тобто масив більший за 2^30 = {1 << 30} елементів");
        Console.WriteLine();
    }

    static void Broken4()
    {
        Console.WriteLine("--- Поломка 4: масив не відсортований ---");
        int[] raw = MakeData(Size, N);
        var rnd = new Random(N + 1);
        int notFound = 0;
        for (int i = 0; i < 100; i++)
        {
            int target = raw[rnd.Next(raw.Length)];
            if (Search.BinarySearch(raw, target, Cmp, out _) < 0) notFound++;
        }
        Console.WriteLine($"  зі 100 запитів наявних значень повернули -1: {notFound}");
        Console.WriteLine();
    }


    static int LinearCountRange(int[] a, int min, int max, out int comparisons)
    {
        int count = 0;
        comparisons = 0;
        foreach (int e in a)
        {
            comparisons++;
            if (Cmp(e, min) < 0) continue;
            comparisons++;
            if (Cmp(e, max) <= 0) count++;
        }
        return count;
    }

    static int LinearCountEqual(int[] a, int x, out int comparisons)
    {
        int count = 0;
        comparisons = 0;
        foreach (int e in a)
        {
            comparisons++;
            if (Cmp(e, x) == 0) count++;
        }
        return count;
    }

    static void Task3(int[] a)
    {
        Console.WriteLine("=== Завдання 3: пошук межі ===");

        int lower = Search.LowerBound(a, X, Cmp, out int cLower);
        int upper = Search.UpperBound(a, X, Cmp, out int cUpper);
        Console.WriteLine($"  LowerBound({X}) = {lower} ({cLower} порівнянь), UpperBound({X}) = {upper} ({cUpper} порівнянь)");

        int countEqual = upper - lower;
        int linEqual = LinearCountEqual(a, X, out int cLinEqual);
        Console.WriteLine($"  елементів, рівних {X}: {countEqual} за {cLower + cUpper} порівнянь; лінійно: {linEqual} за {cLinEqual}");

        int startIdx = Search.LowerBound(a, X, Cmp, out int cStart);
        int endIdx = Search.UpperBound(a, Y, Cmp, out int cEnd);
        int count = endIdx - startIdx;
        int linCount = LinearCountRange(a, X, Y, out int cLinRange);
        Console.WriteLine($"  елементів у [{X}; {Y}]: {count} за {cStart + cEnd} порівнянь; лінійно: {linCount} за {cLinRange}");
        Console.WriteLine($"  результати збігаються: {count == linCount && countEqual == linEqual}");

        int found = Search.FirstWhere(a, e => e % 7 == 0, out int cMult);
        int correct = a.Length;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] % 7 == 0) { correct = i; break; }
        }
        Console.WriteLine($"  перший елемент, кратний 7: пошук межі дав індекс {found} ({cMult} порівнянь), " +
                          $"правильний індекс {correct}, збігається: {found == correct}");
        Console.WriteLine();
    }


    static void Task4(int[] a)
    {
        Console.WriteLine("=== Завдання 4: потрійний пошук ===");
        int[] targets = { X, a[0], Absent };
        string[] names = { "значення X", "перший елемент", "значення, якого немає" };
        Console.WriteLine("  | запит | бінарний: індекс / порівнянь | потрійний: індекс / порівнянь |");
        for (int i = 0; i < targets.Length; i++)
        {
            int ib = Search.BinarySearch(a, targets[i], Cmp, out int cb);
            int it = Search.TernarySearch(a, targets[i], Cmp, out int ct);
            Console.WriteLine($"  | {names[i]} ({targets[i]}) | {ib} / {cb} | {it} / {ct} |");
        }

        var rnd = new Random(N + 3);
        long sumB = 0;
        long sumT = 0;
        bool ok = true;
        for (int i = 0; i < 100; i++)
        {
            int target = rnd.Next(0, 2 * 20 * K + 1);
            int ib = Search.BinarySearch(a, target, Cmp, out int cb);
            int it = Search.TernarySearch(a, target, Cmp, out int ct);
            sumB += cb;
            sumT += ct;
            if ((ib >= 0) != (it >= 0)) ok = false;
            if (it >= 0 && a[it] != target) ok = false;
        }
        Console.WriteLine($"  у середньому по 100 запитах: бінарний {sumB / 100.0:F1}, потрійний {sumT / 100.0:F1} порівнянь");
        Console.WriteLine($"  теорія: log2({a.Length}) = {Math.Log2(a.Length):F1}, log3({a.Length}) = {Math.Log(a.Length, 3):F1}");
        Console.WriteLine($"  обидва пошуки згодні щодо наявності значень: {ok}");
        Console.WriteLine();
    }


    static string Show(int[] a)
    {
        return "{" + string.Join(", ", a) + "}";
    }

    static void EdgeCase(string title, int[] a, int target)
    {
        int idx = Search.BinarySearch(a, target, Cmp, out _);
        int lo = Search.LowerBound(a, target, Cmp, out _);
        int up = Search.UpperBound(a, target, Cmp, out _);
        Console.WriteLine($"  {title}: {Show(a)}, шукаємо {target}");
        Console.WriteLine($"    бінарний = {idx}, LowerBound = {lo}, UpperBound = {up}");
    }

    static void EdgeCases()
    {
        Console.WriteLine("=== Крайові випадки ===");
        EdgeCase("порожній масив", new int[0], 5);
        EdgeCase("один елемент, він же шуканий", new[] { 5 }, 5);
        EdgeCase("два елементи, шуканий — другий", new[] { 3, 5 }, 5);
        EdgeCase("шукане менше за всі", new[] { 5, 6, 7 }, 1);
        EdgeCase("шукане більше за всі", new[] { 5, 6, 7 }, 9);
        EdgeCase("усі елементи однакові й рівні X", new[] { 5, 5, 5, 5 }, 5);

        int one = Broken.BinaryLoLtHi(new[] { 5 }, 5, Cmp, out _);
        Console.WriteLine($"  поломка 1 на {{5}}, шукаємо 5: {one} (правильно 0)");
        int two = Broken.BinaryLoMid(new[] { 3, 5 }, 5, Cmp, 1000, out _, out int lo, out int hi, out int idle);
        Console.WriteLine($"  поломка 2 на {{3, 5}}, шукаємо 5: {two}, межі lo = {lo}, hi = {hi}, кроків без руху: {idle}");
        Console.WriteLine();
    }
}