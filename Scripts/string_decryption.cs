using System;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Unmask;

namespace CustomScripts
{
    /// <summary>
    /// Скрипт расшифровки строк
    /// </summary>
    public class StringDecryptionExampleScript : IDeobfuscationScript
    {
        public string Name => "StringDecryption";
        public string Description => "Расшифровка защищенных строк";
        public string Version => "1.0";

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Запуск расшифровки строк...");
            
            int decryptedCount = 0;
            
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body?.Instructions == null) continue;
                    
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        var instruction = method.Body.Instructions[i];
                        
                        // Ищем зашифрованные строки
                        if (instruction.OpCode == OpCodes.Ldstr && 
                            instruction.Operand is string str)
                        {
                            string decrypted = TryDecryptString(str);
                            if (decrypted != str)
                            {
                                instruction.Operand = decrypted;
                                decryptedCount++;
                                logger.Info("Расшифрована строка: " + str + " -> " + decrypted);
                            }
                        }
                    }
                }
            }
            
            logger.Success("Расшифровано строк: " + decryptedCount);
        }
        
        private string TryDecryptString(string encrypted)
        {
            // Простая XOR расшифровка
            if (encrypted.StartsWith("XOR:"))
            {
                string data = encrypted.Substring(4);
                return XorDecrypt(data, 42);
            }
            
            // Base64 декодирование
            if (encrypted.StartsWith("B64:"))
            {
                try
                {
                    string data = encrypted.Substring(4);
                    byte[] bytes = Convert.FromBase64String(data);
                    return Encoding.UTF8.GetString(bytes);
                }
                catch
                {
                    return encrypted;
                }
            }
            
            // ROT13
            if (encrypted.StartsWith("ROT:"))
            {
                string data = encrypted.Substring(4);
                return Rot13(data);
            }
            
            return encrypted;
        }
        
        private string XorDecrypt(string data, byte key)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(data);
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= key;
                }
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return data;
            }
        }
        
        private string Rot13(string input)
        {
            char[] chars = input.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c >= 'a' && c <= 'z')
                {
                    chars[i] = (char)((c - 'a' + 13) % 26 + 'a');
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    chars[i] = (char)((c - 'A' + 13) % 26 + 'A');
                }
            }
            return new string(chars);
        }
    }
} 