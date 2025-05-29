Console.WriteLine("Starting string decryption...");

int decryptedCount = 0;

foreach (var type in module.Types)
{
    foreach (var method in type.Methods)
    {
        if (method.Body?.Instructions == null) continue;
        
        for (int i = 0; i < method.Body.Instructions.Count - 2; i++)
        {
            var inst1 = method.Body.Instructions[i];
            var inst2 = method.Body.Instructions[i + 1];
            var inst3 = method.Body.Instructions[i + 2];
            
            if (inst1.OpCode == OpCodes.Ldstr &&
                inst2.OpCode == OpCodes.Call &&
                inst2.Operand is MethodDef decryptMethod &&
                decryptMethod.Name.Contains("Decrypt"))
            {
                try
                {
                    var encryptedString = inst1.Operand as string;
                    if (!string.IsNullOrEmpty(encryptedString))
                    {
                        var decrypted = "";
                        var key = 0x42;
                        
                        foreach (char c in encryptedString)
                        {
                            decrypted += (char)(c ^ key);
                        }
                        
                        inst1.Operand = decrypted;
                        
                        method.Body.Instructions.RemoveAt(i + 1);
                        
                        decryptedCount++;
                    }
                }
                catch
                {
                }
            }
        }
    }
}

Console.WriteLine($"Decrypted {decryptedCount} strings");
Console.WriteLine("String decryption completed"); 