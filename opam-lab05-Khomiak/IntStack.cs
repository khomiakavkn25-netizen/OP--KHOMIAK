using System;

namespace OpamLab05;

internal sealed class IntStack
{
    private int[] items;
    private int count;

    public IntStack(int capacity)
    {
        if (capacity < 1)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Місткість має бути додатною.");

        items = new int[capacity];
        count = 0;
    }

    public int Count
    {
        get { return count; }
    }

    public int Capacity
    {
        get { return items.Length; }
    }

    public void Push(int x)
    {
        if (count == items.Length)
        {
            int[] bigger = new int[items.Length * 2];
            for (int i = 0; i < count; i++)
                bigger[i] = items[i];
            items = bigger;
        }

        items[count] = x;
        count++;
    }

    public int Pop()
    {
        if (count == 0)
            throw new InvalidOperationException("Стек порожній.");

        count--;
        return items[count];
    }

    public int Peek()
    {
        if (count == 0)
            throw new InvalidOperationException("Стек порожній.");

        return items[count - 1];
    }
}