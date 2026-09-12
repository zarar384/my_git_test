using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Commands.OfficerLifecycle
{
    /// <summary>
    /// Starts a new game for a player by creating a brand new recruitment
    /// officer for them. This is how a player whose previous officer's career
    /// ended (resigned, retired, fired, died) can play again, and it is also
    /// how the very first officer for a new player is created. A single
    /// player is simply the current special case of this future multiplayer
    /// model.
    /// </summary>
    public sealed record StartOfficerCareerCommand(Guid PlayerId, string FullName, string Department) : IRequest<Guid>;
}
