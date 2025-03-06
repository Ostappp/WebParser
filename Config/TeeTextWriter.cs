using System.Text;

namespace WebParser.Config
{
    internal class TeeTextWriter : TextWriter
    {
        private readonly TextWriter _consoleOut;
        private readonly IEnumerable<TextWriter> _fileOuts;
        private readonly bool _disableConsole;

        public TeeTextWriter(IEnumerable<string> fileOuts, bool disableConsole = false)
        {
            _disableConsole = disableConsole;
            _consoleOut = disableConsole ? Null : Console.Out;
            _fileOuts = fileOuts.Select(path => new StreamWriter(path, true));
        }

        public override void Write(char value)
        {
            _consoleOut.Write(value);
            foreach (var item in _fileOuts)
            {
                item.Write(value);
            }
        }

        public override void Write(string value)
        {
            _consoleOut.Write(value);
            foreach (var item in _fileOuts)
            {
                item.Write(value);
            }
        }

        public override Encoding Encoding => _disableConsole ? Encoding.Default : _consoleOut.Encoding;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var fileOut in _fileOuts)
                {
                    fileOut.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
