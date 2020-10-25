using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Il2CppManagedHandler
{
    public static class ICallHelper
    {
        private static readonly Dictionary<string, Delegate> _icallCache = new Dictionary<string, Delegate>();

        #region Externals

        [DllImport("GameAssembly.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_resolve_icall(string icallName);

        #endregion

        public static Delegate ResolveICall(string icallName, Type delegateType)
        {
            if (_icallCache.ContainsKey(icallName)) return _icallCache[icallName];

            var icallPtr = il2cpp_resolve_icall(icallName);
            if (icallPtr == IntPtr.Zero) return null;
            var icallFunc = Marshal.GetDelegateForFunctionPointer(icallPtr, delegateType);

            _icallCache.Add(icallName, icallFunc);
            return icallFunc;
        }
    }
}
