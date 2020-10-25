using System.Collections.Generic;
using System.IO;
using Mono.Cecil;

namespace DumperMetadataGenerator.MetadataGenerator
{
    public class DumperMetadataGenerator
    {
        private GeneratedMetadata _generatedMetadata;
        private readonly List<ModuleDefinition> _unityModules = new List<ModuleDefinition>();

        public DumperMetadataGenerator(string dllDirectory, string managedHandlerPath)
        {
            if (!Directory.Exists(dllDirectory)) return;
            if (!File.Exists(dllDirectory + "\\mscorlib.dll")) return;

            // Load the generated metadata
            _generatedMetadata = new GeneratedMetadata(ModuleDefinition.ReadModule(dllDirectory + "\\mscorlib.dll"),
                                                       ModuleDefinition.ReadModule(managedHandlerPath));

            // Load all of the dlls into memory
            foreach (var dllFile in Directory.EnumerateFiles(dllDirectory, "*.dll"))
                if (!dllFile.EndsWith("mscorlib.dll"))
                    _unityModules.Add(ModuleDefinition.ReadModule(dllFile));
        }

        public bool GenerateMetadata()
        {
            return false;
        }
    }
}
