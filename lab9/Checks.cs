using System;

namespace OpamLab09;

static class Checks
{

    public static bool IsSorted<T>(T[] a, Func<T, T, int> cmp)
    {
        for (int i = 1; i < a.Length; i++)
        {
            if (cmp(a[i - 1], a[i]) > 0) return false;
        }
        return true;
    }

    public static bool IsPermutationOf<T>(T[] result, T[] original, Func<T, T, int> cmp)
    {
        if (result.Length != original.Length) return false;
        T[] x = (T[])result.Clone();
        T[] y = (T[])original.Clone();
        Array.Sort(x, (p, q) => cmp(p, q));
        Array.Sort(y, (p, q) => cmp(p, q));
        for (int i = 0; i < x.Length; i++)
        {
            if (cmp(x[i], y[i]) != 0) return false;
        }
        return true;
    }
}