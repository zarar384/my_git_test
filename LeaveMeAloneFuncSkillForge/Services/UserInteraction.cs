using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Interfaces;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public class UserInteraction
    {
        private readonly IConsole _console;
        public UserInteraction(IConsole console)
        {
            _console = console;
        }

        public UserInput GetUserInput(string promptMessage)
        {
            var input = this._console.ReadInput(promptMessage);

            UserInput returnValue = input switch
            {
                TextInput x when string.IsNullOrWhiteSpace(x.Input) => new NoInput(),
                TextInput x when int.TryParse(x.Input, out var n) => new IntegerInput(n),
                TextInput x => new TextInput(x.Input),
                ErrorFromConsole e => e,
                NoInput n => n,
                _ => new NoInput()
            };

            return returnValue;
        }
    }
}
