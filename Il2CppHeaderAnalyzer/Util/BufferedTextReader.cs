using System;
using System.IO;
using System.Text;

namespace Il2CppApiAnalyzer.Util
{
    internal class BufferedTextReader : IDisposable
    {
        private readonly StringBuilder buffer;
        private readonly TextReader reader;
        private int bufferOffset;

        public BufferedTextReader(TextReader reader)
        {
            this.reader = reader;
            buffer = new StringBuilder();
        }

        public char Current => buffer[buffer.Length - 1 - bufferOffset];

        public void Dispose()
        {
            buffer.Clear();
            reader?.Dispose();
        }

        public void MoveBackBy(int offset)
        {
            bufferOffset = Math.Min(bufferOffset + offset, buffer.Length);
        }

        public bool Read(out char nextChar)
        {
            if (bufferOffset > 0)
            {
                nextChar = buffer[buffer.Length - bufferOffset];
                bufferOffset--;
                return true;
            }

            var next = reader.Read();
            var ok = next >= 0;
            nextChar = ok ? (char) next : '\0';
            if (ok)
                buffer.Append(nextChar);
            return ok;
        }

        public bool Read(int count, out string result)
        {
            var sb = new StringBuilder();
            var ok = true;
            for (var i = 0; i < count; i++)
                // ReSharper disable once AssignmentInConditionalExpression
                if (ok = Read(out var next))
                    sb.Append(next);
                else
                    break;
            result = sb.ToString();
            return ok;
        }

        public string ReadLineAndReset()
        {
            bufferOffset = 0;
            var result = reader.ReadLine();
            if (result != null)
                buffer.AppendLine(result);
            return result;
        }
    }
}
