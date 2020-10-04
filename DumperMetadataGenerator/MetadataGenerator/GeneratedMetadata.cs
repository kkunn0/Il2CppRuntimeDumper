using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mono.Cecil;

namespace DumperMetadataGenerator.MetadataGenerator
{
    public class GeneratedMetadata
    {
        public GeneratedMetadata() { }

        #region Module including and scanning
        private List<ModuleDefinition> _modules = new List<ModuleDefinition>();
        public bool IncludeModule(ModuleDefinition module)
        {
            if (module == null || !module.HasTypes)
            {
                Console.WriteLine("Module is either null or does not contain types!");
                return false;
            }

            // Scan types and modify methods
            foreach(TypeDefinition type in module.GetTypes())
            {
                if (type.IsEnum || type.IsInterface) continue;
                if (!type.HasMethods) continue;

                // Scan type and see if we succeed
                if (!ScanType(type))
                {
                    Console.WriteLine("Failed to scan typpe " + type.FullName);
                    return false;
                }
            }

            // Finalize
            _modules.Add(module);
            return true;
        }
        public bool ScanType(TypeDefinition type)
        {
            if(type == null || !type.HasMethods)
            {
                Console.WriteLine("Type is either null or does not cotain methods!");
                return false;
            }

            // Scan methods and modify specific ones
            foreach(MethodDefinition method in type.Methods)
            {
                if (!method.HasBody || !method.HasCustomAttributes || !method.IsStatic || !method.IsInternalCall) continue;
                if (!IsInternalMethod(method)) continue;

                // Modify method and see if we succeed
                if(!ModifyMethod(method))
                {
                    Console.WriteLine("Failed to modify internal method " + method.FullName);
                    return false;
                }
            }

            // Finalize
            return true;
        }
        public bool IsInternalMethod(MethodDefinition method)
        {
            foreach(CustomAttribute attrib in method.CustomAttributes)
            {
                if (attrib.Constructor.FullName != "System.Runtime.CompilerServices.MethodImplAttribute") continue;
                if ((int)attrib.ConstructorArguments[0].Value != 4096) continue;

                return true;
            }
            return false;
        }
        public bool ModifyMethod(MethodDefinition method)
        {
            return true;
        }
        #endregion
    }
}
