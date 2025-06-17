using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Unmask;

namespace CustomScripts
{
    /// <summary>
    /// Скрипт деобфускации по умолчанию
    /// </summary>
    public class DefaultDeobfuscationScript : IDeobfuscationScript
    {
        public string Name => "DefaultDeobfuscation";
        public string Description => "Стандартная деобфускация с базовыми операциями";
        public string Version => "1.0";

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Запуск стандартной деобфускации...");
            
            // Выполняем базовые операции деобфускации
            var basicScript = new BasicDeobfuscationScript();
            basicScript.Execute(module, logger);
            
            var stringScript = new StringDecryptionScript();
            stringScript.Execute(module, logger);
            
            var controlFlowScript = new ControlFlowRecoveryScript();
            controlFlowScript.Execute(module, logger);
            
            var metadataScript = new MetadataRepairScript();
            metadataScript.Execute(module, logger);
            
            logger.Success("Стандартная деобфускация завершена");
        }
    }
} 