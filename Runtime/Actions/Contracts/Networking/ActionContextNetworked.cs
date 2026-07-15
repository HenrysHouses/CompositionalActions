#nullable enable

namespace HH.CompositionalActions.Networking
{
    using Fusion;

    public struct ActionContextNetworked : INetworkStruct
    {
        public NetworkBehaviourId InitiatingActor;

        public NetworkBehaviourId ExecutingActor;
    }
}
