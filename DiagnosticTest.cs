using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DiagnosticTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: DiagnosticTest.exe <file.exe>");
                return;
            }

            try
            {
                var module = ModuleDefMD.Load(args[0]);
                Console.WriteLine($"Анализ файла: {args[0]}");
                Console.WriteLine($"Модуль: {module.Name}");
                Console.WriteLine();
                
                AnalyzeMethodCalls(module);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void AnalyzeMethodCalls(ModuleDefMD module)
        {
            Console.WriteLine("=== АНАЛИЗ ВЫЗОВОВ МЕТОДОВ ===");
            
            foreach (var type in module.Types)
            {
                Console.WriteLine($"\nТип: {type.FullName}");
                
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    
                    Console.WriteLine($"  Метод: {method.Name}");
                    
                    foreach (var instruction in method.Body.Instructions)
                    {
                        if (instruction.OpCode == OpCodes.Call && instruction.Operand is MethodDef calledMethod)
                        {
                            Console.WriteLine($"    Вызывает: {calledMethod.FullName}");
                            
                            if (calledMethod.DeclaringType?.Module == module)
                            {
                                Console.WriteLine("      *** ВНУТРЕННИЙ ВЫЗОВ ***");
                            }
                        }
                    }
                }
            }
        }
    }
} 