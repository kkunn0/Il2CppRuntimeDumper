using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace DumperMetadataGenerator.MetadataGenerator
{
    // https://github.com/SusirinkeLaborams/LaborasLang-aka-ForScience-/blob/master/LaborasLangCompiler/Codegen/Types/DelegateEmitter.cs
    public class DelegateGenerator
    {
        private const TypeAttributes DelegateTypeAttributes = TypeAttributes.NestedPublic | TypeAttributes.Sealed;
        private const MethodAttributes ConstructorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        private const MethodAttributes DelegateMethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.VtableLayoutMask;

        private TypeDefinition typeDefinition;
        private DelegateGenerator(string delegateName, TypeDefinition declaringType, TypeReference returnType, IEnumerable<TypeReference> arguments)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType", "Delegate class must be have a valid declaring type!");

            typeDefinition = new TypeDefinition(delegateName, delegateName, DelegateTypeAttributes, declaringType);

            AddConstructor();
            AddBeginInvoke(arguments);
            AddEndInvoke(returnType);
            AddInvoke(returnType, arguments);
        }

        public static TypeDefinition Create(TypeDefinition declaringType, TypeReference returnType, IEnumerable<TypeReference> arguments) =>
            (new DelegateGenerator("Delegate", declaringType, returnType, arguments)).typeDefinition;
        public static TypeDefinition Create(string delegateName, TypeDefinition declaringType, TypeReference returnType, IEnumerable<TypeReference> arguments) =>
            (new DelegateGenerator(delegateName, declaringType, returnType, arguments)).typeDefinition;

        private void AddConstructor()
        {
            var constructor = new MethodDefinition(".ctor", ConstructorAttributes, GeneratedMetadata.Instance.VoidType);
            constructor.Parameters.Add(new ParameterDefinition("objectInstance", ParameterAttributes.None, GeneratedMetadata.Instance.ObjectType));
            constructor.Parameters.Add(new ParameterDefinition("functionPtr", ParameterAttributes.None, GeneratedMetadata.Instance.IntPtrType));
            constructor.ImplAttributes = MethodImplAttributes.Runtime;

            typeDefinition.Methods.Add(constructor);
        }

        private void AddBeginInvoke(IEnumerable<TypeReference> arguments)
        {
            var beginInvoke = new MethodDefinition("BeginInvoke", DelegateMethodAttributes, GeneratedMetadata.Instance.AsyncResultType);
            foreach (var argument in arguments)
            {
                beginInvoke.Parameters.Add(new ParameterDefinition(argument));
            }

            beginInvoke.Parameters.Add(new ParameterDefinition("callback", ParameterAttributes.None, GeneratedMetadata.Instance.AsyncCallbackType));
            beginInvoke.Parameters.Add(new ParameterDefinition("object", ParameterAttributes.None, GeneratedMetadata.Instance.ObjectType));
            beginInvoke.ImplAttributes = MethodImplAttributes.Runtime;

            typeDefinition.Methods.Add(beginInvoke);
        }

        private void AddEndInvoke(TypeReference returnType)
        {
            var endInvoke = new MethodDefinition("EndInvoke", DelegateMethodAttributes, returnType);
            endInvoke.Parameters.Add(new ParameterDefinition("result", ParameterAttributes.None, GeneratedMetadata.Instance.AsyncResultType));
            endInvoke.ImplAttributes = MethodImplAttributes.Runtime;

            typeDefinition.Methods.Add(endInvoke);
        }

        private void AddInvoke(TypeReference returnType, IEnumerable<TypeReference> arguments)
        {
            var invoke = new MethodDefinition("Invoke", DelegateMethodAttributes, returnType);

            foreach (var argument in arguments)
            {
                invoke.Parameters.Add(new ParameterDefinition(argument));
            }

            invoke.ImplAttributes = MethodImplAttributes.Runtime;
            typeDefinition.Methods.Add(invoke);
        }
    }
}
