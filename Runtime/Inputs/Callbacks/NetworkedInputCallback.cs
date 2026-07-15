#nullable enable
namespace HH.Input.Networking
{
    using Fusion;
    using HH.Input.Contracts;
    using UnityEngine;

    /// <summary>The "InputCallback" is partially implemented so that the "InputCallbackSource" class has access to all of the callback data without exposing its setter properties to the public or internal classes. This way the source is the only class that can edit the callback data. As its part of the "InputCallback" implementation. This accessibility works from InnerClass -has access-> UpperClass. Not the other way around.</summary>
    public partial struct NetworkedInputState : INetworkInput
    {
        public NetworkBehaviourId ActorID { get; set; }

        public ActionState Phase { get; set; }

        // public readonly bool Started => Phase == ActionState.Started;
        //
        // public readonly bool Performed => Phase == ActionState.Updated;
        //
        // public readonly bool Canceled => Phase == ActionState.Canceled;
        //
        // public readonly bool Failed => Phase == ActionState.Failed;
        //
        // public readonly float Duration => Time.time - StartTime;

        public int IntValue { get; set; }

        public float FloatValue { get; set; }

        public bool BoolValue { get; set; }

        public NetworkString<_32> ActorCallbackID { get; set; }

        private float StartTime { get; set; }
    }
}
