using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Il2CppManagedHandler
{
    public static class ICallHelper
    {
        #region Externals
        [DllImport("GameAssembly.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private extern static IntPtr il2cpp_resolve_icall(string icallName);
        #endregion

        private static Dictionary<string, Delegate> _icallCache = new Dictionary<string, Delegate>();
        public static Delegate ResolveICall(string icallName, Type delegateType)
        {
            if (_icallCache.ContainsKey(icallName)) return _icallCache[icallName];

            IntPtr icallPtr = il2cpp_resolve_icall(icallName);
            if (icallPtr == IntPtr.Zero) return null;
            Delegate icallFunc = Marshal.GetDelegateForFunctionPointer(icallPtr, delegateType);

            _icallCache.Add(icallName, icallFunc);
            return icallFunc;
        }
    }
}
