namespace Unmask
{
    /// <summary>
    /// Флаги активных защит
    /// </summary>
    public class ProtectionFlags
    {
        public bool EncryptedStrings { get; set; }
        public bool ProxyStrings { get; set; }
        public bool IntConfusion { get; set; }
        public bool LocalField { get; set; }
        public bool JumpControlFlow { get; set; }
        public bool AntiDebug { get; set; }
        public bool AntiTamper { get; set; }
        public bool AntiDump { get; set; }
        public bool InvalidMetadata { get; set; }
        public bool AntiDe4Dot { get; set; }
        public bool StackUnfConfusion { get; set; }
        public bool ProxyConstants { get; set; }
        public bool Callis { get; set; }
        public bool Arithmetic { get; set; }
        public bool ProxyMethods { get; set; }
        public bool Watermarks { get; set; }
        public bool ControlFlow { get; set; }
        public bool Renamer { get; set; }
        public bool ResourceEncryption { get; set; }
        public bool ResourceProtections { get; set; }
        public bool OnlineStringDecryption { get; set; }
        public bool DataStructureRecovery { get; set; }
        public bool JunkCodeRemoval { get; set; }
        public bool DelegateTypeRecovery { get; set; }
        public bool VirtualMachines { get; set; }
        public bool HiddenAssemblyReveal { get; set; }
        public bool InMemoryDecompilation { get; set; }
        public bool ScriptingEngine { get; set; }
        public bool AntiTamperChecks { get; set; }

        /// <summary>
        /// Сброс всех флагов
        /// </summary>
        public void Reset()
        {
            EncryptedStrings = false;
            ProxyStrings = false;
            IntConfusion = false;
            LocalField = false;
            JumpControlFlow = false;
            AntiDebug = false;
            AntiTamper = false;
            AntiDump = false;
            InvalidMetadata = false;
            AntiDe4Dot = false;
            StackUnfConfusion = false;
            ProxyConstants = false;
            Callis = false;
            Arithmetic = false;
            ProxyMethods = false;
            Watermarks = false;
            ControlFlow = false;
            Renamer = false;
            ResourceEncryption = false;
            ResourceProtections = false;
            OnlineStringDecryption = false;
            DataStructureRecovery = false;
            JunkCodeRemoval = false;
            DelegateTypeRecovery = false;
            VirtualMachines = false;
            HiddenAssemblyReveal = false;
            InMemoryDecompilation = false;
            ScriptingEngine = false;
            AntiTamperChecks = false;
        }
    }
} 