using System;
using System.IO;
using Il2CppApiAnalyzer.Util;

namespace Il2CppApiAnalyzer.Parsers
{
    /// <summary>
    ///     A simple parser for Il2Cpp API headers.
    ///     Can be used to analyze and process API definitions into other formats.
    /// </summary>
    internal class Il2CppApiHeaderParser : IDisposable
    {
        private readonly BufferedTextReader apiHeader;

        public Il2CppApiHeaderParser(TextReader apiHeader)
        {
            this.apiHeader = new BufferedTextReader(apiHeader);
        }

        public void Dispose()
        {
            apiHeader?.Dispose();
        }

        public void Parse()
        {
            while (apiHeader.Read(out var cur))
            {
                if (char.IsWhiteSpace(cur))
                    continue;

                if (ParseHelpers.IsAtApiDefinition(apiHeader))
                    ProcessApiDef();
                else if (ParseHelpers.IsAtSingleComment(apiHeader))
                    apiHeader.ReadLineAndReset();
            }
        }

        private void ProcessApiDef()
        {
            Console.WriteLine($"Got DO_API: {apiHeader.ReadLineAndReset()}");
        }
    }
}