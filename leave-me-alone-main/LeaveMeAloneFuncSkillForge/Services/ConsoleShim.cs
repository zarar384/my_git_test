using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Interfaces;

namespace LeaveMeAloneFuncSkillForge.Services
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
