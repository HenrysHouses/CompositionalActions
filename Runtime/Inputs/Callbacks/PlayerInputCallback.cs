#nullable enable
namespace HH.Input.Contracts
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>The "PlayerInputCallback" is partially implemented so that the "PlayerCallbackSource" class has access to all of the callback data without exposing its setter properties to the public or internal classes. This way the source is the only class that can edit the callback data. As its part of the "PlayerInputCallback" implementation. This accessibility works from InnerClass -has access-> UpperClass. Not the other way around.</summary>
    public partial class PlayerInputCallback : InputCallback
    {
        public PlayerInputCallback(InputAction.CallbackContext callbackContext)
        {
            switch (callbackContext.phase)
            {
                case InputActionPhase.Started:
                    Phase = ActionState.Started;
                    break;
                case InputActionPhase.Performed:
                    Phase = ActionState.Updated;
                    break;
                case InputActionPhase.Canceled:
                    Phase = ActionState.Canceled;
                    break;
                case InputActionPhase.Waiting:
                    Phase = ActionState.Waiting;
                    break;
                case InputActionPhase.Disabled:
                    Phase = ActionState.Disabled;
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            InputCallbackContext = callbackContext;
            StartTime = Time.time;
            IntValue = 0;
            FloatValue = 0;
            BoolValue = false;
        }

        protected InputAction.CallbackContext InputCallbackContext { get; set; }
    }
}
