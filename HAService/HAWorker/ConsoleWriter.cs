using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Imps.Services.HA
{
    class ConsoleWriter : TextWriter
    {
        private TextWriter _writer = null;

        public ConsoleWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public override Encoding Encoding
        {
            get { return _writer.Encoding; }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            bool re = Console.CursorLeft == 2;
            if (Console.CursorLeft > 2)
                Console.CursorTop += 1;
            Console.CursorLeft = 0;

            Console.ForegroundColor = ConsoleColor.Blue;
            _writer.Write(buffer, index, count);
            Console.ResetColor();

            if (re)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Error.Write("> ");
                Console.ResetColor();
            }
        }
    }
}
