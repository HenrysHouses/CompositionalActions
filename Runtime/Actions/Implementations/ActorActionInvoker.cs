#nullable enable
namespace HH.CompositionalActions
{
    using System;
    using System.Collections;
    using HH.CompositionalActions.Contracts;
    using UnityEngine;

    [Serializable]
    public class ActorActionInvoker : ActionInvoker
    {
        public ActorActionInvoker(MonoBehaviour executor) : base(executor)
        {
        }

#if UNITY_EDITOR
        public override Type GetDebuggerTarget()
        {
            return typeof(IActorAction);
        }
#endif

        /// <summary>
        /// Invokes a chain of actions in context of each other, where they gain access to each others action result history. re-invokes the current action if called multiple times while the sequence is still executing.
        /// </summary>
        /// <param name="newSequence">Invoke a new sequence.</param>
        /// <param name="context">How the action sequence was started.</param>
        /// <returns>is true when a new sequence begins invocation</returns>
        public override bool Invoke(ActionSequence newSequence, IActionContext context)
        {
            ExecutionState state = new ExecutionState();
            // if there is no current running sequence then start the sequence from the beginning, else re-trigger the current action.
            IEnumerator invocation =
                initiatingContext == null ?
                    ExecuteSequence(newSequence.CopyActions(), context, state) : // true
                    ReExecuteCurrent(context, state); // false

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
            // ! BUG initiating context never clears

            // true if the sequence just started invoking
            return initiatingContext == context;
        }

        private IEnumerator ReExecuteCurrent(IActionContext retriggerContext, ExecutionState state)
        {
            IActorAction retrigger = (IActorAction)ActionHistory.Peek();

            if (!retrigger.CanRetrigger)
            {
                yield break;
            }

            state.SetActions(new IAction[1] { retrigger });
            yield return ExecuteAction(retrigger, retriggerContext, state);
            state.IncrementExecution();
            state.IsFinished = true;
        }

        private IEnumerator ExecuteAction(IActorAction action, IActionContext actionContext, ExecutionState state)
        {
            IActionResult actionSequenceHistory = currentSequenceResults ?? new EmptyActionResult();
            IActionResult executionResult = action.ExecuteAction(actionContext, actionSequenceHistory);
            executionResult.AddResultChain(actionSequenceHistory);
            yield return executionResult;
            currentSequenceResults = executionResult;
            ActionHistory.Push(action);

            // debugging
            state.AddResult(executionResult);
#if UNITY_EDITOR
            DebugActionHistory = ActionHistory.ToArray();
#endif
        }

        // private void ClearCurrentSequenceContext(string callbackID)
        // {
        //     if (initiatingContext == null)
        //     {
        //         return;
        //     }
        //
        //     bool ownsCallback = initiatingContext.ActorCallback.ID.Equals(callbackID);
        //
        //     if (ownsCallback)
        //     {
        //         initiatingContext = null;
        //         currentSequenceResults = null;
        //     }
        // }
    }
}
