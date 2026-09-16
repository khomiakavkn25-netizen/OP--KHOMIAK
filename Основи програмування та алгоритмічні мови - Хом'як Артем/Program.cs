using System;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            int N = 7;
            int K = 1;
            int a = N % 3;
            int b = N % 4;
            int c = N % 5;

            Console.WriteLine($"Студент: Артем. Варіант: N={N}, K={K}, a={a}, b={b}, c={c}\n");

            while (true)
            {
                Console.WriteLine("=== МЕНЮ ===");
                Console.WriteLine("1 - Завдання 1 (Перевірка дати)");
                Console.WriteLine("2 - Завдання 2 (Добуток чисел до появи 0)");
                Console.WriteLine("4 - Завдання 4 (НСД двома способами)");
                Console.WriteLine("0 - Вихід");
                int choice = ReadInt("Оберіть завдання: ");

                if (choice == 0) break;
                else if (choice == 1) Task1();
                else if (choice == 2) Task2(K);
                else if (choice == 4) Task4(N);
                else Console.WriteLine("Невірний вибір.");
                
                Console.WriteLine(new string('-', 30));
            }
        }

        static int ReadInt(string prompt)
        {
            int result;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    return result;
                }
                Console.WriteLine("Помилка: введіть коректне ціле число!");
            }
        }

        static void Task1()
        {
            Console.WriteLine("\n--- Завдання 1 ---");
            int day = ReadInt("Введіть день: ");
            int month = ReadInt("Введіть місяць: ");
            int year = ReadInt("Введіть рік: ");

            bool isLeap = (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
            
            int daysInMonth = 31;
            if (month == 4 || month == 6 || month == 9 || month == 11) daysInMonth = 30;
            else if (month == 2) daysInMonth = isLeap ? 29 : 28;

            if (month < 1 || month > 12 || day < 1 || day > daysInMonth)
            {
                Console.WriteLine("Результат: Дата некоректна.");
            }
            else
            {
                Console.WriteLine("Результат: Дата коректна.");

                string season = "";
                if (month == 12 || month == 1 || month == 2) season = "Зима";
                else if (month >= 3 && month <= 5) season = "Весна";
                else if (month >= 6 && month <= 8) season = "Літо";
                else season = "Осінь";

                Console.WriteLine($"Пора року: {season}");

                int dayOfYear = day;
                for (int m = 1; m < month; m++)
                {
                    int d = 31;
                    if (m == 4 || m == 6 || m == 9 || m == 11) d = 30;
                    else if (m == 2) d = isLeap ? 29 : 28;
                    dayOfYear += d;
                }
                Console.WriteLine($"Номер дня від початку року: {dayOfYear}");
            }
        }

        static void Task2(int K)
        {
            Console.WriteLine("\n--- Завдання 2 ---");
            Console.WriteLine("Вводьте цілі числа. Для завершення введіть 0.");

            long sum = 0;
            int zerosCount = 0; 
            long product = 1;
            bool foundAny = false;

            while (true)
            {
                int num = ReadInt("Введіть число: ");
                
                if (num == 0) 
                {
                    break;
                }

                sum += num;
                
                if (num % K == 0)
                {
                    product *= num;
                    foundAny = true;
                }
            }

            Console.WriteLine($"\nСума всіх елементів: {sum}");
            Console.WriteLine($"Кількість нулів у послідовності: {zerosCount}");

            if (foundAny)
            {
                Console.WriteLine($"Добуток елементів, кратних {K}: {product}");
            }
            else
            {
                Console.WriteLine($"Немає даних (не введено жодного числа для обчислення добутку).");
            }
        }

        static void Task4(int N)
        {
            Console.WriteLine("\n--- Завдання 4 ---");
            int A = 1000000 + N;
            int B = 999999 - N;
            Console.WriteLine($"A = {A}, B = {B}");

            int min = A < B ? A : B;
            int gcd1 = 1;
            long iters1 = 0;
            for (int i = min; i >= 1; i--)
            {
                iters1++;
                if (A % i == 0 && B % i == 0)
                {
                    gcd1 = i;
                    break;
                }
            }
            Console.WriteLine($"Спосіб 1 (Перебір дільників): НСД = {gcd1}, Ітерацій циклу = {iters1}");

            int x = A;
            int y = B;
            long iters2 = 0;
            while (y != 0)
            {
                iters2++;
                int temp = y;
                y = x % y;
                x = temp;
            }
            int gcd2 = x;
            Console.WriteLine($"Спосіб 2 (Алгоритм Евкліда): НСД = {gcd2}, Ітерацій циклу = {iters2}");
        }
    }
}