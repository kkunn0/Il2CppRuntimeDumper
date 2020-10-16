using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DumperMetadataGenerator.MetadataGenerator
{
    public class GeneratedMetadata
    {
        public static GeneratedMetadata Instance { get; private set; }
        public ModuleDefinition MsCorLib { get; private set; }
        public ModuleDefinition ManagedHelperLib { get; private set; }
        public GeneratedMetadata(ModuleDefinition MsCorLib, ModuleDefinition ManagedHelperLib)
        {
            Instance = this;

            // Setup core
            this.MsCorLib = MsCorLib;
            VoidType = MsCorLib.TypeSystem.Void;
            ObjectType = MsCorLib.TypeSystem.Object;
            IntPtrType = MsCorLib.TypeSystem.IntPtr;
            AsyncResultType = MsCorLib.GetType("System.IAsyncResult");
            AsyncCallbackType = MsCorLib.GetType("System.AsyncCallback");
            DelegateType = MsCorLib.GetType("System.Delegate");

            TypeType = MsCorLib.GetType("System.Type");
            GetTypeFromHandle = TypeType.Methods.FirstOrDefault(method => method.Name == "GetTypeFromHandle");

            // Setup managed helper
            this.ManagedHelperLib = ManagedHelperLib;
            ICallHelperType = ManagedHelperLib.GetType("Il2CppManagedHandler.ICallHelper");
            ResolveICall = ICallHelperType.Methods.FirstOrDefault(method => method.Name == "ResolveICall");
        }

        #region Core Instances
        public TypeReference VoidType { get; private set; }
        public TypeReference ObjectType { get; private set; }
        public TypeReference IntPtrType { get; private set; }
        public TypeReference AsyncResultType { get; private set; }
        public TypeReference AsyncCallbackType { get; private set; }
        public TypeReference DelegateType { get; private set; }

        public TypeDefinition TypeType { get; private set; }
        public MethodDefinition GetTypeFromHandle { get; private set; }
        #endregion

        #region Managed Helper Instances
        public TypeDefinition ICallHelperType { get; private set; }
        public MethodDefinition ResolveICall { get; private set; }
        #endregion

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
                    Console.WriteLine("Failed to scan type " + type.FullName);
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
                TypeDefinition methodDelegate = CreateMethodDelegate(method);
                if(methodDelegate == null || !ModifyMethod(method, methodDelegate))
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
                if ((int)attrib.ConstructorArguments[0].Value != 4096) continue; // MethodImplOptions.InternalCall

                return true;
            }
            return false;
        }
        public TypeDefinition CreateMethodDelegate(MethodDefinition method) =>
            DelegateGenerator.Create(method.DeclaringType, method.ReturnType, method.Parameters.Select(param => param.ParameterType));
        public bool ModifyMethod(MethodDefinition method, TypeDefinition methodDelegate)
        {
            if(method == null || methodDelegate == null || method.HasBody)
            {
                Console.WriteLine("Method is either null or already contains a body!");
                return false;
            }

            // Rebind method properties
            method.IsInternalCall = false;
            RemoveInternalAttribute(method);
            method.Body.Variables.Clear();
            method.Body.InitLocals = true;

            // Setup locals
            VariableDefinition func = new VariableDefinition(DelegateType);
            method.Body.Variables.Add(func);

            // Setup internal name
            string internalName = method.DeclaringType.FullName + "::" + method.Name;

            // Get delegate's invoke
            MethodDefinition invoke = methodDelegate.Methods.FirstOrDefault(mth => mth.Name == "Invoke");

            // Setup body
            method.Body = new MethodBody(method);
            ILProcessor processor = method.Body.GetILProcessor();

            // Resolve ICall delegate
            processor.Emit(OpCodes.Ldstr, internalName);
            processor.Emit(OpCodes.Ldtoken, methodDelegate);
            processor.Emit(OpCodes.Call, GetTypeFromHandle);
            processor.Emit(OpCodes.Call, ResolveICall);
            processor.Emit(OpCodes.Stloc, func);

            // Invoke ICall and return the result
            processor.Emit(OpCodes.Ldloc, func);
            processor.Emit(OpCodes.Castclass, methodDelegate);
            foreach(ParameterDefinition param in method.Parameters)
                processor.Emit(OpCodes.Ldarg, param);
            processor.Emit(OpCodes.Callvirt, invoke);
            processor.Emit(OpCodes.Ret);

            return true;
        }
        public void RemoveInternalAttribute(MethodDefinition method)
        {
            foreach (CustomAttribute attrib in method.CustomAttributes)
            {
                if (attrib.Constructor.FullName != "System.Runtime.CompilerServices.MethodImplAttribute") continue;
                if ((int)attrib.ConstructorArguments[0].Value != 4096) continue; // MethodImplOptions.InternalCall

                method.CustomAttributes.Remove(attrib);
                break;
            }
        }
        #endregion
    }
}
