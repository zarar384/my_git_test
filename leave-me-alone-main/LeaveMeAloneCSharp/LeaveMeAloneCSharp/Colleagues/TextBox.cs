using LeaveMeAloneCSharp.Mediators.Interfaces;

namespace LeaveMeAloneCSharp.Colleagues
{
    /// <summary>
    /// The TextBox class represents a text input field in a form. 
    /// It holds the text entered by the user and notifies the mediator whenever the text changes. 
    /// The mediator can then take appropriate actions, such as enabling or disabling other UI elements based on the content of the text box.
    /// </summary>
    /// <param name="mediator"></param>
    public class TextBox(IFormMediator mediator)
    {
        public string Text { get; private set; } = string.Empty;

        public void SetText(string text)
        {
            Text = text;

            mediator.Notify(this, "TextChanged");
        }
    }
}
