using System;

namespace OpamLab08;

static class Search
{
    public static int LinearSearch<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
    {
        comparisons = 0;
        for (int i = 0; i < a.Length; i++)
        {
            comparisons++;
            if (cmp(a[i], target) == 0) return i;
        }
        return -1;
    }

    public static int BinarySearch<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
    {
        comparisons = 0;
        int lo = 0;
        int hi = a.Length - 1;
        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;
            int c = cmp(a[mid], target);
            comparisons++;
            if (c == 0) return mid;
            if (c < 0) lo = mid + 1;
            else hi = mid - 1;
        }
        return -1;
    }

    public static int FirstWhere<T>(T[] a, Func<T, bool> pred, out int comparisons)
    {
        comparisons = 0;
        int lo = 0;
        int hi = a.Length;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            comparisons++;
            if (pred(a[mid])) hi = mid;
            else lo = mid + 1;
        }
        return lo;
    }

    public static int LowerBound<T>(T[] a, T x, Func<T, T, int> cmp, out int comparisons)
    {
        return FirstWhere(a, e => cmp(e, x) >= 0, out comparisons);
    }

    public static int UpperBound<T>(T[] a, T x, Func<T, T, int> cmp, out int comparisons)
    {
        return FirstWhere(a, e => cmp(e, x) > 0, out comparisons);
    }

    public static int TernarySearch<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
    {
        comparisons = 0;
        int lo = 0;
        int hi = a.Length - 1;
        while (lo <= hi)
        {
            int third = (hi - lo) / 3;
            int m1 = lo + third;
            int m2 = hi - third;

            int c1 = cmp(a[m1], target);
            comparisons++;
            if (c1 == 0) return m1;
            if (c1 > 0)
            {
                hi = m1 - 1;
                continue;
            }

            if (m2 == m1)
            {
                lo = m1 + 1;
                continue;
            }

            int c2 = cmp(a[m2], target);
            comparisons++;
            if (c2 == 0) return m2;
            if (c2 > 0)
            {
                lo = m1 + 1;
                hi = m2 - 1;
            }
            else
            {
                lo = m2 + 1;
            }
        }
        return -1;
    }
}