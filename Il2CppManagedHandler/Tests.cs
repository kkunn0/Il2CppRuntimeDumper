namespace Il2CppManagedHandler
{
    internal class Tests
    {
        public delegate int d_TestIntInternal(string abc);

        public delegate void d_TestVoidInternal(int abc);

        public static int TestIntInternal(string abc)
        {
            var func = ICallHelper.ResolveICall("TestIntInternal", typeof(d_TestIntInternal));
            return ((d_TestIntInternal) func)(abc);
        }

        public static void TestVoidInternal(int abc)
        {
            var func = ICallHelper.ResolveICall("TestVoidInternal", typeof(d_TestVoidInternal));
            ((d_TestVoidInternal) func)(abc);
        }
    }
}
