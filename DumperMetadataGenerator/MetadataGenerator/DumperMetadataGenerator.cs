using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mono.Cecil;

namespace DumperMetadataGenerator.MetadataGenerator
{
    public class DumperMetadataGenerator
    {
        private List<ModuleDefinition> _unityModules = new List<ModuleDefinition>();
        private GeneratedMetadata _generatedMetadata;

        public DumperMetadataGenerator(string dllDirectory)
        {
            if (!Directory.Exists(dllDirectory)) return;

            // Load the generated metadata
            _generatedMetadata = new GeneratedMetadata();

            // Load all of the dlls into memory
            foreach(string dllFile in Directory.EnumerateFiles(dllDirectory, "*.dll"))
                _unityModules.Add(ModuleDefinition.ReadModule(dllFile));
        }

        public bool GenerateMetadata()
        {
            return false;
        }
    }
}
