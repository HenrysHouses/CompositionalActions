#nullable enable

namespace HH.CompositionalActions.Contracts
{
    using Fusion;

    public interface INetworkActionContext
    {
        NetworkId InitiatingActor { get; }

        NetworkId ExecutingActor { get; }

        string ActorCallback { get; }
    }
}
