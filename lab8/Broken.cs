using System;

namespace OpamLab08;

static class Broken
{
    public static int BinaryLoLtHi<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
    {
        comparisons = 0;
        int lo = 0;
        int hi = a.Length - 1;
        while (lo < hi)
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

    public static int BinaryLoMid<T>(T[] a, T target, Func<T, T, int> cmp, int maxIterations,
        out int comparisons, out int lo, out int hi, out int idleSteps)
    {
        comparisons = 0;
        idleSteps = 0;
        lo = 0;
        hi = a.Length - 1;
        int iterations = 0;
        while (lo <= hi && iterations < maxIterations)
        {
            iterations++;
            int prevLo = lo;
            int prevHi = hi;
            int mid = lo + (hi - lo) / 2;
            int c = cmp(a[mid], target);
            comparisons++;
            if (c == 0) return mid;
            if (c < 0) lo = mid;
            else hi = mid - 1;
            if (lo == prevLo && hi == prevHi) idleSteps++;
        }
        return -1;
    }

    public static int MidSum(int lo, int hi)
    {
        return unchecked((lo + hi) / 2);
    }

    public static int BinaryOverflowMid<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
    {
        comparisons = 0;
        int lo = 0;
        int hi = a.Length - 1;
        while (lo <= hi)
        {
            int mid = MidSum(lo, hi);
            int c = cmp(a[mid], target);
            comparisons++;
            if (c == 0) return mid;
            if (c < 0) lo = mid + 1;
            else hi = mid - 1;
        }
        return -1;
    }
}