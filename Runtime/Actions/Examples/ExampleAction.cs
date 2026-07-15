#nullable enable
namespace HH.CompositionalActions.Examples
{
    using System;
    using System.Collections;
    using HH.CompositionalActions.Contracts;
    using HH.Actors.Contracts;
    using UnityEngine;

    [Serializable]
    internal sealed class ExampleAction : IAction
    {
        [field: SerializeField, HideInInspector] public string label { get; private set; } = string.Empty;

        [SerializeField] private string actorName;

        public ExampleAction()
        {
            actorName = "PlayerCharacter";
        }

        public bool CanRetrigger => false;

        public IActionResult ExecuteAction(IActionContext context, IActionResult history)
        {
            if (((IGameObjectActor)context.InitiatingActor).GetGameObject().name == actorName)
            {
                return new EmptyActionResultDelay(
                    ExecuteAwaitableAction());
            }

            return ExecuteInstantAction(context, history);
        }

        public IActionResult ExecuteInstantAction(IActionContext context, IActionResult history)
        {
            Debug.Log("This is an example. Do not implement game code here.");

            bool hasResult = history.TryGetResult<ExampleActionResult>(
                out IActionResult storedResult);

            if (!hasResult)
            {
                return new ExampleActionResult(context.InitiatingActor);
            }

            ExampleActionResult exampleStorage = (ExampleActionResult)storedResult;
            exampleStorage.IncrementTriggers();
            return new EmptyActionResult();
        }

        public IEnumerator ExecuteAwaitableAction()
        {
            WaitForSeconds timer = new WaitForSeconds(1);

            Debug.Log("0 second passed");
            yield return timer;
            Debug.Log("1 second passed");
            yield return timer;
            Debug.Log("2 second passed");
            yield return timer;
            Debug.Log("3 second passed");
            yield return timer;
            Debug.Log("4 second passed");
        }

        public IAction Copy()
        {
            return (IAction)MemberwiseClone();
        }

        public void UpdateLabel()
        {
            label = nameof(ExampleAction);
        }
    }
}
