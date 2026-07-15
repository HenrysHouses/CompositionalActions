#nullable enable
namespace HH.Input.Networking
{
    using System;
    using Fusion;
    using HH.Actors.Contracts;
    using HH.Attributes;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [Serializable]
    public class NetworkedInputActionBinding : INetworkedInputBinding
    {
        [SerializeField, Header("Input Action Binding")]
        private InputActionReference? input;
        [SerializeReference, ReferenceInstancer] private INetworkedActionInput action = new NoNetworkInput();
        private NetworkedInputState.NetworkedInputCallbackSource? callbackSource;
        private ActorCallback.ActorCallbackSource? sourceReference;
        private ActorCallback? currentCallback;

        INetworkedActionInput INetworkedInputBinding.Action { get => action; set => action = value; }

        [field: Space(5), Header("Binding Properties")]
        [field: SerializeField] public bool IsSwappable { get; private set; } = false;

        [field: SerializeField] public bool IsEnabled { get; private set; } = true;

        public void SetInput(INetworkedActionInput action)
        {
            if (!IsSwappable)
            {
                Debug.LogWarning($"{action.GetType()} can not be edited during runtime.");
                return;
            }

            this.action = action;
        }

        public void BindInput(NetworkBehaviourId id, ActorCallback.ActorCallbackSource actorCallback)
        {
            if (action == new NoInput())
            {
                return;
            }

            if (input != null)
            {
                input.action.started += StartAction;
                input.action.performed += UpdateAction;
                input.action.canceled += CancelAction;
            }

            callbackSource = new NetworkedInputState.NetworkedInputCallbackSource(id);
            callbackSource.RegisterEnabled(id, action.Failed);
            sourceReference = actorCallback;

            // Input.action.Enable();
            IsEnabled = true;
        }

        public void RemoveInputBinding()
        {
            if (input == null || callbackSource == null)
            {
                return;
            }

            input.action.started -= StartAction;
            input.action.performed -= UpdateAction;
            input.action.canceled -= CancelAction;
            callbackSource.RegisterDisabled();
            sourceReference = null;

            // Input.action.Disable();
            IsEnabled = false;
        }

        // # Translating Unity input callback to our custom one so we have more control
        public void StartAction(InputAction.CallbackContext callbackContext)
        {
            if (!IsEnabled || callbackSource == null)
            {
                Debug.LogWarning("attempted to start an action that is not bound, binding inputs typically happens during Start() or Awake()");
                return;
            }

            if (sourceReference == null)
            {
                Debug.LogWarning("attempted to start an action that do not have a actor callback source associated");
                return;
            }

            Debug.Log("starting");
            currentCallback = sourceReference.CreateCallback();
            Debug.Log("id size: " + currentCallback.ID.Length);
            callbackSource.RegisterStart(currentCallback.ID);
            callbackSource.RegisterInput(callbackContext, currentCallback.ID);
            Start();
        }

        public void UpdateAction(InputAction.CallbackContext callbackContext)
        {
            if (!IsEnabled || callbackSource == null)
            {
                Debug.LogWarning("attempted to update an action that is not bound, binding inputs typically happens during Start() or Awake()");
                return;
            }

            if (currentCallback == null)
            {
                Debug.LogWarning("attempted to update an action that has not started");
                return;
            }

            Debug.Log("updating");
            callbackSource.RegisterInput(callbackContext, currentCallback.ID);
            Update();
        }

        public void CancelAction(InputAction.CallbackContext callbackContext)
        {
            if (!IsEnabled || callbackSource == null)
            {
                Debug.LogWarning("attempted to cancel an action that is not bound, binding inputs typically happens during Start() or Awake()");
                return;
            }

            if (currentCallback == null)
            {
                Debug.LogWarning("attempted to update an action that has not started");
                return;
            }

            Debug.Log("cancelled: " + currentCallback.ID);
            callbackSource.RegisterInput(callbackContext, currentCallback.ID);
            callbackSource.RegisterCancel(currentCallback.ID);
            Cancel();
        }

        // interface implementation
        public void Start()
        {
            if (callbackSource == null)
            {
                return;
            }

            action.Start(callbackSource.State);
        }

        public void Update()
        {
            if (callbackSource == null)
            {
                return;
            }

            action.Update(callbackSource.State);
        }

        public void Cancel()
        {
            if (callbackSource == null)
            {
                return;
            }

            action.Cancel(callbackSource.State);
        }
    }
}
