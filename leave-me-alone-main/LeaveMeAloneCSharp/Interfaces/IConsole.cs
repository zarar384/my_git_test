using LeaveMeAloneCSharp.DiscriminatedUnions;

namespace LeaveMeAloneCSharp.Interfaces
{
    public interface IConsole
    {
        UserInput ReadInput(string userPromptMessage);
    }
}
