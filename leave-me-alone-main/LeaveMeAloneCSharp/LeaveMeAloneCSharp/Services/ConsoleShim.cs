using LeaveMeAloneCSharp.DiscriminatedUnions;
using LeaveMeAloneCSharp.Interfaces;

namespace LeaveMeAloneCSharp.Services
{
    public class ConsoleShim : IConsole
    {
        public UserInput ReadInput(string userPromptMessage)
        {
            try
            {
                Console.WriteLine(userPromptMessage);
                var input = Console.ReadLine();

                return new TextInput(input);
            }
            catch (Exception ex)
            {
                return new ErrorFromConsole(ex);
            }
        }
    }
}
