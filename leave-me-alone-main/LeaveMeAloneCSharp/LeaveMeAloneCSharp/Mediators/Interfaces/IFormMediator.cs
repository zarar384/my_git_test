namespace LeaveMeAloneCSharp.Mediators.Interfaces
{
    /// <summary>
    /// Interface for the <see cref="FormMediator"/>, which defines the contract 
    /// for communication between the mediator and its colleagues (e.g., TextBox and Button).
    /// </summary>
    public interface IFormMediator
    {
        /// <summary>
        /// Notifies the mediator of an event from a colleague.
        /// </summary>
        /// <param name="sender">The colleague that triggered the event.</param>
        /// <param name="ev">The event that occurred.</param>
        void Notify(object sender, string ev);
    }
}
