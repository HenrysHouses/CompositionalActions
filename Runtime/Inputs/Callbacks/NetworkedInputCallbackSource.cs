#nullable enable
namespace HH.Input.Networking
{
    using System;
    using Fusion;
    using HH.Input.Contracts;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.InputSystem;

    /// <summary>The "InputCallback" is partially implemented so that the "InputCallbackSource" class has access to all of the callback data without exposing its setter properties to the public or internal classes. This way the source is the only class that can edit the callback data. As its part of the "InputCallback" implementation. This accessibility works from InnerClass -has access-> UpperClass. Not the other way around.</summary>
    public partial struct NetworkedInputState : INetworkStruct
    {
        public class NetworkedInputCallbackSource
        {
            private UnityEvent<NetworkedInputState> onFailed = new UnityEvent<NetworkedInputState>();

            public NetworkedInputCallbackSource(NetworkBehaviourId id)
            {
                State = new NetworkedInputState
                {
                    ActorID = id,
                    Phase = ActionState.Waiting,
                    StartTime = Mathf.Infinity,
                    IntValue = 0,
                    FloatValue = 0,
                    BoolValue = false,
                    ActorCallbackID = string.Empty,
                };
            }

            public NetworkedInputState State { get; private set; }

            public void RegisterStart(string callbackID)
            {
                if (State.Phase is ActionState.Failed or ActionState.Disabled)
                {
                    return;
                }

                if (State.ActorCallbackID != string.Empty)
                {
                    throw new InvalidOperationException("tried registering start for an action that has an associated callbackID");
                }

                State = new NetworkedInputState
                {
                    ActorID = State.ActorID,
                    Phase = ActionState.Started,
                    StartTime = Time.time,
                    IntValue = State.IntValue,
                    FloatValue = State.FloatValue,
                    BoolValue = State.BoolValue,
                    ActorCallbackID = callbackID,
                };
            }

            public void RegisterInput(InputAction.CallbackContext callbackContext, string callbackID)
            {
                if (State.Phase is ActionState.Failed or ActionState.Disabled)
                {
                    return;
                }

                if (State.ActorCallbackID != callbackID)
                {
                    throw new InvalidOperationException("tried registering input for an actor callback with mismatching ID");
                }

                State = new NetworkedInputState
                {
                    ActorID = State.ActorID,
                    Phase = ActionState.Updated,
                    StartTime = State.StartTime,
                    // IntValue = callbackContext.read<int>(),
                    // FloatValue = callbackContext.ReadValue<float>(),
                    BoolValue = callbackContext.ReadValueAsButton(),
                    ActorCallbackID = State.ActorCallbackID,
                };
            }

            public void RegisterUpdate(int value, string callbackID)
            {
                if (State.Phase is ActionState.Failed or ActionState.Disabled)
                {
                    return;
                }

                if (State.ActorCallbackID != callbackID)
                {
                    throw new InvalidOperationException("tried registering update for an actor callback with mismatching ID");
                }

                State = new NetworkedInputState
                {
                    ActorID = State.ActorID,
                    Phase = ActionState.Updated,
                    StartTime = State.StartTime,
                    IntValue = value,
                    FloatValue = value,
                    BoolValue = value < 0,
                    ActorCallbackID = State.ActorCallbackID,
                };
            }

            public void RegisterUpdate(float value, string callbackID)
            {
                if (State.Phase is ActionState.Failed or ActionState.Disabled)
                {
                    return;
                }

                if (State.ActorCallbackID != callbackID)
                {
                    throw new InvalidOperationException("tried registering update for an actor callback with mismatching ID");
                }

                State = new NetworkedInputState
                {
                    ActorID = State.ActorID,
                    Phase = ActionState.Updated,
                    StartTime = State.StartTime,
                    IntValue = (int)value,
                    FloatValue = value,
                    BoolValue = value < 0,
                    ActorCallbackID = State.ActorCallbackID,
                };
            }

            public void RegisterUpdate(string callbackID)
            {
                if (State.Phase is ActionState.Failed or ActionState.Disabled)
                {
                    return;
                }

                if (State.ActorCallbackID != callbackID)
                {
                    throw new InvalidOperationException("tried registering update for an actor callback with mismatching ID");
                }

                State = new NetworkedInputState
                {
                    ActorID = State.ActorID,
                    Phase = ActionState.Updated,
                    StartTime = State.StartTime,
                    IntValue = 1,
                    FloatValue = 1,
                    BoolValue = true,
                    ActorCallbackID = State.ActorCallbackID,
                };
            }

            public void RegisterCancel(string callbackID)
            {
                if (State.Phase is ActionState.Failed or ActionState.Disabled)
                {
                    return;
                }

                if (State.ActorCallbackID != callbackID)
                {
                    throw new InvalidOperationException("tried registering cancellation for an actor callback with mismatching ID");
                }

                State = new NetworkedInputState
                {
                    ActorID = State.ActorID,
                    Phase = ActionState.Canceled,
                    StartTime = State.StartTime,
                    IntValue = 0,
                    FloatValue = 0,
                    BoolValue = false,
                    ActorCallbackID = string.Empty,
                };
            }

            public void RegisterComplete(string callbackID)
            {
                if (State.Phase is ActionState.Failed or ActionState.Disabled)
                {
                    return;
                }

                if (State.ActorCallbackID != callbackID)
                {
                    throw new InvalidOperationException("tried registering completion for an actor callback with mismatching ID");
                }

                State = new NetworkedInputState
                {
                    ActorID = State.ActorID,
                    Phase = ActionState.Completed,
                    StartTime = State.StartTime,
                    IntValue = 0,
                    FloatValue = 0,
                    BoolValue = false,
                    ActorCallbackID = string.Empty,
                    // ActorCallbackID = State.ActorCallbackID,
                };
            }

            public void RegisterFailed(string callbackID)
            {
                if (State.ActorCallbackID != callbackID)
                {
                    throw new InvalidOperationException("tried registering input for an actor callback with mismatching ID");
                }

                State = new NetworkedInputState
                {
                    ActorID = State.ActorID,
                    Phase = ActionState.Failed,
                    StartTime = State.StartTime,
                    IntValue = 0,
                    FloatValue = 0,
                    BoolValue = false,
                    ActorCallbackID = State.ActorCallbackID,
                };
                onFailed?.Invoke(State);
            }

            public bool TryResetFailState(string callbackID)
            {
                if (State.ActorCallbackID != callbackID)
                {
                    throw new InvalidOperationException("tried registering input for an actor callback with mismatching ID");
                }

                if (State.Phase is ActionState.Failed or ActionState.Disabled)
                {
                    State = new NetworkedInputState
                    {
                        ActorID = State.ActorID,
                        Phase = ActionState.Waiting,
                        StartTime = State.StartTime,
                        IntValue = 0,
                        FloatValue = 0,
                        BoolValue = false,
                        ActorCallbackID = string.Empty,
                    };
                    return true;
                }

                return false;
            }

            public void RegisterDisabled()
            {
                State = new NetworkedInputState
                {
                    ActorID = default,
                    Phase = ActionState.Disabled,
                    StartTime = Mathf.NegativeInfinity,
                    IntValue = 0,
                    FloatValue = 0,
                    BoolValue = false,
                    ActorCallbackID = string.Empty,
                };
                onFailed.RemoveAllListeners();
            }

            public void RegisterEnabled(NetworkBehaviourId id, UnityAction<NetworkedInputState> onFailAction)
            {
                State = new NetworkedInputState
                {
                    ActorID = id,
                    Phase = ActionState.Waiting,
                    StartTime = Mathf.NegativeInfinity,
                    IntValue = 0,
                    FloatValue = 0,
                    BoolValue = false,
                    ActorCallbackID = string.Empty,
                };
                onFailed.AddListener(onFailAction);
            }
        }
    }
}
