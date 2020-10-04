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

        public static Delegate ResolveICall(string icallName, Type delegateType)
        {
            IntPtr icallPtr = il2cpp_resolve_icall(icallName);
            if (icallPtr == IntPtr.Zero) return null;

            return Marshal.GetDelegateForFunctionPointer(icallPtr, delegateType);
        }
    }
}
