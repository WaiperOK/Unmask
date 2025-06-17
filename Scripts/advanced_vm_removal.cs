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
    public class AdvancedVMRemovalScript : IDeobfuscationScript
    {
        public string Name => "AdvancedVMRemoval";
        public string Description => "Продвинутое удаление виртуальных машин с восстановлением методов";
        public string Version => "2.0";

        private readonly Dictionary<string, MethodDef> vmHandlers = new Dictionary<string, MethodDef>();
        private readonly List<TypeDef> vmTypes = new List<TypeDef>();

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Запуск продвинутого удаления VM...");

            try
            {
                DetectVMComponents(module, logger);
                AnalyzeVMHandlers(module, logger);
                RestoreVirtualizedMethods(module, logger);
                RemoveVMInfrastructure(module, logger);
                CleanupAndOptimize(module, logger);

                logger.Success("Продвинутое удаление VM завершено");
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка при удалении VM: " + ex.Message);
            }
        }

        private void DetectVMComponents(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Поиск компонентов виртуальной машины...");

            int vmTypesFound = 0;
            int vmHandlersFound = 0;

            foreach (var type in module.Types)
            {
                if (IsVMTypeByPattern(type))
                {
                    vmTypes.Add(type);
                    vmTypesFound++;
                    logger.Info("Найден VM тип: " + type.Name);

                    foreach (var method in type.Methods)
                    {
                        if (IsVMHandler(method))
                        {
                            vmHandlers[method.Name] = method;
                            vmHandlersFound++;
                        }
                    }
                }
            }

            logger.Info("Найдено VM типов: " + vmTypesFound + ", обработчиков: " + vmHandlersFound);
        }

        private bool IsVMTypeByPattern(TypeDef type)
        {
            if (type.Methods.Count < 5) return false;

            // Поиск характерных паттернов VM
            bool hasVMDispatcher = type.Methods.Any(m => 
                m.Body?.Instructions?.Count > 50 && 
                m.Body.Instructions.Any(i => i.OpCode == OpCodes.Switch));

            bool hasVMHandlers = type.Methods.Count(m => 
                m.Body?.Instructions?.Count > 10 && 
                m.Body?.Instructions?.Count < 30) > 3;

            bool hasVMState = type.Fields.Any(f => 
                f.FieldType.FullName.Contains("Int32") || 
                f.FieldType.FullName.Contains("Object"));

            return hasVMDispatcher && hasVMHandlers && hasVMState;
        }

        private bool IsVMHandler(MethodDef method)
        {
            if (method.Body?.Instructions == null) return false;
            if (method.Body.Instructions.Count < 5) return false;

            // Ищем типичные паттерны VM обработчиков
            var opcodes = method.Body.Instructions.Select(i => i.OpCode).ToArray();
            
            return opcodes.Contains(OpCodes.Ldloc) && 
                   opcodes.Contains(OpCodes.Stloc) &&
                   (opcodes.Contains(OpCodes.Add) || opcodes.Contains(OpCodes.Sub) || opcodes.Contains(OpCodes.Mul));
        }

        private void AnalyzeVMHandlers(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Анализ VM обработчиков...");

            foreach (var handler in vmHandlers.Values)
            {
                AnalyzeHandlerBehavior(handler, logger);
            }
        }

        private void AnalyzeHandlerBehavior(MethodDef handler, IScriptLogger logger)
        {
            if (handler.Body?.Instructions == null) return;

            var instructions = handler.Body.Instructions;
            
            // Определяем тип операции обработчика
            string operationType = "Unknown";
            
            if (instructions.Any(i => i.OpCode == OpCodes.Add))
                operationType = "Arithmetic_Add";
            else if (instructions.Any(i => i.OpCode == OpCodes.Sub))
                operationType = "Arithmetic_Sub";
            else if (instructions.Any(i => i.OpCode == OpCodes.Ldstr))
                operationType = "String_Load";
            else if (instructions.Any(i => i.OpCode == OpCodes.Call))
                operationType = "Method_Call";

            logger.Info($"Обработчик {handler.Name}: {operationType}");
        }

        private void RestoreVirtualizedMethods(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Восстановление виртуализованных методов...");

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

            logger.Success($"Восстановлено методов: {restoredMethods}");
        }

        private bool IsVirtualizedMethod(MethodDef method)
        {
            if (method.Body?.Instructions == null) return false;

            // Ищем вызовы VM диспетчера
            return method.Body.Instructions.Any(i => 
                i.OpCode == OpCodes.Call && 
                i.Operand is MethodDef target && 
                vmTypes.Any(vt => vt.Methods.Contains(target)));
        }

        private bool TryRestoreMethod(MethodDef method, IScriptLogger logger)
        {
            try
            {
                if (method.Body?.Instructions == null) return false;

                var instructions = method.Body.Instructions;
                bool modified = false;

                for (int i = 0; i < instructions.Count; i++)
                {
                    var instruction = instructions[i];
                    
                    if (instruction.OpCode == OpCodes.Call && 
                        instruction.Operand is MethodDef vmCall &&
                        vmHandlers.ContainsValue(vmCall))
                    {
                        // Попытка восстановить оригинальную операцию
                        if (RestoreOriginalOperation(instructions, i, vmCall))
                        {
                            modified = true;
                        }
                    }
                }

                return modified;
            }
            catch (Exception ex)
            {
                logger.Warning($"Не удалось восстановить метод {method.Name}: {ex.Message}");
                return false;
            }
        }

        private bool RestoreOriginalOperation(IList<Instruction> instructions, int index, MethodDef vmHandler)
        {
            // Простая эвристика для восстановления операций
            if (vmHandler.Name.Contains("Add") || vmHandler.Body.Instructions.Any(i => i.OpCode == OpCodes.Add))
            {
                instructions[index] = Instruction.Create(OpCodes.Add);
                return true;
            }
            else if (vmHandler.Name.Contains("Sub") || vmHandler.Body.Instructions.Any(i => i.OpCode == OpCodes.Sub))
            {
                instructions[index] = Instruction.Create(OpCodes.Sub);
                return true;
            }
            else if (vmHandler.Name.Contains("Mul") || vmHandler.Body.Instructions.Any(i => i.OpCode == OpCodes.Mul))
            {
                instructions[index] = Instruction.Create(OpCodes.Mul);
                return true;
            }

            return false;
        }

        private void RemoveVMInfrastructure(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Удаление VM инфраструктуры...");

            int removedTypes = 0;
            var typesToRemove = new List<TypeDef>(vmTypes);

            foreach (var vmType in typesToRemove)
            {
                try
                {
                    module.Types.Remove(vmType);
                    removedTypes++;
                    logger.Info($"Удален VM тип: {vmType.Name}");
                }
                catch (Exception ex)
                {
                    logger.Warning($"Не удалось удалить тип {vmType.Name}: {ex.Message}");
                }
            }

            logger.Success($"Удалено VM типов: {removedTypes}");
        }

        private void CleanupAndOptimize(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Очистка и оптимизация...");

            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions != null)
                    {
                        CleanupMethodInstructions(method);
                    }
                }
            }

            logger.Success("Очистка завершена");
        }

        private void CleanupMethodInstructions(MethodDef method)
        {
            var instructions = method.Body.Instructions;
            var toRemove = new List<Instruction>();

            // Удаляем NOP инструкции
            foreach (var instruction in instructions)
            {
                if (instruction.OpCode == OpCodes.Nop)
                {
                    toRemove.Add(instruction);
                }
            }

            foreach (var instruction in toRemove)
            {
                instructions.Remove(instruction);
            }
        }
    }
} 