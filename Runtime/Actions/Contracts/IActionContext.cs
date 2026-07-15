#nullable enable

namespace HH.CompositionalActions.Contracts
{
    using HH.Actors.Contracts;

    public interface IActionContext
    {
        IDefaultActor InitiatingActor { get; }

        IDefaultActor ExecutingActor { get; }

        ActorCallback ActorCallback { get; }
    }
}
