using LeaveMeAloneCSharp.Colleagues;
using LeaveMeAloneCSharp.Mediators.Interfaces;

namespace LeaveMeAloneCSharp.Mediators
{
    /// <summary>
    /// Serving as the central hub for communication between the TextBox and Button
    /// </summary>
    public class FormMediator: IFormMediator
    {
        public Button Button { get; set; }
        public TextBox TextBox { get; set; }

        public void Notify(object sender, string ev)
        {
            if(ev == "TextChanged")
            {
                // Enable the button only if the text box is not empty
                Button.Enabled = !string.IsNullOrEmpty(TextBox.Text);
            }
        }
    }
}
