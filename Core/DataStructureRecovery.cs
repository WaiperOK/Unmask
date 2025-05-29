using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Unmask
{
    public static class DataStructureRecovery
    {
        public static void RecoverDataStructures(ModuleDefMD module)
        {
            try
            {
                RecoverArrays(module);
                RecoverCollections(module);
                RecoverCustomStructures(module);
                RestoreFieldTypes(module);
                FixPropertyAccessors(module);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to recover data structures: {ex.Message}");
            }
        }

        private static void RecoverArrays(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions == null) continue;

                    var instructions = method.Body.Instructions;
                    for (int i = 0; i < instructions.Count - 2; i++)
                    {
                        if (IsObfuscatedArrayAccess(instructions, i))
                        {
                            RestoreArrayAccess(instructions, i);
                        }
                    }
                }
            }
        }

        private static bool IsObfuscatedArrayAccess(IList<Instruction> instructions, int index)
        {
            if (index + 2 >= instructions.Count) return false;

            var inst1 = instructions[index];
            var inst2 = instructions[index + 1];
            var inst3 = instructions[index + 2];

            return inst1.OpCode == OpCodes.Ldloc &&
                   inst2.OpCode == OpCodes.Ldc_I4 &&
                   inst3.OpCode == OpCodes.Ldelem_Ref;
        }

        private static void RestoreArrayAccess(IList<Instruction> instructions, int index)
        {
            var arrayLoad = instructions[index];
            var indexLoad = instructions[index + 1];
            var elementLoad = instructions[index + 2];

            if (indexLoad.Operand is int constantIndex)
            {
                instructions[index + 1] = Instruction.Create(OpCodes.Ldc_I4, constantIndex);
            }
        }

        private static void RecoverCollections(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var field in type.Fields)
                {
                    if (IsObfuscatedCollection(field))
                    {
                        RestoreCollectionType(field);
                    }
                }
            }
        }

        private static bool IsObfuscatedCollection(FieldDef field)
        {
            return field.FieldType.FullName.Contains("System.Object") &&
                   field.Name.Length == 1;
        }

        private static void RestoreCollectionType(FieldDef field)
        {
            var module = field.Module;
            var listType = module.CorLibTypes.Object;
            field.FieldType = listType;
        }

        private static void RecoverCustomStructures(ModuleDefMD module)
        {
            var obfuscatedTypes = new List<TypeDef>();

            foreach (var type in module.Types)
            {
                if (IsObfuscatedStructure(type))
                {
                    obfuscatedTypes.Add(type);
                }
            }

            foreach (var type in obfuscatedTypes)
            {
                RestoreStructure(type);
            }
        }

        private static bool IsObfuscatedStructure(TypeDef type)
        {
            return type.Name.Length == 1 &&
                   type.Fields.Count > 0 &&
                   type.Fields.All(f => f.Name.Length == 1);
        }

        private static void RestoreStructure(TypeDef type)
        {
            type.Name = $"RestoredStruct_{type.MDToken.Raw:X8}";

            int fieldIndex = 0;
            foreach (var field in type.Fields)
            {
                field.Name = $"Field_{fieldIndex++}";
            }

            int methodIndex = 0;
            foreach (var method in type.Methods)
            {
                if (method.Name.Length == 1)
                {
                    method.Name = $"Method_{methodIndex++}";
                }
            }
        }

        private static void RestoreFieldTypes(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var field in type.Fields)
                {
                    if (IsObfuscatedFieldType(field))
                    {
                        RestoreFieldType(field);
                    }
                }
            }
        }

        private static bool IsObfuscatedFieldType(FieldDef field)
        {
            return field.FieldType.FullName == "System.Object" &&
                   field.HasCustomAttributes;
        }

        private static void RestoreFieldType(FieldDef field)
        {
            foreach (var attr in field.CustomAttributes)
            {
                if (attr.TypeFullName.Contains("Type"))
                {
                    if (attr.ConstructorArguments.Count > 0)
                    {
                        var typeArg = attr.ConstructorArguments[0];
                        if (typeArg.Value is TypeSig typeSig)
                        {
                            field.FieldType = typeSig;
                        }
                    }
                }
            }
        }

        private static void FixPropertyAccessors(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var property in type.Properties)
                {
                    if (property.GetMethod != null && IsObfuscatedAccessor(property.GetMethod))
                    {
                        RestoreAccessor(property.GetMethod, true);
                    }

                    if (property.SetMethod != null && IsObfuscatedAccessor(property.SetMethod))
                    {
                        RestoreAccessor(property.SetMethod, false);
                    }
                }
            }
        }

        private static bool IsObfuscatedAccessor(MethodDef method)
        {
            if (method.Body?.Instructions == null) return false;

            var instructions = method.Body.Instructions;
            return instructions.Count > 10 &&
                   instructions.Count(i => i.OpCode == OpCodes.Call) > 3;
        }

        private static void RestoreAccessor(MethodDef method, bool isGetter)
        {
            if (method.Body?.Instructions == null) return;

            var instructions = method.Body.Instructions;
            instructions.Clear();

            if (isGetter)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                instructions.Add(Instruction.Create(OpCodes.Ldfld, GetBackingField(method)));
                instructions.Add(Instruction.Create(OpCodes.Ret));
            }
            else
            {
                instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
                instructions.Add(Instruction.Create(OpCodes.Stfld, GetBackingField(method)));
                instructions.Add(Instruction.Create(OpCodes.Ret));
            }
        }

        private static FieldDef GetBackingField(MethodDef method)
        {
            var propertyName = method.Name.Replace("get_", "").Replace("set_", "");
            var backingFieldName = $"<{propertyName}>k__BackingField";

            var backingField = method.DeclaringType.Fields
                .FirstOrDefault(f => f.Name == backingFieldName);

            if (backingField == null)
            {
                backingField = method.DeclaringType.Fields.FirstOrDefault();
            }

            return backingField;
        }

        public static void RemoveJunkCode(ModuleDefMD module)
        {
            try
            {
                RemoveEmptyMethods(module);
                RemoveUnusedFields(module);
                RemoveDeadCode(module);
                SimplifyBranches(module);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove junk code: {ex.Message}");
            }
        }

        private static void RemoveEmptyMethods(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                var methodsToRemove = new List<MethodDef>();

                foreach (var method in type.Methods)
                {
                    if (IsEmptyMethod(method))
                    {
                        methodsToRemove.Add(method);
                    }
                }

                foreach (var method in methodsToRemove)
                {
                    type.Methods.Remove(method);
                }
            }
        }

        private static bool IsEmptyMethod(MethodDef method)
        {
            if (method.Body?.Instructions == null) return false;

            var instructions = method.Body.Instructions;
            return instructions.Count == 1 && instructions[0].OpCode == OpCodes.Ret;
        }

        private static void RemoveUnusedFields(ModuleDefMD module)
        {
            var usedFields = new HashSet<FieldDef>();

            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions == null) continue;

                    foreach (var instruction in method.Body.Instructions)
                    {
                        if (instruction.Operand is FieldDef field)
                        {
                            usedFields.Add(field);
                        }
                    }
                }
            }

            foreach (var type in module.Types)
            {
                var fieldsToRemove = type.Fields
                    .Where(f => !usedFields.Contains(f))
                    .ToList();

                foreach (var field in fieldsToRemove)
                {
                    type.Fields.Remove(field);
                }
            }
        }

        private static void RemoveDeadCode(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions == null) continue;

                    RemoveDeadInstructions(method);
                }
            }
        }

        private static void RemoveDeadInstructions(MethodDef method)
        {
            var instructions = method.Body.Instructions;
            var toRemove = new List<Instruction>();

            for (int i = 0; i < instructions.Count - 1; i++)
            {
                var current = instructions[i];
                var next = instructions[i + 1];

                if (current.OpCode == OpCodes.Pop && 
                    (next.OpCode == OpCodes.Ldnull || next.OpCode == OpCodes.Ldc_I4_0))
                {
                    toRemove.Add(current);
                    toRemove.Add(next);
                }
            }

            foreach (var instruction in toRemove)
            {
                instructions.Remove(instruction);
            }
        }

        private static void SimplifyBranches(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions == null) continue;

                    SimplifyMethodBranches(method);
                }
            }
        }

        private static void SimplifyMethodBranches(MethodDef method)
        {
            var instructions = method.Body.Instructions;

            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];

                if (instruction.OpCode == OpCodes.Br)
                {
                    var target = instruction.Operand as Instruction;
                    if (target != null && instructions.IndexOf(target) == i + 1)
                    {
                        instructions.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
} 