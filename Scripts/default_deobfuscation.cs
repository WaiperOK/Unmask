Console.WriteLine("Starting default deobfuscation...");

var emptyMethods = new List<MethodDef>();
foreach (var type in module.Types)
{
    foreach (var method in type.Methods)
    {
        if (method.Body?.Instructions?.Count == 1 && 
            method.Body.Instructions[0].OpCode == OpCodes.Ret)
        {
            emptyMethods.Add(method);
        }
    }
}

foreach (var method in emptyMethods)
{
    method.DeclaringType.Methods.Remove(method);
}

Console.WriteLine($"Removed {emptyMethods.Count} empty methods");

foreach (var type in module.Types)
{
    foreach (var method in type.Methods)
    {
        if (method.Body?.Instructions == null) continue;
        
        for (int i = 0; i < method.Body.Instructions.Count; i++)
        {
            var instruction = method.Body.Instructions[i];
            if (instruction.OpCode == OpCodes.Ldstr && 
                instruction.Operand is string str && 
                str.StartsWith("encrypted_"))
            {
                var decrypted = str.Replace("encrypted_", "");
                instruction.Operand = decrypted;
            }
        }
    }
}

Console.WriteLine("Default deobfuscation completed"); 