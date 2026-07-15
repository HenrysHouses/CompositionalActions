#nullable enable
namespace HH.CompositionalActions.Networking
{
    using Fusion;
    using HH.Input.Networking;

    public struct NetworkedActorInputActionContext : INetworkStruct
    {
        public NetworkBehaviourId InitiatingActor { get; set; }

        public NetworkBehaviourId ExecutingActor { get; set; }

        public NetworkedInputState InputState { get; set; }
    }
}
