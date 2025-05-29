using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Unmask;

namespace CustomScripts
{
    /// <summary>
    /// Пользовательский скрипт для расширенной расшифровки строк
    /// </summary>
    public class CustomStringDecryption : IDeobfuscationScript
    {
        public string Name => "CustomStringDecryption";
        public string Description => "Расширенная расшифровка строк с поддержкой XOR, Base64 и ROT13";
        public string Version => "2.0";

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Запуск расширенной расшифровки строк...");
            
            int totalDecrypted = 0;
            int xorDecrypted = 0;
            int base64Decrypted = 0;
            int rot13Decrypted = 0;

            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;

                    var instructions = method.Body.Instructions;
                    for (int i = 0; i < instructions.Count; i++)
                    {
                        var instruction = instructions[i];
                        if (instruction.OpCode == OpCodes.Ldstr && 
                            instruction.Operand is string str && 
                            !string.IsNullOrEmpty(str))
                        {
                            string decrypted = null;

                            // Попытка XOR расшифровки
                            if (decrypted == null)
                            {
                                decrypted = TryXorDecryption(str);
                                if (decrypted != null) xorDecrypted++;
                            }

                            // Попытка Base64 расшифровки
                            if (decrypted == null)
                            {
                                decrypted = TryBase64Decryption(str);
                                if (decrypted != null) base64Decrypted++;
                            }

                            // Попытка ROT13 расшифровки
                            if (decrypted == null)
                            {
                                decrypted = TryRot13Decryption(str);
                                if (decrypted != null) rot13Decrypted++;
                            }

                            // Применяем расшифровку
                            if (decrypted != null && decrypted != str)
                            {
                                instruction.Operand = decrypted;
                                totalDecrypted++;
                                logger.Info($"Расшифрована строка: '{str}' -> '{decrypted}'");
                            }
                        }
                    }
                }
            }

            logger.Success($"Всего расшифровано {totalDecrypted} строк:");
            logger.Info($"  XOR: {xorDecrypted}");
            logger.Info($"  Base64: {base64Decrypted}");
            logger.Info($"  ROT13: {rot13Decrypted}");
        }

        private string TryXorDecryption(string encrypted)
        {
            try
            {
                // Пробуем различные XOR ключи
                var commonKeys = new byte[] { 0x42, 0x13, 0x37, 0xFF, 0xAA, 0x55 };
                
                foreach (var key in commonKeys)
                {
                    var result = new StringBuilder();
                    bool isValid = true;

                    foreach (char c in encrypted)
                    {
                        var decrypted = (char)(c ^ key);
                        if (char.IsControl(decrypted) && decrypted != '\n' && decrypted != '\r' && decrypted != '\t')
                        {
                            isValid = false;
                            break;
                        }
                        result.Append(decrypted);
                    }

                    if (isValid && result.Length > 0 && IsReadableText(result.ToString()))
                    {
                        return result.ToString();
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки
            }
            return null;
        }

        private string TryBase64Decryption(string encrypted)
        {
            try
            {
                // Проверяем, что строка может быть Base64
                if (encrypted.Length % 4 != 0) return null;
                if (!encrypted.All(c => char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=')) return null;

                var bytes = Convert.FromBase64String(encrypted);
                var decrypted = Encoding.UTF8.GetString(bytes);
                
                if (IsReadableText(decrypted))
                {
                    return decrypted;
                }
            }
            catch
            {
                // Не Base64
            }
            return null;
        }

        private string TryRot13Decryption(string encrypted)
        {
            try
            {
                var result = new StringBuilder();
                
                foreach (char c in encrypted)
                {
                    if (char.IsLetter(c))
                    {
                        var offset = char.IsUpper(c) ? 'A' : 'a';
                        var rotated = (char)(((c - offset + 13) % 26) + offset);
                        result.Append(rotated);
                    }
                    else
                    {
                        result.Append(c);
                    }
                }

                var decrypted = result.ToString();
                if (IsReadableText(decrypted) && decrypted != encrypted)
                {
                    return decrypted;
                }
            }
            catch
            {
                // Ошибка ROT13
            }
            return null;
        }

        private bool IsReadableText(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length < 3) return false;
            
            // Проверяем, что текст содержит читаемые символы
            var readableChars = text.Count(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c));
            var readableRatio = (double)readableChars / text.Length;
            
            return readableRatio > 0.8;
        }
    }
} 