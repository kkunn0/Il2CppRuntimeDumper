namespace Il2CppApiAnalyzer.Util
{
    internal static class ParseHelpers
    {
        private const string DO_API_DEF = "DO_API";
        private const string COMMENT_DEF = "//";

        private static bool CheckOrMoveBack(BufferedTextReader tr, string other)
        {
            // Include the current char
            tr.MoveBackBy(1);
            tr.Read(other.Length, out var str);
            if (str != other)
                tr.MoveBackBy(str.Length - 1); // Make sure we're still at the original place
            return str == other;
        }

        // TODO: Add logic to detext DO_API_NO_RETURN
        public static bool IsAtApiDefinition(BufferedTextReader tr)
        {
            return CheckOrMoveBack(tr, DO_API_DEF);
        }

        public static bool IsAtSingleComment(BufferedTextReader tr)
        {
            return CheckOrMoveBack(tr, COMMENT_DEF);
        }
    }
}
