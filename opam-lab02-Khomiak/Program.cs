using System;

namespace OpamLab02
{
    internal class Program
    {
        private const int N = 27;
        private const int K = 26;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=================================================");
            Console.WriteLine($"Лабораторна робота 2. Варіант:");
            Console.WriteLine($"N = {N}, K = {K}, a = {N % 3}, b = {N % 4}, c = {N % 5}");
            Console.WriteLine("=================================================\n");

            Random rnd = new Random(N);
            int[] arr = GenerateRandomArray(18, -3, 3, rnd);

            Console.WriteLine("--- Початковий масив ---");
            PrintArray("Масив", arr);
            Console.WriteLine();

            Console.WriteLine("--- Завдання 1 · Агрегація ---");
            Task1_Aggregation(arr);
            Console.WriteLine();

            Console.WriteLine("--- Завдання 2 · Фільтрація ---");
            int[] filteredArr = Task2_Filter(arr);
            PrintArray($"Відфільтрований масив (елементи > {K % 5})", filteredArr);
            Console.WriteLine($"Довжина відфільтрованого масиву: {filteredArr.Length}");
            Console.WriteLine();

            Console.WriteLine("--- Завдання 3 · Пошук найдовшої серії ---");
            Task3_Series(arr);
            Console.WriteLine();

            Console.WriteLine("--- Завдання 4 · Перестановка на місці ---");
            PrintArray("До перестановки ", arr);
            Task4_Permutation(arr);
            PrintArray("Після перестановки", arr);
            Console.WriteLine();

            Console.WriteLine("--- Завдання 5 · Матриця (6x3) ---");
            Task5_Matrix(6, 3, -3, 3, rnd);
            Console.WriteLine();

            Console.WriteLine("--- Завдання 6 · Крайові випадки ---");
            Task6_EdgeCases();
            Console.WriteLine();

            Console.WriteLine("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }

        public static int[] GenerateRandomArray(int length, int min, int max, Random rnd)
        {
            int[] result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = rnd.Next(min, max + 1);
            }
            return result;
        }

        public static void PrintArray(string label, int[] arr)
        {
            Console.Write($"{label}: [");
            if (arr != null && arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    Console.Write(arr[i]);
                    if (i < arr.Length - 1)
                    {
                        Console.Write(", ");
                    }
                }
            }
            Console.WriteLine("]");
        }

        public static void Task1_Aggregation(int[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                Console.WriteLine("Масив порожній. Результат агрегації не визначено.");
                return;
            }

            int sum = 0;
            int min = arr[0];
            int minIndex = 0;
            int max = arr[0];
            int maxIndex = 0;
            int zeroCount = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                int val = arr[i];
                sum += val;

                if (val == 0)
                {
                    zeroCount++;
                }

                if (val < min)
                {
                    min = val;
                    minIndex = i;
                }

                if (val > max)
                {
                    max = val;
                    maxIndex = i;
                }
            }

            double average = (double)sum / arr.Length;

            Console.WriteLine($"Сума елементів: {sum}");
            Console.WriteLine($"Середнє арифметичне: {average:F3}");
            Console.WriteLine($"Мінімум: {min} (індекс: {minIndex})");
            Console.WriteLine($"Максимум: {max} (перше входження, індекс: {maxIndex})");
            Console.WriteLine($"Кількість нулів: {zeroCount}");
        }

        public static int[] Task2_Filter(int[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                return new int[0];
            }

            int threshold = K % 5;

            int count = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > threshold)
                {
                    count++;
                }
            }

            int[] result = new int[count];
            int resultIndex = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > threshold)
                {
                    result[resultIndex] = arr[i];
                    resultIndex++;
                }
            }

            return result;
        }

        public static void Task3_Series(int[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                Console.WriteLine("Масив порожній. Серії відсутні.");
                return;
            }

            int maxVal = arr[0];
            int maxLen = 1;
            int maxStart = 0;

            int currentVal = arr[0];
            int currentLen = 1;
            int currentStart = 0;

            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] == currentVal)
                {
                    currentLen++;
                }
                else
                {
                    if (currentLen > maxLen)
                    {
                        maxLen = currentLen;
                        maxVal = currentVal;
                        maxStart = currentStart;
                    }
                    currentVal = arr[i];
                    currentLen = 1;
                    currentStart = i;
                }
            }

            if (currentLen > maxLen)
            {
                maxLen = currentLen;
                maxVal = currentVal;
                maxStart = currentStart;
            }

            Console.WriteLine($"Найдовша серія одинакових елементів:");
            Console.WriteLine($"Значення: {maxVal}, Довжина серії: {maxLen}, Індекс початку: {maxStart}");
        }

        public static void Task4_Permutation(int[] arr)
        {
            if (arr == null || arr.Length <= 1)
            {
                return;
            }

            int temp = arr[arr.Length - 1];
            for (int i = arr.Length - 1; i > 0; i--)
            {
                arr[i] = arr[i - 1];
            }
            arr[0] = temp;
        }

        public static void Task5_Matrix(int rows, int cols, int min, int max, Random rnd)
        {
            int[,] matrix = new int[rows, cols];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = rnd.Next(min, max + 1);
                }
            }

            Console.WriteLine("Матриця:");
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write($"{matrix[i, j],5}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            int[] rowSums = new int[matrix.GetLength(0)];
            int maxRowIndex = 0;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                int currentSum = 0;
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    currentSum += matrix[i, j];
                }
                rowSums[i] = currentSum;

                if (i == 0 || currentSum > rowSums[maxRowIndex])
                {
                    maxRowIndex = i;
                }
            }

            int[] colMaxs = new int[matrix.GetLength(1)];
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                int colMax = matrix[0, j];
                for (int i = 1; i < matrix.GetLength(0); i++)
                {
                    if (matrix[i, j] > colMax)
                    {
                        colMax = matrix[i, j];
                    }
                }
                colMaxs[j] = colMax;
            }

            for (int i = 0; i < rowSums.Length; i++)
            {
                Console.WriteLine($"Сума рядка {i}: {rowSums[i]}");
            }
            Console.WriteLine($"---> Рядок з найбільшою сумою: {maxRowIndex} (сума = {rowSums[maxRowIndex]})\n");

            for (int j = 0; j < colMaxs.Length; j++)
            {
                Console.WriteLine($"Максимум стовпця {j}: {colMaxs[j]}");
            }
        }

        public static void Task6_EdgeCases()
        {
            Console.WriteLine("=== Тест 1: Порожній масив ===");
            int[] empty = new int[0];
            PrintArray("Вхід", empty);
            Task1_Aggregation(empty);
            int[] filteredEmpty = Task2_Filter(empty);
            PrintArray("Завдання 2 (Фільтр)", filteredEmpty);
            Task3_Series(empty);
            Task4_Permutation(empty);
            PrintArray("Завдання 4 (Після зсуву)", empty);
            Console.WriteLine();

            Console.WriteLine("=== Тест 2: Один елемент ===");
            int[] single = new int[] { 5 };
            PrintArray("Вхід", single);
            Task1_Aggregation(single);
            int[] filteredSingle = Task2_Filter(single);
            PrintArray("Завдання 2 (Фільтр)", filteredSingle);
            Task3_Series(single);
            Task4_Permutation(single);
            PrintArray("Завдання 4 (Після зсуву)", single);
            Console.WriteLine();

            Console.WriteLine("=== Тест 3: Усі елементи однакові ===");
            int[] identical = new int[] { 3, 3, 3, 3 };
            PrintArray("Вхід", identical);
            Task1_Aggregation(identical);
            int[] filteredIdentical = Task2_Filter(identical);
            PrintArray("Завдання 2 (Фільтр)", filteredIdentical);
            Task3_Series(identical);
            Task4_Permutation(identical);
            PrintArray("Завдання 4 (Після зсуву)", identical);
        }
    }
}