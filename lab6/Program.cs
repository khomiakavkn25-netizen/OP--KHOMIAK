using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace OpamLab06;

static class Program
{
    const int N = 27;
    const int K = 1;

    const int Scale = 1;

    const string Text =
        "Алгоритм описує послідовність дій. Алгоритм можна записати словами, " +
        "а можна записати кодом. Кодом алгоритм стає тоді, коли його " +
        "виконує машина, а не людина. Одна задача — багато алгоритмів.";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Task1();
        Task2();
        Task3();
        Task4();
        EdgeCases();
        Task5();
    }

    static List<string> SplitWords(string text)
    {
        var words = new List<string>();
        var sb = new StringBuilder();
        foreach (char ch in text)
        {
            if (char.IsLetter(ch))
            {
                sb.Append(char.ToLower(ch));
            }
            else if (sb.Length > 0)
            {
                words.Add(sb.ToString());
                sb.Clear();
            }
        }
        if (sb.Length > 0) words.Add(sb.ToString());
        return words;
    }

    static Dictionary<string, int> CountWordsDict(List<string> words)
    {
        var d = new Dictionary<string, int>();
        foreach (string w in words)
        {
            if (d.TryGetValue(w, out int cnt)) d[w] = cnt + 1;
            else d[w] = 1;
        }
        return d;
    }

    static void CountWordsList(List<string> words, out List<string> keys, out List<int> counts)
    {
        keys = new List<string>();
        counts = new List<int>();
        foreach (string w in words)
        {
            int idx = -1;
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] == w) { idx = i; break; }
            }
            if (idx >= 0) counts[idx]++;
            else { keys.Add(w); counts.Add(1); }
        }
    }

    static bool SameCounts(Dictionary<string, int> d, List<string> keys, List<int> counts)
    {
        if (d.Count != keys.Count) return false;
        for (int i = 0; i < keys.Count; i++)
        {
            if (!d.TryGetValue(keys[i], out int v) || v != counts[i]) return false;
        }
        return true;
    }

    static void Task1()
    {
        Console.WriteLine("=== Завдання 1: частоти слів ===");
        int threshold = K % 3 + 2;
        Console.WriteLine($"Поріг: частота >= {threshold}");

        List<string> words = SplitWords(Text);
        Dictionary<string, int> d = CountWordsDict(words);
        PrintFrequentDict(d, threshold);
        Console.WriteLine($"Різних слів: {d.Count}");

        CountWordsList(words, out List<string> keys, out List<int> counts);
        Console.WriteLine($"Спискова версія дає той самий результат: {SameCounts(d, keys, counts)}");
        Console.WriteLine();
    }

    static void PrintFrequentDict(Dictionary<string, int> d, int threshold)
    {
        foreach (KeyValuePair<string, int> p in d)
        {
            if (p.Value >= threshold) Console.WriteLine($"  {p.Key}: {p.Value}");
        }
    }


    static List<Student> MakeStudents()
    {
        return new List<Student>
        {
            new Student("Коваленко", "КН-3/1", 88), new Student("Мельник", "КН-3/2", 64),
            new Student("Іщенко", "КН-3/1", 93),    new Student("Бондар", "КН-3/1", 71),
            new Student("Савченко", "КН-3/2", 55),  new Student("Ткачук", "КН-3/2", 90),
            new Student("Кравець", "КН-3/1", 76),   new Student("Іванів", "КН-3/1", 48),
            new Student("Мороз", "КН-3/2", 82),     new Student("Шевчук", "КН-3/1", 67),
            new Student("Бойко", "КН-3/2", 95),     new Student("Кушнір", "КН-3/2", 73),
        };
    }

    static int GroupKey(Student s)
    {
        return s.Name.Length;
    }

    static double AverageScore(List<Student> list)
    {
        if (list.Count == 0) return 0;
        int sum = 0;
        foreach (Student s in list) sum += s.Score;
        return (double)sum / list.Count;
    }

    static Dictionary<int, List<Student>> GroupDict(List<Student> students)
    {
        var d = new Dictionary<int, List<Student>>();
        foreach (Student s in students)
        {
            int key = GroupKey(s);
            if (!d.TryGetValue(key, out List<Student>? list))
            {
                list = new List<Student>();
                d[key] = list;
            }
            list.Add(s);
        }
        return d;
    }

    static void GroupList(List<Student> students, out List<int> keys, out List<List<Student>> groups)
    {
        keys = new List<int>();
        groups = new List<List<Student>>();
        foreach (Student s in students)
        {
            int key = GroupKey(s);
            int idx = -1;
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] == key) { idx = i; break; }
            }
            if (idx < 0)
            {
                keys.Add(key);
                groups.Add(new List<Student>());
                idx = keys.Count - 1;
            }
            groups[idx].Add(s);
        }
    }

    static void PrintGroups(Dictionary<int, List<Student>> d)
    {
        foreach (KeyValuePair<int, List<Student>> p in d)
        {
            Console.WriteLine($"  літер: {p.Key}, студентів: {p.Value.Count}, середній бал: {AverageScore(p.Value):F2}");
        }
    }

    static void Task2()
    {
        Console.WriteLine("=== Завдання 2: групування за кількістю літер у прізвищі ===");
        List<Student> students = MakeStudents();

        Dictionary<int, List<Student>> d = GroupDict(students);
        PrintGroups(d);

        GroupList(students, out List<int> keys, out List<List<Student>> groups);
        bool same = keys.Count == d.Count;
        for (int i = 0; i < keys.Count && same; i++)
        {
            if (!d.TryGetValue(keys[i], out List<Student>? g) || g.Count != groups[i].Count) same = false;
        }
        Console.WriteLine($"Спискова версія дає той самий результат: {same}");
        Console.WriteLine();
    }


    static List<int> RandomList(int count, int min, int maxInclusive, int seed)
    {
        var rnd = new Random(seed);
        var list = new List<int>();
        for (int i = 0; i < count; i++) list.Add(rnd.Next(min, maxInclusive + 1));
        return list;
    }

    static List<int> DedupHashSet(List<int> src)
    {
        var set = new HashSet<int>(src);
        return new List<int>(set);
    }

    static List<int> DedupOrdered(List<int> src)
    {
        var seen = new HashSet<int>();
        var result = new List<int>();
        foreach (int x in src)
        {
            if (seen.Add(x)) result.Add(x);
        }
        return result;
    }

    static List<int> DedupList(List<int> src)
    {
        var result = new List<int>();
        foreach (int x in src)
        {
            if (!ContainsList(result, x)) result.Add(x);
        }
        return result;
    }

    static bool ContainsList(List<int> list, int x)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == x) return true;
        }
        return false;
    }

    static string Show(List<int> list)
    {
        return "[" + string.Join(", ", list) + "]";
    }

    static void Task3()
    {
        Console.WriteLine("=== Завдання 3: вилучення дублікатів ===");
        int max = K % 10 + 6;
        List<int> src = RandomList(25, 1, max, N);
        Console.WriteLine($"Діапазон [1; {max}], Random({N})");
        Console.WriteLine($"Вихідний список:       {Show(src)}");
        Console.WriteLine($"HashSet:               {Show(DedupHashSet(src))}");
        Console.WriteLine($"Зі збереженням порядку: {Show(DedupOrdered(src))}");
        Console.WriteLine($"Спискова версія:       {Show(DedupList(src))}");

        var set = new HashSet<int> { 1, 2, 3, 4, 5 };
        set.Remove(2);
        set.Add(99);
        Console.WriteLine($"Демо: {{1,2,3,4,5}}, Remove(2), Add(99) => {Show(new List<int>(set))}");
        Console.WriteLine();
    }
    
    static List<int> SymmetricHashSet(List<int> a, List<int> b)
    {
        var set = new HashSet<int>(a);
        set.SymmetricExceptWith(b);
        return new List<int>(set);
    }

    static List<int> SymmetricManual(List<int> a, List<int> b)
    {
        var result = new List<int>();
        AddOnlyIn(a, b, result);
        AddOnlyIn(b, a, result);
        return result;
    }

    static void AddOnlyIn(List<int> from, List<int> other, List<int> result)
    {
        foreach (int x in from)
        {
            if (!ContainsList(other, x) && !ContainsList(result, x)) result.Add(x);
        }
    }
    static bool SameElements(List<int> a, List<int> b)
    {
        if (a.Count != b.Count) return false;
        foreach (int x in a)
        {
            if (!ContainsList(b, x)) return false;
        }
        return true;
    }

    static void Task4()
    {
        Console.WriteLine("=== Завдання 4: елементи, що є лише в одному з наборів ===");
        int max = 20 + K;
        List<int> a = RandomList(30, 1, max, N);
        List<int> b = RandomList(30, 1, max, N + 100);
        Console.WriteLine($"Діапазон [1; {max}]");
        Console.WriteLine($"Набір A (Random({N})):       {Show(a)}");
        Console.WriteLine($"Набір B (Random({N + 100})): {Show(b)}");

        List<int> viaSet = SymmetricHashSet(a, b);
        List<int> viaLoops = SymmetricManual(a, b);
        Console.WriteLine($"HashSet:       {Show(viaSet)} (кількість: {viaSet.Count})");
        Console.WriteLine($"Вкладені цикли: {Show(viaLoops)} (кількість: {viaLoops.Count})");
        Console.WriteLine($"Результати збігаються: {SameElements(viaSet, viaLoops)}");
        Console.WriteLine();
    }


    static void EdgeCases()
    {
        Console.WriteLine("=== Крайові випадки ===");

        Dictionary<string, int> empty = CountWordsDict(SplitWords(""));
        Console.WriteLine($"1) порожній текст: різних слів = {empty.Count}");

        Dictionary<string, int> allDiff = CountWordsDict(SplitWords("один два три чотири"));
        int shown = 0;
        foreach (KeyValuePair<string, int> p in allDiff)
        {
            if (p.Value >= K % 3 + 2) shown++;
        }
        Console.WriteLine($"1) усі слова різні: різних = {allDiff.Count}, виведено за порогом = {shown}");

        var same = new List<Student>
        {
            new Student("Бойко", "КН-3/1", 80),
            new Student("Мороз", "КН-3/1", 90),
            new Student("Левко", "КН-3/1", 70),
        };
        Console.WriteLine("2) усі студенти в одній групі:");
        PrintGroups(GroupDict(same));

        var noDup = new List<int> { 1, 2, 3, 4, 5 };
        Console.WriteLine($"3) без дублікатів: {Show(DedupOrdered(noDup))}");

        var x = new List<int> { 1, 2, 3 };
        var y = new List<int> { 4, 5, 6 };
        Console.WriteLine($"4) не перетинаються: {Show(SymmetricHashSet(x, y))} / {Show(SymmetricManual(x, y))}");

        // 4. набори однакові
        var z = new List<int> { 1, 2, 3 };
        Console.WriteLine($"4) однакові: {Show(SymmetricHashSet(x, z))} / {Show(SymmetricManual(x, z))}");
        Console.WriteLine();
    }


    static double Measure(Action action)
    {
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds;
    }

    static int CountFoundList(List<int> data, List<int> queries)
    {
        int found = 0;
        foreach (int q in queries)
        {
            if (ContainsList(data, q)) found++;
        }
        return found;
    }

    static int CountFoundSet(HashSet<int> data, List<int> queries)
    {
        int found = 0;
        foreach (int q in queries)
        {
            if (data.Contains(q)) found++;
        }
        return found;
    }

    static int CommonList(List<int> a, List<int> b)
    {
        var result = new List<int>();
        foreach (int x in a)
        {
            if (ContainsList(b, x) && !ContainsList(result, x)) result.Add(x);
        }
        return result.Count;
    }

    static int CommonSet(List<int> a, List<int> b)
    {
        var set = new HashSet<int>(a);
        set.IntersectWith(b);
        return set.Count;
    }

    static List<string> RandomWords(int count, int vocabulary, int seed)
    {
        var rnd = new Random(seed);
        var list = new List<string>();
        for (int i = 0; i < count; i++) list.Add("w" + rnd.Next(vocabulary));
        return list;
    }

    static void PrintRow(string name, double listMs, double dictMs)
    {
        double ratio = listMs / Math.Max(dictMs, 0.001);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
            "| {0} | {1:F2} | {2:F2} | {3:F1} |", name, listMs, dictMs, ratio));
    }

    static void Task5()
    {
        Console.WriteLine("=== Завдання 5: заміри (Scale = " + Scale + ") ===");

        {
            List<int> w = RandomList(500, 0, 1000, 1);
            CountFoundList(w, w); CountFoundSet(new HashSet<int>(w), w);
            List<string> ws = RandomWords(500, 50, 1);
            CountWordsDict(ws); CountWordsList(ws, out _, out _);
            DedupOrdered(w); DedupList(w);
            CommonSet(w, w); CommonList(w, w);
        }

        var rnd = new Random(N);

        int size1 = 100_000 / Scale, queries1 = 10_000 / Scale;
        var data = new List<int>();
        for (int i = 0; i < size1; i++) data.Add(rnd.Next(0, 2 * size1));
        var queries = new List<int>();
        for (int i = 0; i < queries1; i++) queries.Add(rnd.Next(0, 2 * size1));
        var dataSet = new HashSet<int>(data);
        int r1a = 0, r1b = 0;
        double t1a = Measure(() => r1a = CountFoundList(data, queries));
        double t1b = Measure(() => r1b = CountFoundSet(dataSet, queries));

        List<string> words = RandomWords(50_000 / Scale, 5_000 / Scale + 1, N);
        Dictionary<string, int>? dict = null;
        List<string> keys = new List<string>();
        List<int> counts = new List<int>();
        double t2a = Measure(() => CountWordsList(words, out keys, out counts));
        double t2b = Measure(() => dict = CountWordsDict(words));

        int size3 = 20_000 / Scale;
        List<int> nums = RandomList(size3, 0, size3, N);
        List<int> d3a = new List<int>(), d3b = new List<int>();
        double t3a = Measure(() => d3a = DedupList(nums));
        double t3b = Measure(() => d3b = DedupOrdered(nums));

        int size4 = 5_000 / Scale;
        List<int> a = RandomList(size4, 0, 2 * size4, N);
        List<int> b = RandomList(size4, 0, 2 * size4, N + 100);
        int r4a = 0, r4b = 0;
        double t4a = Measure(() => r4a = CommonList(a, b));
        double t4b = Measure(() => r4b = CommonSet(a, b));

        Console.WriteLine("| Що міряємо | Список, мс | Словник/множина, мс | У скільки разів |");
        Console.WriteLine("|---|---|---|---|");
        PrintRow("перевірки наявності", t1a, t1b);
        PrintRow("частоти слів", t2a, t2b);
        PrintRow("вилучення дублікатів", t3a, t3b);
        PrintRow("спільні елементи", t4a, t4b);

        bool ok = r1a == r1b && dict != null && SameCounts(dict, keys, counts)
                  && SameElements(d3a, d3b) && r4a == r4b;
        Console.WriteLine($"Обидві версії дали однакові результати: {ok}");
    }
}