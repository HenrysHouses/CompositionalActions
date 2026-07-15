#nullable enable

namespace HH.CompositionalActions
{
    using HH.CompositionalActions.Contracts;
    using UnityEngine;

    public class EmptyAction : IAction
    {
        [field: SerializeField, HideInInspector] public string label { get; private set; } = string.Empty;

        public bool CanRetrigger => false;

        public IAction Copy()
        {
            return new EmptyAction();
        }

        public IActionResult ExecuteAction(IActionContext context, IActionResult history)
        {
            return new EmptyActionResult();
        }

        public void UpdateLabel()
        {
            label = nameof(EmptyAction);
        }
    }
}
