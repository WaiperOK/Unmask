using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Unmask
{
    public static class VirtualMachineDetector
    {
        private static readonly string[] VMSignatures = {
            "VM_", "Virtual", "Handler", "Opcode", "Context", "Stack", "Interpreter",
            "Execute", "Dispatch", "Runtime", "Engine", "Machine", "Processor"
        };

        private static readonly OpCode[] VMOpCodes = {
            OpCodes.Call, OpCodes.Callvirt, OpCodes.Ldsfld, OpCodes.Stsfld,
            OpCodes.Ldarg, OpCodes.Starg, OpCodes.Ldloc, OpCodes.Stloc,
            OpCodes.Switch, OpCodes.Br, OpCodes.Brtrue, OpCodes.Brfalse
        };

        private static readonly Dictionary<string, int> VMPatternCache = new Dictionary<string, int>();

        public static bool DetectVirtualMachine(ModuleDefMD module)
        {
            try
            {
                return CheckVMSignatures(module) || 
                       CheckVMPatterns(module) || 
                       CheckVMHandlers(module) ||
                       CheckAdvancedVMPatterns(module) ||
                       CheckVMInstructions(module);
            }
            catch
            {
                return false;
            }
        }

        private static bool CheckVMSignatures(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                if (VMSignatures.Any(sig => type.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0))
                    return true;

                foreach (var method in type.Methods)
                {
                    if (VMSignatures.Any(sig => method.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0))
                        return true;
                }

                foreach (var field in type.Fields)
                {
                    if (VMSignatures.Any(sig => field.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0))
                        return true;
                }
            }
            return false;
        }

        private static bool CheckVMPatterns(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions == null) continue;

                    var instructions = method.Body.Instructions;
                    if (instructions.Count < 10) continue;

                    int vmPatternCount = 0;
                    for (int i = 0; i < instructions.Count - 3; i++)
                    {
                        if (IsVMPattern(instructions, i))
                            vmPatternCount++;
                    }

                    if (vmPatternCount > instructions.Count * 0.3)
                        return true;
                }
            }
            return false;
        }

        private static bool IsVMPattern(IList<Instruction> instructions, int index)
        {
            if (index + 3 >= instructions.Count) return false;

            var inst1 = instructions[index];
            var inst2 = instructions[index + 1];
            var inst3 = instructions[index + 2];
            var inst4 = instructions[index + 3];

            return (inst1.OpCode == OpCodes.Ldsfld &&
                    inst2.OpCode == OpCodes.Ldarg &&
                    inst3.OpCode == OpCodes.Call &&
                    inst4.OpCode == OpCodes.Stsfld) ||
                   (inst1.OpCode == OpCodes.Ldloc &&
                    inst2.OpCode == OpCodes.Ldc_I4 &&
                    inst3.OpCode == OpCodes.Add &&
                    inst4.OpCode == OpCodes.Stloc) ||
                   (inst1.OpCode == OpCodes.Switch &&
                    inst2.OpCode == OpCodes.Ldloc &&
                    inst3.OpCode == OpCodes.Ldc_I4 &&
                    inst4.OpCode == OpCodes.Add);
        }

        private static bool CheckVMHandlers(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                if (type.Methods.Count > 50)
                {
                    int handlerCount = 0;
                    int totalMethods = type.Methods.Count;
                    
                    foreach (var method in type.Methods)
                    {
                        if (IsVMHandler(method))
                            handlerCount++;
                    }

                    if (handlerCount > totalMethods * 0.8)
                        return true;
                }
            }
            return false;
        }

        private static bool IsVMHandler(MethodDef method)
        {
            if (method.Body?.Instructions == null) return false;

            var instructions = method.Body.Instructions;
            if (instructions.Count < 5 || instructions.Count > 20) return false;

            int switchCount = instructions.Count(i => i.OpCode == OpCodes.Switch);
            int callCount = instructions.Count(i => i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Callvirt);
            int branchCount = instructions.Count(i => i.OpCode.FlowControl == FlowControl.Cond_Branch);

            return (switchCount > 0 && callCount < 3) || 
                   (branchCount > instructions.Count * 0.5);
        }

        private static bool CheckAdvancedVMPatterns(ModuleDefMD module)
        {
            var vmTypeCount = 0;
            var totalTypes = module.Types.Count;

            foreach (var type in module.Types)
            {
                if (IsAdvancedVMType(type))
                    vmTypeCount++;
            }

            return vmTypeCount > totalTypes * 0.1;
        }

        private static bool IsAdvancedVMType(TypeDef type)
        {
            var methodPatterns = 0;
            var fieldPatterns = 0;

            foreach (var method in type.Methods)
            {
                if (HasVMMethodPattern(method))
                    methodPatterns++;
            }

            foreach (var field in type.Fields)
            {
                if (HasVMFieldPattern(field))
                    fieldPatterns++;
            }

            return methodPatterns > type.Methods.Count * 0.6 ||
                   fieldPatterns > type.Fields.Count * 0.4;
        }

        private static bool HasVMMethodPattern(MethodDef method)
        {
            if (method.Body?.Instructions == null) return false;

            var instructions = method.Body.Instructions;
            var complexityScore = 0;

            foreach (var instruction in instructions)
            {
                if (instruction.OpCode == OpCodes.Switch)
                    complexityScore += 3;
                else if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch)
                    complexityScore += 2;
                else if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                    complexityScore += 1;
            }

            return complexityScore > instructions.Count * 0.4;
        }

        private static bool HasVMFieldPattern(FieldDef field)
        {
            return field.FieldType.IsArray ||
                   field.FieldType.FullName.Contains("Dictionary") ||
                   field.FieldType.FullName.Contains("Stack") ||
                   field.FieldType.FullName.Contains("Queue");
        }

        private static bool CheckVMInstructions(ModuleDefMD module)
        {
            var vmInstructionCount = 0;
            var totalInstructions = 0;

            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions == null) continue;

                    totalInstructions += method.Body.Instructions.Count;

                    foreach (var instruction in method.Body.Instructions)
                    {
                        if (IsVMInstruction(instruction))
                            vmInstructionCount++;
                    }
                }
            }

            return totalInstructions > 0 && vmInstructionCount > totalInstructions * 0.15;
        }

        private static bool IsVMInstruction(Instruction instruction)
        {
            return instruction.OpCode == OpCodes.Switch ||
                   instruction.OpCode == OpCodes.Calli ||
                   (instruction.OpCode == OpCodes.Call && 
                    instruction.Operand is MethodDef method &&
                    VMSignatures.Any(sig => method.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        public static void RemoveVirtualMachine(ModuleDefMD module)
        {
            try
            {
                RemoveVMTypes(module);
                RestoreVirtualizedMethods(module);
                CleanupVMReferences(module);
                OptimizeAfterVMRemoval(module);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove virtual machine: {ex.Message}");
            }
        }

        private static void RemoveVMTypes(ModuleDefMD module)
        {
            var vmTypes = new List<TypeDef>();
            
            foreach (var type in module.Types)
            {
                if (IsVMType(type) || IsAdvancedVMType(type))
                    vmTypes.Add(type);
            }

            foreach (var vmType in vmTypes)
            {
                module.Types.Remove(vmType);
            }
        }

        private static bool IsVMType(TypeDef type)
        {
            return VMSignatures.Any(sig => type.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (type.Methods.Count > 20 && 
                    type.Methods.Count(IsVMHandler) > type.Methods.Count * 0.7);
        }

        private static void RestoreVirtualizedMethods(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions == null) continue;

                    RestoreMethodFromVM(method);
                    SimplifyVMInstructions(method);
                    RemoveVMCallbacks(method);
                }
            }
        }

        private static void RestoreMethodFromVM(MethodDef method)
        {
            var instructions = method.Body.Instructions;
            var toReplace = new List<(int index, Instruction newInst)>();

            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                
                if (IsVMCallInstruction(instruction))
                {
                    var restored = RestoreVMCall(instruction);
                    if (restored != null)
                    {
                        toReplace.Add((i, restored));
                    }
                }
            }

            foreach (var (index, newInst) in toReplace.OrderByDescending(x => x.index))
            {
                instructions[index] = newInst;
            }
        }

        private static bool IsVMCallInstruction(Instruction instruction)
        {
            return instruction.OpCode == OpCodes.Call &&
                   instruction.Operand is MethodDef method &&
                   (method.Name.Length > 15 || 
                    VMSignatures.Any(sig => method.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        private static Instruction RestoreVMCall(Instruction vmCall)
        {
            if (vmCall.Operand is MethodDef method)
            {
                var originalMethod = FindOriginalMethod(method);
                if (originalMethod != null)
                {
                    return Instruction.Create(OpCodes.Call, originalMethod);
                }
            }
            return null;
        }

        private static MethodDef FindOriginalMethod(MethodDef vmMethod)
        {
            if (vmMethod.Body?.Instructions == null) return null;

            foreach (var instruction in vmMethod.Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Call && 
                    instruction.Operand is MethodDef target &&
                    !IsVMMethod(target))
                {
                    return target;
                }
            }
            return null;
        }

        private static bool IsVMMethod(MethodDef method)
        {
            return VMSignatures.Any(sig => method.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static void SimplifyVMInstructions(MethodDef method)
        {
            var instructions = method.Body.Instructions;
            
            for (int i = instructions.Count - 1; i >= 0; i--)
            {
                var instruction = instructions[i];
                
                if (IsRedundantVMInstruction(instruction))
                {
                    instructions.RemoveAt(i);
                }
            }
        }

        private static bool IsRedundantVMInstruction(Instruction instruction)
        {
            return instruction.OpCode == OpCodes.Nop;
        }

        private static void RemoveVMCallbacks(MethodDef method)
        {
            var instructions = method.Body.Instructions;
            var toRemove = new List<int>();

            for (int i = 0; i < instructions.Count - 2; i++)
            {
                if (IsVMCallbackPattern(instructions, i))
                {
                    toRemove.Add(i);
                    toRemove.Add(i + 1);
                    toRemove.Add(i + 2);
                }
            }

            foreach (var index in toRemove.OrderByDescending(x => x))
            {
                if (index < instructions.Count)
                {
                    instructions.RemoveAt(index);
                }
            }
        }

        private static bool IsVMCallbackPattern(IList<Instruction> instructions, int index)
        {
            if (index + 2 >= instructions.Count) return false;

            return instructions[index].OpCode == OpCodes.Ldsfld &&
                   instructions[index + 1].OpCode == OpCodes.Ldarg &&
                   instructions[index + 2].OpCode == OpCodes.Call;
        }

        private static void CleanupVMReferences(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions == null) continue;

                    var toRemove = new List<Instruction>();
                    
                    foreach (var instruction in method.Body.Instructions)
                    {
                        if (instruction.Operand is MemberRef memberRef && IsVMReference(memberRef))
                        {
                            toRemove.Add(instruction);
                        }
                    }

                    foreach (var instruction in toRemove)
                    {
                        method.Body.Instructions.Remove(instruction);
                    }
                }
            }
        }

        private static bool IsVMReference(MemberRef memberRef)
        {
            return VMSignatures.Any(sig => 
                memberRef.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0 || 
                (memberRef.DeclaringType?.Name?.String != null && memberRef.DeclaringType.Name.String.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        private static void OptimizeAfterVMRemoval(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions == null) continue;

                    RemoveEmptyBlocks(method);
                    OptimizeBranches(method);
                    RemoveUnusedLocals(method);
                }
            }
        }

        private static void RemoveEmptyBlocks(MethodDef method)
        {
            var instructions = method.Body.Instructions;
            
            for (int i = instructions.Count - 1; i >= 0; i--)
            {
                var instruction = instructions[i];
                
                if (instruction.OpCode == OpCodes.Br && 
                    instruction.Operand is Instruction target &&
                    i + 1 < instructions.Count &&
                    instructions[i + 1] == target)
                {
                    instructions.RemoveAt(i);
                }
            }
        }

        private static void OptimizeBranches(MethodDef method)
        {
            var instructions = method.Body.Instructions;
            var optimized = true;

            while (optimized)
            {
                optimized = false;
                
                for (int i = 0; i < instructions.Count; i++)
                {
                    var instruction = instructions[i];
                    
                    if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch &&
                        CanOptimizeBranch(instructions, i))
                    {
                        OptimizeBranch(instructions, i);
                        optimized = true;
                        break;
                    }
                }
            }
        }

        private static bool CanOptimizeBranch(IList<Instruction> instructions, int index)
        {
            if (index >= instructions.Count - 1) return false;
            
            var branch = instructions[index];
            var next = instructions[index + 1];
            
            return branch.Operand == next;
        }

        private static void OptimizeBranch(IList<Instruction> instructions, int index)
        {
            var branch = instructions[index];
            
            if (branch.OpCode == OpCodes.Brtrue)
            {
                instructions[index] = Instruction.Create(OpCodes.Pop);
            }
            else if (branch.OpCode == OpCodes.Brfalse)
            {
                instructions[index] = Instruction.Create(OpCodes.Pop);
            }
        }

        private static void RemoveUnusedLocals(MethodDef method)
        {
            if (method.Body?.Variables == null) return;

            var usedLocals = new HashSet<Local>();
            
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.Operand is Local local)
                {
                    usedLocals.Add(local);
                }
            }

            var toRemove = method.Body.Variables.Where(v => !usedLocals.Contains(v)).ToList();
            
            foreach (var local in toRemove)
            {
                method.Body.Variables.Remove(local);
            }
        }
    }
} 