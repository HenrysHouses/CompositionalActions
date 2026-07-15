#nullable enable
namespace HH.Input.Examples
{
    using Fusion;
    using HH.Actors.Contracts;
    using HH.Input.Networking;
    using UnityEngine;
    using static HH.Actors.Contracts.ActorCallback;

    [System.Serializable]
    public class NetworkedPlayerActorInputControllerExample : NetworkBehaviour, IDefaultActor
    {
        private ActorCallbackSource actorCallbackFactory;

        public NetworkedPlayerActorInputControllerExample()
        {
            actorCallbackFactory = new(this);
        }

        ActorCallbackSource IDefaultActor.CallbackSource => throw new System.NotImplementedException();

        [field:SerializeField] private NetworkedInputActionBinding InteractInput { get; set; } = new();

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

            InteractInput.BindInput(Id, actorCallbackFactory);
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
