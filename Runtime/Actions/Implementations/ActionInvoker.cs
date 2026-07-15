#nullable enable
namespace HH.CompositionalActions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using HH.CompositionalActions.Contracts;
    using UnityEngine;

    [Serializable]
    public class ActionInvoker
    {
        protected readonly MonoBehaviour executor;
        private readonly Stack<IAction> history;

        protected virtual Stack<IAction> ActionHistory => history;

        protected IActionContext? initiatingContext;
        protected IActionResult? currentSequenceResults;

        public ActionInvoker(MonoBehaviour executor)
        {
            this.executor = executor;
            history = new Stack<IAction>(20);
            initiatingContext = null;
            currentSequenceResults = null;
#if UNITY_EDITOR
            Executor = executor;
            DebugActionHistory = ActionHistory.ToArray(); // ! This is the real code, for testing im making a dummy array to override this in the editor
#endif
        }

#if UNITY_EDITOR
        public Action<ExecutionState, ActionInvoker> EditorExecutionStateHook { get; set; } = (state, invoker) => { };
                // Debug.Log($"No editor hook was attached for {invoker.executor.GetType().Name}'s Action Invoker on {invoker.executor.gameObject.name}", invoker.executor.gameObject);

        [field: SerializeField] public MonoBehaviour Executor { get; protected set; } // exposing variables for the debugger

        // public IReadOnlyList<IAction> Actions => Array.AsReadOnly(order);
        [field: SerializeReference] public IAction[] DebugActionHistory { get; set; } // ! exposing variables for the debugger, the setter should be private after testing is done

        [field: SerializeReference] public bool ShowHistory { get; set; } // exposing variables for the debugger

        [field: SerializeReference] public bool ShowExecution { get; set; } // exposing variables for the debugger

        public virtual Type GetDebuggerTarget()
        {
            return typeof(IAction);
        }
#endif
        // TODO: method for invoking a single action

        public virtual bool Invoke(ActionSequence newSequence, IActionContext context)
        {
            return InvokeSequence(newSequence, context);
        }

        /// <summary>
        /// Invokes a chain of actions in context of each other, where they gain access to each others action result history. re-invokes the current action if called multiple times while the sequence is still executing.
        /// </summary>
        /// <param name="newSequence">Invoke a new sequence.</param>
        /// <param name="context">How the action sequence was started.</param>
        /// <returns>is true when a new sequence begins invocation</returns>
        protected bool InvokeSequence(ActionSequence newSequence, IActionContext context)
        {
            ExecutionState state = new ExecutionState();
            // if there is no current running sequence then start the sequence from the beginning, else re-trigger the current action.
            IEnumerator invocation = Execute(newSequence.CopyActions(), context, state);

            context.ActorCallback.OnComplete += ClearCurrentSequenceContext;
            initiatingContext ??= context;

#if UNITY_EDITOR
            Coroutine editorRoutine = executor.StartCoroutine(invocation);
            state.Coroutine = editorRoutine;
            EditorExecutionStateHook?.Invoke(state, this);
#else
            executor.StartCoroutine(invocation);
#endif
            context.ActorCallback.MarkExecuting();

            // true if the sequence just started invoking
            return initiatingContext == context;
        }

        protected IEnumerator ExecuteSequence(IAction[] inOrder, IActionContext context, ExecutionState state)
        {
            bool shouldCancel = false;
            state.SetActions(inOrder);

            foreach (IAction action in inOrder)
            {
                IActionResult executionResult = action.ExecuteAction(context, currentSequenceResults ?? new EmptyActionResult());

                if (currentSequenceResults != null)
                {
                    executionResult.AddResultChain(currentSequenceResults);
                }

                ActionHistory.Push(action);
                yield return executionResult;
                currentSequenceResults = executionResult;

                if (executionResult is IActionCancelResult cancellation)
                {
                    cancellation.HandleCancellation(context, currentSequenceResults);
                    shouldCancel = true;
                }

                // debugging
                state.AddResult(executionResult);
                state.IncrementExecution();
#if UNITY_EDITOR
                DebugActionHistory = ActionHistory.ToArray();
#endif

                if (shouldCancel)
                {
                    Debug.Log("Cancelled action");
                    break;
                }
            }

            context.ActorCallback.TryComplete();
            state.IsFinished = true;
        }

        protected virtual IEnumerator Execute(IAction[] inOrder, IActionContext context, ExecutionState state)
        {
            return ExecuteSequence(inOrder, context, state);
        }

// protected virtual IEnumerator ExecuteTarget(IAction action, IActionContext actionContext, ExecutionState state)
//         {
//             IActionResult actionSequenceHistory = currentSequenceResults ?? new EmptyActionResult();
//             IActionResult executionResult = action.ExecuteAction(actionContext, actionSequenceHistory);
//             executionResult.AddResultChain(actionSequenceHistory);
//             yield return executionResult;
//             currentSequenceResults = executionResult;
//             ActionHistory.Push(action);
//
//             // debugging
//             state.AddResult(executionResult);
// #if UNITY_EDITOR
//             DebugActionHistory = ActionHistory.ToArray();
// #endif
//         }

        protected void ClearCurrentSequenceContext(string callbackID)
        {
            if (initiatingContext == null)
            {
                return;
            }

            bool ownsCallback = initiatingContext.ActorCallback.ID.Equals(callbackID);

            if (ownsCallback)
            {
                initiatingContext = null;
                currentSequenceResults = null;
            }
        }

        [Serializable]
        public class ExecutionState
        {
            public int ExecutionIndex { get; set; }

            public List<IActionResult> ActionResults { get; private set; } = new List<IActionResult>();

            public IReadOnlyList<IAction>? Actions { get; private set; }

            public bool IsFinished { get; set; }

            public bool IsFailed { get; set; }

            public Coroutine? Coroutine { get; set; }

            public void AddResult(IActionResult result)
            {
                ActionResults.Add(result);
            }

            public void SetActions(IAction[] actions)
            {
                Actions = Array.AsReadOnly(actions);
            }

            public void IncrementExecution()
            {
                ExecutionIndex++;
            }
        }
    }
}
