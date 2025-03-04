using System.Text;

namespace WebParser
{
    internal class TeeTextWriter : TextWriter
    {
        private readonly TextWriter _consoleOut;
        private readonly TextWriter _fileOut;

        public TeeTextWriter(TextWriter consoleOut, TextWriter fileOut)
        {
            _consoleOut = consoleOut;
            _fileOut = fileOut;
        }

        public override void Write(char value)
        {
            _consoleOut.Write(value);
            _fileOut.Write(value);
        }

        public override void Write(string value)
        {
            _consoleOut.Write(value);
            _fileOut.Write(value);
        }

        public override Encoding Encoding => _consoleOut.Encoding;
    }
}
