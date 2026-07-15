#nullable enable
namespace HH.Input.Contracts
{
    using HH.Actors.Contracts;
    using UnityEngine;
    using UnityEngine.Events;

    public enum ActionState
    {
        /// <summary>The action can not be executed.</summary>
        Disabled,

        /// <summary>The action is waiting to finish execution.</summary>
        Waiting,

        /// <summary>The action was started.</summary>
        Started,

        /// <summary>The action was updated.</summary>
        Updated,

        /// <summary>The action was manually ended.</summary>
        Canceled,

        /// <summary>The action finished execution normally.</summary>
        Completed, // not sure if this will be used

        /// <summary>The action could not finish execution.</summary>
        Failed, // not sure if this will be used
    }

    /// <summary>The "InputCallback" is partially implemented so that the "InputCallbackSource" class has access to all of the callback data without exposing its setter properties to the public or internal classes. This way the source is the only class that can edit the callback data. As its part of the "InputCallback" implementation. This accessibility works from InnerClass -has access-> UpperClass. Not the other way around.</summary>
    public partial class InputCallback
    {
        public IDefaultActor Actor { get; protected set; } = new NoActor();

        public UnityEvent<InputCallback> OnFailed { get; private set; } = new UnityEvent<InputCallback>();

        public ActionState Phase { get; protected set; }

        public bool Started => Phase == ActionState.Started;

        public bool Performed => Phase == ActionState.Updated;

        public bool Canceled => Phase == ActionState.Canceled;

        public bool Failed => Phase == ActionState.Failed;

        public float Duration => Time.time - StartTime;

        public int IntValue { get; protected set; }

        public float FloatValue { get; protected set; }

        public bool BoolValue { get; protected set; }

        protected float StartTime { get; set; }

        public void MarkAsFailed()
        {
            Phase = ActionState.Failed;
            OnFailed?.Invoke(this); // this should prevent the other states from being called the same frame.
        }
    }
}
