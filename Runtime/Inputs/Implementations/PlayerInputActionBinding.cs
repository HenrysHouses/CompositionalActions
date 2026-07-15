#nullable enable
namespace HH.Input
{
    using System;
    using Fusion;
    using HH.Actors.Contracts;
    using HH.Attributes;
    using HH.Input.Contracts;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [Serializable]
    public class PlayerInputActionBinding : IInputBinding
    {
        [SerializeField, Header("Input Action Binding")]
        private InputActionReference? input;
        [SerializeReference, ReferenceInstancer] private IActionInput action = new NoInput();
        private PlayerInputCallback.PlayerInputCallbackSource? callbackSource;

        IActionInput IInputBinding.Action { get => action; set => action = value; }

        [field:Space(5), Header("Binding Properties")]
        [field:SerializeField] public bool IsSwappable { get; private set; } = false;

        [field:SerializeField] public bool IsEnabled { get; private set; } = true;

        public void SetInput(IActionInput action)
        {
            if (!IsSwappable)
            {
                Debug.LogWarning($"{action.GetType()} can not be edited during runtime.");
                return;
            }

            this.action = action;
        }

        public void BindInput(IDefaultActor actor)
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

            callbackSource = new PlayerInputCallback.PlayerInputCallbackSource();
            callbackSource.RegisterEnabled(actor, action.Failed);

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

            callbackSource.UpdatePlayerInput(callbackContext);
            Start();
        }

        public void UpdateAction(InputAction.CallbackContext callbackContext)
        {
            if (!IsEnabled || callbackSource == null)
            {
                Debug.LogWarning("attempted to update an action that is not bound, binding inputs typically happens during Start() or Awake()");
                return;
            }

            callbackSource.UpdatePlayerInput(callbackContext);
            Update();
        }

        public void CancelAction(InputAction.CallbackContext callbackContext)
        {
            if (!IsEnabled || callbackSource == null)
            {
                Debug.LogWarning("attempted to cancel an action that is not bound, binding inputs typically happens during Start() or Awake()");
                return;
            }

            callbackSource.UpdatePlayerInput(callbackContext);
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
