using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Il2CppManagedHandler
{
    internal class Tests
    {
        public delegate int d_TestIntInternal(string abc);
        public static int TestIntInternal(string abc)
        {
            Delegate func = ICallHelper.ResolveICall("TestIntInternal", typeof(d_TestIntInternal));
            if (func == null) return default;

            return ((d_TestIntInternal)func)(abc);
        }
    }
}
