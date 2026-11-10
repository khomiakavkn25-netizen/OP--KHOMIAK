using System;
using System.Linq;

namespace OpamLab09;

static class Stability
{
    static readonly Func<Student, Student, int> ByScore = (x, y) => x.Score.CompareTo(y.Score);
    static readonly Func<Student, Student, int> ByName = (x, y) => string.CompareOrdinal(x.Name, y.Name);

    static Student[] Make()
    {
        string[] surnames =
        {
            "Коваленко", "Мельник", "Іщенко", "Бондар", "Савченко", "Ткачук",
            "Кравець", "Іванів", "Мороз", "Шевчук", "Бойко", "Кушнір",
        };
        var rnd = new Random(Program.N);
        var list = new Student[200];
        for (int i = 0; i < list.Length; i++)
        {
            string name = surnames[i % surnames.Length] + "-" + (i / surnames.Length + 1).ToString("D2");
            string group = i % 2 == 0 ? "КН-3/1" : "КН-3/2";
            list[i] = new Student(name, group, rnd.Next(50, 101));
        }
        Sorts.QuickSort(list, ByName, out _, out _);
        return list;
    }

    static string? FirstCounterexample(Student[] a)
    {
        for (int i = 1; i < a.Length; i++)
        {
            if (a[i - 1].Score == a[i].Score && string.CompareOrdinal(a[i - 1].Name, a[i].Name) > 0)
            {
                return $"{a[i - 1].Name} стоїть перед {a[i].Name} (бал {a[i].Score})";
            }
        }
        return null;
    }

    static void Report(string name, string expected, Student[] original, Student[] sorted)
    {
        bool ok = Checks.IsSorted(sorted, ByScore) && Checks.IsPermutationOf(sorted, original, ByName);
        string? counter = FirstCounterexample(sorted);
        string actual = counter == null ? "так (стабільно)" : "ні (нестабільно)";
        Console.WriteLine($"| {name} | {expected} | {actual} | {counter ?? "—"} |{(ok ? "" : "  ПОМИЛКА: результат неправильний")}");
    }

    public static void Run()
    {
        Console.WriteLine("=== Завдання 5: стабільність ===");
        Student[] source = Make();

        Console.WriteLine("| Алгоритм | Очікувано (стабільний?) | Фактично | Перший контрприклад |");
        Console.WriteLine("|---|---|---|---|");

        Student[] sel = (Student[])source.Clone();
        Sorts.SelectionSort(sel, ByScore, out _, out _);
        Report("Простий (вибором)", "ні", source, sel);

        Student[] quick = (Student[])source.Clone();
        Sorts.QuickSort(quick, ByScore, out _, out _);
        Report("Швидкий (Гоар, медіана трьох)", "ні", source, quick);

        Student[] std = (Student[])source.Clone();
        Array.Sort(std, (x, y) => ByScore(x, y));
        Report("Array.Sort", "ні", source, std);

        Student[] linq = source.OrderBy(s => s.Score).ToArray();
        Report("OrderBy", "так", source, linq);
        Console.WriteLine();
    }
}