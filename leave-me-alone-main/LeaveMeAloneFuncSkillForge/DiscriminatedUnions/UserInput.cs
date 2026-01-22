namespace LeaveMeAloneFuncSkillForge.DiscriminatedUnions
{
    public abstract class UserInput
    {
    }

    public class TextInput : UserInput
    {
        public string Input { get; }
        public TextInput(string input) => Input = input;
    }

    public class IntegerInput : UserInput
    {
        public int Input { get; }
        public IntegerInput(int input) => Input = input;
    }

    public class NoInput : UserInput { }

    public class ErrorFromConsole : UserInput
    {
        public Exception Error { get; }
        public ErrorFromConsole(Exception error) =>Error = error;
    }
}
