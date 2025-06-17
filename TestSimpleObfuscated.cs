using System;

namespace TestSimpleObfuscatedApp
{
    public class Program  
    {
        // Обфусцированные строки
        private static string str1 = "XOR:SGVsbG8gV29ybGQ="; // Hello World
        private static string str2 = "B64:VGVzdCBzdHJpbmc=";   // Test string
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting simple obfuscated test...");
            
            // Простая арифметическая обфускация  
            int result = ((5 + 3) * 2) + 1;
            Console.WriteLine("Result: " + result);
            
            // Обфусцированные строки
            Console.WriteLine(str1);
            Console.WriteLine(str2);
            
            // Junk код
            int temp = 0;
            temp = temp;
            temp = temp + 0;
            
            Console.WriteLine("Test completed!");
        }
    }
} 