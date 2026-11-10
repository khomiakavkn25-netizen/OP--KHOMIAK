using System;

namespace OpamLab09;

sealed class Stats
{
    public long Comparisons;
    public long Moves;
}

static class Sorts
{

    static int Compare<T>(T x, T y, Func<T, T, int> cmp, Stats s)
    {
        s.Comparisons++;
        return cmp(x, y);
    }

    static void Swap<T>(T[] a, int i, int j, Stats s)
    {
        if (i == j) return;
        T tmp = a[i];
        a[i] = a[j];
        a[j] = tmp;
        s.Moves += 2;
    }

    static void Finish(Stats s, out int comparisons, out int moves)
    {
        comparisons = unchecked((int)s.Comparisons);
        moves = unchecked((int)s.Moves);
    }


    public static void SelectionSort<T>(T[] a, Func<T, T, int> cmp, out int comparisons, out int moves)
    {
        var s = new Stats();
        int n = a.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int min = i;
            for (int j = i + 1; j < n; j++)
            {
                if (Compare(a[j], a[min], cmp, s) < 0) min = j;
            }
            if (min != i) Swap(a, i, min, s);
        }
        Finish(s, out comparisons, out moves);
    }

    static int MedianOfThree<T>(T[] a, int lo, int hi, Func<T, T, int> cmp, Stats s)
    {
        int mid = lo + (hi - lo) / 2;
        if (Compare(a[lo], a[mid], cmp, s) < 0)
        {
            if (Compare(a[mid], a[hi], cmp, s) < 0) return mid;
            return Compare(a[lo], a[hi], cmp, s) < 0 ? hi : lo;
        }
        else
        {
            if (Compare(a[lo], a[hi], cmp, s) < 0) return lo;
            return Compare(a[mid], a[hi], cmp, s) < 0 ? hi : mid;
        }
    }

    static void InsertionRange<T>(T[] a, int lo, int hi, Func<T, T, int> cmp, Stats s)
    {
        for (int i = lo + 1; i <= hi; i++)
        {
            T key = a[i];
            int j = i - 1;
            while (j >= lo && Compare(a[j], key, cmp, s) > 0)
            {
                a[j + 1] = a[j];
                s.Moves++;
                j--;
            }
            if (j + 1 != i)
            {
                a[j + 1] = key;
                s.Moves++;
            }
        }
    }

    static void QuickCore<T>(T[] a, int lo, int hi, Func<T, T, int> cmp, int threshold, Stats s)
    {
        while (lo < hi)
        {
            if (hi - lo + 1 <= threshold)
            {
                InsertionRange(a, lo, hi, cmp, s);
                return;
            }

            int p = MedianOfThree(a, lo, hi, cmp, s);
            T pivot = a[p];
            int i = lo;
            int j = hi;
            while (i <= j)
            {
                while (Compare(a[i], pivot, cmp, s) < 0) i++;
                while (Compare(pivot, a[j], cmp, s) < 0) j--;
                if (i <= j)
                {
                    Swap(a, i, j, s);
                    i++;
                    j--;
                }
            }
            if (j - lo < hi - i)
            {
                QuickCore(a, lo, j, cmp, threshold, s);
                lo = i;
            }
            else
            {
                QuickCore(a, i, hi, cmp, threshold, s);
                hi = j;
            }
        }
    }

    public static void QuickSort<T>(T[] a, Func<T, T, int> cmp, out int comparisons, out int moves)
    {
        var s = new Stats();
        QuickCore(a, 0, a.Length - 1, cmp, 1, s);
        Finish(s, out comparisons, out moves);
    }


    public static void HybridSort<T>(T[] a, Func<T, T, int> cmp, int threshold, out int comparisons, out int moves)
    {
        var s = new Stats();
        QuickCore(a, 0, a.Length - 1, cmp, threshold < 1 ? 1 : threshold, s);
        Finish(s, out comparisons, out moves);
    }


    static void LomutoCore<T>(T[] a, int lo, int hi, Func<T, T, int> cmp, Stats s)
    {
        while (lo < hi)
        {
            int p = MedianOfThree(a, lo, hi, cmp, s);
            Swap(a, p, hi, s);
            T pivot = a[hi];
            int i = lo;
            for (int j = lo; j < hi; j++)
            {
                if (Compare(a[j], pivot, cmp, s) < 0)
                {
                    Swap(a, i, j, s);
                    i++;
                }
            }
            Swap(a, i, hi, s);

            if (i - lo < hi - i)
            {
                LomutoCore(a, lo, i - 1, cmp, s);
                lo = i + 1;
            }
            else
            {
                LomutoCore(a, i + 1, hi, cmp, s);
                hi = i - 1;
            }
        }
    }

    public static void QuickSortLomuto<T>(T[] a, Func<T, T, int> cmp, out int comparisons, out int moves)
    {
        var s = new Stats();
        LomutoCore(a, 0, a.Length - 1, cmp, s);
        Finish(s, out comparisons, out moves);
    }
}