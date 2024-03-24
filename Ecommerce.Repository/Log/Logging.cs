using Ecommerce.Core.ILog;

namespace Ecommerce.Repository.Log
{
    public class Logging : ILogging
    {
        public void Log(string message, string type)
        {
            if(type == "Error")
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"---Error----->: {message}");
            }
            
            if(type == "Warning")
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"---Warning----->: {message}");
            }

            if(type == "Information")
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"---Information----->: {message}");
            }
        }
    }
}
