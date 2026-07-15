#nullable enable
namespace HH.Input
{
    using HH.Actors.Contracts;
    using HH.Attributes;
    using HH.Input.Contracts;
    using UnityEngine;

    [System.Serializable]
    public class InputActionBinding : IInputBinding
    {
        // [SerializeField] UnityEvent OnStarted = new UnityEvent();
        // [SerializeField] UnityEvent OnActivated = new UnityEvent();
        // [SerializeField] UnityEvent OnCancelled = new UnityEvent();
        [SerializeReference, ReferenceInstancer] private IActionInput action = new NoInput();

        private InputCallback.InputCallbackSource? inputCallbackSource;

        IActionInput IInputBinding.Action { get => action; set => action = value; }

        [field:Space(5), Header("Binding Properties")]
        [field:SerializeField] public bool IsEnabled { get; private set; } = true;

        [field:SerializeField] public bool IsSwappable { get; private set; } = true;

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

            IsEnabled = true;
            inputCallbackSource = new InputCallback.InputCallbackSource(actor);
            inputCallbackSource.RegisterEnabled(actor, action.Failed);
        }

        public void RemoveInputBinding()
        {
            if (inputCallbackSource == null)
            {
                return;
            }

            IsEnabled = false;
            inputCallbackSource.RegisterDisabled();
        }

        public void Start()
        {
            if (!IsEnabled || inputCallbackSource == null)
            {
                Debug.LogWarning("attempted to start an action that is not bound, binding inputs typically happens during Start() or Awake()");
                return;
            }

            inputCallbackSource.RegisterStart();
            action.Start(inputCallbackSource.State);

            // if(!callbackSource.State.failed)
            //     OnStarted?.Invoke();
        }

        public void Update()
        {
            if (!IsEnabled || inputCallbackSource == null)
            {
                Debug.LogWarning("attempted to update an action that is not bound, binding inputs typically happens during Start() or Awake()");
                return;
            }

            inputCallbackSource.RegisterUpdate();
            action.Update(inputCallbackSource.State);

            // if(!callbackSource.State.failed)
            //     OnActivated?.Invoke();
        }

        public void Cancel()
        {
            if (!IsEnabled || inputCallbackSource == null)
            {
                Debug.LogWarning("attempted to cancel an action that is not bound, binding inputs typically happens during Start() or Awake()");
                return;
            }

            inputCallbackSource.RegisterCancel();
            action.Cancel(inputCallbackSource.State);

            // if(!callbackSource.State.failed)
            //     OnCancelled?.Invoke();
        }

        public bool TryRecoverFailedAction()
        {
            if (!IsEnabled || inputCallbackSource == null)
            {
                Debug.LogWarning("attempted to recover an action that is not bound, binding inputs typically happens during Start() or Awake()");
                return false;
            }

            if (!inputCallbackSource.State.Failed)
            {
                return false;
            }

            if (inputCallbackSource.TryResetFailState())
            {
                return true;
            }

            return false;
        }
    }
}
