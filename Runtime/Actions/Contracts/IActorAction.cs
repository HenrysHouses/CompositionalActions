#nullable enable

namespace HH.CompositionalActions.Contracts
{
    public interface IActorAction : IAction
    {
        bool CanRetrigger { get; }

        new IActorAction Copy();
    }
}
