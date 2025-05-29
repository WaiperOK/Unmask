using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Unmask;

namespace CustomScripts
{
    /// <summary>
    /// Продвинутый скрипт удаления виртуальных машин
    /// </summary>
    public class AdvancedVMRemoval : IDeobfuscationScript
    {
        public string Name => "AdvancedVMRemoval";
        public string Description => "Продвинутое удаление виртуальных машин с восстановлением оригинальных методов";
        public string Version => "2.0";

        private readonly Dictionary<string, MethodDef> vmHandlers = new Dictionary<string, MethodDef>();
        private readonly List<TypeDef> vmTypes = new List<TypeDef>();

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Запуск продвинутого удаления виртуальных машин...");
            
            // Этап 1: Обнаружение VM компонентов
            DetectVMComponents(module, logger);
            
            // Этап 2: Анализ VM обработчиков
            AnalyzeVMHandlers(module, logger);
            
            // Этап 3: Восстановление виртуализированных методов
            RestoreVirtualizedMethods(module, logger);
            
            // Этап 4: Удаление VM инфраструктуры
            RemoveVMInfrastructure(module, logger);
            
            // Этап 5: Очистка и оптимизация
            CleanupAndOptimize(module, logger);
            
            logger.Success("Продвинутое удаление виртуальных машин завершено");
        }

        private void DetectVMComponents(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Обнаружение VM компонентов...");
            
            var vmSignatures = new[] { "VM", "Virtual", "Handler", "Opcode", "Context", "Stack", "Interpreter" };
            int detectedComponents = 0;

            foreach (var type in module.Types)
            {
                bool isVMType = false;

                // Проверка имени типа
                if (vmSignatures.Any(sig => type.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    isVMType = true;
                }

                // Проверка паттернов VM
                if (!isVMType && IsVMTypeByPattern(type))
                {
                    isVMType = true;
                }

                if (isVMType)
                {
                    vmTypes.Add(type);
                    detectedComponents++;
                    logger.Info($"Обнаружен VM тип: {type.Name}");
                }
            }

            logger.Info($"Обнаружено {detectedComponents} VM компонентов");
        }

        private bool IsVMTypeByPattern(TypeDef type)
        {
            // Проверяем соотношение методов-обработчиков
            if (type.Methods.Count < 10) return false;

            int handlerMethods = 0;
            int switchMethods = 0;

            foreach (var method in type.Methods)
            {
                if (!method.HasBody) continue;

                var instructions = method.Body.Instructions;
                if (instructions.Count < 5 || instructions.Count > 50) continue;

                // Подсчет switch инструкций
                int switchCount = instructions.Count(i => i.OpCode == OpCodes.Switch);
                if (switchCount > 0)
                {
                    switchMethods++;
                    if (IsVMHandler(method))
                    {
                        handlerMethods++;
                    }
                }
            }

            // VM тип обычно содержит много обработчиков
            return handlerMethods > type.Methods.Count * 0.6 || switchMethods > type.Methods.Count * 0.4;
        }

        private bool IsVMHandler(MethodDef method)
        {
            if (!method.HasBody) return false;

            var instructions = method.Body.Instructions;
            
            // Характеристики VM обработчика
            int switchCount = instructions.Count(i => i.OpCode == OpCodes.Switch);
            int branchCount = instructions.Count(i => i.OpCode.FlowControl == FlowControl.Cond_Branch);
            int callCount = instructions.Count(i => i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Callvirt);

            return switchCount > 0 && branchCount > instructions.Count * 0.3 && callCount < 5;
        }

        private void AnalyzeVMHandlers(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Анализ VM обработчиков...");
            
            int analyzedHandlers = 0;

            foreach (var vmType in vmTypes)
            {
                foreach (var method in vmType.Methods)
                {
                    if (IsVMHandler(method))
                    {
                        var handlerKey = $"{vmType.Name}::{method.Name}";
                        vmHandlers[handlerKey] = method;
                        analyzedHandlers++;
                        
                        logger.Info($"Проанализирован обработчик: {handlerKey}");
                    }
                }
            }

            logger.Info($"Проанализировано {analyzedHandlers} VM обработчиков");
        }

        private void RestoreVirtualizedMethods(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Восстановление виртуализированных методов...");
            
            int restoredMethods = 0;

            foreach (var type in module.Types)
            {
                if (vmTypes.Contains(type)) continue;

                foreach (var method in type.Methods)
                {
                    if (IsVirtualizedMethod(method))
                    {
                        if (TryRestoreMethod(method, logger))
                        {
                            restoredMethods++;
                        }
                    }
                }
            }

            logger.Success($"Восстановлено {restoredMethods} виртуализированных методов");
        }

        private bool IsVirtualizedMethod(MethodDef method)
        {
            if (!method.HasBody) return false;

            var instructions = method.Body.Instructions;
            
            // Паттерн виртуализированного метода: загрузка аргументов + вызов VM + возврат
            if (instructions.Count >= 3 && instructions.Count <= 10)
            {
                var lastInst = instructions[instructions.Count - 1];
                if (lastInst.OpCode == OpCodes.Ret)
                {
                    // Поиск вызова VM
                    foreach (var inst in instructions)
                    {
                        if (inst.OpCode == OpCodes.Call && inst.Operand is MethodDef target)
                        {
                            if (vmHandlers.Values.Contains(target) || IsVMMethod(target))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool IsVMMethod(MethodDef method)
        {
            var vmSignatures = new[] { "VM", "Virtual", "Handler", "Execute", "Dispatch" };
            return vmSignatures.Any(sig => method.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool TryRestoreMethod(MethodDef method, IScriptLogger logger)
        {
            try
            {
                // Попытка восстановления через анализ VM вызовов
                var instructions = method.Body.Instructions;
                
                foreach (var inst in instructions)
                {
                    if (inst.OpCode == OpCodes.Call && inst.Operand is MethodDef vmMethod)
                    {
                        var originalMethod = FindOriginalMethodInVM(vmMethod);
                        if (originalMethod != null)
                        {
                            // Заменяем вызов VM на вызов оригинального метода
                            inst.Operand = originalMethod;
                            logger.Info($"Восстановлен метод: {method.FullName}");
                            return true;
                        }
                    }
                }

                // Альтернативный подход: восстановление через паттерны
                return RestoreMethodByPattern(method, logger);
            }
            catch (Exception ex)
            {
                logger.Warning($"Ошибка восстановления метода {method.Name}: {ex.Message}");
                return false;
            }
        }

        private MethodDef FindOriginalMethodInVM(MethodDef vmMethod)
        {
            if (!vmMethod.HasBody) return null;

            // Поиск вызова оригинального метода в теле VM обработчика
            foreach (var inst in vmMethod.Body.Instructions)
            {
                if (inst.OpCode == OpCodes.Call && inst.Operand is MethodDef target)
                {
                    // Проверяем, что это не VM метод
                    if (!IsVMMethod(target) && !vmHandlers.Values.Contains(target))
                    {
                        return target;
                    }
                }
            }

            return null;
        }

        private bool RestoreMethodByPattern(MethodDef method, IScriptLogger logger)
        {
            // Простое восстановление: удаление VM вызовов и создание заглушки
            var instructions = method.Body.Instructions;
            bool modified = false;

            for (int i = instructions.Count - 1; i >= 0; i--)
            {
                var inst = instructions[i];
                if (inst.OpCode == OpCodes.Call && inst.Operand is MethodDef target && IsVMMethod(target))
                {
                    // Удаляем VM вызов
                    instructions.RemoveAt(i);
                    modified = true;
                }
            }

            if (modified)
            {
                // Добавляем простой возврат, если метод стал пустым
                if (instructions.Count == 0 || instructions[instructions.Count - 1].OpCode != OpCodes.Ret)
                {
                    if (method.ReturnType.FullName != "System.Void")
                    {
                        // Возвращаем значение по умолчанию
                        if (method.ReturnType.FullName == "System.Int32")
                        {
                            instructions.Add(Instruction.CreateLdcI4(0));
                        }
                        else if (method.ReturnType.FullName == "System.String")
                        {
                            instructions.Add(Instruction.Create(OpCodes.Ldstr, ""));
                        }
                        else
                        {
                            instructions.Add(Instruction.Create(OpCodes.Ldnull));
                        }
                    }
                    instructions.Add(Instruction.Create(OpCodes.Ret));
                }

                logger.Info($"Восстановлен метод по паттерну: {method.Name}");
                return true;
            }

            return false;
        }

        private void RemoveVMInfrastructure(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Удаление VM инфраструктуры...");
            
            int removedTypes = 0;

            // Удаляем VM типы
            foreach (var vmType in vmTypes.ToList())
            {
                if (module.Types.Contains(vmType))
                {
                    module.Types.Remove(vmType);
                    removedTypes++;
                    logger.Info($"Удален VM тип: {vmType.Name}");
                }
            }

            logger.Success($"Удалено {removedTypes} VM типов");
        }

        private void CleanupAndOptimize(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Очистка и оптимизация...");
            
            int optimizedMethods = 0;

            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;

                    // Удаление NOP инструкций
                    var instructions = method.Body.Instructions;
                    bool modified = false;

                    for (int i = instructions.Count - 1; i >= 0; i--)
                    {
                        if (instructions[i].OpCode == OpCodes.Nop)
                        {
                            instructions.RemoveAt(i);
                            modified = true;
                        }
                    }

                    if (modified)
                    {
                        method.Body.OptimizeBranches();
                        method.Body.OptimizeMacros();
                        optimizedMethods++;
                    }

                    // Настройка KeepOldMaxStack
                    method.Body.KeepOldMaxStack = true;
                }
            }

            logger.Success($"Оптимизировано {optimizedMethods} методов");
        }
    }
} 