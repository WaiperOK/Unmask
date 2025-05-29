
// Advanced VM removal script
Console.WriteLine("Starting advanced VM removal...");

var vmTypes = new List<TypeDef>();
var vmMethods = new List<MethodDef>();

// Detect VM types
foreach (var type in module.Types)
{
    if (type.Name.Contains("VM") || type.Name.Contains("Virtual"))
    {
        vmTypes.Add(type);
        continue;
    }
    
    // Check for VM handler patterns
    int handlerCount = 0;
    foreach (var method in type.Methods)
    {
        if (method.Body?.Instructions != null)
        {
            var switchCount = 0;
            foreach (var inst in method.Body.Instructions)
            {
                if (inst.OpCode == OpCodes.Switch)
                    switchCount++;
            }
            
            if (switchCount > 0 && method.Body.Instructions.Count < 50)
                handlerCount++;
        }
    }
    
    if (handlerCount > type.Methods.Count * 0.7)
    {
        vmTypes.Add(type);
    }
}

// Remove VM types
foreach (var vmType in vmTypes)
{
    module.Types.Remove(vmType);
}

Console.WriteLine($"Removed {vmTypes.Count} VM types");

// Restore virtualized methods
foreach (var type in module.Types)
{
    foreach (var method in type.Methods)
    {
        if (method.Body?.Instructions?.Count == 3)
        {
            var instructions = method.Body.Instructions;
            if (instructions[0].OpCode == OpCodes.Ldarg &&
                instructions[1].OpCode == OpCodes.Call &&
                instructions[2].OpCode == OpCodes.Ret)
            {
                // This looks like a virtualized method stub
                vmMethods.Add(method);
            }
        }
    }
}

Console.WriteLine($"Found {vmMethods.Count} virtualized methods");
Console.WriteLine("Advanced VM removal completed");
