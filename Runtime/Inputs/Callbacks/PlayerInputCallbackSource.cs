#nullable enable
namespace HH.Input.Contracts
{
    using HH.Actors.Contracts;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.InputSystem;

    /// <summary>The "PlayerInputCallback" is partially implemented so that the "PlayerCallbackSource" class has access to all of the callback data without exposing its setter properties to the public or internal classes. This way the source is the only class that can edit the callback data. As its part of the "PlayerInputCallback" implementation. This accessibility works from InnerClass -has access-> UpperClass. Not the other way around.</summary>
    public partial class PlayerInputCallback : InputCallback
    {
        public class PlayerInputCallbackSource
        {
            public PlayerInputCallbackSource()
            {
                State = new PlayerInputCallback(default)
                {
                    Phase = ActionState.Waiting,
                    StartTime = Mathf.Infinity,
                    IntValue = 0,
                    FloatValue = 0,
                    BoolValue = false,
                };
            }

            public PlayerInputCallback State { get; private set; }

            public void UpdatePlayerInput(InputAction.CallbackContext callbackContext)
            {
                switch (callbackContext.phase)
                {
                    case InputActionPhase.Started:
                        State.Phase = ActionState.Started;
                        State.StartTime = Time.time;
                        break;
                    case InputActionPhase.Performed:
                        State.Phase = ActionState.Updated;
                        break;
                    case InputActionPhase.Canceled:
                        State.Phase = ActionState.Canceled;
                        break;
                    case InputActionPhase.Waiting:
                        State.Phase = ActionState.Waiting;
                        break;
                    case InputActionPhase.Disabled:
                        State.Phase = ActionState.Disabled;
                        break;
                    default:
                        break;
                }

                // overriding these values instead turned out to be worse
                // than just reading them from the callback context
                State.FloatValue = callbackContext.ReadValue<float>();
                State.IntValue = (int)State.FloatValue;
                State.BoolValue = callbackContext.ReadValueAsButton();
                State.InputCallbackContext = callbackContext;
            }

            public void RegisterEnabled(IDefaultActor actor, UnityAction<InputCallback> onFailAction)
            {
                State.Phase = ActionState.Waiting;
                State.Actor = actor;
                State.OnFailed.AddListener(onFailAction);
            }

            public void RegisterDisabled()
            {
                State.Phase = ActionState.Disabled;
                State.Actor = new NoActor();
                State.OnFailed.RemoveAllListeners();
            }
        }
    }
}
