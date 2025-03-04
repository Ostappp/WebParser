namespace WebParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program started...");
            using (StreamWriter logFile = new(Consts.LogFilePath, true))
            {
                TeeTextWriter teeWriter = new(Console.Out, logFile);
                Console.SetOut(teeWriter);
                Console.SetError(teeWriter);
            }
        }
    }
}
