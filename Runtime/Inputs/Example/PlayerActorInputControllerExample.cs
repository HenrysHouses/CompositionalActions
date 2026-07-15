#nullable enable
namespace HH.Input.Examples
{
    using HH.Actors.Contracts;
    using UnityEngine;
    using static HH.Actors.Contracts.ActorCallback;

    [System.Serializable]
    public class PlayerActorInputControllerExample : MonoBehaviour, IDefaultActor
    {
        ActorCallbackSource IDefaultActor.CallbackSource => throw new System.NotImplementedException();

        [field:SerializeField] private PlayerInputActionBinding InteractInput { get; set; } = new();

        private void Start()
        {
            EnableInputs();
        }

        private void EnableInputs()
        {
            if (InteractInput == null)
            {
                return;
            }

            InteractInput.BindInput(this);
        }

        // void DisableInputs()
        // {
        //     InteractInput.RemoveInputBinding();
        // }

        // void OnDisable()
        // {
        //     DisableInputs();
        // }

        // void OnDestroy()
        // {
        //     DisableInputs();
        // }
    }
}
