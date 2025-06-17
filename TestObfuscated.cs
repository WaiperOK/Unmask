using System;

namespace TestObfuscatedApp
{
    class Program
    {
        // Имитация обфусцированных строк
        private static string str1 = "XOR:SGVsbG8sIFdvcmxkIQ=="; // "Hello, World!" в XOR+Base64
        private static string str2 = "B64:VGVzdCBzdHJpbmc="; // "Test string" в Base64
        private static string str3 = "ROT:Uryyb"; // "Hello" в ROT13
        
        // Обфусцированные методы
        static void Main(string[] a)
        {
            A(); // Обфусцированное имя метода
            B(10, 20);
            C();
        }
        
        // Имитация обфусцированного метода
        static void A()
        {
            Console.WriteLine(str1);
            Console.WriteLine(str2);
            // Много NOP операций (имитация мусорного кода)
            int x = 1;
            x = x;
            x = x;
            x = x;
        }
        
        // Метод с арифметической обфускацией
        static void B(int x, int y)
        {
            // Сложные арифметические операции, которые можно упростить
            int result = ((x * 2) + (y * 2)) / 2;
            result = result + 0; // Бесполезная операция
            result = result * 1; // Еще одна бесполезная операция
            
            Console.WriteLine("ROT:Erfhyg vf: " + result); // "Result is: " в ROT13
        }
        
        // Пустой метод (мусор)
        static void C()
        {
            // Этот метод должен быть удален как пустой
        }
        
        // Имитация прокси-метода
        static int ProxyMeth_1234()
        {
            return 42; // Константа
        }
        
        // Еще один прокси-метод
        static string ProxyMeth_5678()
        {
            return "B64:SGVsbG8gZnJvbSBwcm94eQ=="; // "Hello from proxy" в Base64
        }
        
        // Метод с запутанным потоком управления
        static void D()
        {
            bool flag = true;
            if (flag)
            {
                goto Label1;
            }
            Console.WriteLine("This should not execute");
            
            Label1:
            Console.WriteLine("XOR:VGhpcyBpcyBvYmZ1c2NhdGVk"); // Обфусцированная строка
        }
    }
    
    // Имитация класса с обфусцированными именами
    class a
    {
        public string b; // Обфусцированное поле
        public int c;    // Еще одно обфусцированное поле
        
        public void d() // Обфусцированный метод
        {
            b = "ROT:Bowrepvfrq";
            c = ProxyCalculation();
        }
        
        private int ProxyCalculation()
        {
            // Сложное вычисление, которое всегда возвращает 100
            return ((10 * 10) + (5 * 0)) * 1;
        }
    }
} 