using System;
using System.Collections.Generic;
using System.Text;

namespace OpamLab07;

static class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        int n = 27;
        int k = 1;
        Console.WriteLine($"N = {n}, K = {k}, a = {n % 3}, b = {n % 4}, c = {n % 5}");
        Console.WriteLine();

        List<int> nums = MakeNumbers(n, k);
        List<string> words = MakeWords();
        List<Student> students = MakeStudents();

        Console.WriteLine($"Числа:  {Show(nums)}");
        Console.WriteLine($"Рядки:  {Show(words)}");
        Console.WriteLine($"Студенти: {Show(Algorithms.Map(students, s => s.Name))}");
        Console.WriteLine();

        Task1(nums, words, students, k);
        Task2(nums, words, students);
        Task3(nums, words);
        Task4(words, students);
        Task5(k);
        EdgeCases();
    }


    static List<int> MakeNumbers(int seed, int k)
    {
        var rnd = new Random(seed);
        var list = new List<int>();
        for (int i = 0; i < 20; i++) list.Add(rnd.Next(-k, k + 1));   // [-K; K]
        return list;
    }

    static List<string> MakeWords()
    {
        return new List<string>
        {
            "алгоритм", "код", "веб-сайт", "функція", "дані", "по-перше", "змінна",
            "цикл", "українська", "лямбда", "делегат", "ікс", "що-небудь", "узагальнення",
        };
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


    static string Show<T>(List<T> list)
    {
        return "[" + string.Join(", ", list) + "]";
    }

    static void PrintGroups<T, TKey>(string title, Dictionary<TKey, List<T>> groups) where TKey : notnull
    {
        Console.WriteLine($"  {title}:");
        foreach (KeyValuePair<TKey, List<T>> p in groups)
        {
            Console.WriteLine($"    {p.Key}: {p.Value.Count}");
        }
    }

    static int CountVowels(string s)
    {
        const string vowels = "аеєиіїоуюя";
        int count = 0;
        foreach (char ch in s)
        {
            if (vowels.IndexOf(char.ToLower(ch)) >= 0) count++;
        }
        return count;
    }

    static string ScoreRange(int score)
    {
        if (score < 60) return "0–59";
        if (score < 75) return "60–74";
        if (score < 90) return "75–89";
        return "90–100";
    }

    static int? MaxOf(List<int> list)
    {
        return Algorithms.Aggregate<int, int?>(list, null,
            (acc, x) => acc == null || x > acc ? x : acc);
    }

    static string JoinWithComma(List<string> list)
    {
        (string text, bool any) result = Algorithms.Aggregate<string, (string text, bool any)>(
            list, (text: "", any: false),
            (acc, w) => (acc.any ? acc.text + "," + w : w, true));
        return result.text;
    }

    static string ShowMax(int? max)
    {
        return max.HasValue ? max.Value.ToString() : "немає (список порожній)";
    }


    static void Task1(List<int> nums, List<string> words, List<Student> students, int k)
    {
        Console.WriteLine("=== Завдання 1: Filter ===");

        List<int> r1 = Algorithms.Filter(nums, x => x > k);
        List<string> r2 = Algorithms.Filter(words, w => w.Length > 5);
        List<Student> r3 = Algorithms.Filter(students, s => s.Score >= 75);

        Console.WriteLine($"  числа > {k}: {Show(r1)} (розмір {r1.Count})");
        Console.WriteLine($"  рядки довші за 5: {Show(r2)} (розмір {r2.Count})");
        Console.WriteLine($"  студенти з балом >= 75: {Show(Algorithms.Map(r3, s => s.Name))} (розмір {r3.Count})");

        List<string> upper = Algorithms.Map(r2, w => w.ToUpper());
        Console.WriteLine($"  Filter -> Map (довгі рядки у верхньому регістрі): {Show(upper)}");
        Console.WriteLine();
    }


    static void Task2(List<int> nums, List<string> words, List<Student> students)
    {
        Console.WriteLine("=== Завдання 2: Map ===");

        List<string> m1 = Algorithms.Map(nums, x => "(" + x + ")");
        List<int> m2 = Algorithms.Map(words, CountVowels);
        List<bool> m3 = Algorithms.Map(students, s => s.Score >= 60);

        Console.WriteLine($"  числа в дужках: {Show(m1)}");
        Console.WriteLine($"  кількість голосних: {Show(m2)}");
        Console.WriteLine($"  склали (бал >= 60): {Show(m3)}");
        Console.WriteLine();
    }


    static void Task3(List<int> nums, List<string> words)
    {
        Console.WriteLine("=== Завдання 3: Aggregate ===");

        int sum = Algorithms.Aggregate(nums, 0, (acc, x) => acc + x);
        int product = Algorithms.Aggregate(nums, 1, (acc, x) => acc * x);
        int? max = MaxOf(nums);
        string joined = JoinWithComma(words);

        Console.WriteLine($"  сума: {sum}");
        Console.WriteLine($"  добуток: {product}");
        Console.WriteLine($"  максимум: {ShowMax(max)}");
        Console.WriteLine($"  конкатенація через кому: {joined}");
        Console.WriteLine();
    }


    static void Task4(List<string> words, List<Student> students)
    {
        Console.WriteLine("=== Завдання 4: GroupBy ===");

        PrintGroups("студенти за групою", Algorithms.GroupBy(students, s => s.Group));
        PrintGroups("студенти за першою літерою прізвища", Algorithms.GroupBy(students, s => s.Name.Substring(0, 1)));
        PrintGroups("студенти за діапазоном балу", Algorithms.GroupBy(students, s => ScoreRange(s.Score)));
        PrintGroups("рядки за довжиною", Algorithms.GroupBy(words, w => w.Length));
        Console.WriteLine();
    }


    static void Task5(int k)
    {
        Console.WriteLine("=== Завдання 5: захоплення змінних ===");

        int threshold = k;
        Func<int, bool> above = x => x > threshold;
        Console.WriteLine($"  1) поріг = {threshold}: above(5) = {above(5)}");
        threshold = 1000;
        Console.WriteLine($"     після threshold = 1000: above(5) = {above(5)}");

        var viaFor = new List<Func<int>>();
        for (int i = 0; i < 3; i++)
        {
            viaFor.Add(() => i);
        }
        Console.WriteLine($"  2) for: {viaFor[0]()}, {viaFor[1]()}, {viaFor[2]()}");

        var viaForeach = new List<Func<int>>();
        foreach (int v in new[] { 0, 1, 2 })
        {
            viaForeach.Add(() => v);
        }
        Console.WriteLine($"  3) foreach: {viaForeach[0]()}, {viaForeach[1]()}, {viaForeach[2]()}");

        var fixedFor = new List<Func<int>>();
        for (int i = 0; i < 3; i++)
        {
            int copy = i;
            fixedFor.Add(() => copy);
        }
        Console.WriteLine($"     виправлений for: {fixedFor[0]()}, {fixedFor[1]()}, {fixedFor[2]()}");
        Console.WriteLine();
    }


    static void EdgeCases()
    {
        Console.WriteLine("=== Крайові випадки ===");

        var empty = new List<int>();
        Console.WriteLine($"  порожній список, Filter: {Algorithms.Filter(empty, x => x > 0).Count} елементів");
        Console.WriteLine($"  порожній список, Map: {Algorithms.Map(empty, x => x * 2).Count} елементів");
        Console.WriteLine($"  порожній список, GroupBy: {Algorithms.GroupBy(empty, x => x).Count} груп");
        Console.WriteLine($"  порожній список, максимум: {ShowMax(MaxOf(empty))}");
        Console.WriteLine($"  порожній список, сума: {Algorithms.Aggregate(empty, 0, (acc, x) => acc + x)}, " +
                          $"добуток: {Algorithms.Aggregate(empty, 1, (acc, x) => acc * x)}");

        var some = new List<int> { 3, -2, 7, 0 };
        Console.WriteLine($"  жоден не підходить (x > 100): {Algorithms.Filter(some, x => x > 100).Count} елементів");
        Console.WriteLine($"  усі в одній групі: {Algorithms.GroupBy(some, x => "усі").Count} група");

        var one = new List<int> { 5 };
        Console.WriteLine($"  один елемент {{5}}: сума {Algorithms.Aggregate(one, 0, (acc, x) => acc + x)}, " +
                          $"добуток {Algorithms.Aggregate(one, 1, (acc, x) => acc * x)}, максимум {ShowMax(MaxOf(one))}");
        Console.WriteLine($"  один рядок: \"{JoinWithComma(new List<string> { "слово" })}\"");
    }
}