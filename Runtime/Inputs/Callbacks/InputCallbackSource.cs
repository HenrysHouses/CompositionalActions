#nullable enable
namespace HH.Input.Contracts
{
    using HH.Actors.Contracts;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>The "InputCallback" is partially implemented so that the "InputCallbackSource" class has access to all of the callback data without exposing its setter properties to the public or internal classes. This way the source is the only class that can edit the callback data. As its part of the "InputCallback" implementation. This accessibility works from InnerClass -has access-> UpperClass. Not the other way around.</summary>
    public partial class InputCallback
    {
        public class InputCallbackSource
        {
            public InputCallbackSource(IDefaultActor actor)
            {
                State = new InputCallback
                {
                    Actor = actor,
                    Phase = ActionState.Waiting,
                    StartTime = Mathf.Infinity,
                    IntValue = 0,
                    FloatValue = 0,
                    BoolValue = false,
                };
            }

            public InputCallback State { get; private set; } = new InputCallback();

            public void RegisterStart()
            {
                if (State.Phase == ActionState.Failed)
                {
                    return;
                }

                State.Phase = ActionState.Started;
                State.StartTime = Time.time;
            }

            public void RegisterUpdate(int value)
            {
                if (State.Phase == ActionState.Failed)
                {
                    return;
                }

                State.Phase = ActionState.Updated;
                State.IntValue = value;
                State.FloatValue = value;
                State.BoolValue = value < 0;
            }

            public void RegisterUpdate(float value)
            {
                if (State.Phase == ActionState.Failed)
                {
                    return;
                }

                State.Phase = ActionState.Updated;
                State.IntValue = (int)value;
                State.FloatValue = value;
                State.BoolValue = value < 0;
            }

            public void RegisterUpdate()
            {
                if (State.Phase == ActionState.Failed)
                {
                    return;
                }

                State.Phase = ActionState.Updated;
                State.IntValue = 1;
                State.FloatValue = 1;
                State.BoolValue = true;
            }

            public void RegisterCancel()
            {
                if (State.Phase == ActionState.Failed)
                {
                    return;
                }

                State.Phase = ActionState.Canceled;
                State.IntValue = 0;
                State.FloatValue = 0;
                State.BoolValue = false;
            }

            public void RegisterComplete()
            {
                if (State.Phase == ActionState.Failed)
                {
                    return;
                }

                State.Phase = ActionState.Completed;
                State.IntValue = 0;
                State.FloatValue = 0;
                State.BoolValue = false;
            }

            public void RegisterFailed()
            {
                State.IntValue = 0;
                State.FloatValue = 0;
                State.BoolValue = false;
                State.MarkAsFailed();
            }

            public bool TryResetFailState()
            {
                if (State.Phase == ActionState.Failed)
                {
                    State.Phase = ActionState.Waiting;
                    State.IntValue = 0;
                    State.FloatValue = 0;
                    State.BoolValue = false;
                    return true;
                }

                return false;
            }

            public void RegisterDisabled()
            {
                State.Phase = ActionState.Disabled;
                State.IntValue = 0;
                State.FloatValue = 0;
                State.BoolValue = false;
                State.Actor = new NoActor();
                State.OnFailed.RemoveAllListeners();
            }

            public void RegisterEnabled(IDefaultActor actor, UnityAction<InputCallback> onFailAction)
            {
                State.Phase = ActionState.Waiting;
                State.IntValue = 0;
                State.FloatValue = 0;
                State.BoolValue = false;
                State.Actor = actor;
                State.OnFailed.AddListener(onFailAction);
            }
        }
    }
}
